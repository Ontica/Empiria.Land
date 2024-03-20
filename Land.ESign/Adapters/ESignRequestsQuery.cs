/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Query payload                           *
*  Type     : ESignRequestsQuery                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used for electronic signs requests.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Query payload used for electronic signs requests.</summary>
  public class ESignRequestsQuery {

    internal RecorderOffice RecorderOffice {
      get;
      set;
    } = RecorderOffice.Empty;


    internal SignStatus Status {
      get;
      set;
    } = SignStatus.Unsigned;


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

  }  // class ESignRequestsQuery



  /// <summary>Extension methods for ESignRequestsQuery class.</summary>
  static internal class ESignRequestsQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this ESignRequestsQuery query) {
      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? "TransactionId DESC";
      query.PageSize = query.PageSize <= 0 ? 50 : query.PageSize;
      query.Page = query.Page <= 0 ? 1 : query.Page;
      query.RecorderOffice = GetRecorderOffice(query.RecorderOffice);
    }


    static internal string MapToFilterString(this ESignRequestsQuery query) {
      string recorderOfficeFilter = BuildRecorderOfficeFilter(query.RecorderOffice);
      string eSignStatusFilter = BuildESignStatusFilter(query.Status);
      string transactionKeywordsFilter = BuildTransactionKeywordsFilter(query.Keywords);

      var filter = new Filter(recorderOfficeFilter);
      filter.AppendAnd(eSignStatusFilter);
      filter.AppendAnd(transactionKeywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this ESignRequestsQuery query) {
      if (!String.IsNullOrWhiteSpace(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "TransactionId DESC";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildTransactionKeywordsFilter(string keywords) {
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


    static private string BuildRecorderOfficeFilter(RecorderOffice recorderOffice) {
      if (recorderOffice.IsEmptyInstance) {
        return string.Empty;
      }

      return $"(RecorderOfficeId = {recorderOffice.Id})";
    }


    static private string BuildESignStatusFilter(SignStatus status) {
      return $"(SignStatus = '{(char) status}')";
    }


    static private RecorderOffice GetRecorderOffice(RecorderOffice recorderOffice) {
      if (!recorderOffice.IsEmptyInstance) {
        return recorderOffice;
      }
      try {
        return Permissions.GetUserRecorderOffice();

      } catch {
        return RecorderOffice.Empty;
      }
    }

    #endregion Helpers

  }  // class ESignRequestsQueryExtensions

}  // namespace Empiria.Land.Transactions
