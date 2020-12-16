/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Use case interactor class               *
*  Type     : TransactionUseCases                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction searching and retrieving.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction searching and retrieving.</summary>
  public class TransactionUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionUseCases() {
      // no-op
    }

    static public TransactionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionUseCases>();
    }

    #endregion Constructors and parsers

    #region Query Use cases

    public TransactionDto GetTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var document = LRSTransaction.Parse(transactionUID);

      return TransactionDtoMapper.Map(document);
    }


    public FixedList<TransactionListItemDto> SearchTransactions(SearchTransactionCommand searchCommand) {
      searchCommand.EnsureIsValid();

      string filter = searchCommand.MapToFilterString();
      string sort = searchCommand.MapToSortString();

      var list = LRSTransaction.GetList(filter, sort, searchCommand.PageSize);

      return TransactionListItemDtoMapper.Map(list);
    }


    #endregion Query Use cases

  }  // class TransactionUseCases

}  // namespace Empiria.Land.Transactions.UseCases
