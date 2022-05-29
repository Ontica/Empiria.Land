/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Extension methods                  *
*  Type     : IssuersSearchCommandExtensions             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for type IssuersSearchCommand.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Linq;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Extension methods for type IssuersSearchCommand.</summary>
  static internal class IssuersSearchCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this IssuersSearchCommand command) {
      if (EmpiriaString.NotIsEmpty(command.OnDate)) {
        Assertion.Require(EmpiriaString.IsDate(command.OnDate),
                         $"Unrecognized onDate search value: '{command.OnDate}'");
      }

      command.Keywords = command.Keywords ?? String.Empty;
      command.OrderBy = command.OrderBy ?? String.Empty;
      command.PageSize = command.PageSize > 0 ? command.PageSize : 50;
      command.Page = command.Page > 0 ? command.Page : 1;
    }


    static internal string MapToFilterString(this IssuersSearchCommand command) {
      string issuerTypeStatusFilter = BuildIssuerTypeAndStatusFilter(command.IssuerType);
      string instrumentTypeFilter = BuildInstrumentTypeFilter(command.InstrumentType);
      string instrumentKindFilter = BuildInstrumentKindFilter(command.InstrumentKind);
      string onDateFilter = BuildOnDateFilter(command.OnDate);
      string keywordsFilter = SearchExpression.ParseAndLikeKeywords("IssuerKeywords", command.Keywords);


      Filter filter = new Filter(issuerTypeStatusFilter);

      filter.AppendAnd(instrumentTypeFilter);
      filter.AppendAnd(instrumentKindFilter);
      filter.AppendAnd(onDateFilter);
      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this IssuersSearchCommand command) {
      if (EmpiriaString.NotIsEmpty(command.OrderBy)) {
        return command.OrderBy;
      } else {
        return "IssuerName";
      }
    }


    #endregion Extension methods

    #region Helper methods

    static private string BuildInstrumentKindFilter(string instrumentKind) {
      if (EmpiriaString.IsEmpty(instrumentKind)) {
        return String.Empty;
      }
      return String.Empty;
    }


    static private string BuildInstrumentTypeFilter(InstrumentTypeEnum typeName) {
      if (typeName == InstrumentTypeEnum.All) {
        return String.Empty;
      }

      var instrumentType = InstrumentType.Parse(typeName);

      var issuersIdsArray = instrumentType.IssuerTypes.Select(x => x.Id).ToArray();

      return SearchExpression.ParseInSet("IssuerTypeId", issuersIdsArray);
    }


    static private string BuildOnDateFilter(string onDateString) {
      DateTime onDate = EmpiriaString.IsEmpty(onDateString) ?
                             ExecutionServer.DateMaxValue : DateTime.Parse(onDateString);

      if (onDate == ExecutionServer.DateMaxValue) {
        return String.Empty;
      }

      return $"(IssuerFromDate <= '{onDate.ToString("yyyy-MM-dd")}' AND " +
             $"'{onDate.ToString("yyyy-MM-dd")}' <= IssuerToDate)";
    }


    static private string BuildIssuerTypeAndStatusFilter(IssuerTypeEnum typeName) {
      const string activeIssuersFilter = "IssuerStatus <> 'X'";

      if (typeName == IssuerTypeEnum.All) {
        return activeIssuersFilter;
      }

      var issuerType = IssuerType.Parse(typeName);

      return $"{activeIssuersFilter} AND IssuerTypeId = {issuerType.Id}";
    }

    #endregion Helper methods

  }  // class IssuersSearchCommandExtensions

}  // namespace Empiria.Land.Instruments.Adapters
