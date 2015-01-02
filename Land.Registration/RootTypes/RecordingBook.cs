/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingBook                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording book. A recording book can have a parent recording book and always     *
*              belongs to a recorder of deeds office. Instances of this type have a recording book type.     *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  public enum RecordingBookStatus {
    Pending = 'P',
    Assigned = 'A',
    Revision = 'R',
    Opened = 'O',
    Closed = 'C',
    Deleted = 'X'
  }

  //public enum RecordingBookType {
  //  Root = 'R',
  //  Section = 'S',
  //  Book = 'B',
  //  Volume = 'V'
  //}

  /// <summary>Represents a recording book. A recording book can have a parent recording book and always
  /// belongs to a recorder of deeds office. Instances of this type have a recording book type.</summary>
  public class RecordingBook : BaseObject {

    #region Fields

    static public bool UseBookAttachments = ConfigurationData.GetBoolean("RecordingBook.UseAttachments");
    static public bool UseBookLevel = ConfigurationData.GetBoolean("RecordingBookType.UseBookLevel");

    private FixedList<Recording> recordings = null;

    #endregion Fields

    #region Constructors and parsers

    private RecordingBook() {
      // Required by Empiria Framework.
    }

    internal RecordingBook(RecorderOffice recorderOffice, string recordingBookTag) {
      throw new NotImplementedException();
      //this.RecorderOffice = recorderOffice;
      //this.BookNumber = recordingBookTag;
      //this.AsText = recordingBookTag;
      //this.FullName = recorderOffice.Alias + " " + this.Parent.BuildChildFullName(recordingBookTag);
    }

    static public RecordingBook Parse(int id) {
      return BaseObject.ParseId<RecordingBook>(id);
    }

    static public RecordingBook Empty {
      get { return BaseObject.ParseEmpty<RecordingBook>(); }
    }

    static public RecordingBook GetAssignedBookForRecording(RecorderOffice office, RecordingSection section,
                                                            RecordingDocument document) {
      throw new NotImplementedException();
      //Assertion.AssertObject(document, "document");
      //Assertion.Assert(!document.IsEmptyInstance && !document.IsNew,
      //                 "Document can't be neither an empty or unsaved document.");
      //RecordingBook openedBook = RecordingBooksData.GetOpenedBook(office, section);
      //if (openedBook.HasSpaceForRecording(document)) {
      //  return openedBook;
      //} else {
      //  return openedBook.CloseAndCreateNew();
      //}
    }

    static public FixedList<RecordingBook> GetList(string filter, string sort = "RecordingBookFullName") {
      return RecordingBooksData.GetRecordingBooks(filter, sort);
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

    [DataField("BookExtData")]
    public JsonObject ExtendedData {
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

    public string RecordIntegrityHashCode {
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


    public bool ReuseUnusedRecordingNumbers {
      get {
        return (ExecutionServer.LicenseName == "Tlaxcala");
      }
    }

    public bool UsePerpetualNumbering {
      get {
        return this.RecordingSection.UsePerpetualNumbering;
      }
    }

    #endregion Public properties

    #region Public methods

    public Recording CreateQuickRecording(int recordingNumber, string bisSuffixTag) {
      var recording = new Recording(this, RecordingDocument.Empty, 
                                    this.BuildRecordingNumber(recordingNumber, bisSuffixTag));
      recording.Save();

      return recording;
    }

    public string BuildRecordingNumber(int recordingNumber, string bisSuffixTag) {
      string temp = recordingNumber.ToString("0000");

      if (bisSuffixTag != null && bisSuffixTag.Length != 0) {
        temp += bisSuffixTag;
      }
      return temp;
    }

    //public int GetNextRecordingNumber() {
    //  if (this.ReuseUnusedRecordingNumbers) {
    //    return RecordingBooksData.GetNextRecordingNumberWithReuse(this);
    //  } else {
    //    return RecordingBooksData.GetNextRecordingNumberWithNoReuse(this);
    //  }
    //}

    public Recording FindRecording(int recordingNumber, string bisSuffixTag) {
      return Recordings.Find((x) => x.Number == this.BuildRecordingNumber(recordingNumber, bisSuffixTag));
    }

    public Recording GetRecording(int recordingId) {
      Recording recording = Recording.Parse(recordingId);
      if (recording.RecordingBook.Equals(this)) {
        return recording;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingNotBelongsToRecordingBook,
                                            recordingId, this.AsText);
      }
    }

    public FixedList<Recording> Recordings {
      get {
        if (recordings == null) {
          recordings = RecordingBooksData.GetRecordings(this);
        }
        return recordings;
      }
    }

    public Recording GetFirstRecording() {
      if (Recordings.Count == 0) {
        return null;
      }
      return Recordings[0];
    }

    public Recording GetNextRecording(Recording recording) {
      if (Recordings.Count == 0) {
        return null;
      }
      int rowIndex = Recordings.IndexOf(recording);

      if (rowIndex != -1) {
        return Recordings[Math.Min(rowIndex + 1, Recordings.Count - 1)];
      } else {
        return null;
      }
    }

    public Recording GetPreviousRecording(Recording recording) {
      if (Recordings.Count == 0) {
        return null;
      }
      int rowIndex = Recordings.IndexOf(recording);
      if (rowIndex != -1) {
        return Recordings[Math.Max(rowIndex - 1, 0)];
      } else {
        return null;
      }
    }

    public Recording GetLastRecording() {
      if (Recordings.Count != 0) {
        return Recordings[Recordings.Count - 1];
      } else {
        return null;
      }
    }

    public void Refresh() {
      this.recordings = null;
    }

    static internal RecordingBook Create(RecordBookDirectory directory, RecordingSection recordingSection,
                                         int recordingsControlCount, TimePeriod recordingsControlTimePeriod) {
      throw new NotImplementedException();
      //RecordingBook recordingBook = new RecordingBook();

      //FixedList<RecordingBook> recordingBookList = directory.RecorderOffice.GetRootRecordingBooks();
      //string tag = directory.GetRecordingBookTag(RecordingBookType.Section);
      //RecordingBook currentParent = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
      //if (currentParent == null) {
      //  currentParent = directory.RecorderOffice.AddRootRecordingBook(tag);
      //}
      //if (RecordingBook.UseBookLevel) {
      //  recordingBookList = currentParent.GetChildBooks();
      //  tag = directory.GetRecordingBookTag(RecordingBookType.Book);
      //  recordingBook = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
      //  if (recordingBook == null) {
      //    recordingBook = new RecordingBook(currentParent, tag);
      //    recordingBook.Save();
      //  }
      //  currentParent = recordingBook;
      //}
      //recordingBookList = currentParent.GetChildBooks();
      //tag = directory.GetRecordingBookTag(RecordingBookType.Volume);
      //recordingBook = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
      //if (recordingBook == null) {
      //  recordingBook = new RecordingBook(currentParent, directory, tag);
      //  recordingBook.RecordingSection = recordingSection;
      //  recordingBook.StartRecordingIndex = 1;
      //  recordingBook.EndRecordingIndex = recordingsControlCount;
      //  recordingBook.RecordingsControlTimePeriod = recordingsControlTimePeriod;

      //  recordingBook.Save();
      //}
      //return recordingBook;
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.CreationDate = DateTime.Now;
        this.CreatedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecordingBook(this);
    }

    #endregion Public methods

    #region Private methods

    private int CalculateTotalSheets() {
      throw new NotImplementedException();

      //return RecordingBooksData.GetBookTotalSheets(this);
    }

    private RecordingBook Clone() {
      RecordingBook newBook = new RecordingBook();

      newBook.RecorderOffice = this.RecorderOffice;
      newBook.RecordingSection = this.RecordingSection;
      newBook.RecordingsControlTimePeriod = this.RecordingsControlTimePeriod;
      newBook.AssignedTo = this.AssignedTo;
      newBook.ReviewedBy = this.ReviewedBy;
      newBook.ApprovedBy = this.ApprovedBy;
      newBook.Status = this.Status;

      return newBook;
    }

    private RecordingBook CloseAndCreateNew() {
      throw new NotImplementedException();

      //this.Status = RecordingBookStatus.Closed;
      //this.ClosingDate = DateTime.Now;
      //if (!this.UsePerpetualNumbering) {
      //  this.EndRecordingIndex = this.Recordings.Count;
      //}
      //this.Save();
      //RecordingBook newBook = this.Clone();
      //newBook.Status = RecordingBookStatus.Opened;
      //if (newBook.UsePerpetualNumbering) {
      //  newBook.StartRecordingIndex = this.StartRecordingIndex + 50;
      //  newBook.EndRecordingIndex = this.EndRecordingIndex + 50;
      //  newBook.BookNumber = newBook.StartRecordingIndex.ToString("0000") + "-" +
      //                       newBook.EndRecordingIndex.ToString("0000");
      //} else {
      //  newBook.StartRecordingIndex = 1;
      //  newBook.EndRecordingIndex = 250;
      //  newBook.BookNumber = (int.Parse(this.BookNumber) + 1).ToString("0000");
      //}
      //newBook.AsText = "Volumen " + newBook.BookNumber;
      //newBook.Save();

      //return newBook;
    }

    private bool HasSpaceForRecording(RecordingDocument document) {
      throw new NotImplementedException();

      //if (this.UsePerpetualNumbering) {
      //  return (RecordingBooksData.GetLastBookRecordingNumber(this) < this.EndRecordingIndex);
      //}
      //// !UsePerpetualNumbering  
      //int currentBookSheets = this.CalculateTotalSheets();
      //int newTotalSheets = currentBookSheets + document.SheetsCount;

      //int lowerBound = ExecutionServer.LicenseName == "Tlaxcala" ? 275 : 250;
      //int upperBound = ExecutionServer.LicenseName == "Tlaxcala" ? 286 : 260;

      //if (newTotalSheets <= lowerBound) {
      //  return true;
      //} else if (currentBookSheets < lowerBound && newTotalSheets <= upperBound) {
      //  return true;
      //} else {
      //  return false;
      //}
    }

    #endregion Private methods

    #region Workflow data and methods

    public Contact ApprovedBy {
      get;
      set;
    }

    public Contact AssignedTo {
      get;
      set;
    }

    public DateTime ClosingDate {
      get;
      set;
    }

    //Default = TimePeriod.Default;
    private TimePeriod _timePeriod = TimePeriod.Default;
    public TimePeriod RecordingsControlTimePeriod {
      get { return _timePeriod; }
      set {
        _timePeriod = value;
      }
    }

    public Contact CreatedBy {
      get;
      set;
    }

    public DateTime CreationDate {
      get;
      set;
    }

    public string Description {
      get;
      set;
    }

    public Contact ReviewedBy {
      get;
      set;
    }

    public void Assign(Contact assignedTo, string notes) {
      this.AssignedTo = assignedTo;
      this.Status = RecordingBookStatus.Assigned;
      Save();
    }

    public bool Close(string esign, string notes) {
      /// OOJJJOO
      if (!ExecutionServer.CurrentPrincipal.IsInRole("VerifyESign" + esign)) { // .VerifyElectronicSign(esign)) {
        return false;
      }
      this.AssignedTo = RecorderOffice.Empty;
      this.ClosingDate = DateTime.Now;
      this.ApprovedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.Status = RecordingBookStatus.Closed;
      Save();
      return true;
    }

    public void SendToRevision() {
      this.Status = RecordingBookStatus.Revision;
      Save();
    }

    public void Unassign(string notes) {
      this.AssignedTo = RecorderOffice.Empty;
      this.Status = RecordingBookStatus.Pending;
      Save();
    }

    #endregion Workflow data and methods

  } // class RecordingBook

} // namespace Empiria.Land.Registration
