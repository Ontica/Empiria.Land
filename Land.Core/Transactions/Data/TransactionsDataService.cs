/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                      Component : Data Services Layer                   *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Data Services                         *
*  Type     : TransactionsDataService                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording office transactions.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Transactions.Data {

  /// <summary>Provides database read and write methods for recording office transactions.</summary>
  static internal class TransactionsDataService {

    #region Public methods

    static internal bool ExistsExternalTransactionNo(string externalTransactionNo) {
      var sql = "SELECT * " +
                "FROM LRSTransactions " +
               $"WHERE ExternalTransactionNo = '{externalTransactionNo}'";

      var operation = DataOperation.Parse(sql);

      return (DataReader.Count(operation) > 0);
    }


    static internal int GetLastControlNumber(RecorderOffice recorderOffice) {
      string sql = "SELECT MAX(InternalControlNo) " +
                   "FROM LRSTransactions " +
                   $"WHERE RecorderOfficeId = {recorderOffice.Id}";

      string max = DataReader.GetScalar(DataOperation.Parse(sql), String.Empty);

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 1;
      }
    }


    static internal FixedList<LRSTransaction> GetTransactionsList(string filter, string orderBy, int pageSize) {
      string sql = $"SELECT TOP {pageSize} LRSTransactions.* " +
                    "FROM LRSTransactions INNER JOIN vwLRSLastTransactionTrack " +
                    "ON LRSTransactions.TransactionId = vwLRSLastTransactionTrack.TransactionId " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    static internal FixedList<LRSTransactionService> GetTransactionServicesList(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionItems", transaction.Id);

      return DataReader.GetFixedList<LRSTransactionService>(operation);
    }


    static internal void SetTransactionInstrument(LRSTransaction transaction, IIdentifiable instrument) {
      var sql = $"UPDATE LRSTransactions " +
                $"SET InstrumentId = {instrument.Id} " +
                $"WHERE TransactionId = {transaction.Id}";

      var op = DataOperation.Parse(sql);

      DataWriter.Execute(op);
    }


    static internal void WriteTransaction(LRSTransaction o) {
      var op = DataOperation.Parse("writeLRSTransaction", o.Id, o.TransactionType.Id, o.GUID, o.UID,
                  o.DocumentType.Id, o.DocumentDescriptor, o.LandRecord.Id, o.BaseResource.Id,
                  o.RecorderOffice.Id, o.RequestedBy, o.Agency.Id,
                  o.ExternalTransactionNo, o.InternalControlNumber, o.ExtensionData.ToString(),
                  o.Keywords, o.PresentationTime, o.ExpectedDelivery, o.LastReentryTime, o.ClosingTime,
                  o.LastDeliveryTime, o.NonWorkingTime, o.Services.ComplexityIndex, o.IsArchived,
                  (char) o.Workflow.CurrentStatus, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteTransactionService(LRSTransactionService o) {
      Assertion.Require(o.Fee, "o.Fee");
      Assertion.Require(o.Payment, "o.Payment");

      var op = DataOperation.Parse("writeLRSTransactionItem", o.Id, o.UID, o.Transaction.Id,
                                    o.ServiceType.Id, o.TreasuryCode.Id,
                                    o.Payment.Id, o.Quantity.Amount, o.Quantity.Unit.Id,
                                    o.OperationValue.Amount, o.OperationValue.Currency.Id,
                                    o.Fee.RecordingRights, o.Fee.SheetsRevision,
                                    o.Fee.ForeignRecordingFee, o.Fee.Discount.Amount,
                                    o.Notes, string.Empty, o.Status,
                                    o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class TransactionsDataService

} // namespace Empiria.Land.Transactions.Data
