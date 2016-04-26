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
  public abstract class RecordingAct : BaseObject, IExtensible<RecordingActExtData>, IProtected {

    #region Fields

    private Lazy<List<TractItem>> tractIndex = null;

    #endregion Fields

    #region Constructors and parsers

    protected RecordingAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    protected RecordingAct(RecordingActType recordingActType,
                           RecordingDocument document) : base(recordingActType) {
      Assertion.AssertObject(recordingActType, "recordingActType");

      Assertion.AssertObject(document, "document");

      Assertion.Assert(!document.IsEmptyInstance,
                       "document can't be the empty instance.");

      this.Document = document;
      this.Index = this.Document.AddRecordingAct(this);
    }

    protected RecordingAct(RecordingActType recordingActType, RecordingDocument document,
                           Recording physicalRecording) : this(recordingActType, document) {
      Assertion.AssertObject(physicalRecording, "physicalRecording");

      Assertion.Assert(!physicalRecording.IsEmptyInstance,
                       "physicalRecording can't be the empty instance");

      this.PhysicalRecording = physicalRecording;
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

      if (recordingActType.Autoregister) {
        recordingAct.Status = RecordableObjectStatus.Registered;
      } else {
        recordingAct.Status = RecordableObjectStatus.Pending;
      }
      if (physicalRecording.IsNew) {
        physicalRecording.Save();
      }
      recordingAct.Save();
      if (resource.IsNew) {
        resource.Save();
      }
      var resourceTarget = new TractItem(recordingAct, resource);
      recordingAct.AddTractItem(resourceTarget);

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

    [DataField("RecordingActNotes")]
    public string Notes {
      get;
      set;
    }

    public RecordingActExtData ExtensionData {
      get;
      private set;
    }

    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.RecordingActType.DisplayName , this.Document.UID,
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
        } else {
          return this.RecordingActType.RecordingRule.DynamicActNamePattern + " de " +
                 this.AmendmentOf.RecordingActType.DisplayName.ToLowerInvariant();
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

    public FixedList<TractItem> TractIndex {
      get {
        return tractIndex.Value.ToFixedList();
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
          "ExtensionData", this.ExtensionData.ToJson(), "AmendmentOf", this.AmendmentOf.Id,
          "AmendedBy", this.AmendedBy.Id, "Recording", this.PhysicalRecording.Id,
          "RegisteredBy", this.RegisteredBy.Id, "RegistrationTime", this.RegistrationTime,
          "Status", (char) this.Status
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

    protected void AddTractItem(TractItem tractItem) {
      Assertion.Assert(!this.IsEmptyInstance, "Recording act can't be the empty instance.");
      Assertion.AssertObject(tractItem, "tractItem");
      Assertion.Assert(tractItem.RecordingAct.Equals(this),
                       "TractItem recording act is different to this instance.");

      tractIndex.Value.Add(tractItem);
    }

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

    public void ChangeStatusTo(RecordableObjectStatus newStatus) {
      this.Status = newStatus;
      this.Save();
    }

    internal void Delete() {
      if (this.PhysicalRecording.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, this.Id);
      }
      if (this.IsEmptyInstance) {
        return;
      }
      var tractIndex = this.TractIndex;
      for (int i = 0; i < tractIndex.Count; i++) {
        tractIndex[i].Delete();
      }

      this.Status = RecordableObjectStatus.Deleted;
      this.Save();

      if (this.IsAmendment && this.AmendmentOf.Document.IsEmptyDocument) {
        this.AmendmentOf.Delete();
      } else if (this.IsAmendment && !this.AmendmentOf.Document.IsEmptyDocument) {
        this.AmendmentOf.RemoveAmendment();
      }
    }

    private void RemoveAmendment() {
      this.AmendmentOf = RecordingAct.Empty;
      this.Save();
    }

    protected override void OnInitialize() {
      this.ExtensionData = RecordingActExtData.Empty;
      this.tractIndex = new Lazy<List<TractItem>>(() => RecordingActsData.GetRecordingActTargets(this));
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingActExtData.Parse((string) row["RecordingActExtData"]);
    }

    protected override void OnSave() {
      // writes any chang to the document and the related physical recording
      this.Document.Save();
      if (this.PhysicalRecording.IsNew) {
        this.PhysicalRecording.Save();
      }
      // writes the recording act
      if (base.IsNew) {
        this.RegistrationTime = DateTime.Now;
        this.RegisteredBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingActsData.WriteRecordingAct(this);

      // writes each recording at target
      foreach (TractItem target in this.TractIndex) {
        target.Save();
      }
    }

    #endregion Public methods

    #region Resource methods to refactor

    //public IList<Property> GetProperties() {
    //  var tract = targets.Value;
    //  var list = new List<Property>(tract.Count);
    //  foreach (var item in tract) {
    //    if (!list.Contains((Property) item.Resource)) {
    //      list.Add((Property) item.Resource);
    //    }
    //  }
    //  return list;
    //}

    //public RecordingActTarget GetPropertyEvent(Property property) {
    //  var propertyEvent = this.Targets.Find((x) => x.Resource.Equals(property));
    //  if (propertyEvent != null) {
    //    return propertyEvent;
    //  } else {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
    //                                        property.UID, this.Id);
    //  }
    //}

    //public bool IsFirstRecordingAct() {
    //  if (this.Targets.Count == 0) {
    //    return false;
    //  }
    //  Resource resource = this.Targets[0].Resource;

    //  return resource.IsFirstRecordingAct(this);
    //}

    //public void RemoveProperty(Property property) {
    //  RecordingActTarget propertyEvent = this.Targets.Find((x) => x.Resource.Equals(property));

    //  Assertion.AssertObject(propertyEvent,
    //            new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
    //                                          property.Id, this.Id));

    //  propertyEvent.Delete();
    //  if (property.GetRecordingActsTract().Count == 0) {
    //    property.Status = RecordableObjectStatus.Deleted;
    //    property.Save();
    //  }

    //  //_propertyList.Reset();
    //  if (this.Targets.Count == 0) {
    //    this.Status = RecordableObjectStatus.Deleted;
    //    this.Save();
    //  }

    //  Assertion.Assert(property.Status == RecordableObjectStatus.Deleted &&
    //                   this.Status == RecordableObjectStatus.Deleted, "fail");
    //}

    #endregion

  } // class RecordingAct

} // namespace Empiria.Land.Registration



//static internal RecordingAct Create(RecordingActType recordingActType,
//                                RecordingDocument document, int index) {
//  Assertion.AssertObject(recordingActType, "recordingActType");
//  Assertion.AssertObject(document, "document");

//  RecordingAct recordingAct = recordingActType.CreateInstance();
//  recordingAct.Document = document;
//  recordingAct.Index = index;
//  if (recordingActType.Autoregister) {
//    recordingAct.Status = RecordableObjectStatus.Registered;
//  } else {
//    recordingAct.Status = RecordableObjectStatus.Pending;
//  }
//  recordingAct.Save();

//  return recordingAct;
//}
