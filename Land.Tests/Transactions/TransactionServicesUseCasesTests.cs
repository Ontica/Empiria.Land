/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionServicesUseCasesTests           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case test cases for add and remove services to a transaction.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Use case test cases for add and remove services to a transaction.</summary>
  public class TransactionServicesUseCasesTests {

    #region Fields

    private readonly TransactionServicesUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionServicesUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = TransactionServicesUseCases.UseCaseInteractor();
    }

    ~TransactionServicesUseCasesTests() {
       _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public async Task Should_Add_A_Transaction_Service() {
      TransactionDto transaction = TransactionRandomizer.TryGetAServiceEditableTransaction(false);

      RequestedServiceFields requestedServiceFields =
            TransactionRandomizer.CreateRandomRequestedServiceFields();

      TransactionDto updated = await _usecases.RequestService(transaction.UID, requestedServiceFields);

      Assert.Equal(transaction.UID, updated.UID);
      Assert.Equal(transaction.RequestedServices.Length + 1, updated.RequestedServices.Length);

      var addedService = updated.RequestedServices[updated.RequestedServices.Length - 1];

      Assert.Equal(requestedServiceFields.TaxableBase, addedService.TaxableBase);
      Assert.Equal(requestedServiceFields.Quantity, addedService.Quantity);
      Assert.Equal(requestedServiceFields.Notes, addedService.Notes);
      Assert.Equal(requestedServiceFields.ServiceUID, addedService.Type);
    }


    [Fact]
    public async Task Should_Generate_And_Then_Cancel_A_Transaction_Payment_Order() {
      TransactionDto transaction = TransactionRandomizer.TryGetAServiceEditableTransaction(true);

      if (transaction == null) {
        Assert.True(false, "I didn't find any transaction ready to payment order generation.");
      }

      TransactionDto withPaymentOrder = await _usecases.GeneratePaymentOrder(transaction.UID);

      Assert.Equal(transaction.UID, withPaymentOrder.UID);
      Assert.False(String.IsNullOrWhiteSpace(withPaymentOrder.PaymentOrder.UID));
      Assert.True(withPaymentOrder.PaymentOrder.IssueTime >= DateTime.Now.AddMinutes(-1));
      Assert.False(withPaymentOrder.Actions.Can.GeneratePaymentOrder);
      Assert.True(withPaymentOrder.Actions.Can.CancelPaymentOrder);

      TransactionDto withCanceledPaymentOrder = await _usecases.CancelPaymentOrder(transaction.UID);

      Assert.Equal(transaction.UID, withCanceledPaymentOrder.UID);
      Assert.Null(withCanceledPaymentOrder.PaymentOrder);
      Assert.True(withCanceledPaymentOrder.Actions.Can.GeneratePaymentOrder);
      Assert.False(withCanceledPaymentOrder.Actions.Can.CancelPaymentOrder);
    }


    [Fact]
    public async Task Should_Generate_A_Payment_Order_And_Then_Set_Its_Payment() {
      TransactionDto transaction = TransactionRandomizer.TryGetAServiceEditableTransaction(true);

      if (transaction == null) {
        Assert.True(false, "I didn't find any transaction ready to payment order generation.");
      }

      TransactionDto withPaymentOrder = await _usecases.GeneratePaymentOrder(transaction.UID);

      var payment = new PaymentFields {
        ReceiptNo = EmpiriaString.BuildRandomString(8, 20),
        Total = withPaymentOrder.PaymentOrder.Total
      };

      TransactionDto withPayment = await _usecases.SetPayment(transaction.UID, payment);

      Assert.Equal(transaction.UID, withPayment.UID);
      Assert.False(withPayment.Actions.Can.GeneratePaymentOrder);
      Assert.NotNull(withPayment.PaymentOrder);
      Assert.NotNull(withPayment.Payment);
      Assert.True(!string.IsNullOrEmpty(withPayment.Payment.Status));
      Assert.Equal(withPaymentOrder.PaymentOrder.Total, withPayment.Payment.Total);
    }


    [Fact]
    public void Should_Delete_A_Transaction_Service() {
      var transaction = TransactionRandomizer.TryGetAServiceEditableTransaction(true);

      if (transaction == null) {
        Assert.True(false, "I didn't find any editable transaction with one or more services to delete.");
      }

      var toDeleteService = TransactionRandomizer.GetRandomTransactionService(transaction);

      TransactionDto updated = _usecases.DeleteService(transaction.UID, toDeleteService.UID);

      Assert.Equal(transaction.UID, updated.UID);
      Assert.Equal(transaction.RequestedServices.Length - 1, updated.RequestedServices.Length);

      Assert.True(updated.RequestedServices.FirstOrDefault(x => x.UID == toDeleteService.UID) == null);
    }

    #endregion Facts

  }  // class TransactionServicesUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
