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

    public RecordingActDto AppendParty(string landRecordUID,
                                       string recordingActUID,
                                       RecordingActPartyFields partyFields) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));
      Assertion.Require(partyFields, nameof(partyFields));

      var landRecord = RecordingDocument.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      recordingAct.Parties.AppendParty(partyFields);

      return RecordingActMapper.Map(recordingAct);
    }


    public RecordingActDto RemoveParty(string landRecordUID,
                                       string recordingActUID,
                                       string partyUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));
      Assertion.Require(partyUID, nameof(partyUID));

      var landRecord = RecordingDocument.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      var party = RecordingActParty.Parse(partyUID);

      recordingAct.Parties.RemoveParty(party);

      return RecordingActMapper.Map(recordingAct);
    }


    public FixedList<PartyDto> SearchParties(string landRecordUID,
                                             string recordingActUID,
                                             SearchPartiesCommand command) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));
      Assertion.Require(command, nameof(command));

      var list = Party.GetList(command);

      return PartyMapper.Map(list);
    }


    #endregion Command Use cases

  }  // class PartiesRegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
