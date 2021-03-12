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


    public InstrumentDto RecordingActRegistration(string instrumentUID,
                                                  RegistrationCommand command) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");
      Assertion.AssertObject(command, "command");

      var instrument = Instrument.Parse(instrumentUID);

      instrument.EnsureHasRecordingDocument();

      var recordingDocument = instrument.TryGetRecordingDocument();

      var registrationEngine = new RegistrationEngine(recordingDocument);

      registrationEngine.Execute(command);

      return InstrumentMapper.Map(instrument);
    }


    public InstrumentDto RemoveRecordingActRegistation(string instrumentUID,
                                                       string recordingActUID) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");

      throw new NotImplementedException();
    }


    public InstrumentDto UpdateRecordingActRegistration(string instrumentUID,
                                                        string recordingActUID,
                                                        RegistrationCommand command) {
      Assertion.AssertObject(instrumentUID, "instrumentUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");
      Assertion.AssertObject(command, "command");

      throw new NotImplementedException(command.Type.Name());
    }


    #endregion Command Use cases

  }  // class RegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
