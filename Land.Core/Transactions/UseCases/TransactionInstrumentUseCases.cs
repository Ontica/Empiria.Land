/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionInstrumentUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction instrument edition and retrieving.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction instrument edition and retrieving.</summary>
  public class TransactionInstrumentUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionInstrumentUseCases() {
      // no-op
    }

    static public TransactionInstrumentUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionInstrumentUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public InstrumentDto GetTransactionInstrument(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      string instrumentUID = GetTransactionInstrumentUID(transactionUID);

      var instrument = Instrument.Parse(instrumentUID);

      return InstrumentMapper.Map(instrument);
    }


    public InstrumentDto UpdateTransactionInstrument(string transactionUID, InstrumentFields fields) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      Assertion.AssertObject(fields, "fields");

      string instrumentUID = GetTransactionInstrumentUID(transactionUID);

      var instrument = Instrument.Parse(instrumentUID);

      instrument.Update(fields);

      instrument.Save();

      return InstrumentMapper.Map(instrument);
    }


    #endregion Use cases

    #region Helper methods

    static private string GetTransactionInstrumentUID(string transactionUID) {
      var transaction = LRSTransaction.Parse(transactionUID);

      return transaction.GetInstrumentUID();
    }

    #endregion Helper methods

  }  // class TransactionInstrumentUseCases

}  // namespace Empiria.Land.Transactions.UseCases
