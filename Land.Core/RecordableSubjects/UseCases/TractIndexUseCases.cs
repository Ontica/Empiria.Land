﻿/* Empiria Land **********************************************************************************************
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
using Empiria.Land.Registration.UseCases;

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

    public SubjectHistoryDto AmendableRecordingActs(string recordableSubjectUID,
                                                    string amendmentRecordingActTypeUID,
                                                    string landRecordUID,
                                                    DateTime date) {

      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(amendmentRecordingActTypeUID, nameof(amendmentRecordingActTypeUID));

      if (!string.IsNullOrWhiteSpace(landRecordUID)) {
        var landRecord = LandRecord.ParseGuid(landRecordUID);
        date = landRecord.AuthorizationTime != ExecutionServer.DateMinValue ?
                                      landRecord.AuthorizationTime : landRecord.PresentationTime;
      }

      var amendmentRecordingActType = RecordingActType.Parse(amendmentRecordingActTypeUID);

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      FixedList<RecordingActType> appliesTo = amendmentRecordingActType.GetAppliesToRecordingActTypesList();

      FixedList<RecordingAct> acts = recordableSubject.Tract.GetRecordingActs();

      var amendableActs = acts.FindAll(x => appliesTo.Contains(x.RecordingActType) &&
                                            ((x.LandRecord.AuthorizationTime < date &&
                                              x.LandRecord.AuthorizationTime != ExecutionServer.DateMaxValue) ||
                                              x.LandRecord.PresentationTime < date &&
                                              x.LandRecord.AuthorizationTime == ExecutionServer.DateMaxValue));

      return SubjectHistoryMapper.Map(recordableSubject, amendableActs);
    }


    public void CloseTractIndex(string recordableSubjectUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));

      _ = Resource.ParseGuid(recordableSubjectUID);

    }


    public void CreateRecordingAct(string recordableSubjectUID, RegistrationCommand command) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      Assertion.Require(command.Payload.RecordableSubjectUID.Length == 0 ||
                        command.Payload.RecordableSubjectUID == recordableSubjectUID,
                        "RecordableSubjectUID value is inconsistent.");

      command.Payload.RecordableSubjectUID = recordableSubjectUID;

      EnsureHasBookEntry(command);

      using (var usecase = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        usecase.CreateRecordingAct(command.Payload.RecordingBookUID,
                                   command.Payload.BookEntryUID, command);
      }
    }


    public SubjectHistoryDto TractIndex(string recordableSubjectUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      FixedList<RecordingAct> acts = recordableSubject.Tract.GetFullRecordingActs();

      return SubjectHistoryMapper.Map(recordableSubject, acts);
    }


    public void OpenTractIndex(string recordableSubjectUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));

      _ = Resource.ParseGuid(recordableSubjectUID);
    }


    public void RemoveRecordingAct(string recordableSubjectUID, string recordingActUID) {
      Assertion.Require(recordableSubjectUID, nameof(recordableSubjectUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      var recordableSubject = Resource.ParseGuid(recordableSubjectUID);

      RecordingAct recordingAct = recordableSubject.Tract.GetRecordingAct(recordingActUID);

      LandRecord landRecord = recordingAct.LandRecord;

      landRecord.RemoveRecordingAct(recordingAct);
    }

    #endregion Use cases

    #region Helpers

    private void EnsureHasBookEntry(RegistrationCommand command) {
      if (!String.IsNullOrWhiteSpace(command.Payload.BookEntryUID)) {
        return;
      }

      string bookEntryNo = command.Payload.BookEntryNo;

      var book = RecordingBook.Parse(command.Payload.RecordingBookUID);

      BookEntry bookEntry = book.TryGetBookEntry(bookEntryNo);

      if (bookEntry == null) {
        bookEntry = RegistrationEngine.CreatePrecedentBookEntry(book, bookEntryNo,
                                                                command.Payload.PresentationTime,
                                                                command.Payload.AuthorizationDate);
      }

      command.Payload.BookEntryUID = bookEntry.UID;
      command.Payload.BookEntryNo = string.Empty;
    }


    #endregion Helpers

  }  // class TractIndexUseCases

}  // namespace Empiria.Land.RecordableSubjects.UseCases
