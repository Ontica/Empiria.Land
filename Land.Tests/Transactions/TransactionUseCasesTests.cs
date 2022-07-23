/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : TransactionUseCasesTests                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for transaction related use cases.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests.Transactions {

  /// <summary>Test cases for transaction related use cases.</summary>
  public class TransactionUseCasesTests {

    #region Fields

    private readonly string _TRANSACTION_UID = TestingConstants.TRANSACTION_UID;

    private readonly TransactionUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TransactionUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = TransactionUseCases.UseCaseInteractor();
    }

    ~TransactionUseCasesTests() {
       _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Clone_A_Transaction() {
      TransactionDto baseTransaction = TransactionRandomizer.GetRandomTransaction();

      var clone = _usecases.CloneTransaction(baseTransaction.UID);

     // Assert.Equal(TransactionStatus.Payment, clone.Status);
      Assert.NotEmpty(clone.TransactionID);
      Assert.NotEqual(baseTransaction.TransactionID, clone.TransactionID);
      Assert.Equal(ExecutionServer.DateMaxValue, clone.PresentationTime);

      Assert.Equal(baseTransaction.Type.UID, clone.Type.UID);
      Assert.Equal(baseTransaction.Subtype.UID, clone.Subtype.UID);
      Assert.Equal(baseTransaction.FilingOffice.UID, clone.FilingOffice.UID);
      Assert.Equal(baseTransaction.Agency.UID, clone.Agency.UID);
      Assert.Equal(baseTransaction.RequestedBy.Name, clone.RequestedBy.Name);
      Assert.Equal(baseTransaction.RequestedBy.Email, clone.RequestedBy.Email);
    }


    [Fact]
    public void Should_Create_A_Transaction() {
      TransactionFields fields = TransactionRandomizer.CreateRandomTransactionFields();

      TransactionDto created = _usecases.CreateTransaction(fields);

      Assert.Equal(fields.TypeUID, created.Type.UID);
      Assert.Equal(fields.SubtypeUID, created.Subtype.UID);
      Assert.Equal(fields.FilingOfficeUID, created.FilingOffice.UID);
      Assert.Equal(fields.AgencyUID, created.Agency.UID);
      Assert.Equal(fields.RequestedBy, created.RequestedBy.Name);
      Assert.Equal(fields.RequestedByEmail, created.RequestedBy.Email);
      Assert.Equal(EmpiriaString.TrimAll(fields.InstrumentDescriptor), created.InstrumentDescriptor);

      Assert.NotEmpty(created.TransactionID);
      Assert.Equal(ExecutionServer.DateMaxValue, created.PresentationTime);
    }


    [Fact]
    public void Should_Delete_A_Transaction() {
      TransactionDto transaction = TransactionRandomizer.GetRandomUpdatableTransaction();

      _usecases.DeleteTransaction(transaction.UID);

      transaction = _usecases.GetTransaction(transaction.UID);

      var query = new TransactionsQuery {
        Stage = TransactionStage.Pending,
        Keywords = transaction.UID
      };

      FixedList<TransactionShortModel> list = _usecases.SearchTransactions(query);

      Assert.Empty(list);
    }


    [Fact]
    public void Should_Get_A_Transaction() {
      TransactionDto transaction = _usecases.GetTransaction(_TRANSACTION_UID);

      Assert.Equal(_TRANSACTION_UID, transaction.UID);
    }


    [Fact]
    public void Should_Get_MyInbox_TransactionList() {
      var myInboxQuery = new TransactionsQuery {
        Stage = TransactionStage.MyInbox,
        PageSize = 100,
      };

      FixedList<TransactionShortModel> list = _usecases.SearchTransactions(myInboxQuery);

      Assert.NotEmpty(list);

      var currentUser = TestsCommonMethods.GetCurrentUser();

      Assert.Equal(list.Count, list.CountAll(x => x.AssignedToUID == currentUser.UID));

      Assert.All(list, x => { Assert.True(x.Status != "Deleted"); });
    }


    [Fact]
    public void Should_Search_And_Get_A_TransactionList() {
      var query = new TransactionsQuery {
        Stage = TransactionStage.Completed,
        PageSize = 100,
      };

      FixedList<TransactionShortModel> list = _usecases.SearchTransactions(query);

      Assert.NotEmpty(list);

      int moreGeneralListItemsCount = list.Count;

      query.Keywords = "josé";

      list = _usecases.SearchTransactions(query);

      Assert.True(list.Count <= moreGeneralListItemsCount,
                 "Search transactions by keyword must return the same or fewer items.");
    }


    [Fact]
    public async Task Should_Create_And_Submit_A_Transaction() {
      TransactionFields transactionFields = TransactionRandomizer.CreateRandomTransactionFields();

      TransactionDto transaction = _usecases.CreateTransaction(transactionFields);

      transaction = await this.AddServicesIfApplicable(transaction);

      transaction = await this.GeneratePaymentOrderAndPaymentIfApplicable(transaction);

      Assert.True(transaction.Actions.Can.Submit);

      TransactionDto submitted = await _usecases.SubmitTransaction(transaction.UID);

      Assert.Equal(transaction.UID, submitted.UID);

      Assert.True(DateTime.Now.AddSeconds(-2) <= submitted.PresentationTime &&
                  submitted.PresentationTime <= DateTime.Now);

      Assert.False(submitted.Actions.Can.Submit);
      Assert.False(submitted.Actions.Can.CancelPayment);
      Assert.False(submitted.Actions.Can.CancelPaymentOrder);
      Assert.False(submitted.Actions.Can.EditServices);
      Assert.False(submitted.Actions.Can.Edit);
      Assert.False(submitted.Actions.Can.Delete);
    }


    [Fact]
    public void Should_Update_A_Transaction() {
      TransactionDto transaction = TransactionRandomizer.GetRandomUpdatableTransaction();

      var fields = new TransactionFields {
        SubtypeUID = TransactionRandomizer.GetRandomSubtype(transaction.Type.UID).UID,
        RequestedBy = EmpiriaString.BuildRandomString(20, 340)
      };

      TransactionDto updated = _usecases.UpdateTransaction(transaction.UID, fields);

      Assert.Equal(transaction.UID, updated.UID);
      Assert.Equal(transaction.TransactionID, updated.TransactionID);

      Assert.Equal(transaction.Type.UID, updated.Type.UID);
      Assert.Equal(fields.SubtypeUID, updated.Subtype.UID);

      Assert.Equal(fields.RequestedBy, updated.RequestedBy.Name);
      Assert.Equal(transaction.RequestedBy.Email, updated.RequestedBy.Email);

      Assert.Equal(ExecutionServer.DateMaxValue, updated.PresentationTime);
    }

    #endregion Facts

    #region Helper methods

    internal async Task<TransactionDto> AddServicesIfApplicable(TransactionDto transaction) {
      if (transaction.Actions.Can.EditServices) {
        for (int i = 0; i < 3; i++) {
          RequestedServiceFields serviceFields = TransactionRandomizer.CreateRandomRequestedServiceFields();
          transaction = await _usecases.RequestService(transaction.UID, serviceFields);
        }
      }
      return transaction;
    }

    internal async Task<TransactionDto> GeneratePaymentOrderAndPaymentIfApplicable(TransactionDto transaction) {
      if (transaction.Actions.Can.GeneratePaymentOrder) {
        Assert.False(transaction.Actions.Can.Submit);
        transaction = await _usecases.GeneratePaymentOrder(transaction.UID);
        Assert.True(transaction.Actions.Can.EditPayment);

        PaymentFields paymentFields =
              TransactionRandomizer.GetRandomPaymentFields(transaction.PaymentOrder.Total);

        transaction = await _usecases.SetPayment(transaction.UID, paymentFields);

        Assert.False(transaction.Actions.Can.GeneratePaymentOrder);
      }
      return transaction;
    }

    #endregion Helper methods

  }  // class TransactionUseCasesTests

}  // namespace Empiria.Land.Tests.Transactions
