/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Extension methods                  *
*  Type     : SearchTransactionCommandExtensions         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for transaction searching.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Command payload used for transaction searching.</summary>
  static internal class SearchTransactionCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this SearchTransactionCommand command) {
      command.Keywords = command.Keywords ?? String.Empty;
      command.OrderBy = command.OrderBy ?? "TransactionId DESC";
      command.PageSize = command.PageSize <= 0 ? 50 : command.PageSize;
      command.Page = command.Page <= 0 ? 1 : command.Page;
    }


    static internal string MapToFilterString(this SearchTransactionCommand command) {
      string stageStatusFilter = BuildStageStatusFilter(command.Stage, command.Status);
      string keywordsFilter = BuildKeywordsFilter(command.Keywords);

      var filter = new Filter(stageStatusFilter);

      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this SearchTransactionCommand command) {
      if (!String.IsNullOrWhiteSpace(command.OrderBy)) {
        return command.OrderBy;
      } else {
        return "TransactionId DESC";
      }
    }

    #endregion Extension methods

    #region Private methods

    static private string BuildKeywordsFilter(string keywords) {
      if (EmpiriaString.IsInteger(keywords)) {
        return $"(InternalControlNo = '{keywords}')";

      } else if (LRSTransaction.MatchesWithTransactionUID(keywords)) {
        return $"(TransactionUID = '{keywords}')";

      } else if (LRSTransaction.MatchesWithTransactionOldKey(keywords)) {
        keywords = LRSTransaction.GetTransactionUIDFromOldKey(keywords);

        return $"(TransactionUID = '{keywords}')";
      } else {

        return SearchExpression.ParseAndLikeKeywords("TransactionKeywords", keywords);
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
          throw Assertion.AssertNoReachThisCode();
      }
    }

    #endregion Private methods

  }  // class SearchTransactionCommandExtensions

}  // namespace Empiria.Land.Transactions.Adapters
