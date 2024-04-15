/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionPaymentUseCasesTests            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction payments use cases.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.Land.Transactions.Adapters;

using Empiria.Land.Transactions.Payments.Adapters;
using Empiria.Land.Transactions.Payments.UseCases;

namespace Empiria.Land.Tests.Transactions.Payments {

  /// <summary>Use case test cases for add and remove services to a transaction.</summary>
  public class TransactionPaymentUseCasesTests {

    #region Fields

    private readonly TransactionPaymentUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionPaymentUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = TransactionPaymentUseCases.UseCaseInteractor();
    }

    ~TransactionPaymentUseCasesTests() {
       _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public async Task Should_Generate_And_Then_Cancel_A_Transaction_Payment_Order() {
      TransactionDto transaction = TransactionRandomizer.TryGetAReadyForPaymentOrderGenerationTransaction();

      if (transaction == null) {
        Assert.True(false, "I didn't find any transaction ready to payment order generation.");
      }

      TransactionDto withPaymentOrder = await _usecases.GeneratePaymentOrder(transaction.UID);

      Assert.Equal(transaction.UID, withPaymentOrder.UID);
      Assert.False(String.IsNullOrWhiteSpace(withPaymentOrder.PaymentOrder.UID));
      Assert.True(withPaymentOrder.PaymentOrder.IssueTime >= DateTime.Now.AddMinutes(-1));
      Assert.False(withPaymentOrder.Actions.Can.GeneratePaymentOrder);
      Assert.True(withPaymentOrder.Actions.Can.CancelPaymentOrder);

      TransactionDto withCanceledPaymentOrder = _usecases.CancelPaymentOrder(transaction.UID);

      Assert.Equal(transaction.UID, withCanceledPaymentOrder.UID);
      Assert.Null(withCanceledPaymentOrder.PaymentOrder);
      Assert.True(withCanceledPaymentOrder.Actions.Can.GeneratePaymentOrder);
      Assert.False(withCanceledPaymentOrder.Actions.Can.CancelPaymentOrder);
    }


    [Fact]
    public void Should_Set_And_Cancel_A_Transaction_Payment() {
      TransactionDto transaction = TransactionRandomizer.TryGetAReadyForPaymentTransaction();

      PaymentDto paymentFields =
            TransactionRandomizer.GetRandomPaymentFields(transaction.PaymentOrder.Total);

      TransactionDto withPayment = _usecases.SetPayment(transaction.UID, paymentFields);

      Assert.Equal(transaction.UID, withPayment.UID);
      Assert.False(withPayment.Actions.Can.GeneratePaymentOrder);
      Assert.True(withPayment.Actions.Can.EditPayment);
      Assert.True(withPayment.Actions.Can.CancelPayment);
      Assert.NotNull(withPayment.PaymentOrder);
      Assert.NotNull(withPayment.Payment);
      Assert.True(!string.IsNullOrEmpty(withPayment.Payment.Status));
      Assert.Equal(transaction.PaymentOrder.Total, withPayment.Payment.Total);

      TransactionDto withCanceledPayment = _usecases.CancelPayment(transaction.UID);

      Assert.Equal(transaction.UID, withCanceledPayment.UID);
      Assert.True(withCanceledPayment.Actions.Can.EditPayment);
      Assert.False(withCanceledPayment.Actions.Can.CancelPayment);
      Assert.False(withCanceledPayment.Actions.Can.GeneratePaymentOrder);
      Assert.NotNull(withCanceledPayment.PaymentOrder);
      Assert.Null(withCanceledPayment.Payment);
    }

    #endregion Facts

  }  // class TransactionPaymentUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions.Payments
