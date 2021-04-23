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
using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;
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


    internal void Execute(PhysicalRecording bookEntry,
                          RegistrationCommand command) {
      Assertion.AssertObject(bookEntry, "bookEntry");
      Assertion.AssertObject(command, "command");

      command.EnsureIsValid();

      RecordingTaskFields recordingTaskfields = MapToRecordingTaskFields(command);

      recordingTaskfields.RecordingBookEntryUID = bookEntry.UID;

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

      if (MustCreatePrecedentBookEntry(command)) {
        var newBookEntry = CreatePrecedentBookEntry(command);
        fields.PrecedentBookEntryUID = newBookEntry.UID;
      } else {
        fields.PrecedentBookEntryUID = command.Payload.BookEntryUID;
      }
      return fields;
    }


    private PhysicalRecording CreatePrecedentBookEntry(RegistrationCommand command) {
      var book = RecordingBook.Parse(command.Payload.RecordingBookUID);

      Assertion.Assert(book.IsAvailableForManualEditing,
          $"El {book.AsText} está cerrado, por lo que no es posible agregarle nuevas inscripciones.");

      var bookEntryNo = EmpiriaString.TrimAll(command.Payload.BookEntryNo);

      if (book.ExistsRecording(bookEntryNo)) {
        Assertion.AssertFail(
          "La partida indicada ya existe en el libro seleccionado,\n" +
          "y no es posible generar más de un folio de predio\n" +
          "en una misma partida o antecedente.\n\n" +
          "Si se requiere registrar más de un predio en una partida,\n" +
          "favor de consultarlo con el área de soporte. Gracias.");
      }

      var fields = new InstrumentFields();

      fields.Summary = $"Instrumento de la inscripción {bookEntryNo} del {book.AsText}.";

      var instrument = new Instrument(InstrumentType.Parse(InstrumentTypeEnum.Resumen), fields);

      instrument.Save();

      RecordingDocument document = instrument.TryGetRecordingDocument();

      var bookEntry = book.AddRecording(document, bookEntryNo);

      bookEntry.Save();

      book.Refresh();

      return bookEntry;
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

        case RegistrationCommandType.SelectAssociationAntecedent:
        case RegistrationCommandType.SelectNoPropertyAntecedent:
        case RegistrationCommandType.SelectRealEstateAntecedent:
          return RecordingTaskType.createPropertyOnAntecedent;

        case RegistrationCommandType.CreateRealEstatePartition:
          return RecordingTaskType.createPartition;

        case RegistrationCommandType.CreateRealEstatePartitionForAntecedent:
          return RecordingTaskType.createPartitionAndPropertyOnAntecedent;

        default:
          throw Assertion.AssertNoReachThisCode($"There is not defined a registration rule for commandType '{commandType}'.");
      }
    }


    static private bool MustCreatePrecedentBookEntry(RegistrationCommand command) {
      if (String.IsNullOrWhiteSpace(command.Payload.BookEntryNo)) {
        return false;
      }

      if (command.Type == RegistrationCommandType.SelectAssociationAntecedent ||
          command.Type == RegistrationCommandType.SelectNoPropertyAntecedent ||
          command.Type == RegistrationCommandType.SelectRealEstateAntecedent) {
        return true;
      }
      return false;
    }


    #endregion Private methods

    }  // class RegistrationEngine

}  // namespace Empiria.Land.Registration
