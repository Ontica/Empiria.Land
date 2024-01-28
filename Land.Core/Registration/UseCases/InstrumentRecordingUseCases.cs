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

using Empiria.Land.Registration.Adapters;

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

    public bool ExistsInstrumentRecordingID(string instrumentRecordingID) {
      Assertion.Require(instrumentRecordingID, nameof(instrumentRecordingID));

      var instrumentRecording = RecordingDocument.TryParse(instrumentRecordingID);

      return (instrumentRecording != null);
    }


    public InstrumentRecordingDto GetInstrumentRecording(string instrumentRecordingUID) {
      Assertion.Require(instrumentRecordingUID, nameof(instrumentRecordingUID));

      RecordingDocument instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto CloseInstrumentRecording(string instrumentRecordingUID) {
      Assertion.Require(instrumentRecordingUID, nameof(instrumentRecordingUID));

      RecordingDocument instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      instrumentRecording.Close();

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto OpenInstrumentRecording(string instrumentRecordingUID) {
      Assertion.Require(instrumentRecordingUID, nameof(instrumentRecordingUID));

      RecordingDocument instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      instrumentRecording.Open();

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }

    #endregion Use cases

  }  // class InstrumentRecordingUseCases

}  // namespace Empiria.Land.Registration.UseCases
