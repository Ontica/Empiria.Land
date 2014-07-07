/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingBook                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a recording book. A recording book can have a parent recording book and always     *
*              belongs to a recorder of deeds office. Instances of this type have a recording book type.     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;
using Empiria.Contacts;
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

  public enum RecordingBookType {
    Root = 'R',
    Section = 'S',
    Book = 'B',
    Volume = 'V'
  }

  /// <summary>Represents a recording book. A recording book can have a parent recording book and always
  /// belongs to a recorder of deeds office. Instances of this type have a recording book type.</summary>
  public class RecordingBook : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingBook";
    static public bool UseBookAttachments = ConfigurationData.GetBoolean("RecordingBook.UseAttachments");
    static public bool UseBookLevel = ConfigurationData.GetBoolean("RecordingBookType.UseBookLevel");

    private RecorderOffice recorderOffice = RecorderOffice.Empty;
    private RecordingBookType type = RecordingBookType.Volume;
    private RecordingSection sectionType = RecordingSection.Empty;
    private string bookNumber = String.Empty;
    private string name = String.Empty;
    private string fullName = String.Empty;
    private string description = String.Empty;
    private string keywords = String.Empty;
    private int startRecordingIndex = 0;
    private int endRecordingIndex = 0;
    private TimePeriod recordingsControlTimePeriod = new TimePeriod(ExecutionServer.DateMinValue, ExecutionServer.DateMaxValue);
    private RecordBookDirectory imagingFilesFolder = RecordBookDirectory.Empty;
    private DateTime creationDate = DateTime.Now;
    private DateTime closingDate = ExecutionServer.DateMaxValue;
    private Contact createdBy = Person.Empty;
    private Contact assignedTo = RecorderOffice.Empty;
    private Contact reviewedBy = RecorderOffice.Empty;
    private Contact approvedBy = RecorderOffice.Empty;
    private int parentRecordingBookId = -1;
    // use null initalization instead RecordingBook.Empty because is a fractal object and Empty instance 
    // parsing throws an infinite loop
    private RecordingBook parentRecordingBook = null;
    private RecordingBookStatus status = RecordingBookStatus.Pending;
    private string recordIntegrityHashCode = String.Empty;

    private FixedList<Recording> recordings = null;

    #endregion Fields

    #region Constructors and parsers

    public RecordingBook()
      : base(thisTypeName) {

    }

    protected RecordingBook(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    internal RecordingBook(RecorderOffice recorderOffice, string recordingBookTag)
      : base(thisTypeName) {
      this.RecorderOffice = recorderOffice;
      this.Parent = RecordingBook.Empty;
      this.BookType = RecordingBookType.Section;
      this.BookNumber = recordingBookTag;
      this.Name = this.parentRecordingBook.BuildChildName(recordingBookTag);
      this.FullName = recorderOffice.Alias + " " + this.parentRecordingBook.BuildChildFullName(recordingBookTag);
      this.ImagingFilesFolder = RecordBookDirectory.Empty;
    }

    internal RecordingBook(RecordingBook parent, string recordingBookTag)
      : base(thisTypeName) {
      this.RecorderOffice = parent.RecorderOffice;
      this.Parent = parent;
      this.BookType = parent.ChildsRecordingBookType;
      this.BookNumber = recordingBookTag;
      this.Name = parent.BuildChildName(recordingBookTag);
      this.FullName = parent.BuildChildFullName(recordingBookTag);
      this.ImagingFilesFolder = RecordBookDirectory.Empty;
    }

    internal RecordingBook(RecordingBook parent, RecordBookDirectory imagingDirectory,
                           string recordingBookTag)
      : base(thisTypeName) {
      this.RecorderOffice = parent.RecorderOffice;
      this.Parent = parent;
      this.BookType = parent.ChildsRecordingBookType;
      this.BookNumber = recordingBookTag;
      this.Name = parent.BuildChildName(recordingBookTag);
      this.FullName = parent.BuildChildFullName(recordingBookTag);
      this.ImagingFilesFolder = imagingDirectory;
    }

    static public RecordingBook Parse(int id) {
      return BaseObject.Parse<RecordingBook>(thisTypeName, id);
    }

    static internal RecordingBook Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingBook>(thisTypeName, dataRow);
    }

    static public RecordingBook Empty {
      get { return BaseObject.ParseEmpty<RecordingBook>(thisTypeName); }
    }

    static public RecordingBook GetAssignedBookForRecording(RecorderOffice office, RecordingSection section,
                                                            RecordingDocument document) {
      Assertion.RequireObject(document, "document");
      Assertion.Require(!document.IsEmptyInstance && !document.IsNew,
                        "Document can't be neither an empty or unsaved document.");
      RecordingBook openedBook = RecordingBooksData.GetOpenedBook(office, section);
      if (openedBook.HasSpaceForRecording(document)) {
        return openedBook;
      } else {
        return openedBook.CloseAndCreateNew();
      }
    }

    static public FixedList<RecordingBook> GetList(string filter, string sort = "RecordingBookFullName") {
      return RecordingBooksData.GetRecordingBooks(filter, sort);
    }

    #endregion Constructors and parsers

    #region Public properties

    public Contact ApprovedBy {
      get { return approvedBy; }
      set { approvedBy = value; }
    }

    public Contact AssignedTo {
      get { return assignedTo; }
      set { assignedTo = value; }
    }

    public RecordingBookType ChildsRecordingBookType {
      get {
        switch (this.BookType) {
          case RecordingBookType.Root:
            return RecordingBookType.Section;
          case RecordingBookType.Section:
            if (RecordingBook.UseBookLevel) {
              return RecordingBookType.Book;
            } else {
              return RecordingBookType.Volume;
            }
          case RecordingBookType.Book:
            if (RecordingBook.UseBookLevel) {
              return RecordingBookType.Volume;
            } else {
              throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType,
                                                  RecordingBookType.Book.ToString());
            }
          case RecordingBookType.Volume:
            throw new LandRegistrationException(LandRegistrationException.Msg.VolumeRecordingBooksCantHaveChilds, this.FullName);
          default:
            throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.BookType.ToString());
        }
      }
    }

    public DateTime ClosingDate {
      get { return closingDate; }
      set { closingDate = value; }
    }

    public int StartRecordingIndex {
      get { return startRecordingIndex; }
      set { startRecordingIndex = value; }
    }

    public int EndRecordingIndex {
      get { return endRecordingIndex; }
      set { endRecordingIndex = value; }
    }

    public TimePeriod RecordingsControlTimePeriod {
      get { return recordingsControlTimePeriod; }
      set { recordingsControlTimePeriod = value; }
    }

    public Contact CreatedBy {
      get { return createdBy; }
      set { createdBy = value; }
    }

    public DateTime CreationDate {
      get { return creationDate; }
      set { creationDate = value; }
    }

    public string Description {
      get { return description; }
      set { description = value; }
    }

    public string FullName {
      get { return fullName; }
      set { fullName = value; }
    }

    public RecordBookDirectory ImagingFilesFolder {
      get { return imagingFilesFolder; }
      set { imagingFilesFolder = value; }
    }

    public bool IsAvailableForManualEditing {
      get {
        return (this.status == RecordingBookStatus.Assigned ||
                this.status == RecordingBookStatus.Pending ||
                this.status == RecordingBookStatus.Revision);
      }
    }

    public bool IsRoot {
      get { return (parentRecordingBookId == this.Id); }
    }

    public string Name {
      get { return name; }
      set { name = value; }
    }

    public string Keywords {
      get { return keywords; }
      set { keywords = value; }
    }

    public RecordingBook Parent {
      get {
        if (parentRecordingBook == null) {
          parentRecordingBook = RecordingBook.Parse(parentRecordingBookId);
        }
        return parentRecordingBook;
      }
      internal set {
        parentRecordingBook = value;
        parentRecordingBookId = parentRecordingBook.Id;
      }
    }

    public RecordingSection RecordingSectionType {
      get { return sectionType; }
      set { sectionType = value; }
    }

    public RecorderOffice RecorderOffice {
      get { return recorderOffice; }
      set { recorderOffice = value; }
    }

    public string RecordIntegrityHashCode {
      get { return recordIntegrityHashCode; }
      private set { recordIntegrityHashCode = value; }
    }

    public bool ReuseUnusedRecordingNumbers {
      get {
        return (ExecutionServer.LicenseName == "Tlaxcala");
      }
    }

    public Contact ReviewedBy {
      get { return reviewedBy; }
      set { reviewedBy = value; }
    }

    public RecordingBookStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string BookNumber {
      get { return bookNumber; }
      set { bookNumber = (value == String.Empty || value == "00" || value == "0") ? "N/A" : value; }
    }

    public RecordingBookType BookType {
      get { return type; }
      set { type = value; }
    }

    public bool UsePerpetualNumbering {
      get {
        return sectionType.UsePerpetualNumbering;
      }
    }

    #endregion Public properties

    #region Public methods

    public void Assign(Contact assignedTo, string notes) {
      this.assignedTo = assignedTo;
      this.Status = RecordingBookStatus.Assigned;
      Save();
    }

    internal string BuildChildFullName(string recordingBookTag) {
      switch (this.BookType) {
        case RecordingBookType.Root:
          return "Sección " + recordingBookTag;
        case RecordingBookType.Section:
          if (RecordingBook.UseBookLevel) {
            return recorderOffice.Alias + " Sección " + this.BookNumber + " Libro " + recordingBookTag;
          } else {
            return recorderOffice.Alias + " Sección " + this.BookNumber + " Volumen " + recordingBookTag;
          }
        case RecordingBookType.Book:
          if (RecordingBook.UseBookLevel) {
            return recorderOffice.Alias + " Sección " + this.Parent.BookNumber + " Libro " + this.BookNumber + " Volumen " + recordingBookTag;
          } else {
            throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType,
                                                RecordingBookType.Book.ToString());
          }
        case RecordingBookType.Volume:
          throw new LandRegistrationException(LandRegistrationException.Msg.VolumeRecordingBooksCantHaveChilds, this.FullName);
        default:
          throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.BookType.ToString());
      }
    }

    internal string BuildChildName(string recordingBookTag) {
      switch (this.BookType) {
        case RecordingBookType.Root:
          return "Sección " + recordingBookTag;
        case RecordingBookType.Section:
          if (RecordingBook.UseBookLevel) {
            return "Libro " + recordingBookTag;
          } else {
            return "Volumen " + recordingBookTag;
          }
        case RecordingBookType.Book:
          if (RecordingBook.UseBookLevel) {
            return "Volumen " + recordingBookTag;
          } else {
            throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType,
                                                RecordingBookType.Book.ToString());
          }
        case RecordingBookType.Volume:
          throw new LandRegistrationException(LandRegistrationException.Msg.VolumeRecordingBooksCantHaveChilds, this.FullName);
        default:
          throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.BookType.ToString());
      }
    }

    public bool Close(string esign, string notes) {
      if (!EmpiriaUser.Current.VerifyElectronicSign(esign)) {
        return false;
      }
      this.assignedTo = RecorderOffice.Empty;
      this.closingDate = DateTime.Now;
      this.approvedBy = EmpiriaUser.Current.Contact;
      this.Status = RecordingBookStatus.Closed;
      Save();
      return true;
    }

    public Recording CreateQuickRecording(int recordingNumber, string bisSuffixTag) {
      var recording = new Recording();
      recording.RecordingBook = this;
      recording.Transaction = LRSTransaction.Empty;
      recording.Document = RecordingDocument.Empty;
      recording.SetNumber(recordingNumber, bisSuffixTag);
      recording.Status = RecordableObjectStatus.Incomplete;
      recording.PresentationTime = ExecutionServer.DateMinValue;
      recording.StartImageIndex = -1;
      recording.EndImageIndex = -1;
      recording.Save();

      return recording;
    }

    public Recording CreateRecording(LRSTransaction transaction) {
      Assertion.RequireObject(transaction, "transaction");
      Assertion.RequireObject(transaction.Document, "document");
      Assertion.Require(!transaction.Document.IsEmptyInstance && !transaction.Document.IsNew,
                        "Transaction document cannot be neither empty nor a new document instance.");

      var recording = new Recording();
      recording.RecordingBook = this;
      recording.Transaction = transaction;
      recording.Document = transaction.Document;
      recording.SetNumber (this.GetNextRecordingNumber());
      recording.Status = RecordableObjectStatus.Incomplete;
      recording.StartImageIndex = -1;
      recording.EndImageIndex = -1;
      recording.PresentationTime = transaction.PresentationTime;
      recording.AuthorizedTime = DateTime.Now;
      recording.AuthorizedBy = Contact.Parse(36);
      recording.Save();

      return recording;
    }

    public Recording CreateRecordingForAnnotation(LRSTransaction transaction, Recording antecedent) {
      Assertion.RequireObject(transaction, "transaction");
      Assertion.RequireObject(transaction.Document, "document");
      Assertion.Require(!transaction.Document.IsEmptyInstance && !transaction.Document.IsNew,
                        "Transaction document can not be neither an empty or a new document instance.");
      Assertion.Require(!antecedent.IsEmptyInstance && !antecedent.IsNew,
                        "Annotation precedent can not be empty or a new instance.");

      Recording recording = new Recording();
      recording.BaseRecordingId = antecedent.Id;
      recording.RecordingBook = antecedent.RecordingBook;
      recording.Transaction = transaction;
      recording.Document = transaction.Document;
      string bisSuffixTag = String.Empty;

      int recordingNumber = Recording.SplitRecordingNumber(antecedent.Number, out bisSuffixTag);
      recording.SetNumber(recordingNumber, bisSuffixTag);

      recording.Status = RecordableObjectStatus.Incomplete;
      recording.StartImageIndex = -1;
      recording.EndImageIndex = -1;
      recording.PresentationTime = transaction.PresentationTime;
      recording.AuthorizedTime = DateTime.Now;
      recording.AuthorizedBy = Contact.Parse(36);
      recording.Save();

      recording.RecordingBook.Refresh();

      return recording;
    }

    public int GetNextRecordingNumber() {
      if (this.ReuseUnusedRecordingNumbers) {
        return RecordingBooksData.GetNextRecordingNumberWithReuse(this);
      } else {
        return RecordingBooksData.GetNextRecordingNumberWithNoReuse(this);
      }
    }

    public void DeleteImageAtIndex(int imageIndex) {
      this.ImagingFilesFolder.DeleteImageAtIndex(imageIndex);
      RecordingBooksData.UpdateRecordingsImageIndex(this, imageIndex + 1, -1);
      recordings = null;
    }

    public void InsertEmptyImageAtIndex(int imageIndex) {
      this.ImagingFilesFolder.InsertEmptyImageAtIndex(imageIndex);
      RecordingBooksData.UpdateRecordingsImageIndex(this, imageIndex + 1, 1);
      recordings = null;
    }

    public Recording FindRecording(string recordingNumber) {
      return Recordings.Find((x) => x.Number.Equals(recordingNumber));
    }

    public FixedList<RecordingBook> GetChildBooks() {
      return RecordingBooksData.GetChildsRecordingBooks(this);
    }

    public Recording GetRecording(int recordingId) {
      Recording recording = Recording.Parse(recordingId);
      if (recording.RecordingBook.Equals(this)) {
        return recording;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingNotBelongsToRecordingBook, recordingId, this.Name);
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

    static internal RecordingBook Create(RecordBookDirectory directory, RecordingSection recordingSectionType,
                                         int recordingsControlCount, TimePeriod recordingsControlTimePeriod) {
      RecordingBook recordingBook = new RecordingBook();

      FixedList<RecordingBook> recordingBookList = directory.RecorderOffice.GetRootRecordingBooks();
      string tag = directory.GetRecordingBookTag(RecordingBookType.Section);
      RecordingBook currentParent = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
      if (currentParent == null) {
        currentParent = directory.RecorderOffice.AddRootRecordingBook(tag);
      }
      if (RecordingBook.UseBookLevel) {
        recordingBookList = currentParent.GetChildBooks();
        tag = directory.GetRecordingBookTag(RecordingBookType.Book);
        recordingBook = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
        if (recordingBook == null) {
          recordingBook = new RecordingBook(currentParent, tag);
          recordingBook.Save();
        }
        currentParent = recordingBook;
      }
      recordingBookList = currentParent.GetChildBooks();
      tag = directory.GetRecordingBookTag(RecordingBookType.Volume);
      recordingBook = recordingBookList.Find((x) => x.BookNumber.Equals(tag));
      if (recordingBook == null) {
        recordingBook = new RecordingBook(currentParent, directory, tag);
        recordingBook.sectionType = recordingSectionType;
        recordingBook.StartRecordingIndex = 1;
        recordingBook.EndRecordingIndex = recordingsControlCount;
        recordingBook.recordingsControlTimePeriod = recordingsControlTimePeriod;

        recordingBook.Save();
      }
      return recordingBook;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.RecorderOffice = RecorderOffice.Parse((int) row["RecorderOfficeId"]);
      this.BookType = (RecordingBookType) Convert.ToChar(row["RecordingBookType"]);
      this.sectionType = RecordingSection.Parse((int) row["RecordingsClassId"]);
      this.BookNumber = (string) row["RecordingBookNumber"];
      this.Name = (string) row["RecordingBookName"];
      this.FullName = (string) row["RecordingBookFullName"];
      this.keywords = (string) row["RecordingBookKeywords"];
      this.Description = (string) row["RecordingBookDescription"];
      this.StartRecordingIndex = (int) row["StartRecordingIndex"];
      this.EndRecordingIndex = (int) row["EndRecordingIndex"];
      this.RecordingsControlTimePeriod = new TimePeriod((DateTime) row["RecordingsControlFirstDate"], 
                                                        (DateTime) row["RecordingsControlLastDate"]);
      this.ImagingFilesFolder = RecordBookDirectory.Parse((int) row["RecordingBookFilesFolderId"]);
      this.CreationDate = (DateTime) row["CreationDate"];
      this.ClosingDate = (DateTime) row["ClosingDate"];
      this.CreatedBy = Contact.Parse((int) row["CreatedById"]);
      this.AssignedTo = Contact.Parse((int) row["AssignedToId"]);
      this.ReviewedBy = Contact.Parse((int) row["ReviewedById"]);
      this.ApprovedBy = Contact.Parse((int) row["ApprovedById"]);
      this.parentRecordingBookId = (int) row["ParentRecordingBookId"];
      this.Status = (RecordingBookStatus) Convert.ToChar(row["RecordingBookStatus"]);
      this.recordIntegrityHashCode = (string) row["RecordingBookRIHC"];
    }

    protected override void ImplementsSave() {
      if (createdBy.IsEmptyInstance) {
        this.creationDate = DateTime.Now;
        this.createdBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }

      this.keywords = EmpiriaString.BuildKeywords(this.FullName);

      RecordingBooksData.WriteRecordingBook(this);
    }

    public void SendToRevision() {
      this.Status = RecordingBookStatus.Revision;
      Save();
    }

    public void Unassign(string notes) {
      this.assignedTo = RecorderOffice.Empty;
      this.Status = RecordingBookStatus.Pending;
      Save();
    }

    #endregion Public methods

    #region Private methods

    private int CalculateTotalSheets() {
      return RecordingBooksData.GetBookTotalSheets(this);
    }

    private RecordingBook Clone() {
      RecordingBook newBook = new RecordingBook();

      newBook.RecorderOffice = this.RecorderOffice;
      newBook.BookType = this.BookType;
      newBook.sectionType = this.sectionType;
      newBook.RecordingsControlTimePeriod = this.RecordingsControlTimePeriod;
      newBook.AssignedTo = this.AssignedTo;
      newBook.ReviewedBy = this.ReviewedBy;
      newBook.ApprovedBy = this.ApprovedBy;
      newBook.parentRecordingBookId = this.parentRecordingBookId;
      newBook.Status = this.Status;

      return newBook;
    }

    private RecordingBook CloseAndCreateNew() {
      this.Status = RecordingBookStatus.Closed;
      this.ClosingDate = DateTime.Now;
      if (!this.UsePerpetualNumbering) {
        this.EndRecordingIndex = this.Recordings.Count;
      }
      this.Save();
      RecordingBook newBook = this.Clone();
      newBook.Status = RecordingBookStatus.Opened;
      if (newBook.UsePerpetualNumbering) {
        newBook.StartRecordingIndex = this.StartRecordingIndex + 50;
        newBook.EndRecordingIndex = this.EndRecordingIndex + 50;
        newBook.BookNumber = newBook.StartRecordingIndex.ToString("0000") + "-" + newBook.EndRecordingIndex.ToString("0000");
      } else {
        newBook.StartRecordingIndex = 1;
        newBook.EndRecordingIndex = 250;
        newBook.BookNumber = (int.Parse(this.BookNumber) + 1).ToString("0000");
      }
      newBook.Name = "Volumen " + newBook.BookNumber;
      newBook.FullName = newBook.Parent.FullName + " " + newBook.Name;
      newBook.Save();

      return newBook;
    }

    private bool HasSpaceForRecording(RecordingDocument document) {
      if (this.UsePerpetualNumbering) {
        return (RecordingBooksData.GetLastBookRecordingNumber(this) < this.EndRecordingIndex);
      }
      // !UsePerpetualNumbering  
      int currentBookSheets = this.CalculateTotalSheets();
      int newTotalSheets = currentBookSheets + document.SheetsCount;

      int lowerBound = ExecutionServer.LicenseName == "Tlaxcala" ? 275 : 250;
      int upperBound = ExecutionServer.LicenseName == "Tlaxcala" ? 286 : 260;

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
