/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a general recording in Land Registration System.                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Represents a general recording in Land Registration System.</summary>
  public class Recording : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.Recording";

    private RecordingBook recordingBook = RecordingBook.Empty;
    private LRSTransaction transaction = LRSTransaction.Empty;
    private RecordingDocument document = null;
    private int baseRecordingId = -1;
    private string number = String.Empty;
    private int startImageIndex = 0;
    private int endImageIndex = 0;
    private string notes = String.Empty;
    private string keywords = String.Empty;
    private DateTime presentationTime = ExecutionServer.DateMaxValue;
    private string receiptNumber = String.Empty;
    private decimal receiptTotal = Decimal.Zero;
    private DateTime receiptIssueDate = ExecutionServer.DateMaxValue;
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
    private RecordableObjectStatus status = RecordableObjectStatus.Incomplete;
    private string recordIntegrityHashCode = String.Empty;

    private RecordingDocument recordingDocument = null;
    private FixedList<RecordingAct> recordingActList = null;
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

    static public Recording Parse(int id) {
      return BaseObject.Parse<Recording>(thisTypeName, id);
    }

    static internal Recording Parse(DataRow dataRow) {      
      return BaseObject.ParseFromBelow<Recording>(thisTypeName, dataRow);
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

    public int BaseRecordingId {
      get { return baseRecordingId; }
      set { baseRecordingId = value; }
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
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          return "Partida " + this.Number + " en " + this.RecordingBook.FullName;
        } else {
          return "Inscripción " + this.Number + " en " + this.RecordingBook.FullName;
        }
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

    public string ReceiptNumber {
      get { return receiptNumber; }
      set { receiptNumber = value; }
    }

    public decimal ReceiptTotal {
      get { return receiptTotal; }
      set { receiptTotal = value; }
    }

    public DateTime ReceiptIssueDate {
      get { return receiptIssueDate; }
      set { receiptIssueDate = value; }
    }

    public FixedList<RecordingAct> RecordingActs {
      get {
        if (recordingActList == null) {
          this.recordingActList = RecordingActsData.GetRecordingActs(this);
        }
        return recordingActList;
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

    public RecordableObjectStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.NoLegible:
            return "No legible";
          case RecordableObjectStatus.Incomplete:
            return "Incompleta";
          case RecordableObjectStatus.Pending:
            return "Pendiente";
          case RecordableObjectStatus.Registered:
            return "Registrada";
          case RecordableObjectStatus.Closed:
            return "Cerrada";
          case RecordableObjectStatus.Deleted:
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

    public void Delete() {
      Delete(true);
    }

    private void Delete(bool publicCall) {
      Assertion.Assert(this.RecordingActs.Count == 0,
                       "This recording can't be deleted because it has recording acts.");
      Assertion.Assert(!publicCall || this.RecordingBook.IsAvailableForManualEditing,
                       "This recording can't be deleted because its recording book is not available for manual editing.");
      this.Status = RecordableObjectStatus.Deleted;
      this.canceledBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.canceledTime = DateTime.Now;
      this.Save();
    }

    public RecordingAct CreateAnnotation(LRSTransaction transaction,
                                         RecordingActType recordingActType, Property property) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(transaction.Document, "document");
      Assertion.Assert(!transaction.Document.IsEmptyInstance && !transaction.Document.IsNew,
                        "Transaction document can not be neither an empty or a new document instance");
      Assertion.Assert(!property.IsNew && !property.IsEmptyInstance,
                        "Property can not be empty or a new instance");
      Assertion.Assert(!this.IsEmptyInstance && !this.IsNew,
                        "Can not create an annotation using an empty or new recording");

      var recording = this.RecordingBook.CreateRecordingForAnnotation(transaction, this);

      var recordingAct = RecordingAct.Create(recordingActType, recording, property);

      this.Refresh();
      this.RecordingBook.Refresh();

      return recordingAct;
    }

    public RecordingAct CreateRecordingAct(RecordingActType recordingActType, Property property) {
      if (this.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "CreateRecordingAct");
      }
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordableObjectStatus.Obsolete) {
        this.Status = RecordableObjectStatus.Incomplete;
        this.Save();
      }

      var recordingAct = RecordingAct.Create(recordingActType, this, property);

      this.Refresh();
      this.RecordingBook.Refresh();

      return recordingAct;
    }

    public void DeleteRecordingAct(RecordingAct recordingAct) {
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, 
                                            recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, recordingAct.Id);
      }
      recordingAct.Delete();
      SortRecordingActs();
      this.DeleteMeIfNecessary();
      this.Refresh();
      this.RecordingBook.Refresh();
    }


    private void DeleteMeIfNecessary() {
      if (this.RecordingBook.IsAvailableForManualEditing) {
        return;
      }
      if (this.RecordingActs.Count != 0) {
        return;
      }
      this.Delete(false);
    }

    public RecordingAttachmentFolder GetAttachementFolder(string folderName) {
      RecordingAttachmentFolderList folderList = this.GetAttachmentFolderList();

      foreach (RecordingAttachmentFolder folder in folderList) {
        if (folder.Name == folderName) {
          return folder;
        }
      }
      throw new LandRegistrationException(LandRegistrationException.Msg.AttachmentFolderNotFound, 
                                          folderName);
    }

    public RecordingAttachmentFolderList GetAttachmentFolderList() {
      if (attachmentFolderList != null) {
        return attachmentFolderList;
      }
      attachmentFolderList = new RecordingAttachmentFolderList();

      attachmentFolderList.Append(this, "Raíz");

      FixedList<TractIndexItem> annotations = this.GetPropertiesAnnotationsList();
      for (int i = 0; i < annotations.Count; i++) {
        string alias = Char.ConvertFromUtf32(65 + i);
        attachmentFolderList.Append(annotations[i].RecordingAct.Recording, alias);
      }
      return attachmentFolderList;
    }

    public IList<Property> GetProperties() {
      var list = new List<Property>(this.RecordingActs.Count);
      foreach (RecordingAct recordingAct in this.RecordingActs) {
        foreach (var property in recordingAct.GetProperties()) {
          if (!list.Contains(property)) {
            list.Add(property);
          }
        } // foreach
      } // foreach
      return list;
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
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordableObjectStatus.Closed) {
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

    public FixedList<RecordingAct> GetNoAnnotationActs() {
      FixedList<RecordingAct> recordingActs = this.RecordingActs;

      return new FixedList<RecordingAct>(recordingActs.FindAll((x) => !x.IsAnnotation));
    }

    public FixedList<TractIndexItem> GetPropertiesAnnotationsList() {
      return PropertyData.GetRecordingPropertiesAnnotationsList(this);
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.document = RecordingDocument.Parse((int) row["DocumentId"]);
      this.recordingBook = RecordingBook.Parse((int) row["RecordingBookId"]);
      this.number = (string) row["RecordingNumber"];
      this.notes = (string) row["RecordingNotes"];
      // EXTDATA
      this.keywords = (string) row["RecordingKeywords"];
      this.presentationTime = (DateTime) row["RecordingPresentationTime"];
      this.authorizedTime = (DateTime) row["RecordingAuthorizationTime"];
      this.qualifiedBy = Contact.Parse((int) row["ReviewedById"]);
      this.authorizedBy = Contact.Parse((int) row["AuthorizedById"]);
      this.capturedBy = Contact.Parse((int) row["RecordedById"]);
      this.capturedTime = (DateTime) row["RecordingTime"];
      this.status = (RecordableObjectStatus) Convert.ToChar(row["RecordingStatus"]);
      this.recordIntegrityHashCode = (string) row["RecordingDIF"];

      //this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);

      //this.startImageIndex = (int) row["RecordingBookFirstImage"];
      //this.endImageIndex = (int) row["RecordingBookLastImage"];

      //this.receiptNumber = (string) row["ReceiptNumber"];
      //this.receiptTotal = (decimal) row["ReceiptTotal"];
      //this.receiptIssueDate = (DateTime) row["ReceiptIssueDate"];
   
      //this.qualifiedTime = (DateTime) row["RecordingQualifiedTime"];
      

      //this.canceledBy = Contact.Parse((int) row["RecordingCanceledById"]);
      //this.canceledTime = (DateTime) row["RecordingCanceledTime"];
      //this.cancelationReasonId = (int) row["RecordingCancelationReasonId"];
      //this.cancelationNotes = (string) row["RecordingCancelationNotes"];
      //this.digitalString = (string) row["RecordingDigitalString"];
      //this.digitalSign = (string) row["RecordingDigitalSign"];
      

    }

    protected override void ImplementsSave() {
      if (!this.RecordingDocument.IsEmptyInstance) {
        this.RecordingDocument.Save();
      }
      if (this.IsNew) {
        this.capturedTime = DateTime.Now;
        this.capturedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.keywords = EmpiriaString.BuildKeywords(this.number, this.recordingBook.FullName, 
                                                  this.RecordingDocument.UniqueCode);

      RecordingBooksData.WriteRecording(this);
    }

    public void Refresh() {
      this.recordingActList = null;
    }

    public void SetNumber(int recordingNumber, string bisSuffixTag = "") {
      this.number = RecordingNumber(recordingNumber, bisSuffixTag);
    }

    public void UpwardRecordingAct(RecordingAct recordingAct) {
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (!this.RecordingActs.Contains(recordingAct)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, recordingAct.Id, this.Id);
      }
      if (recordingAct.Status == RecordableObjectStatus.Closed) {
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

} // namespace Empiria.Land.Registration
