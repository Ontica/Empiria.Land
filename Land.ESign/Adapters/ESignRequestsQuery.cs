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

using Empiria.Contacts;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Query payload used for electronic signs requests.</summary>
  public class ESignRequestsQuery {

    internal RecorderOffice RecorderOffice {
      get;
      set;
    } = RecorderOffice.Empty;


    internal Contact SignedBy {
      get;
      set;
    } = Contact.Empty;


    public SignStatus Status {
      get;
      set;
    } = SignStatus.Undefined;


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
    } = 250;


    public int Page {
      get;
      set;
    } = 1;

  }  // class ESignRequestsQuery



  /// <summary>Extension methods for ESignRequestsQuery class.</summary>
  static internal class ESignRequestsQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this ESignRequestsQuery query) {
      Assertion.Require(query.Status != SignStatus.Undefined, $"Undefined status value.");

      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? "TransactionId DESC";
      query.PageSize = query.PageSize <= 0 ? 250 : query.PageSize;
      query.Page = query.Page <= 0 ? 1 : query.Page;
    }


    static internal string MapToFilterString(this ESignRequestsQuery query) {
      string recorderOfficeFilter = BuildRecorderOfficeFilter(query.RecorderOffice);
      string signerFilter = BuildSignedByFilter(query.SignedBy);
      string signStatusFilter = BuildSignStatusFilter(query.Status);
      string transactionKeywordsFilter = BuildTransactionKeywordsFilter(query.Keywords);

      var filter = new Filter(recorderOfficeFilter);

      filter.AppendAnd(signerFilter);
      filter.AppendAnd(signStatusFilter);
      filter.AppendAnd(transactionKeywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this ESignRequestsQuery query) {
      if (!String.IsNullOrWhiteSpace(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "InternalControlNo DESC";
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


    static private string BuildSignStatusFilter(SignStatus status) {
      return $"(SignStatus = '{(char) status}')";
    }


    static private string BuildSignedByFilter(Contact signedBy) {
      if (signedBy.IsEmptyInstance) {
        return string.Empty;
      }

      return $"(SignedById = {signedBy.Id})";
    }

    #endregion Helpers

  }  // class ESignRequestsQueryExtensions

}  // namespace Empiria.Land.Transactions
