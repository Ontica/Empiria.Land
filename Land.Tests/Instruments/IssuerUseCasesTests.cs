/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : IssuerUseCasesTests                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for legal instruments issuers.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Tests.Instruments {

  /// <summary>Test cases for legal instruments issuers.</summary>
  public class IssuerUseCasesTests {

    #region Fields

    private readonly IssuerUseCases _usecases;

    #endregion Fields

    #region Initialization

    public IssuerUseCasesTests() => _usecases = IssuerUseCases.UseCaseInteractor();

    ~IssuerUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Search_Issuers_For_EscrituraPublica() {
      var query = new IssuersQuery();

      FixedList<IssuerDto> list = _usecases.SearchIssuers(query);

      Assert.NotEmpty(list);

      int moreGeneralListItemsCount = list.Count;

      query.InstrumentType = InstrumentTypeEnum.EscrituraPublica;

      list = _usecases.SearchIssuers(query);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search issuers by instrument type must return the same or fewer items.");


      moreGeneralListItemsCount = list.Count;

      query.Keywords = "gOnzález";

      list = _usecases.SearchIssuers(query);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search issuers by keyword must return the same or fewer items.");
    }

    #endregion Facts

  }  // class IssuerUseCasesTests

}  // namespace Empiria.Land.Tests.Instruments
