/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : InstrumentRecordingUseCases                License   : Please read LICENSE.txt file            *
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
  public class InstrumentRecordingUseCases : UseCase {

    #region Constructors and parsers

    protected InstrumentRecordingUseCases() {
      // no-op
    }

    static public InstrumentRecordingUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<InstrumentRecordingUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public InstrumentRecordingDto GetInstrumentRecording(string instrumentRecordingUID) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");

      RecordingDocument instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto GetTransactionInstrumentRecording(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      RecordingDocument instrumentRecording = transaction.Document;

      return InstrumentRecordingMapper.Map(instrumentRecording, transaction);
    }


    public InstrumentRecordingDto CreateTransactionInstrumentRecording(string transactionUID, InstrumentFields fields) {
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

      RecordingDocument recordingDocument = instrument.TryGetRecordingDocument();

      transaction.AttachDocument(recordingDocument);

      return InstrumentRecordingMapper.Map(recordingDocument, transaction);
    }


    public InstrumentRecordingDto UpdateTransactionInstrumentRecording(string transactionUID, InstrumentFields fields) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      Assertion.AssertObject(fields, "fields");

      var transaction = LRSTransaction.Parse(transactionUID);

      Instrument instrument = Instrument.Parse(transaction.InstrumentId);

      instrument.Update(fields);

      instrument.Save();

      RecordingDocument recordingDocument = instrument.TryGetRecordingDocument();

      return InstrumentRecordingMapper.Map(recordingDocument, transaction);
    }


    #endregion Use cases


  }  // class InstrumentRecordingUseCases

}  // namespace Empiria.Land.Registration.UseCases
