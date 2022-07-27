/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TractIndexUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for retrieve recordable subject's tract indexes.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.RecordableSubjects.UseCases {

  /// <summary>Use cases for retrieve recordable subject's tract indexes.</summary>
  public partial class TractIndexUseCases : UseCase {

    #region Constructors and parsers

    protected TractIndexUseCases() {
      // no-op
    }

    static public TractIndexUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TractIndexUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TractIndexDto AmendableRecordingActs(string recordableSubjectUID,
                                                string instrumentRecordingUID,
                                                string amendmentRecordingActTypeUID) {

      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(instrumentRecordingUID, nameof(instrumentRecordingUID));
      Assertion.Require(amendmentRecordingActTypeUID, nameof(amendmentRecordingActTypeUID));

      var amendmentRecordingActType = RecordingActType.Parse(amendmentRecordingActTypeUID);

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      FixedList<RecordingActType> appliesTo = amendmentRecordingActType.GetAppliesToRecordingActTypesList();

      FixedList<RecordingAct> acts = recordableSubject.Tract.GetRecordingActs();

      var amendableActs = acts.FindAll((x) => appliesTo.Contains(x.RecordingActType));

      return TractIndexMapper.Map(recordableSubject, amendableActs);
    }


    public void CreateRecordingAct(string recordableSubjectUID, RegistrationCommand command) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(command, nameof(command));


    }


    public TractIndexDto TractIndex(string recordableSubjectUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      FixedList<RecordingAct> acts = recordableSubject.Tract.GetRecordingActs();

      acts.Reverse();

      return TractIndexMapper.Map(recordableSubject, acts);
    }


    public void RemoveRecordingAct(string recordableSubjectUID, string recordingActUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      RecordingAct recordingAct = recordableSubject.Tract.GetRecordingAct(recordingActUID);

      RecordingDocument instrumentRecording = recordingAct.Document;

      instrumentRecording.RemoveRecordingAct(recordingAct);
    }


    #endregion Use cases

  }  // class TractIndexUseCases

}  // namespace Empiria.Land.RecordableSubjects.UseCases
