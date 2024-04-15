/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Tests                         Component : Test Helpers                            *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Testing randomizer                      *
*  Type     : TransactionRandomizer                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides Empiria Land transaction random values.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Transactions;

using Empiria.Land.Transactions.Payments.Adapters;

using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests {

  /// <summary>Provides Empiria Land transaction random values.</summary>
  static internal class TransactionRandomizer {

    static internal RequestedServiceFields CreateRandomRequestedServiceFields() {
      ProvidedServiceDto providedService = GetRandomProvidedService();

      int feeConceptIndex = EmpiriaMath.GetRandom(0, providedService.FeeConcepts.Length - 1);

      FeeConceptDto feeConcept = providedService.FeeConcepts[feeConceptIndex];

      var fields = new RequestedServiceFields {
        ServiceUID = providedService.UID,
        FeeConceptUID = feeConcept.UID,
        UnitUID = providedService.Unit.UID,
        Quantity = EmpiriaMath.GetRandom(1, 10),
        TaxableBase = EmpiriaMath.GetRandom(0, 20) * 10000,
        Notes = EmpiriaString.BuildRandomString(10, 80),
      };

      return fields;
    }


    static internal TransactionFields CreateRandomTransactionFields() {
      var transactionType = GetRandomTransactionType();

      var fields = new TransactionFields {
        TypeUID = transactionType.UID,
        SubtypeUID = GetRandomSubtype(transactionType).UID,
        FilingOfficeUID = GetRandomFilingOffice().UID,
        AgencyUID = GetRandomAgency().UID,
        RequestedBy = EmpiriaString.BuildRandomString(20, 230),
        InstrumentDescriptor = EmpiriaString.BuildRandomString(5, 145),
      };

      return fields;
    }


    static internal NamedEntityDto GetRandomAgency() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var agencies = usecases.Agencies();

        var index = EmpiriaMath.GetRandom(0, agencies.Count - 1);

        return agencies[index];
      }
    }


    static internal NamedEntityDto GetRandomFilingOffice() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var offices = usecases.FilingOffices();

        var index = EmpiriaMath.GetRandom(0, offices.Count - 1);

        return offices[index];
      }
    }


    static internal TransactionDto GetRandomOnPendingStageTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {

        var command = new TransactionsQuery {
          Stage = TransactionStage.Pending,
          Status = TransactionStatus.Payment
        };

        var transactions = usecases.SearchTransactions(command);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }



    static internal NamedEntityDto GetRandomSubtype(TransactionTypeDto transactionType) {
      var index = EmpiriaMath.GetRandom(0, transactionType.Subtypes.Length - 1);

      return transactionType.Subtypes[index];
    }


    static internal NamedEntityDto GetRandomSubtype(string baseTransactionTypeUID) {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        TransactionTypeDto transactionType = usecases.GetTransactionType(baseTransactionTypeUID);

        return GetRandomSubtype(transactionType);
      }
    }


    internal static PaymentDto GetRandomPaymentFields(decimal total = -1) {
      return new PaymentDto {
        ReceiptNo = EmpiriaString.BuildRandomString(8, 20),
        Total = total != -1 ? total : EmpiriaMath.GetRandom(1, 20) * 200
      };
    }


    static internal ProvidedServiceDto GetRandomProvidedService() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {

        var categories = usecases.ProvidedServices();

        var categoryIndex = EmpiriaMath.GetRandom(0, categories.Count - 1);

        var services = categories[categoryIndex].Services;

        var serviceIndex = EmpiriaMath.GetRandom(0, services.Length - 1);

        return services[serviceIndex];
      }
    }


    static internal TransactionDto GetRandomTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {

        var query = new TransactionsQuery {
          Keywords = "ez"
        };

        var transactions = usecases.SearchTransactions(query);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }


    static internal RequestedServiceDto GetRandomTransactionService(TransactionDto transaction) {
      int index = EmpiriaMath.GetRandom(0, transaction.RequestedServices.Length - 1);

      return transaction.RequestedServices[index];
    }


    static internal TransactionTypeDto GetRandomTransactionType() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var transactionTypesList = usecases.TransactionTypes();

        var index = EmpiriaMath.GetRandom(0, transactionTypesList.Count - 1);

        return transactionTypesList[index];
      }
    }


    static internal TransactionDto GetRandomUpdatableTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        var query = new TransactionsQuery {
          Stage = TransactionStage.Pending
        };
        var transactions = usecases.SearchTransactions(query);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }


    internal static TransactionDto TryGetAReadyForPaymentOrderGenerationTransaction() {
      for (int i = 0; i < 20; i++) {
        TransactionDto transaction = TryGetAReadyForServiceEditionTransaction(true);

        if (transaction == null) {
          continue;
        }

        if (transaction.Actions.Can.GeneratePaymentOrder) {
          return transaction;
        }
      }
      return null;
    }


    internal static TransactionDto TryGetAReadyForPaymentTransaction() {
      for (int i = 0; i < 20; i++) {
        TransactionDto transaction = TransactionRandomizer.GetRandomOnPendingStageTransaction();

        if (transaction.Actions.Can.EditPayment) {
          return transaction;
        }
      }
      return null;
    }


    static internal TransactionDto TryGetAReadyForServiceEditionTransaction(bool withOneOrMoreServices = false) {
      for (int i = 0; i < 20; i++) {
        TransactionDto transaction = TransactionRandomizer.GetRandomOnPendingStageTransaction();

        if (!transaction.Actions.Can.EditServices) {
          continue;
        }

        if (!withOneOrMoreServices) {
          return transaction;
        }

        if (transaction.RequestedServices.Length != 0) {
          return transaction;
        }
      }
      return null;
    }

  }  // class TransactionRandomizer

}  // namespace Empiria.Land.Tests
