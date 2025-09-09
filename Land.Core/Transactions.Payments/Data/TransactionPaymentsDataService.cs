/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Data Services Layer                     *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Services                           *
*  Type     : TransactionPaymentsDataService             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides database read and write methods for land transaction payments.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

namespace Empiria.Land.Transactions.Payments.Data {

  /// <summary>Provides database read and write methods for land transaction payments.</summary>
  static internal class TransactionPaymentsDataService {

    #region Methods

    static internal FixedList<LRSPayment> GetTransactionPayments(LRSTransaction transaction) {

      if (transaction.IsEmptyInstance) {
        return new FixedList<LRSPayment>();
      }

      var sql = "SELECT * FROM LRSPayments " +
               $"WHERE TransactionId = {transaction.Id} AND PaymentStatus <> 'X' " +
               $"ORDER BY ReceiptIssuedTime";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSPayment>(op);
    }


    static internal LRSPayment TryGetPayment(string receiptNo) {
      Assertion.Require(receiptNo, nameof(receiptNo));


      var sql = "SELECT * FROM LRSPayments " +
               $"WHERE ReceiptNo = '{receiptNo}' AND PaymentStatus <> 'X' " +
               $"ORDER BY ReceiptIssuedTime DESC";

      var op = DataOperation.Parse(sql);

      var list = DataReader.GetFixedList<LRSPayment>(op)
                           .FindAll(x => x.Transaction.InternalControlNumber.Length != 0);

      if (list.Count != 0) {
        return list[0];
      }

      return null;
    }


    static internal void WritePayment(LRSPayment o) {
      var op = DataOperation.Parse("writeLRSPayment", o.Id, o.Transaction.Id,
                                   o.PaymentOffice.Id, o.ReceiptNo, o.ReceiptTotal, o.ReceiptIssuedTime,
                                   o.ExtensionData.ToString(), o.PostingTime,
                                   o.PostedBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Methods

  } // class TransactionPaymentsDataService

} // namespace Empiria.Land.Transactions.Payments.Data
