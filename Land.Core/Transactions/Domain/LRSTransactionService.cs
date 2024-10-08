﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                      Component : Domain Layer                          *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Information Holder                    *
*  Type     : LRSTransactionService                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents a service provided in the context of a land registration transaction.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Financial;
using Empiria.Measurement;
using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Transactions.Payments;
using Empiria.Land.Transactions.Data;


namespace Empiria.Land.Transactions {

  /// <summary>Represents a service provided in the context of a land registration transaction.</summary>
  public class LRSTransactionService : BaseObject, IProtected {

    #region Constructors and parsers

    private LRSTransactionService() {
      // Required by Empiria Framework.
    }

    internal LRSTransactionService(LRSTransaction transaction, RecordingActType serviceType,
                                   LRSLawArticle treasuryCode, Money operationValue,
                                   Quantity quantity, LRSFee fee) {
      this.Transaction = transaction;
      this.ServiceType = serviceType;
      this.TreasuryCode = treasuryCode;
      this.Quantity = quantity;
      this.OperationValue = operationValue;
      this.Fee = fee;
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TransactionId")]
    LazyInstance<LRSTransaction> _transaction = LazyInstance<LRSTransaction>.Empty;
    public LRSTransaction Transaction {
      get { return _transaction.Value; }
      private set {
        _transaction = LazyInstance<LRSTransaction>.Parse(value);
      }
    }

    [DataField("TransactionItemTypeId")]
    public RecordingActType ServiceType {
      get;
      private set;
    }

    [DataField("TreasuryCodeId")]
    public LRSLawArticle TreasuryCode {
      get;
      private set;
    }

    [DataField("PaymentId")]
    LazyInstance<LRSPayment> _payment = LazyInstance<LRSPayment>.Empty;
    public LRSPayment Payment {
      get { return _payment.Value; }
      private set {
        _payment = LazyInstance<LRSPayment>.Parse(value);
      }
    }

    public Quantity Quantity {
      get;
      private set;
    }

    public Money OperationValue {
      get;
      private set;
    }

    public LRSFee Fee {
      get;
      private set;
    }


    [DataField("Notes")]
    public string Notes {
      get;
      internal set;
    }

    [DataField("TransactionItemStatus", Default = 'A')]
    public char Status {
      get;
      private set;
    }

    public decimal ComplexityIndex {
      get {
        if (this.Quantity.Amount == decimal.Zero) {
          return decimal.One;
        } else {
          return this.Quantity.Amount;
        }
      }
    }


    public bool IsNotPayable {
      get {
        if (this.TreasuryCode.IsEmptyInstance) {
          return true;
        }
        if (String.IsNullOrWhiteSpace(this.TreasuryCode.FinancialConceptCode)) {
          return true;
        }
        return false;
      }
    }


    public bool IsPayable {
      get {
        return !this.IsNotPayable;
      }
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionId", this.Transaction.Id,
          "TransactionItemTypeId", this.ServiceType.Id,
          "TreasuryCodeId", this.TreasuryCode.Id, "PaymentId", this.Payment.Id,
          "Qty", this.Quantity.Amount, "QtyUnitId", this.Quantity.Unit.Id,
          "OpValue", this.OperationValue.Amount, "OpValueCurrencyId", this.OperationValue.Currency.Id,
          "RecordingRightsFee", this.Fee.RecordingRights, "SheetsRevisionFee", this.Fee.SheetsRevision,
          "ForeignFee", this.Fee.ForeignRecordingFee, "Discount", this.Fee.Discount.Amount,
          "Status", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = 'X';
      this.Save();
    }

    internal LRSTransactionService MakeCopy() {
      return new LRSTransactionService {
        ServiceType = this.ServiceType,
        TreasuryCode = this.TreasuryCode,
        OperationValue = this.OperationValue,
        Quantity = this.Quantity,
        Notes = this.Notes,
        Fee = this.Fee,
    };
  }

    protected override void OnLoadObjectData(DataRow row) {
      this.Quantity = Quantity.Parse(Unit.Parse((int) row["UnitId"]), (decimal) row["Quantity"]);
      this.OperationValue = Money.Parse(Currency.Parse((int) row["OperationValueCurrencyId"]),
                                        (decimal) row["OperationValue"]);
      this.Fee = LRSFee.Parse(row);
    }

    protected override void OnSave() {
      TransactionsDataService.WriteTransactionService(this);
    }

    #endregion Public methods

  } // class LRSTransactionService

} // namespace Empiria.Land.Transactions
