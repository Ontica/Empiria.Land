/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Query payload                           *
*  Type     : IssuersQuery                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to search instrument issuers.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Query payload used to search instrument issuers.</summary>
  public class IssuersQuery {

    #region Properties

    public InstrumentTypeEnum InstrumentType {
      get; set;
    } = InstrumentTypeEnum.All;


    public string InstrumentKind {
      get; set;
    } = String.Empty;


    public IssuerTypeEnum IssuerType {
      get; set;
    } = IssuerTypeEnum.All;


    public string OnDate {
      get; set;
    } = String.Empty;


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

    #endregion Properties

  }  // class IssuersQuery


  /// <summary>Extension methods for type IssuersQuery.</summary>
  static public class IssuersQueryExtensions {

    #region Extension methods

    static public void EnsureIsValid(this IssuersQuery query) {
      if (EmpiriaString.NotIsEmpty(query.OnDate)) {
        Assertion.Require(EmpiriaString.IsDate(query.OnDate),
                         $"Unrecognized onDate search value: '{query.OnDate}'");
      }

      query.Keywords = query.Keywords ?? String.Empty;
      query.OrderBy = query.OrderBy ?? String.Empty;
      query.PageSize = query.PageSize > 0 ? query.PageSize : 50;
      query.Page = query.Page > 0 ? query.Page : 1;
    }


    static internal string MapToFilterString(this IssuersQuery query) {
      string issuerTypeStatusFilter = BuildIssuerTypeAndStatusFilter(query.IssuerType);
      string instrumentTypeFilter = BuildInstrumentTypeFilter(query.InstrumentType);
      string instrumentKindFilter = BuildInstrumentKindFilter(query.InstrumentKind);
      string onDateFilter = BuildOnDateFilter(query.OnDate);
      string keywordsFilter = SearchExpression.ParseAndLikeKeywords("IssuerKeywords", query.Keywords);


      var filter = new Filter(issuerTypeStatusFilter);

      filter.AppendAnd(instrumentTypeFilter);
      filter.AppendAnd(instrumentKindFilter);
      filter.AppendAnd(onDateFilter);
      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this IssuersQuery query) {
      if (EmpiriaString.NotIsEmpty(query.OrderBy)) {
        return query.OrderBy;
      } else {
        return "IssuerName";
      }
    }


    #endregion Extension methods

    #region Helpers

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

    #endregion Helpers

  }  // class IssuersQueryExtensions

}  // namespace Empiria.Land.Instruments.Adapters
