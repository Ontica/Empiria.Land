/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Use case interactor class               *
*  Type     : TransactionPaymentUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partial class with use cases for transaction requested services.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Transactions.Payments.Providers;

namespace Empiria.Land.Transactions.Payments.UseCases {

  /// <summary>Transaction payment use cases.</summary>
  public class TransactionPaymentUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionPaymentUseCases() {
      // no-op
    }

    static public TransactionPaymentUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionPaymentUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public async Task<TransactionDto> CancelPayment(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      transaction.CancelPayment();

      return await Task.FromResult(TransactionMapper.Map(transaction));
    }


    public async Task<TransactionDto> CancelPaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.HasPaymentOrder,
                $"Transaction '{transactionUID}' has not a payment order.");

      Assertion.Require(transaction.ControlData.CanCancelPaymentOrder,
            "The payment order can not be canceled because business rules restrict it, " +
            "or the user account does not has enough privileges.");

      transaction.CancelPaymentOrder();

      return await Task.FromResult(TransactionMapper.Map(transaction));
    }


    public async Task<TransactionDto> GeneratePaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(!transaction.HasPaymentOrder,
          $"A payment order has already been generated for transaction '{transactionUID}'.");

      Assertion.Require(transaction.ControlData.CanGeneratePaymentOrder,
          "The payment order can not be generated because business rules restrict it, " +
          "or the user account does not has enough privileges.");

      var connector = new PaymentServicesConnector();

      var paymentOrder = await connector.GeneratePaymentOrder(transaction);

      transaction.SetPaymentOrder(paymentOrder);

      return TransactionMapper.Map(transaction);
    }


    public async Task<TransactionDto> SetPayment(string transactionUID,
                                                 PaymentFields paymentFields) {
      Assertion.Require(paymentFields, "paymentFields");

      paymentFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.ControlData.CanEditPayment,
                       $"Can not set payment for transaction '{transactionUID}'.");

      //Assertion.Assert(transaction.PaymentOrder.Total == 0 ||
      //                 transaction.PaymentOrder.Total >= paymentFields.Total,
      //                $"Payment total must be less or equal than payment order total.");

      //var connector = new PaymentServicesConnector();

      //string status = await connector.GetPaymentStatus(transaction.PaymentOrder);

      //paymentFields.Status = status;

      transaction.SetPayment(paymentFields);

      return await Task.FromResult(TransactionMapper.Map(transaction));
    }

    #endregion Use cases

    #region Helper methods

    private LRSTransaction ParseTransaction(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID);

      Assertion.Require(transaction,
          $"A transaction with uid = '{transactionUID}' was not found.");

      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionPaymentUseCases

}  // namespace Empiria.Land.Transactions.Payments.UseCases
