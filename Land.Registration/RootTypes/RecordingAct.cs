/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingAct                                   Pattern  : Empiria Object Type                 *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act. All recording acts must be descendents        *
*              of this type.                                                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;
using Empiria.Security;

namespace Empiria.Land.Registration {

  public enum RecordingActStatus {
    Obsolete = 'S',
    Incomplete = 'I',
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Abstract class that represents a recording act. All recording acts types must be 
  /// descendents of this type.</summary>
  public abstract class RecordingAct : BaseObject, IExtensible<RecordingActExtData>, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct";

    private RecordingActType recordingActType = null;  
    private ObjectList<PropertyEvent> propertiesEvents = null;

    #endregion Fields

    #region Constructors and parsers

    protected RecordingAct(string typeName) : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
      Initialize();
    }

    static internal RecordingAct Parse(RecordingTask task) {
      Assertion.AssertObject(task, "task");

      RecordingAct recordingAct = task.RecordingActType.CreateInstance();

      recordingAct.recordingActType = task.RecordingActType; // OOJJOO: Avoid type assignment outside CreateInstance above
      recordingAct.Transaction = task.Transaction;
      recordingAct.Document = task.Document;
      recordingAct.TargetRecordingAct = task.TargetRecordingAct;
      recordingAct.TargetResource = task.TargetResource;

      return recordingAct;
    }

    static internal RecordingAct Create(RecordingActType recordingActType, 
                                        Recording recording, Property resource) {
      RecordingAct recordingAct = recordingActType.CreateInstance();
      recordingAct.recordingActType = recordingActType;
      recordingAct.Recording = recording; 
      recordingAct.propertiesEvents = new ObjectList<PropertyEvent>();
      recordingAct.Index = recording.RecordingActs.Count + 1;
      if (recordingActType.Autoregister) {
        recordingAct.Status = RecordingActStatus.Registered;
      } else {
        recordingAct.Status = RecordingActStatus.Pending;
      }
      recordingAct.Save();
      recordingAct.AppendPropertyEvent(resource);

      return recordingAct;
    }

