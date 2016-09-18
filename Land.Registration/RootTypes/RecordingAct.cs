/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingAct                                   Pattern  : Partitioned type                    *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Partitioned type that represents a recording act. All recording acts must be descendents      *
*              of this type.                                                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;
using Empiria.Security;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Partitioned type that represents a recording act. All recording acts types must be
  /// descendents of this type.</summary>
  [PartitionedType(typeof(RecordingActType))]
  public abstract class RecordingAct : BaseObject, IResourceTractItem, IProtected {

    #region Constructors and parsers

    protected RecordingAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    protected RecordingAct(RecordingActType recordingActType,
                           RecordingDocument document) : base(recordingActType) {
      Assertion.AssertObject(recordingActType, "recordingActType");
      Assertion.AssertObject(document, "document");
      Assertion.Assert(!document.IsEmptyInstance, "document can't be the empty instance.");

      this.Document = document;
    }

    protected RecordingAct(RecordingActType recordingActType, RecordingDocument document,
                           Recording physicalRecording) : base(recordingActType) {
      Assertion.AssertObject(recordingActType, "recordingActType");
      Assertion.AssertObject(document, "document");
      Assertion.Assert(!document.IsEmptyInstance, "document can't be the empty instance.");

      Assertion.AssertObject(physicalRecording, "physicalRecording");
      Assertion.Assert(!physicalRecording.IsEmptyInstance,
                       "physicalRecording can't be the empty instance");

      this.PhysicalRecording = physicalRecording;
      this.Document = document;
    }

    static internal RecordingAct Create(RecordingActType recordingActType,
                                        RecordingDocument document, Resource resource,
                                        RecordingAct amendmentOf, int index,
                                        Recording physicalRecording) {
      Assertion.AssertObject(recordingActType, "recordingActType");
      Assertion.AssertObject(document, "document");
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(amendmentOf, "amendmentOf");
      Assertion.AssertObject(physicalRecording, "physicalRecording");

      Assertion.Assert(!document.IsEmptyInstance, "document can't be the empty instance.");
      Assertion.Assert(!document.IsNew, "document can't be a new instance.");
      Assertion.Assert(!amendmentOf.IsNew, "amendmentOf can't be a new instance.");

      RecordingAct recordingAct = recordingActType.CreateInstance();
      recordingAct.PhysicalRecording = physicalRecording;
      recordingAct.Index = index;
      recordingAct.Document = document;
      recordingAct.AmendmentOf = amendmentOf;

      if (resource.IsNew) {
        resource.Save();
      }
      recordingAct.Resource = resource;

      recordingAct.Status = RecordableObjectStatus.Pending;

      if (physicalRecording.IsNew) {
        physicalRecording.Save();
      }
      recordingAct.Save();

      if (!recordingAct.AmendmentOf.IsEmptyInstance) {
        recordingAct.AmendmentOf.AmendedBy = recordingAct;
        recordingAct.AmendmentOf.Save();
      }

      return recordingAct;
    }

    static public RecordingAct Parse(int id) {
      return BaseObject.ParseId<RecordingAct>(id);
    }

    static public FixedList<RecordingAct> GetList(RecordingDocument document) {
      return RecordingActsData.GetRecordingActs(document).ToFixedList();
    }

    static private readonly RecordingAct _empty = BaseObject.ParseEmpty<RecordingAct>();
    static public RecordingAct Empty {
      get {
        return _empty.Clone<RecordingAct>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingActType RecordingActType {
      get {
        return (RecordingActType) base.GetEmpiriaType();
      }
    }

    [DataField("DocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }

    [DataField("PhysicalRecordingId")]
    private LazyInstance<Recording> _physicalRecording = LazyInstance<Recording>.Empty;
    public Recording PhysicalRecording {
      get {
        return _physicalRecording.Value;
      }
      private set {
        _physicalRecording = LazyInstance<Recording>.Parse(value);
      }
    }

    [DataField("RecordingActIndex")]
    public int Index {
      get;
      internal set;
    }

    public string IndexedName {
      get {
        return "[" + (this.Index + 1).ToString("00") + "] " + this.DisplayName;
      }
    }

    [DataField("ResourceId", Default = "Empiria.Land.Registration.RealEstate.Empty")]
    public Resource Resource {
      get;
      private set;
    }

    [DataField("ResourceRole", Default = ResourceRole.Informative)]
    public ResourceRole ResourceRole {
      get;
      private set;
    }

    [DataField("RelatedResourceId", Default = "Empiria.Land.Registration.RealEstate.Empty")]
    public Resource RelatedResource {
      get;
      private set;
    }

    [DataField("RecordingActPercentage", Default = 1.0)]
    public decimal Percentage {
      get;
      private set;
    }


    [DataField("RecordingActNotes")]
    public string Notes {
      get;
      set;
    }

    public RecordingActExtData ExtensionData {
      get;
      private set;
    } = RecordingActExtData.Empty;


    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.RecordingActType.DisplayName, this.Document.UID,
                                           this.Resource.UID, this.RelatedResource.UID,
                                           !this.PhysicalRecording.IsEmptyInstance ?
                                           this.PhysicalRecording.FullNumber : String.Empty);
      }
    }

    [DataField("AmendmentOfId")]
    private LazyInstance<RecordingAct> _amendmentOf = LazyInstance<RecordingAct>.Empty;
    public RecordingAct AmendmentOf {
      get {
        return _amendmentOf.Value;
      }
      private set {
        _amendmentOf = LazyInstance<RecordingAct>.Parse(value);
      }
    }

    [DataField("AmendedById")]
    private LazyInstance<RecordingAct> _amendedBy = LazyInstance<RecordingAct>.Empty;
    public RecordingAct AmendedBy {
      get {
        return _amendedBy.Value;
      }
      private set {
        _amendedBy = LazyInstance<RecordingAct>.Parse(value);
      }
    }

    [DataField("RegisteredById")]
    public Contact RegisteredBy {
      get;
      private set;
    }

    [DataField("RegistrationTime", Default = "DateTime.Now")]
    public DateTime RegistrationTime {
      get;
      private set;
    }

    [DataField("RecordingActStatus", Default = RecordableObjectStatus.Incomplete)]
    public RecordableObjectStatus Status {
      get;
      private set;
    }

    public string DisplayName {
      get {
        if (!this.RecordingActType.RecordingRule.UseDynamicActNaming) {
          return this.RecordingActType.DisplayName;
        } else if (this.RecordingActType.AppliesTo != RecordingRuleApplication.RecordingAct) {
          return this.RecordingActType.DisplayName;
        } else if (this.RecordingActType.AppliesTo == RecordingRuleApplication.RecordingAct) {
          return this.RecordingActType.RecordingRule.DynamicActNamePattern + " de " +
                 this.AmendmentOf.RecordingActType.DisplayName.ToLowerInvariant();
        } else {
          throw Assertion.AssertNoReachThisCode();
        }
      }
    }

    public bool IsAmendment {
      get {
        return !this.AmendmentOf.IsEmptyInstance;
      }
    }

    public bool IsAnnotation {
      get {
        return this.RecordingActType.IsInformationActType;
      }
    }

    public bool IsCompleted {
      get {
        return (this.Status == RecordableObjectStatus.Registered ||
                this.Status == RecordableObjectStatus.Closed ||
                this.HasCompleteInformation());
      }
    }

    public bool IsHistoric {
      get {
        return !this.PhysicalRecording.IsEmptyInstance;
      }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.Incomplete:
            return "Incompleto";
          case RecordableObjectStatus.Pending:
            return "Pendiente";
          case RecordableObjectStatus.Registered:
            return "Registrado";
          case RecordableObjectStatus.Closed:
            return "Cerrado";
          case RecordableObjectStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
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
          1, "Id", this.Id, "RecordingActType", this.RecordingActType.Id,
          "Document", this.Document.Id, "Index", this.Index, "Notes", this.Notes,
          "ExtensionData", this.ExtensionData.ToString(), "AmendmentOf", this.AmendmentOf.Id,
          "AmendedBy", this.AmendedBy.Id, "Recording", this.PhysicalRecording.Id,
          "RegisteredBy", this.RegisteredBy.Id, "RegistrationTime", this.RegistrationTime,
          "Status", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private bool HasCompleteInformation() {
      return false;
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

    string IResourceTractItem.TractPrelationStamp {
      get {
        return this.Document.PresentationTime.ToString("yyyyMMddTHH:mm@") +
               this.Document.AuthorizationTime.ToString("yyyyMMddTHH:mm@") +
               this.RegistrationTime.ToString("yyyyMMddTHH:mm") +
               this.Id.ToString("000000000000");
      }
    }

    #endregion Public properties

    #region Public methods

    internal void Amend(CancelationAct cancelationAct) {
      cancelationAct.AmendmentOf = this;
      this.AmendedBy = cancelationAct;
      cancelationAct.Save();
      this.Save();
    }

    internal void Amend(ModificationAct modificationAct) {
      modificationAct.AmendmentOf = this;
      this.AmendedBy = modificationAct;
      modificationAct.Save();
      this.Save();
    }

    public RecordingActParty AppendParty(Party party, SecondaryPartyRole role, Party partyOf) {
      var recordingActParty = RecordingActParty.Create(this, party, role, partyOf);

      return recordingActParty;
    }

    public RecordingActParty AppendParty(Party party, DomainActPartyRole role) {
      var recordingActParty = RecordingActParty.Create(this, party, role);

      return recordingActParty;
    }

    public void AssertCanBeClosed() {
      var rule = this.RecordingActType.RecordingRule;
      if (!this.Resource.IsEmptyInstance) {
        this.RecordingActType.AssertIsApplicableResource(this.Resource);
        this.Resource.AssertCanBeClosed();
      } else {
        Assertion.Assert(rule.AppliesTo == RecordingRuleApplication.NoProperty,
                         "El acto jurídico  " + this.IndexedName + " debe aplicar a un predio o asociación.");
      }
      if (!this.PhysicalRecording.IsEmptyInstance) {
        this.PhysicalRecording.AssertCanBeClosed();
      }
      this.ExtensionData.AssertIsComplete(this);

      this.AssertParties();

      this.AssertIsLastInPrelationOrder();
    }

    internal void AssertCanBeOpened() {
      this.AssertIsLastInPrelationOrder();
    }

    public void AssertIsLastInPrelationOrder() {
      var fullTract = this.Resource.GetFullRecordingActsTract();

      var wrongPrelation = fullTract.Contains((x) => !x.Document.Equals(this.Document) &&
                                              x.Document.PresentationTime > this.Document.PresentationTime &&
                                              x.Document.IsClosed);

      if (wrongPrelation) {
        Assertion.AssertFail("El acto jurídico " + this.IndexedName +
                             " hace referencia a un predio o sociedad que tiene registrado " +
                             "cuando menos otro acto jurídico con una prelación posterior " +
                             "a la de este documento.\n\n" +
                             "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                             "Favor de revisar la historia del predio involucrado, quizás lo que " +
                             "procede es iniciar un nuevo trámite de aclaración.");
      }

      var certificates = this.Resource.GetEmittedCerificates();

      wrongPrelation = certificates.Contains((x) => x.Transaction.PresentationTime > this.Document.PresentationTime);

      if (wrongPrelation) {
        Assertion.AssertFail("El acto jurídico " + this.IndexedName +
                             " hace referencia a un predio o sociedad para el cual se ha expedido " +
                             "cuando menos un certificado con una prelación posterior a la de este documento.\n\n" +
                             "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                             "Favor de revisar la historia del predio involucrado, quizás lo que " +
                             "procede es iniciar un nuevo trámite de aclaración.");
      }
    }

    public void ChangeStatusTo(RecordableObjectStatus newStatus) {
      this.Status = newStatus;
      this.Save();
    }

    internal void Delete() {
      if (this.PhysicalRecording.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(
                      LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(
                      LandRegistrationException.Msg.CantAlterClosedRecordingAct, this.Id);
      }
      if (this.IsEmptyInstance) {
        return;
      }

      this.Status = RecordableObjectStatus.Deleted;
      this.Save();

      if (this.IsAmendment && this.AmendmentOf.Document.IsEmptyDocument) {
        this.AmendmentOf.Delete();
      } else if (this.IsAmendment && !this.AmendmentOf.Document.IsEmptyDocument) {
        this.AmendmentOf.RemoveAmendment();
      }
      this.Resource.TryDelete();
    }

    public FixedList<RecordingActParty> GetParties() {
      return PartyData.GetRecordingPartyList(this);
    }

    public RecordingAct GetRecordingAntecedent() {
      return this.Resource.GetRecordingAntecedent(this, true);
    }

    protected void SetResource(Resource resource, ResourceRole role = ResourceRole.Informative,
                               Resource relatedResource = null, decimal percentage = 1m) {
      Assertion.Assert(resource != null  && !resource.IsEmptyInstance,
                       "Resource can't be null  or the empty instance.");
      Assertion.Assert(decimal.Zero < percentage && percentage <= decimal.One,
                       "Percentage should be set between zero and one inclusive.");

      resource.AssertIsStillAlive();

      this.Resource = resource;
      this.ResourceRole = role;
      this.RelatedResource = relatedResource ?? RealEstate.Empty;
      this.Percentage = percentage;

      this.Index = this.Document.AddRecordingAct(this);
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingActExtData.Parse((string) row["RecordingActExtData"]);
    }

    protected override void OnSave() {
      // Writes any change to the document and to the related physical recording
      this.Document.Save();
      if (this.PhysicalRecording.IsNew) {
        this.PhysicalRecording.Save();
      }
      if (this.Resource.IsNew) {
        this.Resource.Save();
      }
      // Writes the recording act
      if (base.IsNew) {
        this.RegistrationTime = DateTime.Now;
        this.RegisteredBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingActsData.WriteRecordingAct(this);
    }

    #endregion Public methods

    #region Private methods

    private void AssertParties() {
      var rule = this.RecordingActType.RecordingRule;
      var parties = this.GetParties();
      var roles = this.RecordingActType.GetRoles();

      if (roles.Count == 0 || rule.AllowNoParties || roles.Count == 0) {
        return;
      }
      Assertion.Assert(parties.Count != 0, "El acto jurídico  " + this.IndexedName +
                                           " requiere cuando menos una persona o propietario.");
      foreach (var role in roles) {
        var found = parties.Contains((x) => x.PartyRole == role);
        if (found) {
          return;
        }
      }
      Assertion.AssertFail("En el acto jurídico  " + this.IndexedName +
                           " no hay registradas personas o propietarios jugando alguno de" +
                           " los roles obligatorios para dicho tipo de acto.");
    }

    private void RemoveAmendment() {
      this.AmendmentOf = RecordingAct.Empty;
      this.Save();
    }

    #endregion Private methods
  } // class RecordingAct

} // namespace Empiria.Land.Registration
