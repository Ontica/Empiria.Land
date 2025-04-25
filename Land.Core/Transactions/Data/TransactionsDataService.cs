/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                      Component : Data Services Layer                   *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Data Services                         *
*  Type     : TransactionsDataService                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for land transactions.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Transactions.Data {

  /// <summary>Provides database read and write methods for land transactions.</summary>
  static internal class TransactionsDataService {

    #region Methods

    static internal bool ExistsExternalTransactionNo(string externalTransactionNo) {

      var sql = "SELECT * FROM LRSTransactions " +
               $"WHERE ExternalTransactionNo = '{externalTransactionNo}'";

      var op = DataOperation.Parse(sql);

      return DataReader.Count(op) > 0;
    }


    static internal int GetLastControlNumber(RecorderOffice recorderOffice) {

      string sql = "SELECT MAX(InternalControlNo) FROM LRSTransactions " +
                   $"WHERE RecorderOfficeId = {recorderOffice.Id}";

      var op = DataOperation.Parse(sql);

      string max = DataReader.GetScalar(op, string.Empty);

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 0;
      }
    }


    static internal FixedList<LRSTransaction> GetTransactionsList(string filter, string orderBy, int pageSize) {

      string sql = $"SELECT TOP {pageSize} LRSTransactions.* " +
                    "FROM LRSTransactions INNER JOIN vwLRSLastTransactionTrack " +
                    "ON LRSTransactions.TransactionId = vwLRSLastTransactionTrack.TransactionId " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(op);
    }


    static internal FixedList<LRSTransactionService> GetTransactionServicesList(LRSTransaction transaction) {

      var sql = "SELECT * FROM LRSTransactionItems " +
               $"WHERE TransactionId = {transaction.Id} AND TransactionItemStatus <> 'X' " +
                "ORDER BY TransactionItemId";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransactionService>(op);
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

      Assertion.Require(o.Fee, nameof(o.Fee));
      Assertion.Require(o.Payment, nameof(o.Payment));

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
