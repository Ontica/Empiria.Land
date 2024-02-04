/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingBook                                  Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Indicates the status of a recording book according to it use in historic capture.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes.Time;
using Empiria.Json;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Indicates the status of a recording book according to it use in historic capture.</summary>
  public enum RecordingBookStatus {
    Pending = 'P',
    Assigned = 'A',
    Revision = 'R',
    Opened = 'O',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Represents a physical recording book. A recording book can have a parent recording book
  /// and belongs to a recorder of deeds office. Instances of this type have a recording book type.</summary>
  public class RecordingBook : BaseObject {

    #region Fields

    private FixedList<BookEntry> bookEntries = null;

    #endregion Fields

    #region Constructors and parsers

    private RecordingBook() {
      // Required by Empiria Framework.
    }


    static public RecordingBook Parse(int id) {
      return BaseObject.ParseId<RecordingBook>(id);
    }


    static public RecordingBook Parse(string uid) {
      return BaseObject.ParseKey<RecordingBook>(uid);
    }


    static public RecordingBook Empty {
      get { return BaseObject.ParseEmpty<RecordingBook>(); }
    }


    static public RecordingBook GetAssignedBookForRecording(RecorderOffice office, RecordingSection section,
                                                            int sheetsCount) {
      Assertion.Require(office, "office");
      Assertion.Require(section, "section");
      Assertion.Require(sheetsCount > 0, "sheetsCount");

      RecordingBook openedBook = RecordingBooksData.GetOpenedBook(office, section);
      if (openedBook.HasSpaceForRecording(sheetsCount)) {
        return openedBook;
      } else {
        return openedBook.CloseAndCreateNew();
      }
    }

    public static FixedList<RecordingBook> GetList(RecorderOffice recorderOffice,
                                                   RecordingSection recordingSection,
                                                   string keywords) {
      return RecordingBooksData.GetRecordingBooksInSection(recorderOffice,
                                                           recordingSection,
                                                           keywords);
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("RecorderOfficeId")]
    public RecorderOffice RecorderOffice {
      get;
      set;
    }

    [DataField("RecordingSectionId")]
    public RecordingSection RecordingSection {
      get;
      set;
    }

    [DataField("BookNo")]
    private string _bookNumber = "N/A";
    public string BookNumber {
      get { return _bookNumber; }
      set { _bookNumber = (value == String.Empty || value == "00" || value == "0") ? "N/A" : value; }
    }

    [DataField("BookAsText")]
    public string AsText {
      get;
      set;
    }


    public bool IsAvailableForManualEditing {
      get {
        return (this.Status == RecordingBookStatus.Assigned ||
                this.Status == RecordingBookStatus.Pending ||
                this.Status == RecordingBookStatus.Revision);
      }
    }

    [DataField("BookExtData")]
    internal JsonObject ExtensionData {
      get;
      private set;
    }

    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.AsText);
      }
    }

    [DataField("StartRecordingIndex")]
    public int StartRecordingIndex {
      get;
      set;
    }

    [DataField("EndRecordingIndex")]
    public int EndRecordingIndex {
      get;
      set;
    }

    [DataField("BookStatus", Default = RecordingBookStatus.Pending)]
    public RecordingBookStatus Status {
      get;
      set;
    }


    public FixedList<BookEntry> BookEntries {
      get {
        if (bookEntries == null) {
          bookEntries = RecordingBooksData.GetRecordingBookEntries(this);
        }
        return bookEntries;
      }
    }

    public bool UsePerpetualNumbering {
      get {
        return this.RecordingSection.UsesPerpetualNumbering;
      }
    }

    #endregion Public properties

    #region Public methods

    public BookEntry AddBookEntry(BookEntryDto dto) {
      Assertion.Require(dto, nameof(dto));

      var bookEntry = new BookEntry(dto);

      bookEntry.Save();

      return bookEntry;
    }


    public BookEntry AddBookEntry(LandRecord landRecord, string bookEntryNumber) {
      Assertion.Require(landRecord, nameof(landRecord));
      Assertion.Require(bookEntryNumber, nameof(bookEntryNumber));

      Assertion.Require(!landRecord.IsEmptyInstance, "landRecord can't be the empty instance.");

      return new BookEntry(this, landRecord, RecordingBook.FormatBookEntryNumber(bookEntryNumber));
    }


    public BookEntry CreateNextBookEntry(LandRecord landRecord) {
      Assertion.Require(landRecord, nameof(landRecord));
      Assertion.Require(landRecord.Instrument.SheetsCount > 0, "Instrument field SheetsCount must be greater than zero.");

      int bookEntryNumber = RecordingBooksData.GetNextBookEntryNumberWithNoReuse(this);

      return new BookEntry(this, landRecord, RecordingBook.FormatBookEntryNumber(bookEntryNumber));
    }


    public bool ExistsBookEntry(string bookEntryNumber) {
      string formatted = RecordingBook.FormatBookEntryNumber(bookEntryNumber);

      return BookEntries.Contains((x) => x.Number == formatted);
    }


    public BookEntry TryGetBookEntry(string bookEntryNumber) {
      string formatted = RecordingBook.FormatBookEntryNumber(bookEntryNumber);

      return BookEntries.Find((x) => x.Number == formatted);
    }


    static public string FormatBookEntryNumber(int bookEntryNumber) {
      return bookEntryNumber.ToString("0000");
    }


    static public string FormatBookEntryNumber(string bookEntryNumber) {
      try {
        bookEntryNumber = bookEntryNumber.Replace(" ", String.Empty);
        bookEntryNumber = bookEntryNumber.Replace("-", "/");

        string[] parts = bookEntryNumber.Split('/');

        string temp = int.Parse(parts[0]).ToString("0000");
        for (int i = 1; i <= parts.Length - 2; i++) {
          temp += "/" + int.Parse(parts[i]).ToString("000");
        }
        if (parts.Length == 1) {                                              // e.g 0456
          // no-op
        } else if (!EmpiriaString.IsInteger(parts[parts.Length - 1])) {
          temp += "-" + parts[parts.Length - 1];                              // e.g 0456/123-bis
        } else {
          temp += "/" + int.Parse(parts[parts.Length - 1]).ToString("000");   // e.g  0456/123/423
        }
        return temp;
      } catch {
        throw new LandRegistrationException(LandRegistrationException.Msg.InvalidBookEntryNumber,
                                            bookEntryNumber);
      }
    }


    public void Refresh() {
      this.bookEntries = null;
    }

    protected override void OnSave() {
      RecordingBooksData.WriteRecordingBook(this);
    }

    #endregion Public methods

    #region Workflow data and methods


    private TimeFrame _timePeriod = TimeFrame.Default;
    public TimeFrame BookEntriesControlTimePeriod {
      get { return _timePeriod; }
      set {
        _timePeriod = value;
      }
    }


    public string Description {
      get;
      set;
    }

    #endregion Workflow data and methods

    #region Private methods

    private int CalculateTotalSheets() {
      return RecordingBooksData.GetBookTotalSheets(this);
    }

    private RecordingBook Clone() {
      RecordingBook newBook = new RecordingBook();

      newBook.RecorderOffice = this.RecorderOffice;
      newBook.RecordingSection = this.RecordingSection;
      newBook.BookEntriesControlTimePeriod = this.BookEntriesControlTimePeriod;
      newBook.Status = this.Status;

      return newBook;
    }

    private RecordingBook CloseAndCreateNew() {
      this.Status = RecordingBookStatus.Closed;
      if (!this.UsePerpetualNumbering) {
        this.EndRecordingIndex = this.BookEntries.Count;
      }
      this.Save();

      RecordingBook newBook = this.Clone();

      newBook.Status = RecordingBookStatus.Opened;

      if (newBook.UsePerpetualNumbering) {
        newBook.StartRecordingIndex = this.StartRecordingIndex + 50;
        newBook.EndRecordingIndex = this.EndRecordingIndex + 50;
        newBook.BookNumber = newBook.StartRecordingIndex.ToString("0000") + "-" +
                             newBook.EndRecordingIndex.ToString("0000");
      } else {
        newBook.StartRecordingIndex = 1;
        newBook.EndRecordingIndex = 250;
        newBook.BookNumber = (int.Parse(this.BookNumber) + 1).ToString("0000");
      }

      newBook.AsText = "Volumen " + newBook.BookNumber;

      newBook.Save();

      return newBook;
    }

    private bool HasSpaceForRecording(int sheetsCount) {
      if (this.UsePerpetualNumbering) {
        return (RecordingBooksData.GetLastBookEntryNumber(this) < this.EndRecordingIndex);
      }
      // !UsePerpetualNumbering
      int currentBookSheets = this.CalculateTotalSheets();
      int newTotalSheets = currentBookSheets + sheetsCount;

      int lowerBound = 275;
      int upperBound = 286;

      if (newTotalSheets <= lowerBound) {
        return true;
      } else if (currentBookSheets < lowerBound && newTotalSheets <= upperBound) {
        return true;
      } else {
        return false;
      }
    }

    #endregion Private methods

  } // class RecordingBook

} // namespace Empiria.Land.Registration
