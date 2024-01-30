/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingAct                                   Pattern  : Partitioned type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Partitioned type that represents a recording act. All recording acts must be descendents      *
*              of this type.                                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Json;
using Empiria.Security;
using Empiria.Ontology;

using Empiria.Land.Data;
using Empiria.Land.Registration.Adapters;

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
      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(document, nameof(document));

      Assertion.Require(!document.IsEmptyInstance, "document can't be the empty instance.");

      this.Document = document;
    }


    protected RecordingAct(RecordingActType recordingActType, RecordingDocument document,
                           BookEntry bookEntry) : base(recordingActType) {
      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(document, nameof(document));
      Assertion.Require(bookEntry, nameof(bookEntry));

      Assertion.Require(!document.IsEmptyInstance, "document can't be the empty instance.");

      this.Document = document;
      this.BookEntry = bookEntry;
    }


    static internal RecordingAct Create(RecordingActType recordingActType,
                                        RecordingDocument document, Resource resource,
                                        RecordingAct amendmentOf, int index,
                                        BookEntry bookEntry) {
      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(document, nameof(document));
      Assertion.Require(resource, nameof(resource));
      Assertion.Require(amendmentOf, nameof(amendmentOf));
      Assertion.Require(bookEntry, nameof(bookEntry));

      Assertion.Require(!document.IsEmptyInstance, "document can't be the empty instance.");
      Assertion.Require(!document.IsNew, "document can't be a new instance.");
      Assertion.Require(!amendmentOf.IsNew, "amendmentOf can't be a new instance.");

      RecordingAct recordingAct = recordingActType.CreateInstance();
      recordingAct.BookEntry = bookEntry;
      recordingAct.Index = index;
      recordingAct.Document = document;
      recordingAct.AmendmentOf = amendmentOf;

      if (resource.IsNew) {
        resource.Save();
      }
      recordingAct.Resource = resource;

      recordingAct.Status = RecordableObjectStatus.Pending;

      if (bookEntry.IsNew) {
        bookEntry.Save();
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


    static public RecordingAct Parse(int id, bool reload) {
      return BaseObject.ParseId<RecordingAct>(id, reload);
    }


    static public RecordingAct Parse(string uid) {
      return BaseObject.ParseKey<RecordingAct>(uid);
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


    public bool IsCanceled {
      get {
        if (!this.AmendedBy.IsEmptyInstance && this.AmendedBy.RecordingActType.IsCancelationActType) {
          return true;
        }
        return false;
      }
    }

    [DataField("DocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }


    [DataField("PhysicalRecordingId")]
    private LazyInstance<BookEntry> _bookEntry = LazyInstance<BookEntry>.Empty;
    public BookEntry BookEntry {
      get {
        return _bookEntry.Value;
      }
      private set {
        _bookEntry = LazyInstance<BookEntry>.Parse(value);
      }
    }


    public bool HasBookEntry {
      get {
        return !this.BookEntry.IsEmptyInstance;
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

    public RecordingActParties Parties {
      get {
        return new RecordingActParties(this);
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


    [DataField("RecordingActKind")]
    public string Kind {
      get;
      private set;
    }


    [DataField("OperationAmount")]
    public decimal OperationAmount {
      get;
      private set;
    }


    [DataField("OperationCurrencyId")]
    public Currency OperationCurrency {
      get;
      private set;
    }


    [DataField("RecordingActSummary")]
    public string Summary {
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


    public bool IsParent {
      get {
        return ParentChidrenExtData.Get("isParent", false);
      }
    }


    public bool IsChild {
      get {
        return ParentId != -1;
      }
    }


    public int ParentId {
      get {
        return ParentChidrenExtData.Get("parentId", -1);
      }
    }


    [DataField("RecordingActExtData")]
    private JsonObject ParentChidrenExtData {
      get;
      set;
    } = new JsonObject();


    internal ResourceShapshotData ResourceShapshotData {
      get;
      private set;
    } = ResourceShapshotData.Empty;



    public bool ResourceWasUpdated {
      get {
        return (!this.ResourceShapshotData.IsEmptyInstance);
      }
    }


    internal protected virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.RecordingActType.DisplayName, this.Document.UID,
                                           !this.BookEntry.IsEmptyInstance ?
                                           this.BookEntry.AsText : String.Empty,
                                           this.Resource.Keywords);
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
        } else if (!this.RecordingActType.AppliesToARecordingAct) {
          return this.RecordingActType.DisplayName;
        } else if (this.RecordingActType.AppliesToARecordingAct) {
          return this.RecordingActType.RecordingRule.DynamicActNamePattern + " de " +
                 this.AmendmentOf.RecordingActType.DisplayName.ToLowerInvariant();
        } else {
          throw Assertion.EnsureNoReachThisCode();
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

    public bool IsAppliedOverNewPartition {
      get {
        return this.Validator.IsAppliedOverNewPartition();
      }
    }

    public bool IsCompleted {
      get {
        return this.Validator.IsCompleted();
      }
    }


    public bool IsEditable {
      get {
        return this.Validator.IsEditable();
      }
    }


    public bool IsHistoric {
      get {
        return !this.BookEntry.IsEmptyInstance;
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
          "AmendmentOf", this.AmendmentOf.Id, "AmendedBy", this.AmendedBy.Id, "BookEntry", this.BookEntry.Id,
          "RegisteredBy", this.RegisteredBy.Id, "RegistrationTime", this.RegistrationTime,
          "Status", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }


    private IntegrityValidator _integrityValidator = null;
    public IntegrityValidator Integrity {
      get {
        if (_integrityValidator == null) {
          _integrityValidator = new IntegrityValidator(this);
        }
        return _integrityValidator;
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


    internal RecordingActValidator Validator {
      get {
        return new RecordingActValidator(this);
      }
    }

    #endregion Public properties

    #region Methods

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


    public void ChangeRecordingActType(RecordingActType recordingActType) {
      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(this.RecordingActType.RecordingRule.ReplaceableBy.Contains(recordingActType),
          $"El acto jurídico {this.DisplayName} no puede ser reemplazado por {recordingActType.DisplayName}.");

      this.ReclassifyAs(recordingActType);

      RecordingActsData.UpdateRecordingActType(this);
    }


    internal void Delete() {
      this.Validator.AssertCanBeDeleted();

      if (this.IsEmptyInstance) {
        return;
      }

      this.Status = RecordableObjectStatus.Deleted;
      this.Save();

      if (this.IsAmendment && this.AmendmentOf.Document.IsEmptyDocumentType) {
        this.AmendmentOf.Delete();
      } else if (this.IsAmendment && !this.AmendmentOf.Document.IsEmptyDocumentType) {
        this.AmendmentOf.RemoveAmendment();
      }
      this.Resource.TryDelete();
    }


    public string GetRecordingAntecedentText() {
      var antecedentText = GetAntecedentOrTargetText();

      if (antecedentText.Length == 0) {
        return "Sin antecedente registral";
      } else {
        return antecedentText;
      }

      string GetAntecedentOrTargetText() {
        if (this.RecordingActType.IsAmendmentActType) {
          return GetAmendedText();
        }

        var antecedent = this.GetAntecedent();

        if (antecedent.IsEmptyInstance) {
          return String.Empty;

        } else if (!antecedent.BookEntry.IsEmptyInstance) {
          return antecedent.BookEntry.AsText;

        } else if (!antecedent.Document.Equals(this.Document)) {
          return antecedent.Document.UID;

        }

        var antecedent2 = antecedent.GetAntecedent();

        if (antecedent2.IsEmptyInstance) {
          return "En este mismo documento";

        } else if (!antecedent2.BookEntry.IsEmptyInstance) {
          return antecedent2.BookEntry.AsText;

        } else {
          return antecedent.Document.UID;
        }
      }  // GetAntecedentOrTarget()


      string GetAmendedText() {
        var amendedAct = this.AmendmentOf;

        if (amendedAct.IsEmptyInstance) {
          return this.GetAntecedent().Document.UID;

        } else if (amendedAct.BookEntry.IsEmptyInstance) {
          return amendedAct.RecordingActType.DisplayName +
                 (amendedAct.RecordingActType.FemaleGenre ?
                                              " registrada en " : " registrado en ") +
                 "Doc: " + amendedAct.Document.UID;

        } else {
          return amendedAct.RecordingActType.DisplayName +
                 (amendedAct.RecordingActType.FemaleGenre ?
                                              " registrada en " : " registrado en ") +
                 amendedAct.BookEntry.AsText;
        }
      }  // GetAmendedItemCell()

    }  // GetRecordingAntecedentText()


    /// <summary>Gets the resource data as it was when it was applied to this recording act.</summary>
    public ResourceShapshotData GetResourceSnapshotData() {
      if (this.ResourceWasUpdated) {
        return this.ResourceShapshotData;
      }

      var tract = this.Resource.Tract.GetRecordingActs();

      /// Look for the first recording act with ResourceExtData added before this act in the tract.
      /// If it is found then return it, if not then return the an empty resource snapshot.
      var lastActWithSnapshot = tract.FindLast(x => x.Document.PresentationTime < this.Document.PresentationTime &&
                                                    x.ResourceWasUpdated);
      if (lastActWithSnapshot != null) {
        return lastActWithSnapshot.ResourceShapshotData;
      } else {
        return ResourceShapshotData.ParseEmptyFor(this.Resource);
      }
    }


    public void OnResourceUpdated(Resource updatedResource) {
      Assertion.Require(updatedResource.Equals(this.Resource),
                       "Recording act resource and the updated resource are not the same.");

      this.ResourceShapshotData = updatedResource.GetSnapshotData();

      RecordingActsData.UpdateRecordingActResourceSnapshot(this);
    }


    protected override void OnLoadObjectData(DataRow row) {
      this.ResourceShapshotData = ResourceShapshotData.Parse(this.Resource, (string) row["RecordingActResourceExtData"]);
    }


    protected override void OnSave() {
      // Writes any change to the document and to the related physical recording
      this.Document.Save();
      if (this.BookEntry.IsNew) {
        this.BookEntry.Save();
      }
      if (this.Resource.IsNew) {
        this.Resource.Save();
      }
      // Writes the recording act
      if (base.IsNew) {
        this.RegistrationTime = DateTime.Now;
        this.RegisteredBy = ExecutionServer.CurrentContact;
      }
      RecordingActsData.WriteRecordingAct(this);
    }


    internal void SetBookEntry(BookEntry bookEntry) {
      Assertion.Require(bookEntry, nameof(bookEntry));

      this.BookEntry = bookEntry;

      this.Save();
    }


    protected void SetResource(Resource resource, ResourceRole role = ResourceRole.Informative,
                               Resource relatedResource = null, decimal percentage = 1m) {
      Assertion.Require(resource != null && !resource.IsEmptyInstance,
                       "Resource can't be null  or the empty instance.");
      Assertion.Require(decimal.Zero < percentage && percentage <= decimal.One,
                       "Percentage should be set between zero and one inclusive.");

      resource.AssertIsStillAlive(this.Document.PresentationTime);

      this.Resource = resource;
      this.ResourceRole = role;
      this.RelatedResource = relatedResource ?? Resource.Empty;
      this.Percentage = percentage;

      this.Index = this.Document.AppendRecordingAct(this);
    }


    public void Update(RecordingActFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.Summary = fields.Description;

      if (fields.Kind.Length != 0) {
        this.Kind = fields.Kind;
      }

      if (fields.OperationAmount != -1m &&
          fields.CurrencyUID.Length != 0) {
        this.OperationAmount = fields.OperationAmount;
        this.OperationCurrency = Currency.Parse(fields.CurrencyUID);
      }
    }


    #endregion Methods

    #region Helpers

    private RecordingAct GetAntecedent() {
      return this.Resource.Tract.GetRecordingAntecedent(this, true);
    }


    private void RemoveAmendment() {
      this.AmendmentOf = RecordingAct.Empty;
      this.Save();
    }

    #endregion Helpers

  } // class RecordingAct

} // namespace Empiria.Land.Registration
