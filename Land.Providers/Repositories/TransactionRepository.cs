/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Integration Layer                       *
*  Assembly : Empiria.Land.dll                           Pattern   : Provider implementation                 *
*  Type     : TransactionRepository                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read and write methods for transactions using a SQL-based database.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Providers {

  /// <summary>Provides data read and write methods for transactions using a SQL-based database.</summary>
  public class TransactionRepository : ITransactionRepository {

    #region Public methods

    public bool ExistsExternalTransactionNo(string externalTransactionNo) {
      var sql = "SELECT * " +
                "FROM LRSTransactions " +
               $"WHERE ExternalTransactionNo = '{externalTransactionNo}'";

      var operation = DataOperation.Parse(sql);

      return (DataReader.Count(operation) > 0);
    }


    public int GetLastControlNumber(RecorderOffice recorderOffice) {
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


    public FixedList<LRSTransaction> GetTransactionsList(string filter, string orderBy, int pageSize) {
      string sql = $"SELECT TOP {pageSize} * " +
                    "FROM LRSTransactions " +
                   $"WHERE {filter} ORDER BY {orderBy}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LRSTransaction>(operation);
    }


    public FixedList<LRSTransactionItem> GetTransactionItemsList(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionItems", transaction.Id);

      return DataReader.GetFixedList<LRSTransactionItem>(operation);
    }


    public FixedList<LRSPayment> GetTransactionPayments(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<LRSPayment>();
      }

      var operation = DataOperation.Parse("qryLRSTransactionPayments", transaction.Id);

      return DataReader.GetFixedList<LRSPayment>(operation);
    }

    #endregion Public methods

  } // class TransactionRepository

} // namespace Empiria.Land.Providers
