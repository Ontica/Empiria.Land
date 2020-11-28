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
using System.Collections.Generic;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording office filings.</summary>
  static public class TransactionData {

    #region Public methods

    static internal bool ExistsExternalTransactionNo(string externalTransactionNo) {
      var sql = "SELECT * FROM LRSTransactions " +
                $"WHERE ExternalTransactionNo = '{externalTransactionNo}'";

      var operation = DataOperation.Parse(sql);

      return (DataReader.Count(operation) > 0);
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


    static public DataSet GetLRSTransactionWithKey(string transactionKey) {
      DataSet dataset = new DataSet("LRSTransaction");

      string sql = "SELECT * FROM vwLRSTransactionForWS " +
                   $"WHERE TransactionKey = '{transactionKey}'";

      DataTable table = DataReader.GetDataTable(DataOperation.Parse(sql), "Header");

      dataset.Tables.Add(table);
      int transactionId = 0;

      if (table.Rows.Count != 0) {
        transactionId = (int) table.Rows[0]["TransactionId"];
      }
      sql = "SELECT * FROM vwLRSTransactionItemsForWS " +
            $"WHERE TransactionId = {transactionId}";

      table = DataReader.GetDataTable(DataOperation.Parse(sql), "Items");

      dataset.Tables.Add(table);

      return dataset;
    }


    static public LRSTransactionItemList GetLRSTransactionItemsList(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionItems", transaction.Id);

      var list = DataReader.GetList<LRSTransactionItem>(operation,
                                                       (x) => BaseObject.ParseList<LRSTransactionItem>(x));

      return new LRSTransactionItemList(list);
    }


    static internal List<LRSPayment> GetLRSRecordingPayments(PhysicalRecording recording) {
      if (recording.IsEmptyInstance) {
        return new List<LRSPayment>();
      }

      var operation = DataOperation.Parse("qryLRSRecordingPayments", recording.Id);

      return DataReader.GetList<LRSPayment>(operation, (x) => BaseObject.ParseList<LRSPayment>(x));
    }


    static internal List<LRSPayment> GetLRSTransactionPayments(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new List<LRSPayment>();
      }

      var operation = DataOperation.Parse("qryLRSTransactionPayments", transaction.Id);

      return DataReader.GetList<LRSPayment>(operation, (x) => BaseObject.ParseList<LRSPayment>(x));
    }


    static internal string GenerateAssociationUID() {
      while (true) {
        string newAssociationUID = UIDGenerators.CreateAssociationUID();

        var checkIfExistAssociation = Resource.TryParseWithUID(newAssociationUID);

        if (checkIfExistAssociation == null) {
          return newAssociationUID;
        }
      }
    }


    static internal string GenerateNoPropertyResourceUID() {
      while (true) {
        string newNoPropertyUID = UIDGenerators.CreateNoPropertyResourceUID();

        var checkIfExistNoPropertyResource = Resource.TryParseWithUID(newNoPropertyUID);

        if (checkIfExistNoPropertyResource == null) {
          return newNoPropertyUID;
        }
      }
    }


    static internal string GeneratePropertyUID() {
      while (true) {
        string newPropertyUID = UIDGenerators.CreatePropertyUID();

        var checkIfExistProperty = Resource.TryParseWithUID(newPropertyUID);

        if (checkIfExistProperty == null) {
          return newPropertyUID;
        }
      }
    }


    static internal string GenerateTransactionUID() {
      while (true) {
        string newTransactionUID = UIDGenerators.CreateTransactionUID();

        var checkIfExistTransaction = LRSTransaction.TryParse(newTransactionUID);

        if (checkIfExistTransaction == null) {
          return newTransactionUID;
        }
      }
    }


    static internal int GetLastControlNumber(RecorderOffice recorderOffice) {
      string sql = "SELECT MAX(ControlNumber) FROM vwLRSTransactions " +
                   $"WHERE RecorderOfficeId = {recorderOffice.Id.ToString()}";

      string max = DataReader.GetScalar<String>(DataOperation.Parse(sql), String.Empty);

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 1;
      }
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
