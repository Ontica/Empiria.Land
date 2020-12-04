/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : GetTransactionsUseCasesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction searching and retrieving.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

namespace Empiria.Land.Transactions.UseCases.Tests {

  /// <summary>Test cases for transaction searching and retrieving.</summary>
  public class GetTransactionsUseCasesTests {

    #region Fields

    private readonly string _TRANSACTION_UID;

    private readonly GetTransactionsUseCases _usecases;

    #endregion Fields

    #region Initialization

    public GetTransactionsUseCasesTests() {
      _TRANSACTION_UID = "TR-78AY2-3NH94-8";

      _usecases = GetTransactionsUseCases.UseCaseInteractor();
    }


    ~GetTransactionsUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_A_Transaction() {
      TransactionDto transaction = _usecases.GetTransaction(_TRANSACTION_UID);

      Assert.Equal(_TRANSACTION_UID, transaction.UID);
    }


    [Fact]
    public void Should_Search_And_Get_A_TransactionList() {
      var searchCommand = new SearchTransactionCommand() {
        Stage = TransactionStage.Completed,
        PageSize = 100,
      };

      FixedList<TransactionListItemDto> list = _usecases.SearchTransactions(searchCommand);

      Assert.NotEmpty(list);

      int count = list.Count;

      searchCommand = new SearchTransactionCommand() {
        Stage = TransactionStage.Completed,
        Keywords = "josé",
        PageSize = 100,
      };

      list = _usecases.SearchTransactions(searchCommand);

      Assert.True(list.Count <= count,
                 "Search transactions by keyword must return the same or fewer items.");
    }

    #endregion Facts

  }  // class GetTransactionsUseCasesTests

}  // namespace Empiria.Land.Transactions.UseCases.Tests
