/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use cases class                         *
*  Type     : RecordingBookRegistrationUseCases          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for registration of documents in recording books.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Instruments;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.UseCases {

  /// <summary>Use cases for registration of documents in recording books.</summary>
  public class RecordingBookRegistrationUseCases : UseCase {

    #region Constructors and parsers

    protected RecordingBookRegistrationUseCases() {
      // no-op
    }

    static public RecordingBookRegistrationUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RecordingBookRegistrationUseCases>();
    }


    #endregion Constructors and parsers

    #region Command Use cases


    public FixedList<NamedEntityDto> GetRecordingSections() {
      FixedList<RecordingSection> list = RecordingSection.GetListForRecording();

      return list.MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> GetRecordingBooksList(string recorderOfficeUID,
                                                           string recordingSectionUID,
                                                           string keywords) {
      Assertion.AssertObject(recorderOfficeUID, "recorderOfficeUID");
      Assertion.AssertObject(recordingSectionUID, "recordingSectionUID");

      keywords = keywords ?? string.Empty;

      var recorderOffice = RecorderOffice.Parse(recorderOfficeUID);
      var recordingSection = RecordingSection.Parse(recordingSectionUID);


      FixedList<RecordingBook> books = RecordingBook.GetList(recorderOffice, recordingSection, keywords);

      return new FixedList<NamedEntityDto>(books.Select(x => new NamedEntityDto(x.UID, x.BookNumber)));
    }


    public RecordingBookDto GetRecordingBook(string recordingBookUID) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      return RecordingBookMapper.Map(recordingBook);
    }


    public FixedList<BookEntryShortDto> GetRecordingBookEntries(string recordingBookUID) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      FixedList<PhysicalRecording> bookEntries = recordingBook.GetRecordings();

      return RecordingBookMapper.MapBookEntriesListShortDto(bookEntries);
    }


    public FixedList<NamedEntityDto> InstrumentTypesForRecordingBooks() {
      FixedList<InstrumentType> instrumentTypes = InstrumentType.GetListForRecordingBooks();

      return new FixedList<NamedEntityDto>(instrumentTypes.Select(x => new NamedEntityDto(x.UID, x.DisplayName)));
    }


    public RecordingBookDto CreateBookEntry(string recordingBookUID,
                                            ManualEditBookEntryFields fields) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      Assertion.Assert(recordingBook.IsAvailableForManualEditing,
          $"The selected book '{recordingBook.AsText}' is not available for manual editing." +
          "It is not possible to add it a new book entry.");

      Assertion.Assert(!recordingBook.ExistsRecording(fields.BookEntry.RecordingNo),
                       $"There is a book entry with the same number {fields.BookEntry.RecordingNo}");

      var instrumentType = InstrumentType.Parse(fields.Instrument.Type.Value);

      var instrument = new Instrument(instrumentType, fields.Instrument);

      instrument.Save();

      Assertion.Assert(instrument.HasDocument,
                       "Instruments must have a recording document to be linked to a transaction.");

      var document = instrument.TryGetRecordingDocument();

      RecordingDTO recordingDTO = fields.MapToRecordingDTO(recordingBook, document);

      recordingBook.AddRecording(recordingDTO);

      document.Save();

      recordingBook.Refresh();

      return RecordingBookMapper.Map(recordingBook);
    }


    public InstrumentRecordingDto CreateNextBookEntry(string instrumentRecordingUID,
                                                      CreateNextBookEntryFields fields) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(fields, "fields");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      var instrument = Instrument.Parse(instrumentRecording.InstrumentId);

      var office = RecorderOffice.Parse(fields.RecorderOfficeUID);
      var section = RecordingSection.Parse(fields.SectionUID);

      var book = RecordingBook.GetAssignedBookForRecording(office, section,
                                                           instrument.SheetsCount);

      var nextBookEntry = book.CreateNextRecording(instrumentRecording);

      nextBookEntry.Save();

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto CreateRecordingAct(string recordingBookUID,
                                                     string bookEntryUID,
                                                     RegistrationCommand command) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");
      Assertion.AssertObject(command, "command");

      var book = RecordingBook.Parse(recordingBookUID);
      var bookEntry = PhysicalRecording.Parse(bookEntryUID);

      var instrumentRecording = bookEntry.MainDocument;

      var registrationEngine = new RegistrationEngine(instrumentRecording);

      registrationEngine.Execute(bookEntry, command);

      book.Refresh();
      bookEntry.Refresh();

      return InstrumentRecordingMapper.Map(instrumentRecording,
                                           instrumentRecording.GetTransaction());

    }


    public RecordingBookDto RemoveBookEntry(string recordingBookUID,
                                            string bookEntryUID) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");

      var book = RecordingBook.Parse(recordingBookUID);

      var bookEntry = PhysicalRecording.Parse(bookEntryUID);

      Assertion.Assert(book.Recordings.Contains(bookEntry),
                       $"Book entry '{bookEntryUID}', does not belong to book '{book.AsText}'.");

      bookEntry.Delete();

      book.Refresh();

      return RecordingBookMapper.Map(book);
    }


    public InstrumentRecordingDto RemoveRecordingAct(string recordingBookUID,
                                                     string bookEntryUID,
                                                     string recordingActUID) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");
      Assertion.AssertObject(recordingActUID, "recordingActUID");

      var book = RecordingBook.Parse(recordingBookUID);

      var bookEntry = PhysicalRecording.Parse(bookEntryUID);

      var recordingAct = RecordingAct.Parse(recordingActUID);

      var instrumentRecording = recordingAct.Document;

      Assertion.Assert(book.Recordings.Contains(bookEntry),
                       $"Book entry '{bookEntryUID}', does not belong to book '{book.AsText}'.");

      Assertion.Assert(bookEntry.RecordingActs.Contains(recordingAct),
                      $"Book entry '{bookEntryUID}', does not contains recording act '{recordingAct.UID}'.");


      instrumentRecording.RemoveRecordingAct(recordingAct);

      book.Refresh();
      bookEntry.Refresh();

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto RemoveBookEntryFromInstrument(string instrumentRecordingUID,
                                                                string bookEntryUID) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      var instrumentRecordings = PhysicalRecording.GetDocumentRecordings(instrumentRecording.Id);

      var bookEntry = instrumentRecordings.Find(x => x.UID == bookEntryUID);

      Assertion.AssertObject(bookEntry,
            $"Book recording entry '{bookEntryUID}', does not belong to instrument recording '{instrumentRecordingUID}'.");

      bookEntry.Delete();

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    public InstrumentRecordingDto UpdateBookEntryInstrument(string instrumentRecordingUID,
                                                            string bookEntryUID,
                                                            ManualEditBookEntryFields fields) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");
      Assertion.AssertObject(fields, "fields");

      fields.EnsureIsValid();

      var bookEntry = PhysicalRecording.Parse(bookEntryUID);

      bookEntry.Update(fields.BookEntry.MapToRecordingDTO(bookEntry.RecordingBook,
                       bookEntry.MainDocument));

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      Instrument instrument = Instrument.Parse(instrumentRecording.InstrumentId);

      instrument.Update(fields.Instrument);

      instrument.Save();

      if (!instrumentRecording.HasTransaction) {
        instrumentRecording.SetDates(fields.BookEntry.PresentationTime,
                                     fields.BookEntry.AuthorizationDate);
      }

      return InstrumentRecordingMapper.Map(instrumentRecording);
    }


    #endregion Command Use cases

  }  // class RecordingBookRegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
