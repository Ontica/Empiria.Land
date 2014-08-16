﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingAct                                   Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act. All recording acts must be descendents        *
*              of this type.                                                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;
using Empiria.Security;

namespace Empiria.Land.Registration {

  /// <summary>Abstract class that represents a recording act. All recording acts types must be 
  /// descendents of this type.</summary>
  public abstract class RecordingAct : BaseObject, IExtensible<RecordingActExtData>, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct";
    
    private Lazy<List<TractIndexItem>> attachedResources = null;

    #endregion Fields

    #region Constructors and parsers

    protected RecordingAct(string typeName) : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete.
      Initialize();
    }

    private void Initialize() {
      attachedResources = new Lazy<List<TractIndexItem>>(() => RecordingActsData.GetTractIndex(this));
    }

    static internal RecordingAct Create(RecordingActType recordingActType, 
                                        Recording recording, Property resource) {
      RecordingAct recordingAct = recordingActType.CreateInstance();
      recordingAct.Recording = recording;
      recordingAct.Index = recording.RecordingActs.Count + 1;

      if (recordingActType.Autoregister) {
        recordingAct.Status = RecordableObjectStatus.Registered;
      } else {
        recordingAct.Status = RecordableObjectStatus.Pending;
      }
      recordingAct.AttachResource(resource);
      recordingAct.Save();

      return recordingAct;
    }

    static public RecordingAct Parse(int id) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, id);
    }

    static internal RecordingAct Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, dataRow);
    }

    static internal RecordingAct Parse(RecordingTask task) {
      Assertion.AssertObject(task, "task");

      RecordingAct recordingAct = task.RecordingActType.CreateInstance();
      recordingAct.Document = task.Document;
      recordingAct.AmendmentOf = task.TargetRecordingAct;

      if (!task.TargetProperty.IsEmptyInstance) {
        recordingAct.AttachResource(task.TargetProperty);
      }
      return recordingAct;
    }

    static public FixedList<RecordingAct> GetList(RecordingDocument document) {
      return RecordingActsData.GetRecordingActs(document);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("RecordingActTypeId")]
    public RecordingActType RecordingActType {
      get;
      internal set;
    }

    [DataField("DocumentId")]
    private LazyObject<RecordingDocument> _document = LazyObject<RecordingDocument>.Empty;
    public RecordingDocument Document {
      get { return _document.Instance; }
      private set { _document.Instance = value; }
    }

    [DataField("RecordingId")]
    private LazyObject<Recording> _recording = LazyObject<Recording>.Empty;
    public Recording Recording {
      get { return _recording.Instance; }
      private set { _recording.Instance = value; }
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
        return EmpiriaString.BuildKeywords(this.RecordingActType.DisplayName, 
                                           this.Recording.FullNumber);
      }
    }

    [DataField("AmendmentOfId")]
    private LazyObject<RecordingAct> _amendmentOf = LazyObject<RecordingAct>.Empty;
    public RecordingAct AmendmentOf {
      get { return _amendmentOf.Instance; }
      private set { _amendmentOf.Instance = value; }
    }

    [DataField("AmendedById")]
    private LazyObject<RecordingAct> _amendedBy = LazyObject<RecordingAct>.Empty;
    public RecordingAct AmendedBy {
      get { return _amendedBy.Instance; }
      private set { _amendedBy.Instance = value; }
    }

    [DataField("RegisteredById", Default = "Contacts.Person.Empty")]
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

    public bool IsAnnotation {
      get {
        return this.RecordingActType.IsAnnotationType;
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

    public FixedList<TractIndexItem> TractIndex {
      get {
        return new FixedList<TractIndexItem>(attachedResources.Value);
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
          "AmendedBy", this.AmendedBy.Id, "Recording", this.Recording.Id, 
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

    public TractIndexItem AttachResource(Property resource) {
      Assertion.AssertObject(resource, "resource");

      var item = new TractIndexItem(resource, this);
     
      attachedResources.Value.Add(item);

      return item;
    }

    public void ChangeStatusTo(RecordableObjectStatus newStatus) {
      this.Status = newStatus;
      this.Save();
    }

    internal void Delete() {
      if (this.Recording.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordableObjectStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, this.Id);
      }
      var properties = this.TractIndex;
      for (int i = 0; i < properties.Count; i++) {
        var propertyEvent = properties[i];
        var property = TractIndex[i].Property;
        propertyEvent.Delete();

        var tract = property.GetRecordingActsTract();
        if (tract.Count == 0) {
          property.Status = RecordableObjectStatus.Deleted;
          property.Save();
        }
      }
      this.Status = RecordableObjectStatus.Deleted;
      this.Save();
    }

    public IList<Property> GetProperties() {
      var tract = attachedResources.Value;
      var list = new List<Property>(tract.Count);
      foreach (var item in tract) {
        if (!list.Contains(item.Property)) {
          list.Add(item.Property);
        }
      }
      return list;
    }

    public TractIndexItem GetPropertyEvent(Property property) {
      var propertyEvent = this.TractIndex.Find((x) => x.Property.Equals(property));
      if (propertyEvent != null) {
        return propertyEvent;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
                                            property.UniqueCode, this.Id);
      }
    }
    
    public bool IsFirstRecordingAct() {
      if (this.TractIndex.Count == 0) {
        return false;
      }
      Property property = this.TractIndex[0].Property;

      return property.IsFirstRecordingAct(this);
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingActExtData.Parse((string) row["RecordingActExtData"]);
    }

    protected override void OnSave() {
      if (base.IsNew) {
        this.RegistrationTime = DateTime.Now;
        this.RegisteredBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      foreach (TractIndexItem tractItem in this.TractIndex) {
        tractItem.Save();
      }
      RecordingActsData.WriteRecordingAct(this);
    }

    public void RemoveProperty(Property property) {
      TractIndexItem propertyEvent = this.TractIndex.Find((x) => x.Property.Equals(property));

      Assertion.AssertObject(propertyEvent, 
                new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
                                              property.Id, this.Id));

      propertyEvent.Delete();
      if (property.GetRecordingActsTract().Count == 0) {
        property.Status = RecordableObjectStatus.Deleted;
        property.Save();
      }

      //_propertyList.Reset();
      if (this.TractIndex.Count == 0) {
        this.Status = RecordableObjectStatus.Deleted;
        this.Save();
      }

      Assertion.Assert(property.Status == RecordableObjectStatus.Deleted &&
                       this.Status == RecordableObjectStatus.Deleted, "fail");
    }

    internal RecordingAct WriteOn(RecorderOffice recorderOffice, RecordingSection recordingSection) {
      Assertion.AssertObject(recorderOffice, "recorderOffice");
      Assertion.AssertObject(recordingSection, "recordingSection");

      RecordingBook book = this.GetOpenedRecordingBook(recorderOffice, recordingSection);
      //this.Recording = book.CreateRecording(this.Document.Transaction);
      //OOJJOO
      this.Save();

      return this;
    }

    //internal RecordingAct WriteOn(RecorderOffice recorderOffice, RecordingSection recordingSection) {
    //  if (base.IsNew) {
    //    this.PostingTime = DateTime.Now;
    //    this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    //  }
    //  //RecordingBook book = this.GetRecordingBook(); 
    //  //Recording recording = book.CreateRecording();

    //  //return recording.CreateRecordingAct(Task.RecordingActType, Property.Empty);
    //  RecordingBook book = this.GetRecordingBook(RecorderOffice recorderOffice, RecordingSection recordingSection);
    //  Recording recording = book.CreateRecording();

    //  return recording.CreateRecordingAct(Task.RecordingActType, Property.Empty);

    //  RecordingBook 
    //  this.Recording = 
    //  RecordingActsData.WriteRecordingAct(this);

    //  return this;
    //}

    #endregion Public methods

    #region Private methods

    private RecordingBook GetOpenedRecordingBook(RecorderOffice recorderOffice, 
                                                 RecordingSection recordingSection) {
      return RecordingBook.GetAssignedBookForRecording(recorderOffice, recordingSection, this.Document);
    }

    #endregion Private methods

  } // class RecordingAct

} // namespace Empiria.Land.Registration
