﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingBook                                  Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Indicates the status of a recording book according to it use in historic capture.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Json;

using Empiria.Land.Registration.Data;

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

    static public FixedList<RecordingBook> GetList(string filter, string sort = "BookAsText") {
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

    public bool HasImageSet {
      get {
        return (this.ImageSetId != -1);
      }
    }

    private int _imageSetId = 0;
    public int ImageSetId {
      get {
        if (_imageSetId == 0) {
          _imageSetId = this.ExtensionData.Get<int>("ImageSetId", -1);
        }
        return _imageSetId;
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

    public Recording AddRecording(RecordingDTO dto) {
      Assertion.AssertObject(dto, "dto");

      this.AssertValidDTOForAppend(dto);

      var recording = new Recording(dto);

      recording.Save();

      return recording;
    }

    public Recording AddRecording(RecordingDocument document, string recordingNumber) {
      Assertion.AssertObject(document, "document");
      Assertion.AssertObject(recordingNumber, "recordingNumber");

      return new Recording(this, document, this.FormatRecordingNumber(recordingNumber));
    }

    public Recording CreateNew() {
      return new Recording(this, RecordingDocument.Empty, "Nueva partida");
    }

    public string FormatRecordingNumber(string rawRecordingNumber) {
      try {
        rawRecordingNumber = rawRecordingNumber.Replace(" ", String.Empty);
        rawRecordingNumber = rawRecordingNumber.Replace("-", "/");

        string[] parts = rawRecordingNumber.Split('/');

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
        throw new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingNumber,
                                            rawRecordingNumber);
      }
    }

    public bool ExistsRecording(string rawRecordingNumber) {
      string recordingNo = this.FormatRecordingNumber(rawRecordingNumber);

      return Recordings.Contains((x) => x.Number == recordingNo);
    }

    public Recording FindRecording(string rawRecordingNumber) {
      string recordingNo = this.FormatRecordingNumber(rawRecordingNumber);

      return Recordings.Find((x) => x.Number == recordingNo);
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

    public FixedList<Recording> GetRecordings() {
      return RecordingBooksData.GetRecordings(this);
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

    protected override void OnSave() {
      if (this.IsNew) {
        this.CreationDate = DateTime.Now;
        this.CreatedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecordingBook(this);
    }

    #endregion Public methods

    #region Private methods

    private void AssertValidDTOForAppend(RecordingDTO dto) {
      //throw new NotImplementedException();
    }

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
    private TimeFrame _timePeriod = TimeFrame.Default;
    public TimeFrame RecordingsControlTimePeriod {
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
