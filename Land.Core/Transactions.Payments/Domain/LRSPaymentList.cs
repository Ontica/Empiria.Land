/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data holder                             *
*  Type     : LRSPaymentList                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : List of LRSPayment instances.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Transactions.Payments.Data;

namespace Empiria.Land.Transactions.Payments {

  /// <summary>List of LRSPayment instances.</summary>
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
      FixedList<LRSPayment> list = TransactionPaymentsDataService.GetTransactionPayments(transaction);

      return new LRSPaymentList(list);
    }

    #endregion Constructors and parsers

    #region Properties

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

    #endregion Properties

    #region Methods

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

    #endregion Methods

    #region Helpers

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

    #endregion Helpers

  } // class LRSPaymentList

} // namespace Empiria.Land.Transactions.Payments
