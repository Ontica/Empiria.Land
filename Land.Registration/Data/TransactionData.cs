/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing                                       Component : Filing Data Services                  *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data Services                         *
*  Type     : TransactionData                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording office filings.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording office filings.</summary>
  static public class TransactionData {

    #region Public methods

    static public DataView GetLRSTransactionsForUI(string filter, string sort) {
      string sql = "SELECT TOP 200 * FROM vwLRSTransactionsAndCurrentTrack";
      if (filter.Length != 0 && sort.Length != 0) {
        sql += " WHERE " + filter + " ORDER BY " + sort;
      } else if (filter.Length != 0 && sort.Length == 0) {
        sql += " WHERE " + filter;
      } else if (filter.Length == 0 && sort.Length != 0) {
        sql += " ORDER BY " + sort;
      } else if (filter.Length == 0 && sort.Length == 0) {
        // no-op
      }
      return DataReader.GetDataView(DataOperation.Parse(sql));
    }


    static internal void WritePayment(LRSPayment o) {
      var op = DataOperation.Parse("writeLRSPayment", o.Id, o.Transaction.Id,
                                   o.PaymentOffice.Id, o.ReceiptNo, o.ReceiptTotal, o.ReceiptIssuedTime,
                                   o.ExtensionData.ToString(), o.Recording.Id, o.PostingTime,
                                   o.PostedBy.Id, 'C', String.Empty);

      DataWriter.Execute(op);
    }


    static internal void WriteTransaction(LRSTransaction o) {
      var op = DataOperation.Parse("writeLRSTransaction", o.Id, o.TransactionType.Id, o.UID,
                  o.DocumentType.Id, o.DocumentDescriptor, o.Document.Id, o.BaseResource.Id,
                  o.RecorderOffice.Id, o.RequestedBy, o.Agency.Id,
                  o.ExternalTransactionNo, o.ExtensionData.ToString(),
                  o.Keywords, o.PresentationTime, o.ExpectedDelivery, o.LastReentryTime, o.ClosingTime,
                  o.LastDeliveryTime, o.NonWorkingTime, o.ComplexityIndex, o.IsArchived,
                  (char) o.Workflow.CurrentStatus, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteTransactionItem(LRSTransactionItem o) {
      var op = DataOperation.Parse("writeLRSTransactionItem", o.Id, o.Transaction.Id,
                                    o.TransactionItemType.Id, o.TreasuryCode.Id,
                                    o.Payment.Id, o.Quantity.Amount, o.Quantity.Unit.Id,
                                    o.OperationValue.Amount, o.OperationValue.Currency.Id,
                                    o.Fee.RecordingRights, o.Fee.SheetsRevision,
                                    o.Fee.ForeignRecordingFee, o.Fee.Discount.Amount,
                                    o.ExtensionData.ToString(), o.Status,
                                    o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class TransactionData

} // namespace Empiria.Land.Data
