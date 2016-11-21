/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocument                              Pattern  : Partitioned type                    *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Partitioned type that represents a document that is attached to recordings.                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Documents;
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
      if (powerType.Equals(RecordingDocumentType.Empty)) {
        this.Status = RecordableObjectStatus.Closed;
      }
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

    public Recording TryGetHistoricRecording() {
      if (!this.IsHistoricDocument) {
        return null;
      }
      Recording historicRecording = this.RecordingActs[0].PhysicalRecording;

      Assertion.Assert(!historicRecording.IsEmptyInstance,
                      "historicRecording can't be the empty instance.");

      return historicRecording;
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

    public bool IsClosed {
      get {
        return (this.Status == RecordableObjectStatus.Closed);
      }
    }

    public bool IsEmptyDocument {
      get {
        return (this.DocumentType == RecordingDocumentType.Empty);
      }
    }

    public bool IsReadyForEdition() {
      if (this.IsEmptyInstance) {
        return false;
      }
      if (this.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this);
    }


    public bool IsReadyToClose() {
      if (this.IsEmptyInstance) {
        return false;
      }
      if (this.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this);
    }

    public bool IsReadyToOpen() {
      if (this.IsEmptyInstance) {
        return false;
      }
      if (this.Status != RecordableObjectStatus.Closed) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this);
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
                                           Recording physicalRecording = null) {
      amendmentOf = (amendmentOf != null) ? amendmentOf : RecordingAct.Empty;
      physicalRecording = (physicalRecording != null) ? physicalRecording : Recording.Empty;

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


    public void AssertCanBeClosed() {
      if (!this.IsReadyToClose()) {
        Assertion.AssertFail("El usuario no tiene permisos para cerrar el documento o éste no tiene un estado válido.");
      }

      //this.AssertGraceDaysForEdition();

      Assertion.Assert(this.RecordingActs.Count > 0, "El documento no tiene actos jurídicos.");

      foreach (var recordingAct in this.RecordingActs) {
        recordingAct.AssertCanBeClosed();
      }
    }


    public void AssertCanBeOpened() {
      if (!this.IsReadyToOpen()) {
        Assertion.AssertFail("El usuario no tiene permisos para abrir este documento.");
      }

      //this.AssertGraceDaysForEdition();

      foreach (var recordingAct in this.RecordingActs) {
        recordingAct.AssertCanBeOpened();
      }
    }

    private void AssertGraceDaysForEdition() {
      var transaction = this.GetTransaction();

      if (transaction.IsEmptyInstance) {
        return;
      }

      const int graceDaysForEdition = 45;
      DateTime lastDate = transaction.PresentationTime;
      if (transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
        lastDate = transaction.LastReentryTime;
      }
      if (lastDate.AddDays(graceDaysForEdition) < DateTime.Today) {
        Assertion.AssertFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             "no es posible modificar documentos de trámites de más de 45 días.\n\n" +
                             "En su lugar se puede optar por registrar un nuevo trámite, " +
                             "o quizás se pueda hacer un reingreso si no han transcurrido los " +
                             "90 días de gracia.");
      }
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

    public void Close() {
      this.AssertCanBeClosed();
      this.SetAuthorizationTime();
      this.Status = RecordableObjectStatus.Closed;
      this.Save();
    }

    private void SetAuthorizationTime() {
      if (!this.IsHistoricDocument) {
        this.AuthorizationTime = DateTime.Now;
      }
    }

    public void Open() {
      this.AssertCanBeOpened();
      this.Status = RecordableObjectStatus.Incomplete;
      this.Save();
    }

    public void GenerateImagingControlID() {
      Assertion.Assert(!this.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Assert(this.RecordingActs.Count > 0, "Document should have recording acts.");
      Assertion.Assert(this.RecordingActs.CountAll((x) => !x.PhysicalRecording.IsEmptyInstance) == 0,
                       "Document can't have any recording acts that are related to physical recordings.");
      Assertion.Assert(this.ImagingControlID.Length == 0,
                       "Document already has assigned an imaging control number.");

      this.ImagingControlID = DocumentsData.GetNextImagingControlID(this);
      DocumentsData.SaveImagingControlID(this);
    }

    public string GetDigitalSeal() {
      var transaction = this.GetTransaction();

      string s = "||" + transaction.UID + "|" + this.UID;
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        s += "|" + this.RecordingActs[i].Id.ToString();
      }
      s += "||";

      return Cryptographer.CreateDigitalSign(s);
    }

    public string GetDigitalSignature() {
      var transaction = this.GetTransaction();

      string s = "||" + transaction.UID + "|" + this.UID;
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        s += "|" + this.RecordingActs[i].Id.ToString();
      }
      return Cryptographer.CreateDigitalSign(s + "eSign");
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
      if (this.IsEmptyInstance || this.IsEmptyDocument) {
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

    public void RemoveRecordingAct(RecordingAct recordingAct) {
      Assertion.AssertObject(recordingAct, "recordingAct");

      Assertion.Assert(this.IsHistoricDocument || !this.IsClosed,
                       "Recording acts can't be removed from closed documents.");

      Assertion.Assert(recordingAct.Document.Equals(this),
                       "The recording act doesn't belong to this document.");

      recordingAct.Delete();
      recordingActList.Value.Remove(recordingAct);

      if (this.RecordingActs.Count == 0 && this.IsEmptyDocument) {
        this.Delete();
      }
    }

    public void SetAuxiliarImageSet(ImagingItem image) {
      Assertion.AssertObject(image, "image");

      this.ExtensionData.AuxiliarImageSetId = image.Id;
      this.Save();
    }

    public void SetImageSet(ImagingItem image) {
      Assertion.AssertObject(image, "image");

      this.ExtensionData.DocumentImageSetId = image.Id;
      this.Save();
    }

    public bool HasAuxiliarImageSet {
      get {
        return (this.ExtensionData.AuxiliarImageSetId != -1);
      }
    }

    public bool HasImageSet {
      get {
        return (this.ExtensionData.DocumentImageSetId != -1);
      }
    }

    public int ImageSetId {
      get {
        return this.ExtensionData.DocumentImageSetId;
      }
    }

    public int AuxiliarImageSetId {
      get {
        return this.ExtensionData.AuxiliarImageSetId;
      }
    }

    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return Cryptographer.CreateHashCode(this.Id.ToString("00000000") +
                                            this.AuthorizationTime.Ticks.ToString(),
                                            this.UID)
                                            .Substring(0, 8)
                                            .ToUpperInvariant();
      } else {
        return String.Empty;
      }
    }

    public ImagingItem TryGetAuxiliarImageSet() {
      if (this.ExtensionData.AuxiliarImageSetId != -1) {
        return ImagingItem.Parse(this.ExtensionData.AuxiliarImageSetId);
      } else {
        return null;
      }
    }

    public ImagingItem TryGetImageSet() {
      if (this.ExtensionData.DocumentImageSetId != -1) {
        return ImagingItem.Parse(this.ExtensionData.DocumentImageSetId);
      } else {
        return null;
      }
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
