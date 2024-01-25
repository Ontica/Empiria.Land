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
  public class RecordingDocument : BaseObject {

    #region Fields

    private Lazy<List<RecordingAct>> recordingActList = new Lazy<List<RecordingAct>>();

    #endregion Fields

    #region Constructors and parsers

    public RecordingDocument(RecordingDocumentType powerType) : base(powerType) {
      if (powerType.Equals(RecordingDocumentType.Empty)) {
        this.Status = RecordableObjectStatus.Closed;
      }
      this.Imaging = new RecordingDocumentImaging(this);
      this.Security = new RecordingDocumentSecurity(this);
    }


    static public RecordingDocument Parse(int id) {
      return BaseObject.ParseId<RecordingDocument>(id);
    }


    static public RecordingDocument Parse(int id, bool reload) {
      return BaseObject.ParseId<RecordingDocument>(id, reload);
    }


    static public RecordingDocument ParseGuid(string guid) {
      var recordingDocument = BaseObject.TryParse<RecordingDocument>($"DocumentGUID = '{guid}'");

      Assertion.Require(recordingDocument,
                             $"There is not registered a recording document with guid '{guid}'.");

      return recordingDocument;
    }


    static public RecordingDocument TryParse(string documentUID, bool reload = false) {
      return BaseObject.TryParse<RecordingDocument>($"DocumentUID = '{documentUID}'", reload);
    }


    static public RecordingDocument TryParse(int id, bool reload = false) {
      return BaseObject.TryParse<RecordingDocument>($"DocumentId = {id}", reload);
    }


    static internal RecordingDocument TryParse(BookEntry bookEntry) {
      return DocumentsData.TryGetBookEntryMainDocument(bookEntry);
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
        if (this.GetTransaction().IsEmptyInstance) {
          return true;
        }
        if (this.IsRegisteredInRecordingBook) {
          return true;
        }
        return false;
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
    } = new RecordingDocumentExtData();


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

        return recordingActList.Value.FindAll(match)
                                     .ToFixedList();
      }
    }


    public bool HasRecordingActs {
      get {
        return (this.RecordingActs.Count > 0);
      }
    }


    public bool IsRegisteredInRecordingBook {
      get {
        return (BookEntry.GetBookEntriesForDocument(this.Id).Count != 0);
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
    /// <param name="recordingAct">The item to be added at the end of the RecordingActs collection.</param>
    /// <returns> The recording act's index inside the RecordingActs collection.</returns>
    internal int AppendRecordingAct(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, "recordingAct");

      Assertion.Require(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be added to closed documents.");

      recordingActList.Value.Add(recordingAct);

      /// returns the collection's index of the recording act
      return recordingActList.Value.Count - 1;
    }

    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Resource resource,
                                           RecordingAct amendmentOf = null,
                                           BookEntry bookEntry = null) {
      Assertion.Require(resource, nameof(resource));

      Assertion.Require(!resource.IsEmptyInstance, "Resource can't be an empty instance.");

      amendmentOf = amendmentOf ?? RecordingAct.Empty;
      bookEntry = bookEntry ?? BookEntry.Empty;

      Assertion.Require(!this.IsEmptyInstance, "Document can't be the empty instance.");

      Assertion.Require(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be added to closed documents");

      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(amendmentOf, nameof(amendmentOf));


      if (this.IsNew) {
        this.Save();
      }

      var recordingAct = RecordingAct.Create(recordingActType, this, resource, amendmentOf,
                                             this.RecordingActs.Count, bookEntry);
      recordingActList.Value.Add(recordingAct);

      return recordingAct;
    }

    internal void SetAuthorizationTime(DateTime authorizationTime) {
      Assertion.Require(this.IsNew || this.IsHistoricDocument,
         "AutorizationTime can be set only over new or historic documents.");

      this.AuthorizationTime = authorizationTime;
    }


    public void SetDates(DateTime presentationTime, DateTime authorizationTime) {
      Assertion.Require(this.IsHistoricDocument,
        "Autorization and Presentation dates can be set only over new or historic documents.");
      Assertion.Require(!this.IsClosed,
        "Autorization and Presentation dates can be set only over opened documents.");

      if (!this.HasTransaction) {
        this.PresentationTime = presentationTime;
      }

      this.AuthorizationTime = authorizationTime;
    }


    public void ChangeDocumentType(RecordingDocumentType newRecordingDocumentType) {
      if (this.DocumentType.Equals(newRecordingDocumentType)) {
        return;
      }
      base.ReclassifyAs(newRecordingDocumentType);
      this.IssueDate = ExecutionServer.DateMinValue;
      this.ExtensionData = RecordingDocumentExtData.Empty;
      this.PostedBy = ExecutionServer.CurrentContact;
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


    public RecordingAct GetRecordingAct(string recordingActUID) {
      RecordingAct recordingAct = this.RecordingActs.Find(x => x.UID == recordingActUID);

      Assertion.Require(recordingAct, "recordingAct");

      return recordingAct;
    }


    public List<Contact> GetRecordingOfficials() {
      var recordingOfficials = new List<Contact>();

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

    public bool HasTransaction {
      get {
        return !GetTransaction().Equals(LRSTransaction.Empty);
      }
    }

    public RecorderOffice RecorderOffice {
      get {
        if (HasTransaction) {
          return GetTransaction().RecorderOffice;
        } else {
          return RecorderOffice.Empty;
        }
      }
    }


    public Person AuthorizedBy {
      get {
        if (AuthorizationTime == ExecutionServer.DateMinValue) {
          return Person.Parse(-1);
        } else if (this.AuthorizationTime >= new DateTime(2022, 4, 1)) {
          return Person.Parse(401);  // Roberto López Arellano
        } else {
          return Person.Parse(4);   // Tere Alvarado
        }
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

    protected override void OnLoadObjectData(DataRow row) {
      RefreshRecordingActs();
      this.ExtensionData = RecordingDocumentExtData.Parse((string) row["DocumentExtData"]);
    }

    public void RefreshRecordingActs() {
      recordingActList = new Lazy<List<RecordingAct>>(() => RecordingActsData.GetDocumentRecordingActs(this));
    }

    protected override void OnBeforeSave() {
      if (this.IsNew) {
        IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

        _documentUID  = provider.GenerateRecordID();
      }
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = ExecutionServer.CurrentContact;
      }
      RecordingBooksData.WriteRecordingDocument(this);
    }

    public void RemoveRecordingAct(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, "recordingAct");

      Assertion.Require(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be removed from closed documents.");

      Assertion.Require(recordingAct.Document.Equals(this),
                        "The recording act doesn't belong to this document.");

      recordingAct.Delete();
      recordingActList.Value.Remove(recordingAct);

      if (this.RecordingActs.Count == 0 && this.IsEmptyDocumentType) {
        this.Delete();
      }
    }

    public BookEntry TryGetBookEntry() {
      if (!this.IsHistoricDocument) {
        return null;
      }
      BookEntry bookEntry = this.RecordingActs[0].BookEntry;

      Assertion.Require(!bookEntry.IsEmptyInstance,
                        "bookEntry can't be the empty instance.");

      return bookEntry;
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
