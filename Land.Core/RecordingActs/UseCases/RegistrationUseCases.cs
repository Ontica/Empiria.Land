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

using Empiria.Land.Registration.Adapters;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases that perform registrar tasks over legal instruments.</summary>
  public class RegistrationUseCases : UseCase {

    #region Constructors and parsers

    static public RegistrationUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RegistrationUseCases>();
    }

    #endregion Constructors and parsers


    #region Query Use cases

    public SubjectHistoryDto GetTractIndex(string landRecordUID, string recordingActUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));


      var landRecord = LandRecord.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      FixedList<RecordingAct> tract = recordingAct.Resource.Tract.GetRecordingActs();

      return SubjectHistoryMapper.Map(recordingAct.Resource, tract);
    }


    public RecordingActDto GetRecordingAct(string landRecordUID, string recordingActUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      var landRecord = LandRecord.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      return RecordingActMapper.Map(recordingAct);
    }

    #endregion Query Use cases

    #region Command Use cases

    public LandRecordDto CreateRecordingAct(string landRecordUID,
                                                     RegistrationCommand command) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(command, nameof(command));

      var landRecord = LandRecord.ParseGuid(landRecordUID);

      var registrationEngine = new RegistrationEngine(landRecord);

      registrationEngine.Execute(command);

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto RemoveRecordingAct(string landRecordUID,
                                                     string recordingActUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      var landRecord = LandRecord.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      landRecord.RemoveRecordingAct(recordingAct);

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto UpdateRecordableSubject(string landRecordUID,
                                                          string recordingActUID,
                                                          RecordableSubjectFields fields) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));
      Assertion.Require(fields, nameof(fields));

      var landRecord = LandRecord.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      var updater = new RecordableSubjectUpdater();

      updater.Update(recordingAct, fields);

      return LandRecordMapper.Map(landRecord);
    }


    public RecordingActDto UpdateRecordingAct(string landRecordUID,
                                              string recordingActUID,
                                              RecordingActFields fields) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));
      Assertion.Require(fields, nameof(fields));

      var landRecord = LandRecord.ParseGuid(landRecordUID);

      RecordingAct recordingAct = landRecord.GetRecordingAct(recordingActUID);

      if (fields.TypeUID.Length != 0) {
        var newRecordingActType = RecordingActType.Parse(fields.TypeUID);

        if (!newRecordingActType.Equals(recordingAct.RecordingActType)) {
          recordingAct.ChangeRecordingActType(newRecordingActType);
        }
      }

      recordingAct.Update(fields);

      recordingAct.Save();

      return RecordingActMapper.Map(recordingAct);
    }


    #endregion Command Use cases

  }  // class RegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
