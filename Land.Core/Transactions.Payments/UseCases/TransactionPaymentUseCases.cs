/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Use case interactor class               *
*  Type     : TransactionPaymentUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction payments.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.Payments.Adapters;

using Empiria.Land.Transactions.Payments.Data;

namespace Empiria.Land.Transactions.Payments.UseCases {

  /// <summary>Use cases for transaction payments.</summary>
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

    public TransactionDto CancelPayment(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      transaction.PaymentData.CancelPayment();

      return TransactionMapper.Map(transaction);
    }


    public TransactionDto CancelPaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.PaymentData.HasPaymentOrder,
                $"Transaction '{transactionUID}' has not a payment order.");

      Assertion.Require(transaction.ControlData.CanCancelPaymentOrder,
            "The payment order can not be canceled because business rules restrict it, " +
            "or the user account does not has enough privileges.");

      transaction.PaymentData.CancelPaymentOrder();

      return TransactionMapper.Map(transaction);
    }


    public async Task<TransactionDto> GeneratePaymentOrder(string transactionUID) {
      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(!transaction.PaymentData.HasPaymentOrder,
          $"A payment order has already been generated for transaction '{transactionUID}'.");

      Assertion.Require(transaction.ControlData.CanGeneratePaymentOrder,
          "The payment order can not be generated because business rules restrict it, " +
          "or the user account does not has enough privileges.");

      var connector = new PaymentServicesConnector();

      var paymentOrder = await connector.GeneratePaymentOrder(transaction);

      transaction.PaymentData.SetPaymentOrder(paymentOrder);

      return TransactionMapper.Map(transaction);
    }


    public async Task<TransactionDto> SetPayment(string transactionUID,
                                                 PaymentDto paymentFields) {
      Assertion.Require(paymentFields, nameof(paymentFields));

      paymentFields.AssertValid();

      LRSTransaction transaction = ParseTransaction(transactionUID);

      Assertion.Require(transaction.ControlData.CanEditPayment,
                       $"Can not set payment for transaction '{transactionUID}'.");

      await EnsureIsPayed(transaction, paymentFields);

      EnsureReceiptNumberIsNotReused(paymentFields);

      transaction.PaymentData.SetPayment(paymentFields);

      return TransactionMapper.Map(transaction);
    }

    #endregion Use cases

    #region Helper methods

    private async Task EnsureIsPayed(LRSTransaction transaction, PaymentDto paymentFields) {

      if (!transaction.PaymentData.HasPaymentOrder) {
        return;
      }

      if (!EmpiriaString.IsInteger(transaction.PaymentData.PaymentOrder.UID)) {
        transaction.PaymentData.PaymentOrder.UID = paymentFields.ReceiptNo;
      }

      var connector = new PaymentServicesConnector();

      string status = await connector.GetPaymentStatus(transaction.PaymentData.PaymentOrder);

      if (status == "PAGADO") {
        return;

      } else if (status == "PENDIENTE DE PAGO") {
        Assertion.RequireFail($"Este trámite no ha sido pagado, o el pago aún está marcado como " +
                              $"pendiente en la Secretaría de Finanzas: '{transaction.UID}'.");

      } else if (string.IsNullOrWhiteSpace(status)) {
        Assertion.RequireFail($"No existe el pago registrado en la Secretaría " +
                              $"de Finanzas para el trámite: '{transaction.UID}'.");

      } else {
        throw Assertion.EnsureNoReachThisCode($"Estado de pago no reconocido '{status}'.");
      }
    }


    private void EnsureReceiptNumberIsNotReused(PaymentDto paymentFields) {
      var payment = TransactionPaymentsDataService.TryGetPayment(paymentFields.ReceiptNo);

      if (payment != null) {
        Assertion.RequireFail($"El recibo {paymentFields.ReceiptNo} ya fue " +
                              $"utilizado en el trámite {payment.Transaction.UID}.");
      }
    }


    private LRSTransaction ParseTransaction(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.TryParse(transactionUID);

      Assertion.Require(transaction,
          $"A transaction with uid = '{transactionUID}' was not found.");

      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionPaymentUseCases

}  // namespace Empiria.Land.Transactions.Payments.UseCases
