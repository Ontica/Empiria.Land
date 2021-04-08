/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Extension methods                  *
*  Type     : SearchRecordableSubjectsCommandExtensions  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for SearchRecordableSubjectsCommand interface adapter.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Extension methods for SearchRecordableSubjectsCommand interface adapter.</summary>
  static internal class SearchRecordableSubjectsCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this SearchRecordableSubjectsCommand command) {
      command.Keywords = command.Keywords ?? String.Empty;
      command.OrderBy = command.OrderBy ?? "PropertyUID";
      command.PageSize = command.PageSize <= 0 ? 50 : command.PageSize;
      command.Page = command.Page <= 0 ? 1 : command.Page;
    }


    static internal string MapToFilterString(this SearchRecordableSubjectsCommand command) {
      string typeFilter = BuildRecordableSubjectTypeFilter(command.Type);

      string keywordsFilter = BuildKeywordsFilter(command.Keywords);

      var filter = new Filter(typeFilter);

      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }

    private static string BuildRecordableSubjectTypeFilter(RecordableSubjectType type) {
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


    static internal string MapToSortString(this SearchRecordableSubjectsCommand command) {
      if (!String.IsNullOrWhiteSpace(command.OrderBy)) {
        return command.OrderBy;
      } else {
        return "PropertyUID";
      }
    }

    #endregion Extension methods

    #region Private methods

    static private string BuildKeywordsFilter(string keywords) {
      if (Resource.MatchesWithUID(keywords)) {
        return $"(PropertyUID = '{keywords}')";

      } else {

        return SearchExpression.ParseAndLikeKeywords("PropertyKeywords", keywords);
      }
    }


    #endregion Private methods

  }  // class SearchRecordableSubjectsCommandExtensions

}  // namespace Empiria.Land.RecordableSubjects.Adapters
