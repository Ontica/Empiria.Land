/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Use case interactor class               *
*  Type     : TransactionServicesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partial class with use cases for transaction requested services.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Transactions.Payments.Providers;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Partial class with use cases for transaction requested services.</summary>
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
      Assertion.Require(requestedServiceUID, nameof(requestedServiceUID));

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.ControlData.CanEditServices,
                       $"Can not delete services for transaction '{transactionUID}'.");

      LRSTransactionService service = transaction.Services.Find((x) => x.UID == requestedServiceUID);

      Assertion.Require(service,
          $"Transaction {transactionUID} do not have a service with uid '{requestedServiceUID}'.");

      transaction.RemoveService(service);

      transaction.Save();

      return TransactionMapper.Map(transaction);
    }


    public async Task<TransactionDto> RequestService(string transactionUID,
                                                     RequestedServiceFields requestedServiceFields) {
      Assertion.Require(requestedServiceFields, nameof(requestedServiceFields));

      requestedServiceFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.ControlData.CanEditServices,
                 $"Can not request services on transaction '{transactionUID}'.");

      var connector = new PaymentServicesConnector();

      decimal fee = await connector.CalculateFee(requestedServiceFields);

      requestedServiceFields.Subtotal = fee;

      transaction.AddService(requestedServiceFields);

      return TransactionMapper.Map(transaction);
    }


    #endregion Use cases

    #region Helper methods

    private LRSTransaction ParseTransaction(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.TryParse(transactionUID);

      Assertion.Require(transaction,
          $"A transaction with uid = '{transactionUID}' was not found.");

      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionServicesUseCases

}  // namespace Empiria.Land.Transactions.UseCases
