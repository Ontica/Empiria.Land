/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics                                    Component : Transaction Analytics                 *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data Services                         *
*  Type     : TransactionsAnalyticsData                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides data methods for transaction analytics.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Analytics {

  /// <summary>Provides data methods for transaction analytics.</summary>
  static public class TransactionsAnalyticsData {

    #region Public methods

    static public DataView PendingTransactionsByResponsible(DateTime fromDate, DateTime toDate) {

      DataOperation dataOperation = DataOperation.Parse("rptLRSTransactionsInProcess", fromDate, toDate);

      return DataReader.GetDataView(dataOperation);
    }

    static public DataView PendingTransactionsByStatus(DateTime fromDate, DateTime toDate) {

      DataOperation dataOperation = DataOperation.Parse("rptLRSTransactionsInProcess", fromDate, toDate);

      return DataReader.GetDataView(dataOperation);
    }


    static public DataView PendingTransactionsByResponsibleAndStatus(DateTime fromDate, DateTime toDate) {

      DataOperation dataOperation = DataOperation.Parse("rptLRSTransactionsInProcess", fromDate, toDate);

      return DataReader.GetDataView(dataOperation);
    }


    #endregion Public methods

  } // class TransactionsAnalyticsData

} // namespace Empiria.Land.Analytics
