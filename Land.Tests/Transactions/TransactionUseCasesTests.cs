/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : TransactionUseCasesTests                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction related use cases.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.Land.Transactions;
using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction related use cases.</summary>
  public class TransactionUseCasesTests {

    #region Fields

    private readonly string _TRANSACTION_UID;

    private readonly TransactionUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionUseCasesTests() {
      _TRANSACTION_UID = "TR-78AY2-3NH94-8";

      _usecases = TransactionUseCases.UseCaseInteractor();
    }


    ~TransactionUseCasesTests() {
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
      var searchCommand = new SearchTransactionCommand {
        Stage = TransactionStage.Completed,
        PageSize = 100,
      };

      FixedList<TransactionListItemDto> list = _usecases.SearchTransactions(searchCommand);

      Assert.NotEmpty(list);

      int moreGeneralListItemsCount = list.Count;

      searchCommand.Keywords = "josé";

      list = _usecases.SearchTransactions(searchCommand);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search transactions by keyword must return the same or fewer items.");
    }

    #endregion Facts

  }  // class TransactionUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
