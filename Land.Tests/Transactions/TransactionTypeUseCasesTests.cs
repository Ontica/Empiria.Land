/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionTypeUseCasesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction types use cases.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction types use cases.</summary>
  public class TransactionTypeUseCasesTests {

    #region Fields

    private readonly TransactionTypeUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionTypeUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = TransactionTypeUseCases.UseCaseInteractor();
    }

    ~TransactionTypeUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_The_Transaction_Types_List() {
      FixedList<TransactionTypeDto> list = _usecases.GetTransactionTypes();

      Assert.NotEmpty(list);

      foreach (var item in list) {
        Assert.NotEmpty(item.UID);

        Assert.NotEmpty(item.Subtypes);

        Assert.All(item.Subtypes, x => { Assert.NotEmpty(x.UID); } );
      }

    }

    #endregion Facts

  }  // class TransactionTypeUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
