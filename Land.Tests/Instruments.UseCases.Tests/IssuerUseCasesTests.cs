/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : IssuerUseCasesTests                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for legal instruments issuers.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Instruments.UseCases.Tests {

  /// <summary>Test cases for legal instruments issuers.</summary>
  public class IssuerUseCasesTests {

    #region Fields

    private readonly IssuerUseCases _usecases;

    #endregion Fields

    #region Initialization

    public IssuerUseCasesTests() {
      _usecases = IssuerUseCases.UseCaseInteractor();
    }


    ~IssuerUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Search_Issuers_For_EscrituraPublica() {
      var searchCommand = new IssuersSearchCommand();

      FixedList<IssuerDto> list = _usecases.SearchIssuers(searchCommand);

      Assert.NotEmpty(list);

      int moreGeneralListItemsCount = list.Count;

      searchCommand.InstrumentType = InstrumentTypeEnum.EscrituraPublica;

      list = _usecases.SearchIssuers(searchCommand);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search issuers by instrument type must return the same or fewer items.");


      moreGeneralListItemsCount = list.Count;

      searchCommand.Keywords = "gOnzález";

      list = _usecases.SearchIssuers(searchCommand);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search issuers by keyword must return the same or fewer items.");
    }


    #endregion Facts

  }  // class IssuerUseCasesTests

}  // namespace Empiria.Land.Instruments.UseCases.Tests
