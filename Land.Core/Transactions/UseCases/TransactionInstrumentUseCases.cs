/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionInstrumentUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction instrument edition and retrieving.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

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

      var transaction = LRSTransaction.Parse(transactionUID);

      Instrument instrument = Instrument.Parse(transaction.InstrumentId);

      return InstrumentMapper.Map(instrument, transaction);
    }


    public InstrumentDto CreateTransactionInstrument(string transactionUID, InstrumentFields fields) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      Assertion.AssertObject(fields, "fields");
      Assertion.Assert(fields.Type.HasValue, "Instrument.Type value is required.");

      var transaction = LRSTransaction.Parse(transactionUID);

      var instrumentType = InstrumentType.Parse(fields.Type.Value);

      var instrument = new Instrument(instrumentType, fields);

      instrument.Save();

      Assertion.Assert(instrument.HasDocument,
                        "Instruments must have a recording document to be linked to a transaction.");

      transaction.SetInstrument(instrument);

      transaction.AttachDocument(instrument.TryGetRecordingDocument());

      return InstrumentMapper.Map(instrument, transaction);
    }


    public InstrumentDto UpdateTransactionInstrument(string transactionUID, InstrumentFields fields) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      Assertion.AssertObject(fields, "fields");

      var transaction = LRSTransaction.Parse(transactionUID);

      Instrument instrument = Instrument.Parse(transaction.InstrumentId);

      instrument.Update(fields);

      instrument.Save();

      return InstrumentMapper.Map(instrument, transaction);
    }


    #endregion Use cases

    #region Helper methods

    static private Instrument ParseTransactionInstrument(string transactionUID) {
      var transaction = LRSTransaction.Parse(transactionUID);

      return Instrument.Parse(transaction.InstrumentId);
    }

    #endregion Helper methods

  }  // class TransactionInstrumentUseCases

}  // namespace Empiria.Land.Transactions.UseCases
