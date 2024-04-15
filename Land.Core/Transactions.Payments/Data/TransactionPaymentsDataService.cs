/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Data Services Layer                     *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Services                           *
*  Type     : TransactionPaymentsDataService             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording office filings.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Payments.Data {

  /// <summary>Provides database read and write methods for recording office filings.</summary>
  static internal class TransactionPaymentsDataService {

    #region Public methods

    static internal FixedList<LRSPayment> GetTransactionPayments(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<LRSPayment>();
      }

      var operation = DataOperation.Parse("qryLRSTransactionPayments", transaction.Id);

      return DataReader.GetFixedList<LRSPayment>(operation);
    }


    static internal void WritePayment(LRSPayment o) {
      var op = DataOperation.Parse("writeLRSPayment", o.Id, o.Transaction.Id,
                                   o.PaymentOffice.Id, o.ReceiptNo, o.ReceiptTotal, o.ReceiptIssuedTime,
                                   o.ExtensionData.ToString(), o.PostingTime,
                                   o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class TransactionPaymentsDataService

} // namespace Empiria.Land.Transactions.Payments.Data
