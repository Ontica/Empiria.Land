/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionServicesUseCases (Partial)      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partial class with use cases for transaction requested services.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.Providers;

using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Partial class with use cases for transaction requested services.</summary>
  public partial class TransactionUseCases {

    #region Use cases

    public async Task<TransactionDto> CancelPayment(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      transaction.CancelPayment();

      return await Task.FromResult(TransactionDtoMapper.Map(transaction));
    }

    public TransactionDto DeleteService(string transactionUID, string requestedServiceUID) {
      Assertion.AssertObject(requestedServiceUID, "requestedServiceUID");

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Assert(transaction.ControlData.CanEditServices,
                       $"Can not delete services for transaction '{transactionUID}'.");

      LRSTransactionItem item = transaction.Items.Find((x) => x.UID == requestedServiceUID);

      Assertion.AssertObject(item,
          $"Transaction {transactionUID} do not have a service with uid '{requestedServiceUID}'.");

      transaction.RemoveItem(item);

      transaction.Save();

      return TransactionDtoMapper.Map(transaction);
    }


    public async Task<TransactionDto> CancelPaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Assert(transaction.HasPaymentOrder,
                $"Transaction '{transactionUID}' has not a payment order.");

      Assertion.Assert(transaction.ControlData.CanCancelPaymentOrder,
            "The payment order can not be canceled because business rules restrict it, " +
            "or the user account does not has enough privileges.");

      transaction.CancelPaymentOrder();

      return await Task.FromResult(TransactionDtoMapper.Map(transaction));
    }


    public async Task<TransactionDto> GeneratePaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Assert(!transaction.HasPaymentOrder,
          $"A payment order has already been generated for transaction '{transactionUID}'.");

      Assertion.Assert(transaction.ControlData.CanGeneratePaymentOrder,
          "The payment order can not be generated because business rules restrict it, " +
          "or the user account does not has enough privileges.");

      var connector = new PaymentServicesConnector();

      var paymentOrder = await connector.GeneratePaymentOrder(transaction);

      transaction.SetPaymentOrder(paymentOrder);

      return TransactionDtoMapper.Map(transaction);
    }


    public async Task<TransactionDto> RequestService(string transactionUID,
                                                     RequestedServiceFields requestedServiceFields) {
      Assertion.AssertObject(requestedServiceFields, "requestedServiceFields");

      requestedServiceFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Assert(transaction.ControlData.CanEditServices,
                 $"Can not request services on transaction '{transactionUID}'.");

      var connector = new PaymentServicesConnector();

      decimal fee = await connector.CalculateFee(requestedServiceFields);

      requestedServiceFields.Subtotal = fee;

      transaction.AddItem(requestedServiceFields);

      return TransactionDtoMapper.Map(transaction);
    }


    public async Task<TransactionDto> SetPayment(string transactionUID,
                                                 PaymentFields paymentFields) {
      Assertion.AssertObject(paymentFields, "paymentFields");

      paymentFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Assert(transaction.ControlData.CanEditPayment,
                       $"Can not set payment for transaction '{transactionUID}'.");

      Assertion.Assert(transaction.PaymentOrder.Total >= paymentFields.Total,
                      $"Payment total must be less or equal than payment order total.");

      var connector = new PaymentServicesConnector();

      string status = await connector.GetPaymentStatus(transaction.PaymentOrder);

      paymentFields.Status = status;

      transaction.SetPayment(paymentFields);

      return TransactionDtoMapper.Map(transaction);
    }

    #endregion Use cases

    #region Helper methods

    private LRSTransaction ParseTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID);

      Assertion.AssertObject(transaction,
          $"A transaction with uid = '{transactionUID}' was not found.");

      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionServicesUseCases

}  // namespace Empiria.Land.Transactions.UseCases
