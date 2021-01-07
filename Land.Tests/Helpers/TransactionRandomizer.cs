/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Tests                         Component : Test Helpers                            *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Testing randomizer                      *
*  Type     : TransactionRandomizer                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides Empiria Land transaction random values.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Tests {

  /// <summary>Provides Empiria Land transaction random values.</summary>
  static internal class TransactionRandomizer {

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


    static internal NamedEntityDto GetRandomAgency() {
      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        var agencies = usecases.GetAgencies();

        var index = EmpiriaMath.GetRandom(0, agencies.Count - 1);

        return agencies[index];
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
