/* Empiria� Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria� Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a general recording in Land Registration System.                                   *
*                                                                                                            *
**************************************************** Copyright � La V�a �ntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;
using Empiria.Contacts;
using Empiria.Government.LandRegistration.Data;
using Empiria.Government.LandRegistration.Transactions;

namespace Empiria.Government.LandRegistration {

  public enum RecordingStatus {
    Obsolete = 'S',
    NoLegible = 'L',
    Incomplete = 'I',
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Represents a general recording in Land Registration System.</summary>
  public class Recording : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.Recording";

    private RecordingBook recordingBook = RecordingBook.Empty;
    private LRSTransaction transaction = LRSTransaction.Empty;
    private RecordingDocument document = null;
    private string number = String.Empty;
    private int startImageIndex = 0;
    private int endImageIndex = 0;
    private string notes = String.Empty;
    private string keywords = String.Empty;
    private DateTime presentationTime = ExecutionServer.DateMaxValue;
    private Contact capturedBy = Person.Empty;
    private DateTime capturedTime = DateTime.Now;
    private Contact qualifiedBy = RecorderOffice.Empty;
    private DateTime qualifiedTime = ExecutionServer.DateMaxValue;
    private Contact authorizedBy = RecorderOffice.Empty;
    private DateTime authorizedTime = ExecutionServer.DateMaxValue;
    private Contact canceledBy = RecorderOffice.Empty;
    private DateTime canceledTime = ExecutionServer.DateMaxValue;
    private int cancelationReasonId = -1;
    private string cancelationNotes = String.Empty;
    private string digitalString = String.Empty;
    private string digitalSign = String.Empty;
    private RecordingStatus status = RecordingStatus.Incomplete;
    private string recordIntegrityHashCode = String.Empty;

    private RecordingDocument recordingDocument = null;
    private ObjectList<RecordingAct> recordingActList = null;
    private ObjectList<RecordingPayment> recordingPaymentList = null;
    private RecordingAttachmentFolderList attachmentFolderList = null;

    #endregion Fields

    #region Constructors and parsers

    public Recording()
      : base(thisTypeName) {
      document = RecordingDocument.Empty;
    }

    protected Recording(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
      document = RecordingDocument.Empty;
    }

    //static internal Recording Create(Transactions.RecorderOfficeTransaction transaction) {
    //  Recording recording = new Recording();

    //  recording.recordingBook = RecordingBook.Empty;
    //  recording.number = transaction.Key;
    //  recording.presentationTime = transaction.PresentationTime;
    //  recording.capturedBy = transaction.CapturedBy;

    //  return recording;
    //}

    static public Recording Parse(int id) {
      return BaseObject.Parse<Recording>(thisTypeName, id);
    }

    static internal Recording Parse(DataRow dataRow) {
      return BaseObject.Parse<Recording>(thisTypeName, dataRow);
    }

    static public Recording Empty {
      get { return BaseObject.ParseEmpty<Recording>(thisTypeName); }
    }

    static public string RecordingNumber(int recordingNumber, string bisSuffixTag) {
      string temp = recordingNumber.ToString("0000");

      if (bisSuffixTag != null && bisSuffixTag.Length != 0) {
        temp += bisSuffixTag;
      }
      return temp;
    }

    static public int SplitRecordingNumber(string fullRecordingNumber, out string bisSuffixTag) {
      if (EmpiriaString.IsInteger(fullRecordingNumber)) {
        bisSuffixTag = String.Empty;
        return int.Parse(fullRecordingNumber);
      }
      if (fullRecordingNumber.Contains("-")) {
        int index = fullRecordingNumber.IndexOf("-");
        bisSuffixTag = fullRecordingNumber.Substring(index);
        return int.Parse(fullRecordingNumber.Substring(0, index));
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingNumber,
                                            fullRecordingNumber);
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public Contact AuthorizedBy {
      get { return authorizedBy; }
      set { authorizedBy = value; }
    }

    public DateTime AuthorizedTime {
      get { return authorizedTime; }
      set { authorizedTime = value; }
    }

    public Contact CanceledBy {
      get { return canceledBy; }
    }

    public DateTime CanceledTime {
      get { return canceledTime; }
    }

    public int CancelationReasonId {
      get { return cancelationReasonId; }
      set { cancelationReasonId = value; }
    }

    public string CancelationNotes {
      get { return cancelationNotes; }
      set { cancelationNotes = EmpiriaString.TrimAll(value); }
    }

    public Contact CapturedBy {
      get { return capturedBy; }
    }

    public DateTime CapturedTime {
      get { return capturedTime; }
    }

    public string DigitalString {
      get { return digitalString; }
    }

    public string DigitalSign {
      get { return digitalSign; }
    }

    public RecordingDocument Document {
      get { return document; }
      set { document = value; }
    }

    public int EndImageIndex {
      get { return endImageIndex; }
      set { endImageIndex = value; }
    }

    public string Keywords {
      get { return keywords; }
      protected set { keywords = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = EmpiriaString.TrimAll(value); }
    }

    public string Number {
      get { return number; }
    }

    public string FullNumber {
      get {
        return "Inscripci�n " + this.Number + " en " + this.RecordingBook.FullName;
      }
    }

    public DateTime PresentationTime {
      get { return presentationTime; }
      set { presentationTime = value; }
    }

    public Contact QualifiedBy {
      get { return qualifiedBy; }
    }

    public DateTime QualifiedTime {
      get { return qualifiedTime; }
    }

    public ObjectList<RecordingAct> RecordingActs {
      get {
        if (recordingActList == null) {
          this.recordingActList = RecordingBooksData.GetRecordingActs(this);
        }
        return recordingActList;
      }
    }

    public ObjectList<RecordingPayment> RecordingPaymentList {
      get {
        if (recordingPaymentList == null) {
          this.recordingPaymentList = RecordingBooksData.GetRecordingPaymentList(this);
        }
        return recordingPaymentList;
      }
    }

    public RecordingBook RecordingBook {
      get { return recordingBook; }
      set { recordingBook = value; }
    }

    public RecordingDocument RecordingDocument {
      get {
        if (recordingDocument == null) {
          recordingDocument = RecordingDocument.Parse(this);
        }
        return recordingDocument;
      }
      set { recordingDocument = value; }
    }

    public string RecordIntegrityHashCode {
      get { return recordIntegrityHashCode; }
    }

    public int StartImageIndex {
      get { return startImageIndex; }
      set { startImageIndex = value; }
    }

    public RecordingStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
          case RecordingStatus.Obsolete:
            return "No vigente";
          case RecordingStatus.NoLegible:
            return "No legible";
          case RecordingStatus.Incomplete:
            return "Incompleta";
          case RecordingStatus.Pending:
            return "Pendiente";
          case RecordingStatus.Registered:
            return "Registrada";
          case RecordingStatus.Closed:
            return "Cerrada";
          case RecordingStatus.Deleted:
            return "Eliminada";
          default:
            return "No determinado";
        }
      }
    }

    public LRSTransaction Transaction {
      get { return transaction; }
      set { transaction = value; }
    }

    #endregion Public properties

    #region Public methods

    public void AppendRecordingPayment(RecordingPayment payment) {
      if (this.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "CreateRecordingPayment");
      }
      if (this.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      payment.Recording = this;
      payment.Save();
      this.recordingPaymentList = null;
    }

    public void Cancel() {
      this.Status = RecordingStatus.Deleted;
      this.canceledBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.canceledTime = DateTime.Now;
      this.Save();
    }

    public RecordingAct CreateRecordingAct(RecordingActType recordingActType, Property property) {
      if (this.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "CreateRecordingAct");
      }
      if (this.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordingStatus.Obsolete) {
        this.Status = RecordingStatus.Incomplete;
        this.Save();
      }

      RecordingAct recordingAct = RecordingAct.Create(recordingActType, this, property);
      this.recordingActList = null;
      return recordingAct;
    }


    public void DeleteRecordingAct(RecordingAct recordingAct) {
      if (this.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordingActStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, recordingAct.Id);
      }
      recordingAct.Delete();
      SortRecordingActs();
    }

    public RecordingAttachmentFolder GetAttachementFolder(string folderName) {
      RecordingAttachmentFolderList folderList = this.GetAttachmentFolderList();

      foreach (RecordingAttachmentFolder folder in folderList) {
        if (folder.Name == folderName) {
          return folder;
        }
      }
      throw new LandRegistrationException(LandRegistrationException.Msg.AttachmentFolderNotFound, folderName);
    }

    public RecordingAttachmentFolderList GetAttachmentFolderList() {
      if (attachmentFolderList != null) {
        return attachmentFolderList;
      }
      attachmentFolderList = new RecordingAttachmentFolderList();

      attachmentFolderList.Append(this, "Ra�z");

      ObjectList<PropertyEvent> annotations = this.GetPropertiesAnnotationsList();
      for (int i = 0; i < annotations.Count; i++) {
        string alias = Char.ConvertFromUtf32(65 + i);
        attachmentFolderList.Append(annotations[i].RecordingAct.Recording, alias);
      }
      return attachmentFolderList;
    }

    public void SortRecordingActs() {
      this.recordingActList = null;
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        RecordingActs[i].Index = i + 1;
        RecordingActs[i].Save();
      }
      this.recordingActList = null;
    }

    public void DownwardRecordingAct(RecordingAct recordingAct) {
      if (this.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordingActStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, recordingAct.Id);
      }
      int currentIndex = recordingAct.Index - 1;
      this.recordingActList[currentIndex + 1].Index -= 1;
      this.recordingActList[currentIndex + 1].Save();
      recordingAct.Index += 1;
      recordingAct.Save();
      this.recordingActList = null;
    }

    public RecordingAct GetRecordingAct(int recordingActId) {
      RecordingAct recordingAct = RecordingAct.Parse(recordingActId);
      if (this.RecordingActs.Contains(recordingAct)) {
        return recordingAct;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording,
                                            recordingActId, this.Id);
      }
    }

    public ObjectList<RecordingAct> GetNoAnnotationActs() {
      ObjectList<RecordingAct> recordingActs = this.RecordingActs;

      return new ObjectList<RecordingAct>(recordingActs.FindAll((x) => !x.IsAnnotation));
    }

    public ObjectList<PropertyEvent> GetPropertiesAnnotationsList() {
      return PropertyData.GetRecordingPropertiesAnnotationsList(this);
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.recordingBook = RecordingBook.Parse((int) row["RecordingBookId"]);
      this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.document = RecordingDocument.Parse((int) row["DocumentId"]);
      this.number = (string) row["RecordingNumber"];
      this.startImageIndex = (int) row["RecordingBookFirstImage"];
      this.endImageIndex = (int) row["RecordingBookLastImage"];
      this.notes = (string) row["RecordingNotes"];
      this.keywords = (string) row["RecordingKeywords"];
      this.presentationTime = (DateTime) row["RecordingPresentationTime"];
      this.capturedBy = Contact.Parse((int) row["RecordingCapturedById"]);
      this.capturedTime = (DateTime) row["RecordingCapturedTime"];
      this.qualifiedBy = Contact.Parse((int) row["RecordingQualifiedById"]);
      this.qualifiedTime = (DateTime) row["RecordingQualifiedTime"];
      this.authorizedBy = Contact.Parse((int) row["RecordingAuthorizedById"]);
      this.authorizedTime = (DateTime) row["RecordingAuthorizedTime"];
      this.canceledBy = Contact.Parse((int) row["RecordingCanceledById"]);
      this.canceledTime = (DateTime) row["RecordingCanceledTime"];
      this.cancelationReasonId = (int) row["RecordingCancelationReasonId"];
      this.cancelationNotes = (string) row["RecordingCancelationNotes"];
      this.digitalString = (string) row["RecordingDigitalString"];
      this.digitalSign = (string) row["RecordingDigitalSign"];
      this.status = (RecordingStatus) Convert.ToChar(row["RecordingStatus"]);
      this.recordIntegrityHashCode = (string) row["RecordingRIHC"];
    }

    protected override void ImplementsSave() {
      if (!this.RecordingDocument.IsEmptyInstance) {
        this.RecordingDocument.Save();
      }
      if (capturedBy.IsEmptyInstance) {
        this.capturedTime = DateTime.Now;
        this.capturedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.keywords = EmpiriaString.BuildKeywords(this.number, this.recordingBook.FullName, this.RecordingDocument.DocumentKey);

      RecordingBooksData.WriteRecording(this);
    }

    public void Refresh() {
      this.recordingActList = null;
    }

    public void SetNumber(int recordingNumber, string bisSuffixTag = "") {
      this.number = RecordingNumber(recordingNumber, bisSuffixTag);
    }

    public void UpwardRecordingAct(RecordingAct recordingAct) {
      if (this.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordingActStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, recordingAct.Id);
      }
      int currentIndex = recordingAct.Index - 1;
      this.RecordingActs[currentIndex - 1].Index += 1;
      this.RecordingActs[currentIndex - 1].Save();
      recordingAct.Index -= 1;
      recordingAct.Save();
      this.recordingActList = null;
    }

    #endregion Public methods

  } // class Recording

} // namespace Empiria.Government.LandRegistration