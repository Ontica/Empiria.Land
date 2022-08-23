/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionUseCases                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction searching and retrieving.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Messaging;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction searching and retrieving.</summary>
  public partial class TransactionUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionUseCases() {
      // no-op
    }

    static public TransactionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TransactionDto CloneTransaction(string baseTransactionUID) {
      Assertion.Require(baseTransactionUID, "baseTransactionUID");

      var transaction = LRSTransaction.Parse(baseTransactionUID);

      var clone = transaction.MakeCopy();

      clone.Save();

      return TransactionMapper.Map(clone);
    }


    public TransactionDto CreateTransaction(TransactionFields fields) {
      Assertion.Require(fields, "fields");

      var transaction = new LRSTransaction(fields);

      transaction.Save();

      transaction.AddPreconfiguredServicesIfApplicable();

      // ToDo: Call it in a service or in IIS Application.OnStart()
      if (!LandMessenger.IsRunning) {
        LandMessenger.Start();
      }

      return TransactionMapper.Map(transaction);
    }


    public void DeleteTransaction(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      Assertion.Require(transaction.ControlData.CanDelete,
                       $"Transaction {transaction.UID} can not be deleted.");

      transaction.Delete();
    }


    public bool ExistsTransactionID(string transactionID) {
      Assertion.Require(transactionID, "transactionID");

      var transaction = LRSTransaction.TryParse(transactionID);

      return (transaction != null);
    }


    public TransactionDto GetTransaction(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      return TransactionMapper.Map(transaction);
    }


    public FixedList<TransactionDescriptor> SearchTransactions(TransactionsQuery query) {
      Assertion.Require(query, nameof(query));

      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();

      var list = LRSTransaction.GetList(filter, sort, query.PageSize);

      return TransactionMapper.MapToDescriptor(list);
    }


    public async Task<TransactionDto> SubmitTransaction(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      transaction.Workflow.Receive(string.Empty);

      return await Task.FromResult(TransactionMapper.Map(transaction));
    }


    public TransactionDto UpdateTransaction(string transactionUID, TransactionFields fields) {
      Assertion.Require(transactionUID, "transactionUID");
      Assertion.Require(fields, "fields");

      var transaction = LRSTransaction.Parse(transactionUID);

      transaction.Update(fields);

      transaction.Save();

      return TransactionMapper.Map(transaction);
    }

    #endregion Use cases

  }  // class TransactionUseCases

}  // namespace Empiria.Land.Transactions.UseCases
