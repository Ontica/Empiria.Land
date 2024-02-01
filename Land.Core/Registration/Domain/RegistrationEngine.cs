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

  public class RegistrationEngine {

    private readonly RecordingDocument _landRecord;

    #region Public methods

    public RegistrationEngine(RecordingDocument landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));

      _landRecord = landRecord;
    }


    static public BookEntry CreatePrecedentBookEntry(RecordingBook book,
                                                     string bookEntryNumber,
                                                     DateTime presentationTime,
                                                     DateTime authorizationDate) {
      Assertion.Require(book.IsAvailableForManualEditing,
          $"El {book.AsText} está cerrado, por lo que no es posible agregarle nuevas inscripciones.");

      if (book.ExistsBookEntry(bookEntryNumber)) {
        Assertion.RequireFail(
          "La partida indicada ya existe en el libro seleccionado,\n" +
          "y no es posible generar más de un folio de predio\n" +
          "en una misma partida o antecedente.\n\n" +
          "Si se requiere registrar más de un predio en una partida,\n" +
          "favor de consultarlo con el área de soporte. Gracias.");
      }

      var fields = new InstrumentFields();

      fields.Summary = $"Instrumento de la inscripción {bookEntryNumber} del {book.AsText}.";

      var instrument = new Instrument(InstrumentType.Parse(InstrumentTypeEnum.Resumen), fields);

      instrument.Save();

      var landRecord = RecordingDocument.CreateFromInstrument(instrument);

      landRecord.SetDates(presentationTime, authorizationDate);

      landRecord.Save();

      BookEntry bookEntry = book.AddBookEntry(landRecord, bookEntryNumber);

      bookEntry.Save();

      book.Refresh();

      return bookEntry;
    }

    internal void Execute(RegistrationCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      RecordingTaskFields recordingTaskfields = MapToRecordingTaskFields(command);

      RecordingTask task = new RecordingTask(recordingTaskfields);

      RecorderExpert.Execute(task);
    }


    internal void Execute(BookEntry bookEntry,
                          RegistrationCommand command) {
      Assertion.Require(bookEntry, nameof(bookEntry));
      Assertion.Require(command, nameof(command));

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
      fields.LandRecordUID = _landRecord.GUID;
      fields.RecordingActTypeUID = command.Payload.RecordingActTypeUID;
      fields.RecordableSubjectUID = command.Payload.RecordableSubjectUID;
      fields.PartitionType = command.Payload.PartitionType;
      fields.PartitionNo = command.Payload.PartitionNo;

      if (MustCreatePrecedentBookEntry(command)) {
        var book = RecordingBook.Parse(command.Payload.RecordingBookUID);
        var bookEntryNo = EmpiriaString.TrimAll(command.Payload.BookEntryNo);
        var newBookEntry = CreatePrecedentBookEntry(book, bookEntryNo,
                                                    command.Payload.PresentationTime,
                                                    command.Payload.AuthorizationDate);
        fields.PrecedentBookEntryUID = newBookEntry.UID;

      } else {
        fields.PrecedentBookEntryUID = command.Payload.BookEntryUID;

      }

      fields.TargetRecordingActUID = command.Payload.AmendedRecordingActUID;

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
        case RegistrationCommandType.AssociationTractIndex:
        case RegistrationCommandType.NoPropertyTractIndex:
        case RegistrationCommandType.RealEstateTractIndex:
          return RecordingTaskType.selectProperty;

        case RegistrationCommandType.SelectAssociationAntecedent:
        case RegistrationCommandType.SelectNoPropertyAntecedent:
        case RegistrationCommandType.SelectRealEstateAntecedent:
          return RecordingTaskType.createPropertyOnAntecedent;

        case RegistrationCommandType.CreateRealEstatePartition:
        case RegistrationCommandType.RealEstateTractIndexPartition:
          return RecordingTaskType.createPartition;

        case RegistrationCommandType.CreateRealEstatePartitionForAntecedent:
          return RecordingTaskType.createPartitionAndPropertyOnAntecedent;

        case RegistrationCommandType.SelectAssociationAct:
        case RegistrationCommandType.SelectNoPropertyAct:
        case RegistrationCommandType.SelectRealEstateAct:
        case RegistrationCommandType.AmendAssociationTractIndexAct:
        case RegistrationCommandType.AmendNoPropertyTractIndexAct:
        case RegistrationCommandType.AmendRealEstateTractIndexAct:

          return RecordingTaskType.actAppliesToOtherRecordingAct;

        default:
          throw Assertion.EnsureNoReachThisCode($"There is not defined a registration rule for commandType '{commandType}'.");
      }
    }


    static private bool MustCreatePrecedentBookEntry(RegistrationCommand command) {
      if (!String.IsNullOrWhiteSpace(command.Payload.BookEntryUID)) {
        return false;
      }

      if (String.IsNullOrWhiteSpace(command.Payload.BookEntryNo)) {
        return false;
      }

      if (command.Type == RegistrationCommandType.SelectAssociationAntecedent ||
          command.Type == RegistrationCommandType.SelectNoPropertyAntecedent ||
          command.Type == RegistrationCommandType.SelectRealEstateAntecedent ||
          command.Type == RegistrationCommandType.CreateRealEstatePartitionForAntecedent) {
        return true;
      }

      return false;
    }


    #endregion Private methods

    }  // class RegistrationEngine

}  // namespace Empiria.Land.Registration
