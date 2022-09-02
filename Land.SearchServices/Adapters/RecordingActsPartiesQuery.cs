/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Query payload                           *
*  Type     : RecordingActsPartiesQuery                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to search recording act parties.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Query payload used to search recording act parties.</summary>
  public class RecordingActsPartiesQuery {

    public RecordingActPartyType Type {
      get; set;
    } = RecordingActPartyType.Undefined;


    public string Keywords {
      get; set;
    } = string.Empty;


    public string OrderBy {
      get; set;
    } = String.Empty;


    public int PageSize {
      get; set;
    } = 50;


    public int Page {
      get; set;
    } = 1;

  }  // class // RecordingActPartiesQuery



  /// <summary>Extension methods for RecordingActsPartiesQuery type.</summary>
  static internal class RecordingActsPartiesQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this RecordingActsPartiesQuery query) {
      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? "PartyFullName";
      query.PageSize = query.PageSize <= 0 ? 50 : query.PageSize;
      query.Page = query.Page <= 0 ? 1 : query.Page;
    }


    static internal string MapToFilterString(this RecordingActsPartiesQuery query) {
      string typeFilter = BuildPartiesTypeFilter(query.Type);

      string keywordsFilter = BuildKeywordsFilter(query.Keywords);

      var filter = new Filter(typeFilter);

      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this RecordingActsPartiesQuery query) {
      if (!String.IsNullOrWhiteSpace(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "PartyFullName";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildKeywordsFilter(string keywords) {
      return SearchExpression.ParseAndLikeKeywords("PartyKeywords", keywords);
    }

    static private string BuildPartiesTypeFilter(RecordingActPartyType type) {
      switch (type) {
        case RecordingActPartyType.Primary:
          return "(PartyTypeId = 2435)";
        case RecordingActPartyType.Secondary:
          return "(PartyTypeId = 2436)";
        default:
          return "(PartyTypeId <> 0)";
      }
    }

    #endregion Helpers

  }  // class RecordingActsPartiesQueryExtensions

}  // namespace Empiria.Land.SearchServices
