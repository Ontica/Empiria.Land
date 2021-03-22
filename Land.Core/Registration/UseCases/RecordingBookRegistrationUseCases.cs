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


    public FixedList<NamedEntityDto> GetRecordingBookEntries(string recordingBookUID) {
      Assertion.AssertObject(recordingBookUID, "recordingBookUID");

      var recordingBook = RecordingBook.Parse(recordingBookUID);

      FixedList<PhysicalRecording> bookEntries = recordingBook.GetRecordings();

      return new FixedList<NamedEntityDto>(bookEntries.Select(x => new NamedEntityDto(x.UID, x.Number)));
    }


    public InstrumentRecordingDto CreateNextBookEntry(string instrumentRecordingUID,
                                                      RecordingBookEntryFields fields) {
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

      return InstrumentRecordingMapper.Map(instrumentRecording, instrumentRecording.GetTransaction());
    }

    public InstrumentRecordingDto RemoveBookEntry(string instrumentRecordingUID,
                                                  string bookEntryUID) {
      Assertion.AssertObject(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.AssertObject(bookEntryUID, "bookEntryUID");

      var instrumentRecording = RecordingDocument.ParseGuid(instrumentRecordingUID);

      var instrumentRecordings = PhysicalRecording.GetDocumentRecordings(instrumentRecording.Id);

      var bookEntry = instrumentRecordings.Find(x => x.UID == bookEntryUID);

      Assertion.AssertObject(bookEntry,
            $"Book recording entry '{bookEntryUID}', does not belong to instrument recording '{instrumentRecordingUID}'.");

      bookEntry.Delete();

      return InstrumentRecordingMapper.Map(instrumentRecording, instrumentRecording.GetTransaction());
    }

    #endregion Command Use cases

  }  // class RecordingBookRegistrationUseCases

}  // namespace Empiria.Land.Registration.UseCases
