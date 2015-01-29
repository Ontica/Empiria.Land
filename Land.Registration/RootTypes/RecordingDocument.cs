/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocument                              Pattern  : Partitioned type                    *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Partitioned type that represents a document that is attached to recordings.                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;
using Empiria.Ontology;
using Empiria.Security;

namespace Empiria.Land.Registration {

  /// <summary>Partitioned type that represents a document that is attached to recordings.</summary>
  [PartitionedType(typeof(RecordingDocumentType))]
  public class RecordingDocument : BaseObject, IExtensible<RecordingDocumentExtData>, IProtected {

    #region Fields

    private Lazy<List<RecordingAct>> recordingActList = null;

    #endregion Fields

    #region Constructors and parsers

    public RecordingDocument(RecordingDocumentType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public RecordingDocument Parse(int id) {
      return BaseObject.ParseId<RecordingDocument>(id);
    }

    static public RecordingDocument TryParse(string documentUID) {
      return BaseObject.TryParse<RecordingDocument>("DocumentUID = '" + documentUID + "'");
    }

    static internal RecordingDocument TryParse(Recording recording) {
      DataRow dataRow = DocumentsData.GetRecordingMainDocument(recording);
      if (dataRow != null) {
        return BaseObject.ParseDataRow<RecordingDocument>(dataRow);
      } else {
        return null;
      }
    }

    static public RecordingDocument Empty {
      get { return BaseObject.ParseEmpty<RecordingDocument>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingDocumentType DocumentType {
      get {
        return (RecordingDocumentType) base.GetEmpiriaType();
      }
    }

    [DataField("DocumentSubtypeId")]
    public LRSDocumentType Subtype {
      get;
      set;
    }

    [DataField("DocumentUID", IsOptional = false)]
    public string UID {
      get;
      private set;
    }

    [DataField("ImagingControlID")]
    public string ImagingControlID {
      get;
      private set;
    }

    [DataField("PresentationTime", Default = "ExecutionServer.DateMaxValue")]
    public DateTime PresentationTime {
      get;
      set;
    }

    [DataField("AuthorizationTime", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AuthorizationTime {
      get;
      set;
    }

    [DataField("IssuePlaceId")]
    private LazyInstance<GeographicRegion> _issuePlace = LazyInstance<GeographicRegion>.Empty;
    public GeographicRegion IssuePlace {
      get { return _issuePlace.Value; }
      set {
        _issuePlace = LazyInstance<GeographicRegion>.Parse(value);
      }
    }

    [DataField("IssueOfficeId")]
    private LazyInstance<Organization> _issueOffice = LazyInstance<Organization>.Empty;
    public Organization IssueOffice {
      get { return _issueOffice.Value; }
      set {
        _issueOffice = LazyInstance<Organization>.Parse(value);
      }
    }

    [DataField("IssuedById")]
    private LazyInstance<Contact> _issuedBy = LazyInstance<Contact>.Empty;
    public Contact IssuedBy {
      get { return _issuedBy.Value; }
      set {
        _issuedBy = LazyInstance<Contact>.Parse(value);
      }
    }

    [DataField("IssueDate", Default = "ExecutionServer.DateMinValue")]
    public DateTime IssueDate {
      get;
      set;
    }

    public string Number {
      get {
        return ExtensionData.DocumentNo;
      }
      set {
        ExtensionData.DocumentNo = value;
      }
    }

    public string ExpedientNo {
      get {
        return ExtensionData.ExpedientNo;
      }
      set {
        ExtensionData.ExpedientNo = value;
      }
    }

    [DataField("DocumentAsText")]
    public string AsText {
      get;
      private set;
    }

    [DataField("DocumentOverview")]
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
        return EmpiriaString.BuildKeywords(this.UID,
                    !this.Subtype.IsEmptyInstance ? this.AsText : this.DocumentType.DisplayName,
                    this.AsText);
      }
    }

    [DataField("PostedById")]
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

    public FixedList<RecordingAct> RecordingActs {
      get {
        Predicate<RecordingAct> match = (x) => x.Status != RecordableObjectStatus.Deleted;

        return recordingActList.Value.FindAll(match).ToFixedList();
      }
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "DocumentTypeId", this.DocumentType.Id,
          "SubtypeId", this.Subtype.Id, "UID", this.UID,
          "IssuePlaceId", this.IssuePlace.Id, "IssueOfficeId", this.IssueOffice.Id,
          "IssuedById", this.IssuedBy.Id, "IssueDate", this.IssueDate,
          "AsText", this.AsText, "SheetsCount", this.SheetsCount,
          "ExtensionData", this.ExtensionData.ToJson(),
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

    public bool IsEmptyDocument {
      get {
        return (this.DocumentType == RecordingDocumentType.Empty);
      }
    }

    #endregion Public properties

    #region Public methods

    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Property resource,
                                           RecordingAct amendmentOf = null,
                                           Recording physicalRecording = null) {
      amendmentOf = (amendmentOf != null) ? amendmentOf : InformationAct.Empty;
      physicalRecording = (physicalRecording != null) ? physicalRecording : Recording.Empty;

      Assertion.AssertObject(recordingActType, "recordingActType");
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(amendmentOf, "amendmentOf");
      Assertion.AssertObject(physicalRecording, "physicalRecording");

      Assertion.Assert(!this.IsEmptyInstance, "Document can't be the empty instance");
      Assertion.Assert(this.Status != RecordableObjectStatus.Closed,
                       "Recording acts can't be appended to closed documents");

      if (this.IsNew) {
        this.Save();
      }

      var recordingAct = RecordingAct.Create(recordingActType, this, resource, amendmentOf,
                                             this.RecordingActs.Count, physicalRecording);
      recordingActList.Value.Add(recordingAct);

      this.AuthorizationTime = DateTime.Now;
      this.Save();

      return recordingAct;
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
      this.RecordingActs[currentIndex + 1].Index -= 1;
      this.RecordingActs[currentIndex + 1].Save();
      recordingAct.Index += 1;
      recordingAct.Save();
      this.recordingActList = null;
    }

    public void RemoveRecordingAct(RecordingAct recordingAct) {
      Assertion.AssertObject(recordingAct, "recordingAct");

      Assertion.Assert(this.Status != RecordableObjectStatus.Closed,
                       "Recording acts can't be removed from closed documents");

      Assertion.Assert(recordingAct.Document == this,
                       "The recording act doesn't belong to this document");

      recordingAct.Delete();
      recordingActList.Value.Remove(recordingAct);

      if (this.RecordingActs.Count == 0 && this.IsEmptyDocument) {
        this.Delete();
      }
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

    public void ChangeDocumentType(RecordingDocumentType newRecordingDocumentType) {
      if (this.DocumentType.Equals(newRecordingDocumentType)) {
        return;
      }
      base.ReclassifyAs(newRecordingDocumentType);
      this.IssueDate = ExecutionServer.DateMinValue;
      this.ExtensionData = RecordingDocumentExtData.Empty;
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    }

    protected override void OnInitialize() {
      this.ExtensionData = new RecordingDocumentExtData();
      this.Number = String.Empty;
      this.ExpedientNo = String.Empty;
      recordingActList = new Lazy<List<RecordingAct>>(() => RecordingActsData.GetRecordingActs(this));
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingDocumentExtData.Parse((string) row["DocumentExtData"]);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.UID = DocumentsData.GenerateDocumentUID();
      }
      RecordingBooksData.WriteRecordingDocument(this);
    }

    private void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      this.Save();
    }

    #endregion Public methods

  } // class RecordingDocument

} // namespace Empiria.Land.Registration
