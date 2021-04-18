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

      RecordingTaskFields recordingTaskfields = MapToRecordingTaskFields(command);

      RecordingTask task = new RecordingTask(recordingTaskfields);

      RecorderExpert.Execute(task);
    }


    internal void Execute(RecordingBook book, PhysicalRecording bookEntry,
                          RegistrationCommand command) {
      Assertion.AssertObject(book, "book");
      Assertion.AssertObject(bookEntry, "bookEntry");
      Assertion.AssertObject(command, "command");

      command.EnsureIsValid();

      RecordingTaskFields recordingTaskfields = MapToRecordingTaskFields(command);

      recordingTaskfields.RecordingBookUID = book.UID;
      recordingTaskfields.BookEntryUID = bookEntry.UID;

      RecordingTask task = new RecordingTask(recordingTaskfields);

      RecorderExpert.Execute(task);
    }


    #endregion Public methods

    #region Private methods


    private RecordingTaskFields MapToRecordingTaskFields(RegistrationCommand command) {
      var fields = new RecordingTaskFields();

      fields.RecordingTaskType = MapToRecordingTaskType(command.Type);
      fields.RecordingDocumentUID = _recordingDocument.GUID;
      fields.RecordingActTypeUID = command.Payload.RecordingActTypeUID;
      fields.RecordableSubjectUID = command.Payload.RecordableSubjectUID;
      fields.PartitionType = command.Payload.PartitionType;
      fields.PartitionNo = command.Payload.PartitionNo;

      return fields;
    }


    static private RecordingTaskType MapToRecordingTaskType(RegistrationCommandType commandType) {
      switch (commandType) {
        case RegistrationCommandType.CreateAssociation:
        case RegistrationCommandType.CreateNoProperty:
          return RecordingTaskType.actNotApplyToProperty;

        case RegistrationCommandType.CreateRealEstate:
          return RecordingTaskType.createProperty;

        case RegistrationCommandType.SelectAssociation:
        case RegistrationCommandType.SelectNoProperty:
        case RegistrationCommandType.SelectRealEstate:
          return RecordingTaskType.selectProperty;

        case RegistrationCommandType.SelectRealEstatePartition:
          return RecordingTaskType.createPartition;

        default:
          throw Assertion.AssertNoReachThisCode($"There is not defined a registration rule for commandType '{commandType}'.");
      }
    }


    #endregion Private methods

  }  // class RegistrationEngine

}  // namespace Empiria.Land.Registration
