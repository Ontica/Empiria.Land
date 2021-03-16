/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : RegistrationUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that perform registrar tasks over legal instruments.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Services;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases that perform registrar tasks over legal instruments.</summary>
  public class RegistrationUseCases : UseCase {

    #region Constructors and parsers

    static public RegistrationUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RegistrationUseCases>();
    }

    #endregion Constructors and parsers

    #region Command Use cases


    public InstrumentRecordingDto CreateRecordingAct(string instrumentRecordingUID,
                                                     RegistrationCommand command) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(command, "command");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      var registrationEngine = new RegistrationEngine(instrumentRecording);

      registrationEngine.Execute(command);

      return InstrumentRecordingMapper.Map(instrumentRecording,
                                           instrumentRecording.GetTransaction());
    }


    public InstrumentRecordingDto RemoveRecordingAct(string instrumentRecordingUID,
                                                            string recordingActUID) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      var recordingAct = RecordingAct.Parse(recordingActUID);

      instrumentRecording.RemoveRecordingAct(recordingAct);

      return InstrumentRecordingMapper.Map(instrumentRecording,
                                           instrumentRecording.GetTransaction());
    }


    #endregion Command Use cases

  }  // class RegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
