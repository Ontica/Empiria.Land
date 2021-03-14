/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Partitioned type                      *
*  Type     : RecordingDocumentSecurity                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Partitioned type that represents a recording document with one or more recording acts.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Ontology;

using Empiria.Land.Data;
using Empiria.Land.Providers;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Partitioned type that represents a document that is attached to recordings.</summary>
  [PartitionedType(typeof(RecordingDocumentType))]
  public class RecordingDocument : BaseObject, IExtensible<RecordingDocumentExtData> {

    #region Fields

    private Lazy<List<RecordingAct>> recordingActList = null;

    #endregion Fields

    #region Constructors and parsers

    public RecordingDocument(RecordingDocumentType powerType) : base(powerType) {
      if (powerType.Equals(RecordingDocumentType.Empty)) {
        this.Status = RecordableObjectStatus.Closed;
      }
    }


    static public RecordingDocument Parse(int id) {
      return BaseObject.ParseId<RecordingDocument>(id);
    }

    static public RecordingDocument Parse(int id, bool reload) {
      return BaseObject.ParseId<RecordingDocument>(id, reload);
    }

    static public RecordingDocument Parse(string uid) {
      return BaseObject.ParseKey<RecordingDocument>(uid);
    }

    static public RecordingDocument ParseGuid(string guid) {
      return BaseObject.TryParse<RecordingDocument>($"DocumentGUID = '{guid}'");
    }

    static public RecordingDocument TryParse(string documentUID, bool reload = false) {
      return BaseObject.TryParse<RecordingDocument>($"DocumentUID = '{documentUID }'", reload);
    }

    static public RecordingDocument TryParse(int id, bool reload = false) {
      return BaseObject.TryParse<RecordingDocument>($"DocumentId = {id}", reload);
    }


    static internal RecordingDocument TryParse(PhysicalRecording recording) {
      DataRow dataRow = DocumentsData.GetRecordingMainDocument(recording);
      if (dataRow != null) {
        return BaseObject.ParseDataRow<RecordingDocument>(dataRow);
      } else {
        return null;
      }
    }

    static public RecordingDocument TryParseForInstrument(int instrumentId) {
      return BaseObject.TryParse<RecordingDocument>($"InstrumentId = {instrumentId}", true);
    }

    static public RecordingDocument Empty {
      get { return BaseObject.ParseEmpty<RecordingDocument>(); }
    }

    static public FixedList<RecordingDocument> SearchClosed(string filter, string keywords = "") {
      return DocumentsData.SearchClosedDocuments(filter, keywords);
    }

    public static RecordingDocument CreateFromInstrument(int instrumentId,
                                                         int instrumentTypeId,
                                                         string kind) {
      var documentType = RecordingDocumentType.ParseFromInstrumentTypeId(instrumentTypeId);
      var doc = new RecordingDocument(documentType);

      doc.GUID = Guid.NewGuid().ToString().ToLower();
      doc.InstrumentId = instrumentId;
      doc.Subtype = LRSDocumentType.ParseFromInstrumentKind(kind);

      return doc;
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("DocumentGuid")]
    public string GUID {
      get;
      private set;
    }

    [DataField("InstrumentId")]
    public int InstrumentId {
      get;
      private set;
    }

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
    private string _documentUID = String.Empty;

    public override string UID {
      get {
        return _documentUID;
      }
    }


    public bool IsHistoricDocument {
      get {
        if (this.IsEmptyInstance) {
          return false;
        }
        if (this.DocumentType.Equals(RecordingDocumentType.Empty)) {
          return true;
        }
        if (this.RecordingActs.Count == 0) {
          return false;
        }
        return (!this.RecordingActs[0].PhysicalRecording.IsEmptyInstance);
      }
    }


    [DataField("PresentationTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime PresentationTime {
      get;
      internal set;
    }

    [DataField("AuthorizationTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime AuthorizationTime {
      get;
      private set;
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
                    this.Notes);
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

    public bool IsClosed {
      get {
        return (this.Status == RecordableObjectStatus.Closed);
      }
    }

    public bool IsEmptyDocumentType {
      get {
        return (this.DocumentType == RecordingDocumentType.Empty);
      }
    }

    [DataObject]
    public RecordingDocumentImaging Imaging {
      get;
      private set;
    }

    [DataObject]
    public RecordingDocumentSecurity Security {
      get;
      private set;
    }


    #endregion Public properties

    #region Public methods

    /// <summary>Adds a recording act to the document's recording acts collection.</summary>
    /// <param name="recordingAct">The item to be added to the end of the RecordingActs collection.</param>
    /// <returns> The recording act's index inside the RecordingActs collection.</returns>
    internal int AddRecordingAct(RecordingAct recordingAct) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.Assert(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be added to closed documents.");

      recordingActList.Value.Add(recordingAct);

      /// returns the collection's index of the recording act
      return recordingActList.Value.Count - 1;
    }

    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Resource resource,
                                           RecordingAct amendmentOf = null,
                                           PhysicalRecording physicalRecording = null) {
      amendmentOf = amendmentOf ?? RecordingAct.Empty;
      physicalRecording = physicalRecording ?? PhysicalRecording.Empty;

      Assertion.Assert(!this.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Assert(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be added to closed documents");

      Assertion.AssertObject(recordingActType, "recordingActType");
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(amendmentOf, "amendmentOf");
      Assertion.AssertObject(!resource.IsEmptyInstance, "Resource can't be an empty instance.");

      if (this.IsNew) {
        this.Save();
      }

      var recordingAct = RecordingAct.Create(recordingActType, this, resource, amendmentOf,
                                             this.RecordingActs.Count, physicalRecording);
      recordingActList.Value.Add(recordingAct);

      return recordingAct;
    }

    internal void SetAuthorizationTime(DateTime authorizationTime) {
      Assertion.Assert(this.IsNew || this.IsHistoricDocument,
                      "AutorizationTime can be set only over new or historic documents.");
      this.AuthorizationTime = authorizationTime;
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

    private void SetAuthorizationTime() {
      if (!this.IsHistoricDocument) {
        this.AuthorizationTime = DateTime.Now;
      }
    }

    internal void Close() {
      this.SetAuthorizationTime();
      this.Status = RecordableObjectStatus.Closed;
      this.Save();
    }

    internal void Open() {
      this.Status = RecordableObjectStatus.Incomplete;
      this.Save();
    }


    public List<Contact> GetRecordingOfficials() {
      var recordingOfficials = new List<Contact>();

      string temp = String.Empty;

      var recordingActs = this.RecordingActs;
      for (int i = 0; i < recordingActs.Count; i++) {
        if (!recordingOfficials.Contains(recordingActs[i].RegisteredBy)) {
          recordingOfficials.Add(recordingActs[i].RegisteredBy);
        }
      }
      return recordingOfficials;
    }

    public Resource GetUniqueInvolvedResource() {
      var recordingActs = this.RecordingActs;

      if (recordingActs.Count == 0) {
        return Resource.Empty;
      } else if (recordingActs.Count == 1) {
        return recordingActs[0].Resource;
      }

      var distinctResources = recordingActs.Select((x) => x.Resource).GroupBy((x) => x.Id).ToList();

      if (distinctResources.Count == 1) {
        return recordingActs[0].Resource;
      } else {
        return Resource.Empty;
      }
    }

    private LRSTransaction _transaction = null;
    public LRSTransaction GetTransaction() {
      if (this.IsEmptyInstance || this.IsEmptyDocumentType) {
        return LRSTransaction.Empty;
      }
      if (_transaction == null) {
        _transaction = DocumentsData.GetDocumentTransaction(this);
        if (_transaction.IsEmptyInstance) {
          _transaction = null;
          return LRSTransaction.Empty;
        }
      }
      return _transaction;
    }

    protected override void OnInitialize() {
      this.ExtensionData = new RecordingDocumentExtData();
      this.Number = String.Empty;
      this.ExpedientNo = String.Empty;

      recordingActList = new Lazy<List<RecordingAct>>(() => RecordingActsData.GetDocumentRecordingActs(this));

      this.Imaging = new RecordingDocumentImaging(this);
      this.Security = new RecordingDocumentSecurity(this);
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingDocumentExtData.Parse((string) row["DocumentExtData"]);
    }

    protected override void OnBeforeSave() {
      if (this.IsNew) {
        this._documentUID = ExternalProviders.UniqueIDGeneratorProvider.GenerateDocumentUID();
      }
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecordingDocument(this);
    }

    public void RemoveRecordingAct(RecordingAct recordingAct) {
      Assertion.AssertObject(recordingAct, "recordingAct");

      Assertion.Assert(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be removed from closed documents.");

      Assertion.Assert(recordingAct.Document.Equals(this),
                       "The recording act doesn't belong to this document.");

      recordingAct.Delete();
      recordingActList.Value.Remove(recordingAct);

      if (this.RecordingActs.Count == 0 && this.IsEmptyDocumentType) {
        this.Delete();
      }
    }

    public PhysicalRecording TryGetHistoricRecording() {
      if (!this.IsHistoricDocument) {
        return null;
      }
      PhysicalRecording historicRecording = this.RecordingActs[0].PhysicalRecording;

      Assertion.Assert(!historicRecording.IsEmptyInstance,
                      "historicRecording can't be the empty instance.");

      return historicRecording;
    }

    #endregion Public methods

    #region Private methods

    internal void Delete() {
      if (this.RecordingActs.Count == 0) {
        this.Status = RecordableObjectStatus.Deleted;
        this.Save();
        _transaction = null;
      }
    }

    #endregion Private methods

  } // class RecordingDocument

} // namespace Empiria.Land.Registration
