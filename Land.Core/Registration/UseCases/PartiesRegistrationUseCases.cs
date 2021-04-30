/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : PartiesRegistrationUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that perform parties registration over recording acts.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases that perform parties registration over recording acts.</summary>
  public class PartiesRegistrationUseCases : UseCase {

    #region Constructors and parsers

    static public PartiesRegistrationUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<PartiesRegistrationUseCases>();
    }

    #endregion Constructors and parsers

    #region Command Use cases

    public RecordingActDto AppendParty(string instrumentRecordingUID,
                                       string recordingActUID,
                                       RecordingActPartyFields partyFields) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");
      Assertion.AssertObject(partyFields, "partyFields");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      RecordingAct recordingAct = instrumentRecording.GetRecordingAct(recordingActUID);

      recordingAct.AppendParty(partyFields);

      return RecordingActMapper.Map(recordingAct);
    }


    public RecordingActDto RemoveParty(string instrumentRecordingUID,
                                       string recordingActUID,
                                       string partyUID) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");
      Assertion.AssertObject(partyUID, "partyUID");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      RecordingAct recordingAct = instrumentRecording.GetRecordingAct(recordingActUID);

      var party = RecordingActParty.Parse(partyUID);

      recordingAct.RemoveParty(party);

      return RecordingActMapper.Map(recordingAct);
    }


    public FixedList<PartyDto> SearchParties(string instrumentRecordingUID,
                                             string recordingActUID,
                                             SearchPartiesCommand command) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");
      Assertion.AssertObject(command, "command");

      var list = Party.GetList(command);

      return PartyMapper.Map(list);
    }


    #endregion Command Use cases

  }  // class PartiesRegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
