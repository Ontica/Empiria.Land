/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Data Layer                              *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Service                            *
*  Type     : ESignDataService                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read and write methods for land electronic sign of documents.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.Land.Transactions;

using Empiria.Land.ESign.Adapters;

namespace Empiria.Land.ESign.Data {

  /// <summary>Provides data read methods for ESign.</summary>
  static internal class ESignDataService {

    static internal FixedList<SignableDocument> GetESignRequestedDocuments(string filter, string orderBy,
                                                                           int pageSize) {
      string sql = $"SELECT TOP {pageSize} vwLRSESignableDocuments.* " +
                    "FROM LRSTransactions INNER JOIN vwLRSESignableDocuments " +
                    "ON LRSTransactions.TransactionId = vwLRSESignableDocuments.TransactionId " +
                    "INNER JOIN vwLRSLastTransactionTrack " +
                    "ON LRSTransactions.TransactionId = vwLRSLastTransactionTrack.TransactionId " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<SignableDocument>(op);
    }


    static internal FixedList<LRSTransaction> GetESignRequestedTransactions(string filter, string orderBy,
                                                                            int pageSize) {
      string sql = $"SELECT DISTINCT TOP {pageSize} LRSTransactions.* " +
                    "FROM LRSTransactions INNER JOIN vwLRSESignableDocuments " +
                    "ON LRSTransactions.TransactionId = vwLRSESignableDocuments.TransactionId " +
                    "INNER JOIN vwLRSLastTransactionTrack " +
                    "ON LRSTransactions.TransactionId = vwLRSLastTransactionTrack.TransactionId " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(op);
    }

  } // class ESignDataService

} // namespace Empiria.Land.ESign.Data
