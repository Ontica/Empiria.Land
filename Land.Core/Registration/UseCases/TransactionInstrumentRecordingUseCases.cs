/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionInstrumentRecordingUseCases     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction instrument edition and retrieving.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for transaction instrument edition and retrieving.</summary>
  public class TransactionInstrumentRecordingUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionInstrumentRecordingUseCases() {
      // no-op
    }

    static public TransactionInstrumentRecordingUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionInstrumentRecordingUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public InstrumentRecordingDto GetTransactionInstrumentRecording(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.Parse(transactionUID);

      RecordingDocument instrumentRecording = transaction.Document;

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto CreateTransactionInstrumentRecording(string transactionUID,
                                                                       InstrumentFields fields) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(fields, nameof(fields));
      Assertion.Require(fields.Type.HasValue, "Instrument.Type value is required.");

      var transaction = LRSTransaction.Parse(transactionUID);

      var instrumentType = InstrumentType.Parse(fields.Type.Value);

      var instrument = new Instrument(instrumentType, fields);

      instrument.Save();

      transaction.SetInstrument(instrument);

      var landRecord = RecordingDocument.CreateFromInstrument(instrument.Id, instrument.InstrumentType.Id, instrument.Kind);

      instrument.FillRecordingDocument(landRecord);

      landRecord.Save();

      transaction.AttachDocument(landRecord);

      return InstrumentRecordingMapper.Map(landRecord);
    }


    public InstrumentRecordingDto UpdateTransactionInstrumentRecording(string transactionUID,
                                                                       InstrumentFields fields) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(fields, nameof(fields));

      var transaction = LRSTransaction.Parse(transactionUID);

      Instrument instrument = Instrument.Parse(transaction.InstrumentId);

      instrument.Update(fields);

      instrument.Save();

      var landRecord = transaction.Document;

      instrument.FillRecordingDocument(landRecord);

      landRecord.Save();

      return InstrumentRecordingMapper.Map(landRecord);
    }

    #endregion Use cases

  }  // class TransactionInstrumentRecordingUseCases

}  // namespace Empiria.Land.Registration.UseCases
