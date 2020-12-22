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

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Instruments.UseCases;
using Empiria.Land.Registration.Transactions;
using System;

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


    public InstrumentDto GetTransactionInstrument(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      using (var usecase = InstrumentUseCases.UseCaseInteractor()) {

        string instrumentUID = GetTransactionInstrumentUID(transactionUID);

        return usecase.GetInstrument(instrumentUID);
      }
    }


    public FixedList<TransactionListItemDto> SearchTransactions(SearchTransactionCommand searchCommand) {
      Assertion.AssertObject(searchCommand, "searchCommand");

      searchCommand.EnsureIsValid();

      string filter = searchCommand.MapToFilterString();
      string sort = searchCommand.MapToSortString();

      var list = LRSTransaction.GetList(filter, sort, searchCommand.PageSize);

      return TransactionListItemDtoMapper.Map(list);
    }


    public InstrumentDto UpdateTransactionInstrument(string transactionUID, InstrumentFields fields) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      Assertion.AssertObject(fields, "fields");

      using (var usecase = InstrumentUseCases.UseCaseInteractor()) {

        string instrumentUID = GetTransactionInstrumentUID(transactionUID);

        return usecase.UpdateInstrument(instrumentUID, fields);
      }
    }


    #endregion Query Use cases


    static private string GetTransactionInstrumentUID(string transactionUID) {
      var transaction = LRSTransaction.Parse(transactionUID);

      return transaction.GetInstrumentUID();
    }

  }  // class TransactionUseCases

}  // namespace Empiria.Land.Transactions.UseCases
