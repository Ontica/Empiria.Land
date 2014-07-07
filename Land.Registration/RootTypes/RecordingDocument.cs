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
using Empiria.Security;

namespace Empiria.Land.Registration {

  public class RecordingDocument : BaseObject, IExtensible<RecordingDocumentExtData>, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingDocument";

    #endregion Fields

    #region Constructors and parsers

    private RecordingDocument()
      : base(thisTypeName) {
      // Instance creation of this type may be invoked with RecordingDocument.Create method
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
      return documentType.CreateInstance();
    }

    static public RecordingDocument Empty {
      get { return BaseObject.ParseEmpty<RecordingDocument>(thisTypeName); }
    }

    #endregion Constructors and parsers

    #region Public properties

    private RecordingDocumentType _documentType = null;
    public RecordingDocumentType DocumentType {
      get {
        if (_documentType == null) {
          _documentType = RecordingDocumentType.Parse(base.ObjectTypeInfo);
        }
        return _documentType;
      }
      internal set {
        _documentType = value;
      }
    }

    public LRSDocumentType Subtype {
      get;
      set;
    }

    public string UniqueCode {
      get;
      private set;
    }

    public GeographicRegionItem IssuePlace {
      get;
      set;
    }

    public Organization IssueOffice {
      get;
      set;
    }

    public Contact IssuedBy {
      get;
      set;
    }

    public DateTime IssueDate {
      get;
      set;
    }

    public string ExpedientNo {
      get;
      set;
    }

    public string Number {
      get;
      set;
    }

    public string Title {
      get;
      private set;
    }

    public string Notes {
      get;
      set;
    }

    public int SheetsCount {
      get;
      set;
    }

    public RecordingDocumentExtData ExtensionData {
      get;
      private set;
    }

    public string Keywords {
      get;
      private set;
    }

    public Contact PostedBy {
      get;
      private set;
    }

    public DateTime PostingTime {
      get;
      private set;
    }

    public RecordableObjectStatus Status {
      get;
      private set;
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "DocumentTypeId", this.ObjectTypeInfo.Id,
          "SubtypeId", this.Subtype.Id, "UniqueCode", this.UniqueCode,
          "IssuePlaceId", this.IssuePlace.Id, "IssueOfficeId", this.IssueOffice.Id,
          "IssuedById", this.IssuedBy.Id, "IssueDate", this.IssueDate,
          "DocumentNo", this.Number, "ExpedientNo", this.ExpedientNo,
          "SheetsCount", this.SheetsCount, "ExtensionData", this.ExtensionData.ToJson(),
          "PostedBy", this.PostedBy.Id, "PostingTime", this.PostingTime,
          "Status", (char) this.Status,
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion Public properties

    #region Public methods

    public void ChangeDocumentType(RecordingDocumentType newRecordingDocumentType) {
      if (this.DocumentType.Equals(newRecordingDocumentType)) {
        return;
      }
      this.DocumentType = newRecordingDocumentType;
      this.Subtype = LRSDocumentType.Empty;
      this.IssuePlace = GeographicRegionItem.Empty;
      this.IssueOffice = Organization.Empty;
      this.IssuedBy = Person.Empty;
      this.IssueDate = ExecutionServer.DateMinValue;
      this.Number = String.Empty;
      this.ExpedientNo = String.Empty;
      this.Title = String.Empty;
      this.Notes = String.Empty;
      this.ExtensionData = RecordingDocumentExtData.Empty;
      this.Keywords = String.Empty;
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.PostingTime = DateTime.Now;
      this.Status = RecordableObjectStatus.Incomplete;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.Subtype = LRSDocumentType.Parse((int) row["DocumentSubtypeId"]);
      this.UniqueCode = (string) row["DocumentUniqueCode"];
      this.IssuePlace = GeographicRegionItem.Parse((int) row["IssuePlaceId"]);
      this.IssueOffice = Organization.Parse((int) row["IssueOfficeId"]);
      this.IssuedBy = Contact.Parse((int) row["IssuedById"]);
      this.IssueDate = (DateTime) row["IssueDate"];
      this.Number = (string) row["DocumentNo"];
      this.ExpedientNo = (string) row["ExpedientNo"];
      this.Title = (string) row["DocumentTitle"];
      this.Notes = (string) row["DocumentNotes"];
      this.SheetsCount = (int) row["SheetsCount"];
      this.ExtensionData = RecordingDocumentExtData.Parse((string) row["DocumentExtData"]);
      this.Keywords = (string) row["DocumentKeywords"];
      this.PostedBy = Contact.Parse((int) row["PostedById"]);
      this.PostingTime = (DateTime) row["PostingTime"];
      this.Status = (RecordableObjectStatus) Convert.ToChar(row["DocumentStatus"]);

      Integrity.Assert((string) row["DocumentDIF"]);
    }

    protected override void ImplementsSave() {
      PrepareForSave();
      RecordingBooksData.WriteRecordingDocument(this);
    }

    internal void PrepareForSave() {
      if (this.PostedBy.IsEmptyInstance) {      // IsNew
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      if (String.IsNullOrWhiteSpace(this.UniqueCode)) {
        this.UniqueCode = TransactionData.GenerateDocumentKey();
      }
      this.Keywords = EmpiriaString.BuildKeywords(this.UniqueCode, 
                                    !this.Subtype.IsEmptyInstance ? this.Title : this.DocumentType.DisplayName,
                                    this.Title, this.ExpedientNo, this.Number);
    }

    #endregion Public methods

  } // class RecordingDocument

} // namespace Empiria.Land.Registration
