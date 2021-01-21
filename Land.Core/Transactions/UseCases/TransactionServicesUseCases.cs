/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionServicesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction requested services.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.Providers;

using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction requested services.</summary>
  public class TransactionServicesUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionServicesUseCases() {
      // no-op
    }

    static public TransactionServicesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionServicesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TransactionDto DeleteService(string transactionUID, string requestedServiceUID) {
      Assertion.AssertObject(requestedServiceUID, "requestedServiceUID");

      LRSTransaction transaction = ParseTransaction(transactionUID);

      LRSTransactionItem item = transaction.Items.Find((x) => x.UID == requestedServiceUID);

      Assertion.AssertObject(item,
          $"Transaction {transactionUID} do not have a service with uid '{requestedServiceUID}'.");

      transaction.RemoveItem(item);

      transaction.Save();

      return TransactionDtoMapper.Map(transaction);
    }


    public async Task<TransactionDto> RequestService(string transactionUID,
                                                     RequestedServiceFields requestedServiceFields) {
      Assertion.AssertObject(requestedServiceFields, "requestedServiceFields");

      requestedServiceFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      requestedServiceFields.Subtotal = await CalculateServiceSubtotal(requestedServiceFields);

      transaction.AddItem(requestedServiceFields);

      return TransactionDtoMapper.Map(transaction);
    }


    #endregion Use cases

    #region Helper methods

    private Task<decimal> CalculateServiceSubtotal(RequestedServiceFields requestedServiceFields) {
      return Task.FromResult(125 * requestedServiceFields.Quantity);
    }

    private LRSTransaction ParseTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID);

      Assertion.AssertObject(transaction,
          $"A transaction with uid = '{transactionUID}' was not found.");

      Assertion.Assert(transaction.ControlData.CanEditServices,
          $"Transaction '{transactionUID}' is in a status that doesn't allow its service's edition.");

      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionServicesUseCases

}  // namespace Empiria.Land.Transactions.UseCases
