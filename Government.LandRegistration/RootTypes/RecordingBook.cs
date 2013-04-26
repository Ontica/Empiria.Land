/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingBook                                  Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a recording book. A recording book can have a parent recording book and always     *
*              belongs to a recorder of deeds office. Instances of this type have a recording book type.     *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;
using Empiria.Contacts;
using Empiria.Government.LandRegistration.Data;
using Empiria.Government.LandRegistration.Transactions;

namespace Empiria.Government.LandRegistration {

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
    private RecordingActTypeCategory recordingsClass = RecordingActTypeCategory.Empty;
    private string tag = String.Empty;
    private string name = String.Empty;
    private string fullName = String.Empty;
    private string description = String.Empty;
    private string keywords = String.Empty;
    private int recordingsControlCount = 0;
    private TimePeriod recordingsControlTimePeriod = new TimePeriod(ExecutionServer.DateMinValue, ExecutionServer.DateMaxValue);
    private RecordBookDirectory imagingFilesFolder = RecordBookDirectory.Empty;
    private DateTime creationDate = DateTime.Now;
    private DateTime closingDate = ExecutionServer.DateMaxValue;
    private Contact createdBy = Contact.Parse(ExecutionServer.CurrentUser.Id);
    private Contact assignedTo = RecorderOffice.Empty;
    private Contact reviewedBy = RecorderOffice.Empty;
    private Contact approvedBy = RecorderOffice.Empty;
    private int parentRecordingBookId = -1;
    // use null initalization instead RecordingBook.Empty because is a fractal object and Empty instance 
    // parsing throws an infinite loop
    private RecordingBook parentRecordingBook = null;
    private RecordingBookStatus status = RecordingBookStatus.Pending;
    private string recordIntegrityHashCode = String.Empty;

