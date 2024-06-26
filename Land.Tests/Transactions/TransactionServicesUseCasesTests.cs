﻿/* Empiria Land **********************************************************************************************
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

using Empiria.Tests;

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
      TestsCommonMethods.Authenticate();

      _usecases = TransactionServicesUseCases.UseCaseInteractor();
    }

    ~TransactionServicesUseCasesTests() {
       _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public async Task Should_Add_A_Transaction_Service() {
      TransactionDto transaction = TransactionRandomizer.TryGetAReadyForServiceEditionTransaction(false);

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
    public void Should_Delete_A_Transaction_Service() {
      var transaction = TransactionRandomizer.TryGetAReadyForServiceEditionTransaction(true);

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
