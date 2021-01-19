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

using Empiria.Land.Transactions.Adapters;
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
        RecorderOfficeUID = GetRandomRecorderOffice().UID,
        AgencyUID = GetRandomAgency().UID,
        RequestedBy = EmpiriaString.BuildRandomString(20, 230),
        InstrumentDescriptor = EmpiriaString.BuildRandomString(5, 145),
      };

      return fields;
    }


    static internal Tuple<TransactionDto, RequestedServiceDto> GetAnEditableTransactionWithARequestedService() {
      for (int i = 0; i < 20; i++) {
        TransactionDto transaction = TransactionRandomizer.GetRandomEditableTransaction();

        if (transaction.RequestedServices.Length > 0) {
          int indexOfServiceToDelete = EmpiriaMath.GetRandom(0, transaction.RequestedServices.Length - 1);
          RequestedServiceDto service = transaction.RequestedServices[indexOfServiceToDelete];

          return new Tuple<TransactionDto, RequestedServiceDto>(transaction, service);
        }
      }
      return new Tuple<TransactionDto, RequestedServiceDto>(null, null);
    }


    static internal NamedEntityDto GetRandomAgency() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var agencies = usecases.GetAgencies();

        var index = EmpiriaMath.GetRandom(0, agencies.Count - 1);

        return agencies[index];
      }
    }


    static internal TransactionDto GetRandomEditableTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {

        var command = new SearchTransactionCommand {
          Stage = TransactionStage.Pending,
          Status = TransactionStatus.Payment
        };

        var transactions = usecases.SearchTransactions(command);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }


    static internal NamedEntityDto GetRandomRecorderOffice() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var offices = usecases.GetRecorderOffices();

        var index = EmpiriaMath.GetRandom(0, offices.Count - 1);

        return offices[index];
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


    static internal ProvidedServiceDto GetRandomProvidedService() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {

        var categories = usecases.GetProvidedServices();

        var categoryIndex = EmpiriaMath.GetRandom(0, categories.Count - 1);

        var services = categories[categoryIndex].Services;

        var serviceIndex = EmpiriaMath.GetRandom(0, services.Length - 1);

        return services[serviceIndex];
      }
    }


    static internal TransactionDto GetRandomTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {

        var command = new SearchTransactionCommand {
          Keywords = "ez"
        };

        var transactions = usecases.SearchTransactions(command);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }


    static internal TransactionTypeDto GetRandomTransactionType() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var transactionTypesList = usecases.GetTransactionTypes();

        var index = EmpiriaMath.GetRandom(0, transactionTypesList.Count - 1);

        return transactionTypesList[index];
      }
    }


    static internal TransactionDto GetRandomUpdatableTransaction() {
      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        var command = new SearchTransactionCommand {
          Stage = TransactionStage.Pending
        };
        var transactions = usecases.SearchTransactions(command);

        var index = EmpiriaMath.GetRandom(0, transactions.Count - 1);

        return usecases.GetTransaction(transactions[index].UID);
      }
    }

  }  // class TransactionRandomizer

}  // namespace Empiria.Land.Tests
