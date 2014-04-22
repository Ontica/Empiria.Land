/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocument                              Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a Land Registration System document that is attached to a Recording.               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;
using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  public enum DocumentRecordingRole {
    Appendix = 'A',
    Main = 'M',
  }

  public class RecordingDocument : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingDocument";

    private LRSDocumentType documentSubtype = LRSDocumentType.Empty;
    private string documentKey = String.Empty;
    private DocumentRecordingRole documentRecordingRole = DocumentRecordingRole.Main;
    private GeographicRegionItem issuePlace = GeographicRegionItem.Empty;
    private Organization issueOffice = Organization.Empty;
    private Contact issuedBy = Person.Empty;
    private TypeAssociationInfo issuedByPosition = TypeAssociationInfo.Empty;

    private DateTime issueDate = ExecutionServer.DateMinValue;
    private Contact mainWitness = Person.Empty;
    private TypeAssociationInfo mainWitnessPosition = TypeAssociationInfo.Empty;

    private Contact secondaryWitness = Person.Empty;
    private TypeAssociationInfo secondaryWitnessPosition = TypeAssociationInfo.Empty;

    private string name = String.Empty;
    private string fileName = String.Empty;
    private string bookNumber = String.Empty;
    private string expedientNumber = String.Empty;
    private string number = String.Empty;
    private int sheetsCount = -1;
    private decimal sealUpperPosition = -1;
    private string startSheet = String.Empty;
    private string endSheet = String.Empty;
    private string notes = String.Empty;
    private string keywords = String.Empty;
    private Contact reviewedBy = Person.Empty;
    private string authorizationKey = String.Empty;

    private string digitalString = String.Empty;
    private string digitalSign = String.Empty;
    private Contact postedBy = Person.Empty;
    private DateTime postingTime = DateTime.Now;
    private RecordableObjectStatus status = RecordableObjectStatus.Incomplete;
    private string recordIntegrityHashCode = String.Empty;

    private RecordingDocumentType recordingDocumentType = null;

    #endregion Fields

    #region Constructors and parsers

    private RecordingDocument()
      : base(thisTypeName) {
      // Instance creation of this type may be invoked with ....
    }

    protected RecordingDocument(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public RecordingDocument Parse(int id) {
      return BaseObject.Parse<RecordingDocument>(thisTypeName, id);
    }

    static internal RecordingDocument Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingDocument>(thisTypeName, dataRow);
    }

    static internal RecordingDocument Parse(Recording recording) {
      DataRow dataRow = RecordingBooksData.GetRecordingMainDocument(recording);
      if (dataRow != null) {
        return RecordingDocument.Parse(dataRow);
      } else {
        RecordingDocument recordingDocument = RecordingDocument.Empty;
        // recordingDocument.recording = recording;
        return recordingDocument;
      }
    }

    static public RecordingDocument Create(RecordingDocumentType documentType) {
      RecordingDocument newDocument = documentType.CreateInstance();

      newDocument.documentRecordingRole = DocumentRecordingRole.Main;

      return newDocument;
    }

    static public RecordingDocument Empty {
      get { return BaseObject.ParseEmpty<RecordingDocument>(thisTypeName); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public string AuthorizationKey {
      get { return authorizationKey; }
    }

    public string BookNumber {
      get { return bookNumber; }
      set { bookNumber = value.ToUpperInvariant(); }
    }

    public string DigitalSign {
      get { return digitalSign; }
    }

    public string DigitalString {
      get { return digitalString; }
    }

    public string DocumentKey {
      get { return documentKey; }
    }

    public DocumentRecordingRole DocumentRecordingRole {
      get { return documentRecordingRole; }
      set { documentRecordingRole = value; }
    }

    public string EndSheet {
      get { return endSheet; }
      set { endSheet = value; }
    }

    public string ExpedientNumber {
      get { return expedientNumber; }
      set { expedientNumber = value.ToUpperInvariant(); }
    }

    public string FileName {
      get { return fileName; }
      set { fileName = value; }
    }

    public DateTime IssueDate {
      get { return issueDate; }
      set { issueDate = value; }
    }

    public Contact IssuedBy {
      get { return issuedBy; }
      set { issuedBy = value; }
    }

    public TypeAssociationInfo IssuedByPosition {
      get { return issuedByPosition; }
      set { issuedByPosition = value; }
    }

    public GeographicRegionItem IssuePlace {
      get { return issuePlace; }
      set { issuePlace = value; }
    }

    public Organization IssueOffice {
      get { return issueOffice; }
      set { issueOffice = value; }
    }

    public string Keywords {
      get { return keywords; }
      protected set { keywords = value; }
    }

    public Contact MainWitness {
      get { return mainWitness; }
      set { mainWitness = value; }
    }

    public TypeAssociationInfo MainWitnessPosition {
      get { return mainWitnessPosition; }
      set { mainWitnessPosition = value; }
    }

    public string Name {
      get { return name; }
      set { name = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public string Number {
      get { return number; }
      set { number = value; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public RecordingDocumentType RecordingDocumentType {
      get {
        if (this.recordingDocumentType == null) {
          recordingDocumentType = RecordingDocumentType.Parse(base.ObjectTypeInfo);
        }
        return recordingDocumentType;
      }
      internal set {
        recordingDocumentType = value;        
      }
    }

    public string RecordIntegrityHashCode {
      get { return recordIntegrityHashCode; }
    }

    public Contact ReviewedBy {
      get { return reviewedBy; }
      set { reviewedBy = value; }
    }

    public decimal SealUpperPosition {
      get { return sealUpperPosition; }
      set { sealUpperPosition = value; }
    }

    public Contact SecondaryWitness {
      get { return secondaryWitness; }
      set { secondaryWitness = value; }
    }


    public TypeAssociationInfo SecondaryWitnessPosition {
      get { return secondaryWitnessPosition; }
      set { secondaryWitnessPosition = value; }
    }

    public int SheetsCount {
      get { return sheetsCount; }
      set { sheetsCount = value; }
    }

    public string StartSheet {
      get { return startSheet; }
      set { startSheet = value; }
    }

    public RecordableObjectStatus Status {
      get { return status; }
    }

    public LRSDocumentType Subtype {
      get { return documentSubtype; }
      set { documentSubtype = value; }
    }

    #endregion Public properties

    #region Public methods

    public void ChangeDocumentType(RecordingDocumentType newRecordingDocumentType) {
      if (this.RecordingDocumentType.Equals(newRecordingDocumentType)) {
        return;
      }
      this.RecordingDocumentType = newRecordingDocumentType;
      this.documentSubtype = LRSDocumentType.Empty;
      this.issuePlace = GeographicRegionItem.Empty;
      this.issueOffice = Organization.Empty;
      this.issuedBy = Person.Empty;
      this.issuedByPosition = TypeAssociationInfo.Empty;
      this.issueDate = ExecutionServer.DateMinValue;
      this.mainWitness = Person.Empty;
      this.mainWitnessPosition = TypeAssociationInfo.Empty;
      this.secondaryWitness = Person.Empty;
      this.secondaryWitnessPosition = TypeAssociationInfo.Empty;
      this.name = String.Empty;
      this.fileName = String.Empty;
      this.bookNumber = String.Empty;
      this.expedientNumber = String.Empty;
      this.number = String.Empty;
      this.startSheet = String.Empty;
      this.endSheet = String.Empty;
      this.notes = String.Empty;
      this.keywords = String.Empty;
      this.reviewedBy = Person.Empty;
      this.authorizationKey = String.Empty;
      this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.postingTime = DateTime.Now;
      this.digitalString = String.Empty;
      this.digitalSign = String.Empty;
      this.status = RecordableObjectStatus.Incomplete;
      this.recordIntegrityHashCode = String.Empty;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.documentSubtype = LRSDocumentType.Parse((int) row["DocumentSubtypeId"]);
      this.documentKey = (string) row["DocumentKey"];
      this.documentRecordingRole = (DocumentRecordingRole) Convert.ToChar(row["DocumentRecordingRole"]);
      this.issuePlace = GeographicRegionItem.Parse((int) row["IssuePlaceId"]);
      this.issueOffice = Organization.Parse((int) row["IssueOfficeId"]);
      this.issuedBy = Contact.Parse((int) row["IssuedById"]);
      this.issuedByPosition = TypeAssociationInfo.Parse((int) row["IssuedByPositionId"]);
      this.issueDate = (DateTime) row["IssueDate"];
      this.mainWitness = Contact.Parse((int) row["MainWitnessId"]);
      this.mainWitnessPosition = TypeAssociationInfo.Parse((int) row["MainWitnessPositionId"]);
      this.secondaryWitness = Contact.Parse((int) row["SecondaryWitnessId"]);
      this.secondaryWitnessPosition = TypeAssociationInfo.Parse((int) row["SecondaryWitnessPositionId"]);
      this.name = (string) row["DocumentName"];
      this.fileName = (string) row["DocumentFileName"];
      this.bookNumber = (string) row["DocumentBookNumber"];
      this.expedientNumber = (string) row["DocumentExpedient"];
      this.number = (string) row["DocumentNumber"];

      this.sheetsCount = (int) row["DocumentSheets"];
      this.sealUpperPosition = (decimal) row["DocumentSealUpperPosition"];

      this.startSheet = (string) row["DocumentStartSheet"];
      this.endSheet = (string) row["DocumentEndSheet"];
      this.notes = (string) row["DocumentNotes"];
      this.keywords = (string) row["DocumentKeywords"];
      this.reviewedBy = Person.Parse((int) row["DocumentReviewedById"]);
      this.authorizationKey = (string) row["DocumentAuthorizationKey"];

      this.digitalString = (string) row["DocumentDigitalString"];
      this.digitalSign = (string) row["DocumentDigitalSign"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.postingTime = (DateTime) row["PostingTime"];
      this.status = (RecordableObjectStatus) Convert.ToChar(row["DocumentStatus"]);
      this.recordIntegrityHashCode = (string) row["DocumentRIHC"];
    }

    protected override void ImplementsSave() {
      PrepareForSave();
      RecordingBooksData.WriteRecordingDocument(this);
    }

    internal void PrepareForSave() {
      if (this.PostedBy.IsEmptyInstance) {      // IsNew
        this.postingTime = DateTime.Now;
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      if (String.IsNullOrWhiteSpace(this.documentKey)) {
        this.documentKey = TransactionData.GenerateDocumentKey();
      }
      this.keywords = EmpiriaString.BuildKeywords(this.DocumentKey, !this.Subtype.IsEmptyInstance ? Name : this.RecordingDocumentType.DisplayName, 
                                                  this.Name, this.FileName, this.bookNumber, this.expedientNumber, this.number);
    }

    #endregion Public methods

  } // class RecordingDocument

} // namespace Empiria.Geography