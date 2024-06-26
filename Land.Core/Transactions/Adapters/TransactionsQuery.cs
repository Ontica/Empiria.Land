﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Query payload                           *
*  Type     : TransactionsQuery                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used for transactions searching.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Query payload used for transactions searching.</summary>
  public class TransactionsQuery {

    public string RecorderOfficeUID {
      get;
      set;
    } = string.Empty;


    public TransactionStage Stage {
      get;
      set;
    } = TransactionStage.All;


    public TransactionStatus Status {
      get;
      set;
    } = TransactionStatus.All;


    public string Keywords {
      get;
      set;
    } = string.Empty;


    public string OrderBy {
      get;
      set;
    } = String.Empty;


    public int PageSize {
      get;
      set;
    } = 50;


    public int Page {
      get;
      set;
    } = 1;

  }  // class TransactionQuery



  /// <summary>Extension methods for TransactionsQuery class.</summary>
  static internal class TransactionsQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this TransactionsQuery query) {
      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? "InternalControlNo DESC";
      query.PageSize = query.PageSize <= 0 ? 50 : query.PageSize;
      query.Page = query.Page <= 0 ? 1 : query.Page;
    }

    static internal string MapToFilterString(this TransactionsQuery query) {
      string recorderOfficeFilter = BuildRecorderOfficeFilter(query.RecorderOfficeUID);
      string stageStatusFilter = BuildStageStatusFilter(query.Stage, query.Status);
      string keywordsFilter = BuildKeywordsFilter(query.Keywords);

      var filter = new Filter(recorderOfficeFilter);
      filter.AppendAnd(stageStatusFilter);
      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this TransactionsQuery query) {
      if (!String.IsNullOrWhiteSpace(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "TransactionId DESC";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildKeywordsFilter(string keywords) {
      if (EmpiriaString.IsInteger(keywords)) {
        return $"(InternalControlNo = '{int.Parse(keywords):000000}')";

      } else if (LRSTransaction.MatchesWithTransactionUID(keywords)) {
        return $"(TransactionUID = '{keywords}')";

      } else if (LRSTransaction.MatchesWithTransactionOldKey(keywords)) {
        keywords = LRSTransaction.GetTransactionUIDFromOldKey(keywords);

        return $"(TransactionUID = '{keywords}')";
      } else {

        return SearchExpression.ParseAndLikeKeywords("TransactionKeywords", keywords);
      }
    }


    static private string BuildRecorderOfficeFilter(string recorderOfficeUID) {
      RecorderOffice recorderOffice;

      if (!string.IsNullOrWhiteSpace(recorderOfficeUID)) {
        recorderOffice = RecorderOffice.Parse(recorderOfficeUID);
      } else {
        recorderOffice = RecorderOffice.Empty;
      }

      bool hasPermission = Permissions.HasPermission(recorderOffice);

      if (!recorderOffice.IsEmptyInstance && hasPermission) {
        return $"(RecorderOfficeId = {recorderOffice.Id})";
      } else if (!recorderOffice.IsEmptyInstance && !hasPermission) {
        return SearchExpression.NoRecordsFilter;
      } else if (recorderOffice.IsEmptyInstance) {
        return $"(RecorderOfficeId = {Permissions.GetUserDefaultRecorderOffice().Id})";
      } else {
        throw Assertion.EnsureNoReachThisCode("Unhandled condition building recorder office filter.");
      }
    }


    static private string BuildStageStatusFilter(TransactionStage stage, TransactionStatus status) {
      if (status != TransactionStatus.All) {
        return $"(TransactionStatus = '{(char) status}')";
      }

      if (stage != TransactionStage.All) {
        return GetStageTransactionStatusListFilter(stage);
      }

      return GetStageTransactionStatusListFilter(TransactionStage.All);
    }


    static private string GetStageTransactionStatusListFilter(TransactionStage stage) {
      switch (stage) {
        case TransactionStage.All:
          return "TransactionStatus <> 'X'";

        case TransactionStage.MyInbox:
          return $"ResponsibleId = {ExecutionServer.CurrentUserId} AND " +
                 $"TransactionStatus IN ('G', 'E', 'S', 'V', 'P', 'J')";

        case TransactionStage.Completed:
          return "TransactionStatus IN ('D', 'C', 'H', 'L', 'Q')";

        case TransactionStage.ControlDesk:
          return "TransactionStatus IN ('K', 'R', 'N')";

        case TransactionStage.InProgress:
          return "TransactionStatus IN ('G', 'E', 'V', 'P')";

        case TransactionStage.OnHold:
          return "TransactionStatus IN ('V', 'J')";

        case TransactionStage.Pending:
          return "TransactionStatus IN ('Y')";

        case TransactionStage.Returned:
          return "TransactionStatus IN ('L', 'Q')";

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }

    #endregion Helpers

  }  // class TransactionsQueryExtensions

}  // namespace Empiria.Land.Transactions.Adapters
