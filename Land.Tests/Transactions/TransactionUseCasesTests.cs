/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionUseCasesTests                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction related use cases.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction related use cases.</summary>
  public class TransactionUseCasesTests {

    #region Fields

    private readonly string _TRANSACTION_UID = TestingConstants.TRANSACTION_UID;

    private readonly TransactionUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = TransactionUseCases.UseCaseInteractor();
    }


    ~TransactionUseCasesTests() => _usecases.Dispose();

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Get_A_Transaction() {
      TransactionDto transaction = _usecases.GetTransaction(_TRANSACTION_UID);

      Assert.Equal(_TRANSACTION_UID, transaction.UID);
    }


    [Fact]
    public void Should_Get_MyInbox_TransactionList() {
      var myInboxCommand = new SearchTransactionCommand {
        Stage = TransactionStage.MyInbox,
        PageSize = 100,
      };

      FixedList<TransactionShortModel> list = _usecases.SearchTransactions(myInboxCommand);

      Assert.NotEmpty(list);

      var currentUser = CommonMethods.GetCurrentUser();

      Assert.Equal(list.Count, list.CountAll(x => x.AssignedToUID == currentUser.UID));
    }


    [Fact]
    public void Should_Search_And_Get_A_TransactionList() {
      var searchCommand = new SearchTransactionCommand {
        Stage = TransactionStage.Completed,
        PageSize = 100,
      };

      FixedList<TransactionShortModel> list = _usecases.SearchTransactions(searchCommand);

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
