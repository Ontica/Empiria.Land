/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingAct                                   Pattern  : Empiria Object Type                 *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act. All recording acts must be descendents        *
*              of this type.                                                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Geography;

using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration {

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
  public abstract class RecordingAct : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingAct";

    private RecordingActType recordingActType = null;
    private Recording recording = Recording.Empty;
    private int index = 0;
    private string notes = String.Empty;
    private Money appraisalAmount = Money.Empty;
    private Money operationAmount = Money.Empty;
    private int termPeriods = 0;
    private Unit termUnit = Unit.Unknown;
    private decimal interestRate = decimal.Zero;
    private InterestRateType interestRateType = InterestRateType.Empty;
    private DateTime contractDate = ExecutionServer.DateMaxValue;
    private GeographicRegionItem contractPlace = GeographicRegionItem.Empty;
    private string contractNumber = String.Empty;
    private Contact postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    private DateTime postingTime = DateTime.Now;
    private RecordingActStatus status = RecordingActStatus.Incomplete;

    private ObjectList<PropertyEvent> propertiesEvents = null;


    #endregion Fields

    #region Constructors and parsers

    protected RecordingAct(string typeName)
      : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
    }

    static internal RecordingAct Create(RecordingActType recordingActType, Recording recording, Property property) {
      RecordingAct recordingAct = recordingActType.CreateInstance();
      recordingAct.recordingActType = recordingActType;
      recordingAct.recording = recording;
      recordingAct.propertiesEvents = new ObjectList<PropertyEvent>();
      recordingAct.index = recording.RecordingActs.Count + 1;
      if (recordingActType.Autoregister) {
        recordingAct.status = RecordingActStatus.Registered;
      } else {
        recordingAct.status = RecordingActStatus.Pending;
      }
      recordingAct.Save();
      recordingAct.AppendPropertyEvent(property);

      return recordingAct;
    }

    static public RecordingAct Parse(int id) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, id);
    }

    static internal RecordingAct Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingAct>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public Money AppraisalAmount {
      get { return appraisalAmount; }
      set { appraisalAmount = value; }
    }

    public string ContractNumber {
      get { return contractNumber; }
      set { contractNumber = value; }
    }

    public DateTime ContractDate {
      get { return contractDate; }
      set { contractDate = value; }
    }

    public GeographicRegionItem ContractPlace {
      get { return contractPlace; }
      set { contractPlace = value; }
    }

    public bool HasFirstKnownOwner {
      get {
        if (this.PropertiesEvents.Count == 0) {
          return false;
        }
        return (this.PropertiesEvents[0].Property.FirstKnownOwner.Length != 0);
      }
    }

    public int Index {
      get { return index; }
      internal set { index = value; }
    }

    public decimal InterestRate {
      get { return interestRate; }
      set { interestRate = value; }
    }

    public InterestRateType InterestRateType {
      get { return interestRateType; }
      set { interestRateType = value; }
    }

    public bool IsAnnotation {
      get { return this.RecordingActType.IsAnnotation; }
    }

    public string Notes {
      get { return notes; }
      set { notes = EmpiriaString.TrimAll(value); }
    }

    public Money OperationAmount {
      get { return operationAmount; }
      set { operationAmount = value; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public ObjectList<PropertyEvent> PropertiesEvents {
      get {
        if (propertiesEvents == null) {
          propertiesEvents = PropertyData.GetPropertiesEventsList(this);
        }
        return propertiesEvents;
      }
    }

    public Recording Recording {
      get { return recording; }
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

    public RecordingActStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
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

    public int TermPeriods {
      get { return termPeriods; }
      set { termPeriods = value; }
    }

    public Unit TermUnit {
      get { return termUnit; }
      set { termUnit = value; }
    }

    #endregion Public properties

    #region Public methods

    public void AppendPropertyEvent(Property property) {
      if (this.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecordingAct, "AppendProperty");
      }
      if (this.recording.IsNew) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "AppendProperty");
      }

      if (property.IsNew) {
        property.Save();
      } else if (PropertiesEvents.Contains((x) => x.Property.Equals(property))) {
        throw new LandRegistrationException(LandRegistrationException.Msg.PropertyAlreadyExistsOnRecordingAct,
                                            property.TractKey, this.Id);
      }

      PropertyEvent propertyEvent = new PropertyEvent(property, this);
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
      for (int i = 0; i < PropertiesEvents.Count; i++) {
        PropertyEvent propertyEvent = PropertiesEvents[i];
        Property property = PropertiesEvents[i].Property;
        if (property.FirstRecordingAct.Equals(this)) {
          property.Status = PropertyStatus.Deleted;
          property.Save();
        }
        propertyEvent.Delete();
      }
      this.Status = RecordingActStatus.Deleted;
      this.Save();
      this.propertiesEvents = null;
    }

    public PropertyEvent GetPropertyEvent(Property property) {
      PropertyEvent propertyEvent = this.PropertiesEvents.Find((x) => x.Property.Equals(property));
      if (propertyEvent != null) {
        return propertyEvent;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
                                            property.TractKey, this.Id);
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
      this.recording = Recording.Parse((int) row["RecordingId"]);
      this.index = (int) row["RecordingActIndex"];
      this.notes = (string) row["RecordingActNotes"];
      this.appraisalAmount = Money.Parse(Currency.Parse((int) row["AppraisalCurrencyId"]), (decimal) row["AppraisalAmount"]);
      this.operationAmount = Money.Parse(Currency.Parse((int) row["OperationCurrencyId"]), (decimal) row["OperationAmount"]);
      this.termPeriods = (int) row["TermPeriods"];
      this.termUnit = Unit.Parse((int) row["TermUnitId"]);
      this.interestRate = (decimal) row["InterestRate"];
      this.interestRateType = InterestRateType.Parse((int) row["InterestRateTypeId"]);
      this.contractDate = (DateTime) row["ContractDate"];
      this.contractPlace = GeographicRegionItem.Parse((int) row["ContractPlaceId"]);
      this.contractNumber = (string) row["ContractNumber"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.postingTime = (DateTime) row["PostingTime"];
      this.status = (RecordingActStatus) Convert.ToChar(row["RecordingActStatus"]);
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.postingTime = DateTime.Now;
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecordingAct(this);
    }

    public void RemoveProperty(Property property) {
      PropertyEvent propertyEvent = this.PropertiesEvents.Find((x) => x.Property.Equals(property));

      Assertion.RequireObject(propertyEvent, new LandRegistrationException(LandRegistrationException.Msg.PropertyNotBelongsToRecordingAct,
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

    public void SetFirstPropertyOwner(string firstPropertyOwner) {
      for (int i = 0; i < PropertiesEvents.Count; i++) {
        Property property = PropertiesEvents[i].Property;

        property.FirstKnownOwner = firstPropertyOwner;
        property.Save();
      }
    }

    #endregion Public methods

  } // class RecordingAct

} // namespace Empiria.Government.LandRegistration