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

    #region Query Use cases

    public FixedList<NamedEntityDto> GetRecordingSections(RecorderOffice recorderOffice) {
      Assertion.Require(recorderOffice, nameof(recorderOffice));

      FixedList<RecordingSection> list = RecordingSection.GetList(recorderOffice);

      return list.MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> GetRecordingBooksList(string recorderOfficeUID,
                                                           string recordingSectionUID,
                                                           string keywords) {
      Assertion.Require(recorderOfficeUID, nameof(recorderOfficeUID));
      Assertion.Require(recordingSectionUID, nameof(recordingSectionUID));

      keywords = keywords ?? string.Empty;

      var recorderOffice = RecorderOffice.Parse(recorderOfficeUID);
      var recordingSection = RecordingSection.Parse(recordingSectionUID);

      FixedList<RecordingBook> books = RecordingBook.GetList(recorderOffice, recordingSection, keywords);

      return books.Select(x => new NamedEntityDto(x.UID, x.BookNumber))
                  .ToFixedList();
    }


    public RecordingBookDto GetRecordingBook(string recordingBookUID) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      return RecordingBookMapper.Map(recordingBook);
    }


    public FixedList<BookEntryShortDto> GetRecordingBookEntries(string recordingBookUID) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      FixedList<BookEntry> bookEntries = recordingBook.GetBookEntries();

      return RecordingBookMapper.MapBookEntriesListShortDto(bookEntries);
    }


    public FixedList<NamedEntityDto> InstrumentTypesForRecordingBooks() {
      FixedList<InstrumentType> instrumentTypes = InstrumentType.GetListForRecordingBooks();

      return instrumentTypes.Select(x => new NamedEntityDto(x.UID, x.DisplayName))
                            .ToFixedList();
    }

    #endregion Query Use cases

    #region Command Use cases

    public RecordingBookDto CreateBookEntry(string recordingBookUID,
                                            ManualEditBookEntryFields fields) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureIsValid();

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      Assertion.Require(recordingBook.IsAvailableForManualEditing,
          $"The selected book '{recordingBook.AsText}' is not available for manual editing." +
          "It is not possible to add it a new book entry.");

      Assertion.Require(!recordingBook.ExistsBookEntry(fields.BookEntry.RecordingNo),
                       $"There is a book entry with the same number {fields.BookEntry.RecordingNo}");

      var instrumentType = InstrumentType.Parse(fields.Instrument.Type.Value);

      var instrument = new Instrument(instrumentType, fields.Instrument);

      instrument.Save();

      var landRecord = RecordingDocument.CreateFromInstrument(instrument.Id, instrument.InstrumentType.Id, instrument.Kind);

      instrument.FillRecordingDocument(landRecord);

      landRecord.Save();

      BookEntryDto bookEntryDto = fields.MapToBookEntryDto(recordingBook, landRecord);

      recordingBook.AddBookEntry(bookEntryDto);


      recordingBook.Refresh();

      return RecordingBookMapper.Map(recordingBook);
    }


    public LandRecordDto CreateNextBookEntry(string landRecordUID,
                                             CreateNextBookEntryFields fields) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(fields, nameof(fields));

      var landRecord = RecordingDocument.ParseGuid(landRecordUID);

      var instrument = Instrument.Parse(landRecord.InstrumentId);

      var office = RecorderOffice.Parse(fields.RecorderOfficeUID);
      var section = RecordingSection.Parse(fields.SectionUID);

      var book = RecordingBook.GetAssignedBookForRecording(office, section,
                                                           instrument.SheetsCount);

      var nextBookEntry = book.CreateNextBookEntry(landRecord);

      nextBookEntry.Save();

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto CreateRecordingAct(string recordingBookUID,
                                            string bookEntryUID,
                                            RegistrationCommand command) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));
      Assertion.Require(command, nameof(command));

      var book = RecordingBook.Parse(recordingBookUID);
      var bookEntry = BookEntry.Parse(bookEntryUID);

      var landRecord = bookEntry.LandRecord;

      var registrationEngine = new RegistrationEngine(landRecord);

      registrationEngine.Execute(bookEntry, command);

      book.Refresh();
      bookEntry.Refresh();

      return LandRecordMapper.Map(landRecord);
    }


    public RecordingBookDto RemoveBookEntry(string recordingBookUID,
                                            string bookEntryUID) {
      Assertion.Require(recordingBookUID, nameof(recordingBookUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));

      var book = RecordingBook.Parse(recordingBookUID);

      var bookEntry = BookEntry.Parse(bookEntryUID);

      Assertion.Require(book.BookEntries.Contains(bookEntry),
                       $"Book entry '{bookEntryUID}', does not belong to book '{book.AsText}'.");

      bookEntry.Delete();

      book.Refresh();

      return RecordingBookMapper.Map(book);
    }


    public LandRecordDto RemoveRecordingAct(string recordingBookUID,
                                            string bookEntryUID,
                                            string recordingActUID) {

      Assertion.Require(recordingBookUID, nameof(recordingBookUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      var book = RecordingBook.Parse(recordingBookUID);

      var bookEntry = BookEntry.Parse(bookEntryUID);

      var recordingAct = RecordingAct.Parse(recordingActUID);

      var landRecord = recordingAct.LandRecord;

      Assertion.Require(book.BookEntries.Contains(bookEntry),
                       $"Book entry '{bookEntryUID}', does not belong to book '{book.AsText}'.");

      Assertion.Require(bookEntry.RecordingActs.Contains(recordingAct),
                      $"Book entry '{bookEntryUID}', does not contains recording act '{recordingAct.UID}'.");


      landRecord.RemoveRecordingAct(recordingAct);

      book.Refresh();
      bookEntry.Refresh();

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto RemoveBookEntryFromInstrument(string landRecordUID,
                                                       string bookEntryUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));

      var landRecord = RecordingDocument.ParseGuid(landRecordUID);

      var instrumentBookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      var bookEntry = instrumentBookEntries.Find(x => x.UID == bookEntryUID);

      Assertion.Require(bookEntry,
            $"Book recording entry '{bookEntryUID}' does not belong to instrument recording '{landRecordUID}'.");

      bookEntry.Delete();

      return LandRecordMapper.Map(landRecord);
    }


    public LandRecordDto UpdateBookEntryInstrument(string landRecordUID,
                                                   string bookEntryUID,
                                                   ManualEditBookEntryFields fields) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(bookEntryUID, nameof(bookEntryUID));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureIsValid();

      var bookEntry = BookEntry.Parse(bookEntryUID);

      bookEntry.Update(fields.BookEntry.MapToBookEntryDto(bookEntry.RecordingBook,
                       bookEntry.LandRecord));

      var landRecord = RecordingDocument.ParseGuid(landRecordUID);

      Instrument instrument = Instrument.Parse(landRecord.InstrumentId);

      instrument.Update(fields.Instrument);

      instrument.Save();

      if (!landRecord.HasTransaction) {
        landRecord.SetDates(fields.BookEntry.PresentationTime,
                                     fields.BookEntry.AuthorizationDate);
      }

      return LandRecordMapper.Map(landRecord);
    }

    #endregion Command Use cases

  }  // class RecordingBookRegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