    private ObjectList<Recording> recordings = null;

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
      this.Type = RecordingBookType.Section;
      this.Tag = recordingBookTag;
      this.Name = this.parentRecordingBook.BuildChildName(recordingBookTag);
      this.FullName = recorderOffice.Alias + " " + this.parentRecordingBook.BuildChildFullName(recordingBookTag);
      this.ImagingFilesFolder = RecordBookDirectory.Empty;
    }

    internal RecordingBook(RecordingBook parent, string recordingBookTag)
      : base(thisTypeName) {
      this.RecorderOffice = parent.RecorderOffice;
      this.Parent = parent;
      this.Type = parent.ChildsRecordingBookType;
      this.Tag = recordingBookTag;
      this.Name = parent.BuildChildName(recordingBookTag);
      this.FullName = parent.BuildChildFullName(recordingBookTag);
      this.ImagingFilesFolder = RecordBookDirectory.Empty;
    }

    internal RecordingBook(RecordingBook parent, RecordBookDirectory imagingDirectory,
                           string recordingBookTag)
      : base(thisTypeName) {
      this.RecorderOffice = parent.RecorderOffice;
      this.Parent = parent;
      this.Type = parent.ChildsRecordingBookType;
      this.Tag = recordingBookTag;
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

    static public RecordingBook GetAssignedBookForRecording(RecorderOffice office, RecordingActTypeCategory section,
                                                            RecordingDocument document) {

      RecordingBook openedBook = RecordingBooksData.GetOpenedBook(office, section);
      int currentBookSheets = openedBook.CalculateTotalSheets();
      int newTotalSheets = currentBookSheets + document.SheetsCount;

      int lowerBound = ExecutionServer.LicenseName == "Tlaxcala" ? 275 : 250;
      int upperBound = ExecutionServer.LicenseName == "Tlaxcala" ? 286 : 260;

      if (newTotalSheets <= lowerBound) {
        return openedBook;
      } else if (currentBookSheets < lowerBound && newTotalSheets <= upperBound) {
        return openedBook;
      }
      return openedBook.CloseAndCreateNew();
    }

    private RecordingBook CloseAndCreateNew() {
      this.Status = RecordingBookStatus.Closed;
      this.Save();
      RecordingBook newBook = this.Clone();
      newBook.Status = RecordingBookStatus.Opened;
      newBook.Tag = (int.Parse(this.Tag) + 1).ToString("0000");
      newBook.Name = "Volumen " + newBook.Tag;
      newBook.FullName = newBook.Parent.Name + " Volumen " + newBook.Tag;
      newBook.Save();

      return newBook;
    }

    private RecordingBook Clone() {
      RecordingBook newBook = new RecordingBook();

      newBook.RecorderOffice = this.RecorderOffice;
      newBook.Type = this.Type;
      newBook.RecordingsClass = this.RecordingsClass;
      newBook.Tag = String.Empty;
      newBook.Name = String.Empty;
      newBook.FullName = String.Empty;
      newBook.keywords = String.Empty;
      newBook.Description = String.Empty;
      newBook.RecordingsControlCount = this.RecordingsControlCount;
      newBook.RecordingsControlTimePeriod = this.RecordingsControlTimePeriod;
      newBook.ImagingFilesFolder = this.ImagingFilesFolder;
      newBook.CreationDate = this.CreationDate;
      newBook.ClosingDate = this.ClosingDate;
      newBook.CreatedBy = this.CreatedBy;
      newBook.AssignedTo = this.AssignedTo;
      newBook.ReviewedBy = this.ReviewedBy;
      newBook.ApprovedBy = this.ApprovedBy;
      newBook.parentRecordingBookId = this.parentRecordingBookId;
      newBook.Status = this.Status;

      return newBook;
    }

    private int CalculateTotalSheets() {
      return RecordingBooksData.GetBookTotalSheets(this);
    }

    public Recording CreateRecording(LRSTransaction transaction, RecordingDocument document) {
      Recording recording = new Recording();
      recording.RecordingBook = this;
      recording.Transaction = transaction;
      recording.Document = document;
      recording.UseBisNumberTag = false;
      recording.Number = GetNextRecordingNumber();
      recording.Status = RecordingStatus.Incomplete;
      recording.StartImageIndex = -1;
      recording.EndImageIndex = -1;
      recording.PresentationTime = transaction.PresentationTime;
      recording.AuthorizedTime = DateTime.Now;
      recording.AuthorizedBy = Contact.Parse(36);
      recording.Save();

      return recording;
    }

    public string GetNextRecordingNumber() {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        return RecordingBooksData.GetNextRecordingNumber(this);
      } else {
        return RecordingBooksData.GetNextRecordingNumber2(this);
      }
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
        switch (this.Type) {
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
            throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.Type.ToString());
        }
      }
    }

    public DateTime ClosingDate {
      get { return closingDate; }
      set { closingDate = value; }
    }

    public int RecordingsControlCount {
      get { return recordingsControlCount; }
      set { recordingsControlCount = value; }
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

    public RecordingActTypeCategory RecordingsClass {
      get { return recordingsClass; }
      set { recordingsClass = value; }
    }

    public RecorderOffice RecorderOffice {
      get { return recorderOffice; }
      set { recorderOffice = value; }
    }

    public string RecordIntegrityHashCode {
      get { return recordIntegrityHashCode; }
      private set { recordIntegrityHashCode = value; }
    }

    public Contact ReviewedBy {
      get { return reviewedBy; }
      set { reviewedBy = value; }
    }

    public RecordingBookStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string Tag {
      get { return tag; }
      set { tag = (value == String.Empty || value == "00" || value == "0") ? "N/A" : value; }
    }

    public RecordingBookType Type {
      get { return type; }
      set { type = value; }
    }

    #endregion Public properties

    #region Public methods

    public void Assign(Contact assignedTo, string notes) {
      this.assignedTo = assignedTo;
      this.Status = RecordingBookStatus.Assigned;
      Save();
    }

    internal string BuildChildFullName(string recordingBookTag) {
      switch (this.Type) {
        case RecordingBookType.Root:
          return "Sección " + recordingBookTag;
        case RecordingBookType.Section:
          if (RecordingBook.UseBookLevel) {
            return recorderOffice.Alias + " Sección " + this.Tag + " Libro " + recordingBookTag;
          } else {
            return recorderOffice.Alias + " Sección " + this.Tag + " Volumen " + recordingBookTag;
          }
        case RecordingBookType.Book:
          if (RecordingBook.UseBookLevel) {
            return recorderOffice.Alias + " Sección " + this.Parent.Tag + " Libro " + this.Tag + " Volumen " + recordingBookTag;
          } else {
            throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType,
                                                RecordingBookType.Book.ToString());
          }
        case RecordingBookType.Volume:
          throw new LandRegistrationException(LandRegistrationException.Msg.VolumeRecordingBooksCantHaveChilds, this.FullName);
        default:
          throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.Type.ToString());
      }
    }

    internal string BuildChildName(string recordingBookTag) {
      switch (this.Type) {
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
          throw new LandRegistrationException(LandRegistrationException.Msg.UnrecognizedRecordingBookType, this.Type.ToString());
      }
    }

    public bool Close(string esign, string notes) {
      if (!ExecutionServer.CurrentUser.VerifyElectronicSign(esign)) {
        return false;
      }
      this.assignedTo = RecorderOffice.Empty;
      this.closingDate = DateTime.Now;
      this.approvedBy = Contact.Parse(ExecutionServer.CurrentUser.Id);
      this.Status = RecordingBookStatus.Closed;
      Save();
      return true;
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

    public ObjectList<RecordingBook> GetChildBooks() {
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

    public ObjectList<Recording> Recordings {
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

    static internal RecordingBook Create(RecordBookDirectory directory, RecordingActTypeCategory recordingsClass,
                                         int recordingsControlCount, TimePeriod recordingsControlTimePeriod) {
      RecordingBook recordingBook = new RecordingBook();

      ObjectList<RecordingBook> recordingBookList = directory.RecorderOffice.GetRootRecordingBooks();
      string tag = directory.GetRecordingBookTag(RecordingBookType.Section);
      RecordingBook currentParent = recordingBookList.Find((x) => x.Tag.Equals(tag));
      if (currentParent == null) {
        currentParent = directory.RecorderOffice.AddRootRecordingBook(tag);
      }
      if (RecordingBook.UseBookLevel) {
        recordingBookList = currentParent.GetChildBooks();
        tag = directory.GetRecordingBookTag(RecordingBookType.Book);
        recordingBook = recordingBookList.Find((x) => x.Tag.Equals(tag));
        if (recordingBook == null) {
          recordingBook = new RecordingBook(currentParent, tag);
          recordingBook.Save();
        }
        currentParent = recordingBook;
      }
      recordingBookList = currentParent.GetChildBooks();
      tag = directory.GetRecordingBookTag(RecordingBookType.Volume);
      recordingBook = recordingBookList.Find((x) => x.Tag.Equals(tag));
      if (recordingBook == null) {
        recordingBook = new RecordingBook(currentParent, directory, tag);
        recordingBook.RecordingsClass = recordingsClass;
        recordingBook.recordingsControlCount = recordingsControlCount;
        recordingBook.recordingsControlTimePeriod = recordingsControlTimePeriod;

        recordingBook.Save();
      }
      return recordingBook;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.RecorderOffice = RecorderOffice.Parse((int) row["RecorderOfficeId"]);
      this.Type = (RecordingBookType) Convert.ToChar(row["RecordingBookType"]);
      this.RecordingsClass = RecordingActTypeCategory.Parse((int) row["RecordingsClassId"]);
      this.Tag = (string) row["RecordingBookNumber"];
      this.Name = (string) row["RecordingBookName"];
      this.FullName = (string) row["RecordingBookFullName"];
      this.keywords = (string) row["RecordingBookKeywords"];
      this.Description = (string) row["RecordingBookDescription"];
      this.RecordingsControlCount = (int) row["RecordingsControlCount"];
      this.RecordingsControlTimePeriod = new TimePeriod((DateTime) row["RecordingsControlFirstDate"], (DateTime) row["RecordingsControlLastDate"]);
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
      this.keywords = this.name + " " + EmpiriaString.BuildKeywords(this.Tag);

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

  } // class RecordingBook

} // namespace Empiria.Government.LandRegistration