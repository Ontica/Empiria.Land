/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Type Extension methods                  *
*  Type     : SearchTransactionCommandExtensions         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for transaction searching.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Command payload used for transaction searching.</summary>
  static internal class SearchTransactionCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this SearchTransactionCommand command) {
      command.Keywords = command.Keywords ?? String.Empty;
      command.OrderBy = command.OrderBy ?? "PresentationTime";
      command.PageSize = command.PageSize <= 0 ? 50 : command.PageSize;
      command.Page = command.Page <= 0 ? 1 : command.Page;
    }


    static internal string MapToFilterString(this SearchTransactionCommand command) {
      string filter = String.Empty;

      filter = BuildStageStatusFilter(command.Stage, command.Status);

      if (!String.IsNullOrWhiteSpace(command.Keywords)) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += SearchExpression.ParseAndLikeKeywords("TransactionKeywords", command.Keywords);
      }

      return filter;
    }

    static internal string MapToSortString(this SearchTransactionCommand command) {
      if (!String.IsNullOrWhiteSpace(command.OrderBy)) {
        return command.OrderBy;
      } else {
        return "PresentationTime";
      }
    }

    #endregion Extension methods

    #region Private methods

    static private string BuildStageStatusFilter(TransactionStage stage, TransactionStatus status) {
      if (status != TransactionStatus.All) {
        return $"(TransactionStatus = '{(char) status}')";
      }

      if (stage != TransactionStage.All) {
        return GetStageTransactionStatusList(stage);
      }

      return GetStageTransactionStatusList(TransactionStage.All);
    }


    static private string GetStageTransactionStatusList(TransactionStage stage) {
      switch (stage) {
        case TransactionStage.All:
          return "TransactionStatus <> 'X'";

        case TransactionStage.Completed:
          return "TransactionStatus IN ('D', 'C', 'H')";

        case TransactionStage.InProgress:
          return "TransactionStatus IN ('R', 'N', 'K', 'G', 'E', 'J', 'P', 'S', 'A')";

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

  }  // class SearchTransactionCommand

}  // namespace Empiria.Land.Recording.UseCases
