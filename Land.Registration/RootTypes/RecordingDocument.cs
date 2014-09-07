/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocument                              Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Partitioned type that represents a registration document that is attached to recordings.      *
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

  /// <summary>Partitioned type that represents a registration document that is attached
  /// to recordings.</summary>
  public class RecordingDocument : BaseObject, IExtensible<RecordingDocumentExtData>, IProtected {

    #region Constructors and parsers

    private RecordingDocument(RecordingDocumentType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public RecordingDocument Parse(int id) {
      return BaseObject.ParseId<RecordingDocument>(id);
    }

    static internal RecordingDocument Parse(DataRow dataRow) {
      return BaseObject.ParseDataRow<RecordingDocument>(dataRow);
    }

    static internal RecordingDocument Parse(Recording recording) {
      DataRow dataRow = RecordingBooksData.GetRecordingMainDocument(recording);
      if (dataRow != null) {
        return RecordingDocument.Parse(dataRow);
      } else {
        return RecordingDocument.Empty;
      }
    }

    static public RecordingDocument Create(RecordingDocumentType documentType) {
      return documentType.CreateInstance();
    }

    static public RecordingDocument Empty {
      get { return BaseObject.ParseEmpty<RecordingDocument>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingDocumentType DocumentType {
      get {
        return (RecordingDocumentType) base.ObjectTypeInfo;
      }
    }

    [DataField("DocumentSubtypeId")]
    public LRSDocumentType Subtype {
      get;
      set;
    }

    [DataField("DocumentUniqueCode", IsOptional = false)]
    public string UniqueCode {
      get;
      private set;
    }

    [DataField("IssuePlaceId")]
    private LazyObject<GeographicRegion> _issuePlace = LazyObject<GeographicRegion>.Empty;
    public GeographicRegion IssuePlace {
      get { return _issuePlace.Instance; }
      set { _issuePlace.Instance = value; }
    }

    [DataField("IssueOfficeId")]
    private LazyObject<Organization> _issueOffice = LazyObject<Organization>.Empty;
    public Organization IssueOffice {
      get { return _issueOffice.Instance; }
      set { _issueOffice.Instance = value; }
    }

    [DataField("IssuedById")]
    private LazyObject<Contact> _issuedBy = LazyObject<Contact>.Empty;
    public Contact IssuedBy {
      get { return _issuedBy.Instance; }
      set { _issuedBy.Instance = value; }
    }

    [DataField("IssueDate", Default = "ExecutionServer.DateMinValue")]
    public DateTime IssueDate {
      get;
      set;
    }

    [DataField("ExpedientNo")]
    public string ExpedientNo {
      get;
      set;
    }

    [DataField("DocumentNo")]
    public string Number {
      get;
      set;
    }

    [DataField("DocumentTitle")]
    public string Title {
      get;
      private set;
    }

    [DataField("DocumentNotes")]
    public string Notes {
      get;
      set;
    }

    [DataField("SheetsCount")]
    public int SheetsCount {
      get;
      set;
    }

    public RecordingDocumentExtData ExtensionData {
      get;
      set;
    }

    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UniqueCode, 
                    !this.Subtype.IsEmptyInstance ? this.Title : this.DocumentType.DisplayName,
                    this.Title, this.ExpedientNo, this.Number);
      }
    }

    [DataField("PostedById", Default = "Contacts.Person.Empty")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("DocumentStatus", Default = RecordableObjectStatus.Incomplete)]
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
      base.ReclassifyAs(newRecordingDocumentType);
      this.IssueDate = ExecutionServer.DateMinValue;
      this.ExtensionData = RecordingDocumentExtData.Empty;
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingDocumentExtData.Parse((string) row["DocumentExtData"]);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.UniqueCode = TransactionData.GenerateDocumentKey();
      }
      RecordingBooksData.WriteRecordingDocument(this);
    }

    #endregion Public methods

  } // class RecordingDocument

} // namespace Empiria.Land.Registration
