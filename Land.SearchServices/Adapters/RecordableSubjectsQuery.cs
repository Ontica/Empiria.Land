/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Query payload                           *
*  Type     : RecordableSubjectsQuery                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to search recordable subjects.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Query payload used to search recordable subjects.</summary>
  public class RecordableSubjectsQuery {

    public RecordableSubjectType Type {
      get; set;
    } = RecordableSubjectType.None;


    public RecorderOffice RecorderOffice {
      get; set;
    } = RecorderOffice.Empty;


    public string Keywords {
      get; set;
    } = string.Empty;


    public string OrderBy {
      get; set;
    } = string.Empty;


    public int PageSize {
      get; set;
    } = 50;


    public int Page {
      get; set;
    } = 1;


  }  // class // RecordableSubjectsQuery



  /// <summary>Extension methods for RecordableSubjectsQuery type.</summary>
  static internal class RecordableSubjectsQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this RecordableSubjectsQuery query) {
      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? "PropertyUID";
      query.PageSize = query.PageSize <= 0 ? 50 : query.PageSize;
      query.Page = query.Page <= 0 ? 1 : query.Page;
      query.RecorderOffice = GetRecorderOffice(query.RecorderOffice);
    }


    static internal string MapToFilterString(this RecordableSubjectsQuery query) {
      string recorderOfficeFilter = BuildRecorderOfficeFilter(query.RecorderOffice);
      string typeFilter = BuildRecordableSubjectTypeFilter(query.Type);

      string keywordsFilter = BuildKeywordsFilter(query.Keywords);

      var filter = new Filter(recorderOfficeFilter);

      filter.AppendAnd(typeFilter);
      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static private string BuildRecordableSubjectTypeFilter(RecordableSubjectType type) {
      switch (type) {
        case RecordableSubjectType.Association:
          return "(PropertyTypeId = 2084)";
        case RecordableSubjectType.RealEstate:
          return "(PropertyTypeId = 2085)";
        case RecordableSubjectType.NoProperty:
          return "(PropertyTypeId = 2086)";
        default:
          return "(PropertyTypeId <> 0)";
      }
    }


    static private string BuildRecorderOfficeFilter(RecorderOffice recorderOffice) {
      if (recorderOffice.IsEmptyInstance) {
        return string.Empty;
      }

      return $"(RecorderOfficeId = {recorderOffice.Id} OR PropertyId = -2)";
    }


    static internal string MapToSortString(this RecordableSubjectsQuery query) {
      if (!String.IsNullOrWhiteSpace(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "PropertyUID";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildKeywordsFilter(string keywords) {
      if (Resource.MatchesWithUID(keywords)) {
        return $"(PropertyUID = '{keywords}')";

      } else {

        return SearchExpression.ParseAndLikeKeywords("PropertyKeywords", keywords);
      }
    }


    static private RecorderOffice GetRecorderOffice(RecorderOffice recorderOffice) {
      if (!recorderOffice.IsEmptyInstance) {
        return recorderOffice;
      }

      return Permissions.GetUserRecorderOffice();
    }

    #endregion Helpers

  }  // class RecordableSubjectsQueryExtensions

}  // namespace Empiria.Land.SearchServices
