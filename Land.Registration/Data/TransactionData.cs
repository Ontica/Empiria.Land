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

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording office filings.</summary>
  static public class TransactionData {

    #region Public methods

    static internal bool ExistsExternalTransactionNo(string externalTransactionNo) {
      var sql = "SELECT * " +
                "FROM LRSTransactions " +
               $"WHERE ExternalTransactionNo = '{externalTransactionNo}'";

      var operation = DataOperation.Parse(sql);

      return (DataReader.Count(operation) > 0);
    }


    static internal int GetLastControlNumber(RecorderOffice recorderOffice) {
      string sql = "SELECT MAX(ControlNumber) " +
                   "FROM vwLRSTransactions " +
                   $"WHERE RecorderOfficeId = {recorderOffice.Id}";

      string max = DataReader.GetScalar(DataOperation.Parse(sql), String.Empty);

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 1;
      }
    }


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


    static internal string GetTransactionInstrumentUID(LRSTransaction transaction) {
      var sql = $"SELECT InstrumentUID " +
                $"FROM LRSInstruments " +
                $"WHERE (InstrumentId = {transaction.InstrumentId})";

      var op = DataOperation.Parse(sql);

      return DataReader.GetScalar<string>(op, "Empty");
    }


    static internal FixedList<LRSTransaction> GetTransactionsList(string filter, string orderBy, int pageSize) {
      string sql = $"SELECT TOP {pageSize} * " +
                    "FROM LRSTransactions " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    static internal FixedList<LRSTransactionItem> GetTransactionItemsList(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionItems", transaction.Id);

      return DataReader.GetFixedList<LRSTransactionItem>(operation);
    }


    static internal FixedList<LRSPayment> GetTransactionPayments(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<LRSPayment>();
      }

      var operation = DataOperation.Parse("qryLRSTransactionPayments", transaction.Id);

      return DataReader.GetFixedList<LRSPayment>(operation);
    }

    static internal void SetTransactionInstrument(LRSTransaction transaction, IIdentifiable instrument) {
      var sql = $"UPDATE LRSTransactions SET InstrumentId = {instrument.Id} WHERE TransactionId = {transaction.Id}";

      var op = DataOperation.Parse(sql);

      DataWriter.Execute(op);
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
