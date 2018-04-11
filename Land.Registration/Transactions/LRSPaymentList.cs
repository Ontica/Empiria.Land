/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPaymentList                                 Pattern  : Empiria List Class                  *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of LRSPayment instances. Can hold one or heterogeneous transaction payments.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Land.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>List of LRSPayment instances. Can hold one or heterogeneous transaction payments.</summary>
  public class LRSPaymentList : FixedList<LRSPayment> {

    #region Fields

    private string receiptNumbers = null;
    private decimal total = -1m;

    #endregion Fields

    #region Constructors and parsers

    internal LRSPaymentList() {
      this.CalculateTotals();
    }

    internal LRSPaymentList(List<LRSPayment> list) : base(list) {
      this.CalculateTotals();
    }

    static internal LRSPaymentList Parse(LRSTransaction transaction) {
      List<LRSPayment> list = TransactionData.GetLRSTransactionPayments(transaction);

      return new LRSPaymentList(list);
    }

    static internal LRSPaymentList Parse(Recording recording) {
      List<LRSPayment> list = TransactionData.GetLRSRecordingPayments(recording);

      return new LRSPaymentList(list);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSPayment this[int index] {
      get {
        return (LRSPayment) base[index];
      }
    }

    public string ReceiptNumbers {
      get {
        if (receiptNumbers == null) {
          this.CalculateTotals();
        }
        return receiptNumbers;
      }
    }

    public decimal Total {
      get {
        if (total == -1m) {
          this.CalculateTotals();
        }
        return total;
      }
    }

    #endregion Public properties

    #region Public methods

    protected internal new void Add(LRSPayment item) {
      base.Add(item);
      this.CalculateTotals();
    }

    public new bool Contains(LRSPayment item) {
      return base.Contains(item);
    }

    public new bool Contains(Predicate<LRSPayment> match) {
      return (base.Find(match) != null);
    }

    public override void CopyTo(LRSPayment[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    public new LRSPayment Find(Predicate<LRSPayment> match) {
      return base.Find(match);
    }

    public new FixedList<LRSPayment> FindAll(Predicate<LRSPayment> match) {
      return base.FindAll(match);
    }

    protected internal new bool Remove(LRSPayment item) {
      bool result = base.Remove(item);

      this.CalculateTotals();

      return result;
    }

    public new void Sort(Comparison<LRSPayment> comparison) {
      base.Sort(comparison);
    }

    #endregion Public methods

    #region Private methods

    private void CalculateTotals() {
      total = 0;
      receiptNumbers = String.Empty;
      for (int i = 0; i < this.Count; i++) {
        total += this[i].ReceiptTotal;
        if (receiptNumbers.Length == 0) {
          receiptNumbers = this[i].ReceiptNo;
        } else {
          receiptNumbers = this[0].ReceiptNo + " (+ " + i.ToString() + ")";
        }
      }
    }

    #endregion Private methods;

  } // class LRSPaymentList

} // namespace Empiria.Land.Registration.Transactions
