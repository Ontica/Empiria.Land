/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPaymentList                                 Pattern  : Empiria List Class                  *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : List of LRSPayment instances. Can hold one or heterogeneous transaction payments.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
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
      this.CalculateTotalsAndReceiptNumbers();
    }

    internal LRSPaymentList(IEnumerable<LRSPayment> list) : base(list) {
      this.CalculateTotalsAndReceiptNumbers();
    }

    static internal LRSPaymentList Parse(LRSTransaction transaction) {
      FixedList<LRSPayment> list = TransactionData.GetTransactionPayments(transaction);

      return new LRSPaymentList(list);
    }

    #endregion Constructors and parsers

    #region Public properties

    public override LRSPayment this[int index] {
      get {
        return base[index];
      }
    }

    public string ReceiptNumbers {
      get {
        if (receiptNumbers == null) {
          this.CalculateTotalsAndReceiptNumbers();
        }
        return receiptNumbers;
      }
    }

    public decimal Total {
      get {
        if (total == -1m) {
          this.CalculateTotalsAndReceiptNumbers();
        }
        return total;
      }
    }

    #endregion Public properties

    #region Public methods

    protected internal new void Add(LRSPayment item) {
      base.Add(item);

      this.CalculateTotalsAndReceiptNumbers();
    }

    public override void CopyTo(LRSPayment[] array, int index) {
      for (int i = index, j = Count; i < j; i++) {
        array.SetValue(base[i], i);
      }
    }

    protected internal new bool Remove(LRSPayment item) {
      bool result = base.Remove(item);

      this.CalculateTotalsAndReceiptNumbers();

      return result;
    }

    #endregion Public methods

    #region Private methods

    private void CalculateTotalsAndReceiptNumbers() {
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
