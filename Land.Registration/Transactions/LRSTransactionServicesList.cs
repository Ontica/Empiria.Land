/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction services                         Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LRSTransactionServicesList                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : List of services to be provided in a transaction context.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.DataTypes;

using Empiria.Land.Data;
using Empiria.Land.Transactions;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of services to be provided in a transaction context.</summary>
  public class LRSTransactionServicesList : FixedList<LRSTransactionService> {

    #region Fields

    private static readonly decimal BASE_SALARY_VALUE = decimal.Parse(ConfigurationData.GetString("BaseSalaryValue"));

    private readonly LRSTransaction _transaction;

    private LRSFee totalFee = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSTransactionServicesList(LRSTransaction transaction) {
      _transaction = transaction;

      this.RecalculateTotalFee();
    }

    internal LRSTransactionServicesList(LRSTransaction transaction,
                                        IEnumerable<LRSTransactionService> services) : base(services) {
      _transaction = transaction;

      this.RecalculateTotalFee();
    }

    static internal LRSTransactionServicesList Parse(LRSTransaction transaction) {
      var services = TransactionData.GetTransactionServicesList(transaction);

      return new LRSTransactionServicesList(transaction, services);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSTransactionService this[int index] {
      get {
        return (LRSTransactionService) base[index];
      }
    }


    public decimal ComplexityIndex {
      get {
        return this.Sum(x => x.ComplexityIndex);
      }
    }


    public bool HasPayableServices {
      get {
        return PayableServices.Count != 0;
      }
    }


    public FixedList<LRSTransactionService> PayableServices {
      get {
        return base.FindAll(x => x.IsPayable);
      }
    }


    public LRSFee TotalFee {
      get {
        if (this.totalFee == null) {
          this.totalFee = LRSFee.Parse(this);
        }
        return this.totalFee;
      }
    }


    #endregion Public properties

    #region Public methods

    public LRSTransactionService Add(RecordingActType serviceType,
                                     LRSLawArticle treasuryCode, decimal recordingRights) {
      this.EnsureCanEditServices();

      var service = new LRSTransactionService(_transaction, serviceType, treasuryCode,
                                              Money.Zero, Quantity.One,
                                              new LRSFee() { RecordingRights = recordingRights });

      return this.ExecuteAddService(service);
    }


    public LRSTransactionService Add(RequestedServiceFields requestedService) {
      this.EnsureCanEditServices();

      var serviceType = RecordingActType.Parse(requestedService.ServiceUID);
      var treasuryCode = LRSLawArticle.Parse(requestedService.FeeConceptUID);
      var operationValue = Money.Parse(requestedService.TaxableBase);
      var quantity = Quantity.Parse(Unit.Parse(requestedService.UnitUID),
                                    requestedService.Quantity);

      var fee = new LRSFee {
        RecordingRights = requestedService.Subtotal
      };

      var service = new LRSTransactionService(_transaction, serviceType, treasuryCode,
                                              operationValue, quantity, fee);

      if (requestedService.Notes.Length != 0) {
        service.Notes = requestedService.Notes;
      }

      return this.ExecuteAddService(service);
    }


    public void AddPreconfiguredServicesIfApplicable() {
      if (!_transaction.ControlData.CanEditServices || this.Count > 0) {
        return;
      }

      foreach (var item in _transaction.DocumentType.DefaultRecordingActs) {
        this.Add(item, item.GetFinancialLawArticles()[0],
                 BASE_SALARY_VALUE * item.GetFeeUnits());
      }
    }


    public override void CopyTo(LRSTransactionService[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }


    public new void Remove(LRSTransactionService service) {
      EnsureCanEditServices();

      service.Delete();

      base.Remove(service);

      this.RecalculateTotalFee();
    }

    #endregion Public methods

    #region Helpers

    private void EnsureCanEditServices() {
      Assertion.Require(_transaction.ControlData.CanEditServices,
          "The transaction is in a status that doesn't permit aggregate new services or products," +
          "or the user doesn't have enough privileges.");

    }

    private LRSTransactionService ExecuteAddService(LRSTransactionService service) {

      service.Save();

      base.Add(service);

      this.RecalculateTotalFee();

      return service;
    }


    private void RecalculateTotalFee() {
      this.totalFee = LRSFee.Parse(this);
    }

    #endregion Helpers;

  } // class LRSTransactionServicesList

} // namespace Empiria.Land.Registration.Transactions
