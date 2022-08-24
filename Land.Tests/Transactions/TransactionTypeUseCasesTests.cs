/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionTypeUseCasesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction types use cases.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.Tests;

using Empiria.Land.Transactions;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction types use cases.</summary>
  public class TransactionTypeUseCasesTests {

    #region Fields

    private readonly TransactionTypeUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionTypeUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = TransactionTypeUseCases.UseCaseInteractor();
    }

    ~TransactionTypeUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_Groups_Of_Provided_Services() {
      FixedList<ProvidedServiceGroupDto> list = _usecases.ProvidedServices();

      Assert.NotEmpty(list);

      foreach (var item in list) {
        Assert.NotEmpty(item.UID);

        Assert.NotEmpty(item.Services);

        Assert.All(item.Services, x => { Assert.NotEmpty(x.UID); });


        foreach (var service in item.Services) {
          Assert.NotEmpty(service.FeeConcepts);

          Assert.All(service.FeeConcepts, x => { Assert.NotEmpty(x.UID); });
          Assert.All(service.FeeConcepts, x => { Assert.NotEmpty(x.LegalBasis); });
        }

      }
    }


    [Fact]
    public void Should_Get_The_Agencies_List() {
      FixedList<NamedEntityDto> list = _usecases.Agencies();

      Assert.NotEmpty(list);

      Assert.All(list, x => { Assert.NotEmpty(x.UID); });
    }


    [Fact]
    public void Should_Get_The_Filing_Offices_List() {
      FixedList<NamedEntityDto> list = _usecases.FilingOffices();

      Assert.NotEmpty(list);

      Assert.All(list, x => { Assert.NotEmpty(x.UID); });
    }


    [Fact]
    public void Should_Get_The_Transaction_Types_List() {
      FixedList<TransactionTypeDto> list = _usecases.TransactionTypes();

      Assert.NotEmpty(list);

      foreach (var item in list) {
        Assert.NotEmpty(item.UID);

        Assert.NotEmpty(item.Subtypes);

        Assert.All(item.Subtypes, x => { Assert.NotEmpty(x.UID); });
      }
    }

    #endregion Facts

  }  // class TransactionTypeUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
