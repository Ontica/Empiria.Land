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

using Empiria.Land.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of services to be provided in a transaction context.</summary>
  public class LRSTransactionServicesList : FixedList<LRSTransactionService> {

    #region Fields

    private LRSFee totalFee = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSTransactionServicesList() {
      this.CalculateTotals();
    }

    internal LRSTransactionServicesList(IEnumerable<LRSTransactionService> list) : base(list) {
      this.CalculateTotals();
    }

    static internal LRSTransactionServicesList Parse(LRSTransaction transaction) {
      var services = TransactionData.GetTransactionServicesList(transaction);

      return new LRSTransactionServicesList(services);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSTransactionService this[int index] {
      get {
        return (LRSTransactionService) base[index];
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

    protected internal new void Add(LRSTransactionService service) {
      base.Add(service);
      this.CalculateTotals();
    }

    public bool ContainsPayableItems() {
      return (base.Find(x => x.IsPayable) != null);
    }


    public FixedList<LRSTransactionService> PayableItems {
      get {
        return base.FindAll(x => x.IsPayable);
      }
    }

    public override void CopyTo(LRSTransactionService[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public new LRSTransactionService Find(Predicate<LRSTransactionService> match) {
      return base.Find(match);
    }

    protected internal new bool Remove(LRSTransactionService service) {
      bool result = base.Remove(service);

      this.CalculateTotals();

      return result;
    }

    public new void Sort(Comparison<LRSTransactionService> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

    #region Private methods

    private void CalculateTotals() {
      this.totalFee = LRSFee.Parse(this);
    }

    #endregion Private methods;

  } // class LRSTransactionServicesList

} // namespace Empiria.Land.Registration.Transactions
