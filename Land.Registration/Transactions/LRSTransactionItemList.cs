/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionItemList                         Pattern  : Empiria List Class                  *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of transaction items or lines. Can be used to hold lines of one or many transactions.    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of transaction items or lines. Can be used to hold
  /// lines of one or many transactions.</summary>
  public class LRSTransactionItemList : FixedList<LRSTransactionItem> {

    #region Fields

    private LRSFee totalFee = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSTransactionItemList() {
      this.CalculateTotals();
    }

    internal LRSTransactionItemList(List<LRSTransactionItem> list) : base(list) {
      this.CalculateTotals();
    }

    static internal LRSTransactionItemList Parse(LRSTransaction transaction) {
      return TransactionData.GetLRSTransactionItemsList(transaction);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSTransactionItem this[int index] {
      get {
        return (LRSTransactionItem) base[index];
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

    protected internal new void Add(LRSTransactionItem item) {
      base.Add(item);
      this.CalculateTotals();
    }

    public new bool Contains(LRSTransactionItem item) {
      return base.Contains(item);
    }

    public bool Contains(RecordingActType item) {
      return base.Contains((x) => x.TransactionItemType.Equals(item));
    }

    public new bool Contains(Predicate<LRSTransactionItem> match) {
      return (base.Find(match) != null);
    }

    public override void CopyTo(LRSTransactionItem[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public LRSTransactionItem Find(RecordingActType item) {
      return base.Find((x) => x.TransactionItemType.Equals(item));
    }

    public new LRSTransactionItem Find(Predicate<LRSTransactionItem> match) {
      return base.Find(match);
    }

    public new FixedList<LRSTransactionItem> FindAll(Predicate<LRSTransactionItem> match) {
      return base.FindAll(match);
    }

    protected internal new bool Remove(LRSTransactionItem item) {
      bool result = base.Remove(item);

      this.CalculateTotals();

      return result;
    }

    public new void Sort(Comparison<LRSTransactionItem> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

    #region Private methods

    private void CalculateTotals() {
      this.totalFee = LRSFee.Parse(this);
    }

    #endregion Private methods;

  } // class LRSTransactionItemList

} // namespace Empiria.Land.Registration.Transactions
