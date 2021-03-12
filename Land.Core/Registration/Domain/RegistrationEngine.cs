/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Coordinator                             *
*  Type     : RegistrationEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs recording act registration through Registration commands.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  internal class RegistrationEngine {

    private readonly RecordingDocument _recordingDocument;

    #region Public methods

    public RegistrationEngine(RecordingDocument recordingDocument) {
      _recordingDocument = recordingDocument;
    }

    internal void Execute(RegistrationCommand command) {
      Assertion.AssertObject(command, "command");

      command.EnsureIsValid();

      RecordingTask task = BuildRecordingTask(command);

      RecorderExpert.Execute(task);
    }


    #endregion Public methods

    #region Private methods

    private RecordingTask BuildRecordingTask(RegistrationCommand command) {
      var fields = new RecordingTaskFields();

      fields.RecordingTaskType = MapToRecordingTaskType(command.Type);
      fields.RecordingDocumentUID = _recordingDocument.UID;
      fields.RecordingActTypeUID = command.Payload.RecordingActTypeUID;
      fields.RecordableSubjectUID = command.Payload.RecordableSubjectUID;

      return new RecordingTask(fields);
    }


    static private RecordingTaskType MapToRecordingTaskType(RegistrationCommandType commandType) {
      switch (commandType) {
        case RegistrationCommandType.CreateAssociation:
          return RecordingTaskType.actNotApplyToProperty;
        case RegistrationCommandType.CreateRealEstate:
          return RecordingTaskType.createProperty;
        default:
          throw Assertion.AssertNoReachThisCode($"There is not defined a registration rule for commandType '{commandType}'.");
      }
    }


    #endregion Private methods

  }  // class RegistrationEngine

}  // namespace Empiria.Land.Registration