    static public RecordingAct Parse(int id) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, id);
    }

    static internal RecordingAct Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, dataRow);
    }

    static public ObjectList<RecordingAct> GetList(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new ObjectList<RecordingAct>();
      }
      return RecordingActsData.GetRecordingActs(transaction);
    }

    private void Initialize() {
      this.Transaction = LRSTransaction.Empty;
      this.Document = RecordingDocument.Empty;
      this.Recording = Recording.Empty;
      this.ExtensionData = RecordingActExtData.Empty;
      this.Keywords = String.Empty;
      this.Index = 0;
      this.Notes = String.Empty;
      this.CanceledBy = Person.Empty;
      this.CancelationTime = ExecutionServer.DateMaxValue;
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.PostingTime = DateTime.Now;
      this.Status = RecordingActStatus.Incomplete;
    }

    #endregion Constructors and parsers

    #region Public properties


    public LRSTransaction Transaction {
      get; 
      private set;
    }

    public RecordingDocument Document {
      get; 
      private set;
    }

    
    private RecordingAct _targetRecordingAct = null;
    public RecordingAct TargetRecordingAct {
      get {
        if (_targetRecordingAct == null) {
          _targetRecordingAct = InformationAct.Empty;
        }
        return _targetRecordingAct;
      }
      private set {
        _targetRecordingAct = value;
      }
    }

    private Property _targetResource = null;
    public Property TargetResource {
      get {
        if (_targetResource == null) {
          _targetResource = Property.Empty;
        }
        return _targetResource;
      }
      private set {
        _targetResource = value;
      }
    }

    public Recording Recording {
      get;
      private set;
    }
    
    public int Index {
      get;
      internal set;
    }

    private string _notes = String.Empty;
    public string Notes {
      get { return _notes; }
      set { 
        _notes = EmpiriaString.TrimAll(value);
      }
    }

    public RecordingActExtData ExtensionData {
      get;
      private set;
    }

    internal string Keywords {
      get;
      private set;
    }

    public Contact CanceledBy {
      get;
      private set;
    }

    public DateTime CancelationTime {
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

    public RecordingActStatus Status {
      get;
      set;
    }

    int IProtected.CurrentDataIntegrityVersion {
      get { return 1; }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "RecordingActType", this.RecordingActType.Id, 
          "Transaction", this.Transaction.Id, "Document", this.Document.Id, 
          "TargetRecordingAct", this.TargetRecordingAct.Id, "TargetResource", this.TargetResource.Id, 
          "Recording", this.Recording.Id, "Index", this.Index, "Notes", this.Notes, 
          "ExtensionData", this.ExtensionData.ToJson(), 
          "CanceledBy", this.CanceledBy.Id, "CancelationTime", this.CancelationTime,
          "PostedBy", this.PostedBy.Id, "PostingTime", this.PostingTime, "Status", (char) this.Status, 
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongRequestedVersionForDIF, version);
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

    public bool IsAnnotation {
      get { return this.RecordingActType.IsAnnotationType; }
    }

    public ObjectList<PropertyEvent> PropertiesEvents {
      get {
        if (propertiesEvents == null) {
          propertiesEvents = RecordingActsData.GetPropertiesEventsList(this);
        }
        return propertiesEvents;
      }
    }

    public RecordingActType RecordingActType {
      get {
        if (recordingActType == null) {
          recordingActType = RecordingActType.Parse(base.ObjectTypeInfo);
        }
        return recordingActType;
      }
      set { recordingActType = value; }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordingActStatus.Obsolete:
            return "No vigente";
          case RecordingActStatus.Incomplete:
            return "Incompleto";
          case RecordingActStatus.Pending:
            return "Pendiente";
          case RecordingActStatus.Registered:
            return "Registrado";
          case RecordingActStatus.Closed:
            return "Cerrado";
          case RecordingActStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      }
    }

    #endregion Public properties

    #region Public methods

    public void AttachResource(IRecordable resource) {

    }

    public void AppendPropertyEvent(Property property) {
      if (this.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecordingAct, "AppendProperty");
      }
      if (this.Recording.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "AppendProperty");
      }

      if (property.IsNew) {
        property.Save();
      } else if (PropertiesEvents.Contains((x) => x.Property.Equals(property))) {
        throw new LandRegistrationException(LandRegistrationException.Msg.PropertyAlreadyExistsOnRecordingAct,
                                            property.UniqueCode, this.Id);
      }

      var propertyEvent = new PropertyEvent(property, this);
      propertyEvent.Save();

      RecordingAct antecedent = property.GetAntecedent(this);
      if (antecedent != InformationAct.Empty) {
        PropertyEvent e = antecedent.PropertiesEvents.Find((x) => x.Property.Equals(property));
        if (e != null) {
          propertyEvent.MetesAndBounds = e.MetesAndBounds;
          propertyEvent.FloorArea = e.FloorArea;
          propertyEvent.CommonArea = e.CommonArea;
          propertyEvent.TotalArea = e.TotalArea;
          propertyEvent.Save();
        }
      }
      this.propertiesEvents = null;
    }

    internal void Delete() {
      if (this.Recording.Status == RecordingStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
      }
      if (this.Status == RecordingActStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, this.Id);
      }
      for (int i = 0; i < this.PropertiesEvents.Count; i++) {
        var propertyEvent = PropertiesEvents[i];
        var property = PropertiesEvents[i].Property;
        propertyEvent.Delete();

        var tract = property.GetRecordingActsTract();
        if (tract.Count == 0) {
          property.Status = PropertyStatus.Deleted;
          property.Save();
        }
      }
      this.Status = RecordingActStatus.Deleted;
      this.Save();
      this.propertiesEvents = null;
    }

    public PropertyEvent GetPropertyEvent(Property property) {
      var propertyEvent = this.PropertiesEvents.Find((x) => x.Property.Equals(property));
      if (propertyEvent != null) {
        return propertyEvent;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
                                            property.UniqueCode, this.Id);
      }
    }
    
    public bool IsFirstRecordingAct() {
      if (this.PropertiesEvents.Count == 0) {
        return false;
      }

      Property property = this.PropertiesEvents[0].Property;

      return property.IsFirstRecordingAct(this);
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.Recording = Recording.Parse((int) row["RecordingId"]);
      this.Index = (int) row["RecordingActIndex"];
      this.Notes = (string) row["RecordingActNotes"];
      this.PostedBy = Contact.Parse((int) row["PostedById"]);
      this.PostingTime = (DateTime) row["PostingTime"];
      this.Status = (RecordingActStatus) Convert.ToChar(row["RecordingActStatus"]);

      ///OOJJOO  Delete ASAP and use JSON read/write only
      
      this.ExtensionData.AppraisalAmount = Money.Parse(Currency.Parse((int) row["AppraisalCurrencyId"]), 
                                                                      (decimal) row["AppraisalAmount"]);
      this.ExtensionData.OperationAmount = Money.Parse(Currency.Parse((int) row["OperationCurrencyId"]), 
                                                                      (decimal) row["OperationAmount"]);
      this.ExtensionData.Contract.Interest.TermPeriods = (int) row["TermPeriods"];
      this.ExtensionData.Contract.Interest.TermUnit = Unit.Parse((int) row["TermUnitId"]);
      this.ExtensionData.Contract.Interest.Rate = (decimal) row["InterestRate"];
      this.ExtensionData.Contract.Interest.RateType = InterestRateType.Parse((int) row["InterestRateTypeId"]);
      this.ExtensionData.Contract.Date = (DateTime) row["ContractDate"];
      this.ExtensionData.Contract.Place = Empiria.Geography.GeographicRegionItem.Parse((int) row["ContractPlaceId"]);
      this.ExtensionData.Contract.Number = (string) row["ContractNumber"];

      //if (((string) row["RecordingActDIF"]).Length != 0) {
      //  Integrity.Assert((string) row["RecordingActDIF"]);
      //}
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingActsData.WriteRecordingAct(this);
    }

    public void RemoveProperty(Property property) {
      PropertyEvent propertyEvent = this.PropertiesEvents.Find((x) => x.Property.Equals(property));

      Assertion.RequireObject(propertyEvent, 
                new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
                                              property.Id, this.Id));
      propertyEvent.Delete();
      if (property.GetRecordingActsTract().Count == 0) {
        property.Status = PropertyStatus.Deleted;
        property.Save();
      }
      this.propertiesEvents = null;
      if (this.PropertiesEvents.Count == 0) {
        this.Status = RecordingActStatus.Deleted;
        this.Save();
      }

      Assertion.Ensure(property.Status == PropertyStatus.Deleted &&
                       this.Status == RecordingActStatus.Deleted, "fail");
    }


    internal RecordingAct WriteOn(RecorderOffice recorderOffice, RecordingSection recordingSection) {
      Assertion.AssertObject(recorderOffice, "recorderOffice");
      Assertion.AssertObject(recordingSection, "recordingSection");

      RecordingBook book = this.GetOpenedRecordingBook(recorderOffice, recordingSection);
      this.Recording = book.CreateRecording(this.Transaction);

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