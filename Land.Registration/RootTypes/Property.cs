/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a property.                                                                        *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Security;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property.</summary>
  public class Property : BaseObject, IProtected {

    #region Constructors and parsers

    internal Property() {
      // Required by Empiria Framework.
    }

    static public Property Parse(int id) {
      return BaseObject.ParseId<Property>(id);
    }

    static public Property TryParseWithUID(string propertyUID) {
      DataRow row = PropertyData.GetPropertyWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Property>(row);
      } else {
        return null;
      }
    }

    static public Property Empty {
      get { return BaseObject.ParseEmpty<Property>(); }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyUID", IsOptional = false)]
    public string UID {
      get;
      private set;
    }

    [DataField("PropertyName")]
    public string Name {
      get;
      set;
    }

    [DataField("PropertyKind")]
    public PropertyKind PropertyKind {
      get;
      set;
    }

    [DataField("PropertyRecordingNotes")]
    public string RecordingNotes {
      get;
      set;
    }

    [DataField("AntecedentNotes")]
    public string AntecedentNotes {
      get;
      set;
    }

    public Address Location {
      get;
      private set;
    }

    public CadastralInfo CadastralData {
      get;
      private set;
    }

    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID, this.CadastralData.CadastralCode, this.Name, 
                                           this.PartitionNo, this.Location.Keywords, this.PropertyKind.Value);
      }
    }

    [DataField("PartitionOfId")]
    private LazyInstance<Property> _partitionOf = LazyInstance<Property>.Empty;
    public Property PartitionOf {
      get { return _partitionOf.Value; }
      private set {
        _partitionOf = LazyInstance<Property>.Parse(value);
      }
    }

    [DataField("PartitionNo")]
    public string PartitionNo {
      get;
      private set;
    }

    [DataField("MergedIntoId")]
    private LazyInstance<Property> _mergedInto = LazyInstance<Property>.Empty;
    public Property MergedInto {
      get { return _mergedInto.Value; }
      private set {
        _mergedInto = LazyInstance<Property>.Parse(value);
      }
    }

    [DataField("PostedById")]
    private LazyInstance<Contact> _postedBy = LazyInstance<Contact>.Empty;
    public Contact PostedBy {
      get { return _postedBy.Value; }
      private set {
        _postedBy = LazyInstance<Contact>.Parse(value);
      }
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("PropertyStatus", Default = RecordableObjectStatus.Incomplete)]
    public RecordableObjectStatus Status {
      get;
      set;      // OOJJOO -- Remove public set and change status through a special change status method.
    }

    int IProtected.CurrentDataIntegrityVersion {
      get { return 1; }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "PropertyType", this.GetEmpiriaType().Id, "UID", this.UID, "PostedBy",
          this.PostedBy.Id, "PostingTime", this.PostingTime, "PartitionOf", _partitionOf.Id,
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

    public FixedList<RecordingAct> Annotations {
      get {
        return RecordingBooksData.GetPropertyAnnotationList(this);
      }
    }

    public RecordingAct FirstRecordingAct {
      get {
        FixedList<RecordingAct> domainActs = this.GetRecordingActsTract();
        if (domainActs.Count != 0) {
          return domainActs[0];
        } else {
          throw new LandRegistrationException(LandRegistrationException.Msg.PropertyDoesNotHaveAnyRecordingActs,
                                              this.UID);
        }
      }
    }

    public RecordingAct LastRecordingAct {
      get {
        FixedList<RecordingAct> domainActs = this.GetRecordingActsTract();
        if (domainActs.Count != 0) {
          return domainActs[domainActs.Count - 1];
        } else {
          return InformationAct.Empty;
        }
      }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.NoLegible:
            return "No legible";
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
      } // get
    }

    #endregion Public properties

    #region Public methods

    public bool IsFirstRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.IsAnnotation) {
        return false;
      }

      RecordingAct firstRecordingAct = this.FirstRecordingAct;
      if (firstRecordingAct != InformationAct.Empty) {
        return firstRecordingAct.Equals(recordingAct);
      } else {
        return true;
      }
    }

    public bool IsLastRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.IsAnnotation) {
        return false;
      }

      RecordingAct lastRecordingAct = this.LastRecordingAct;
      if (lastRecordingAct != InformationAct.Empty) {
        return lastRecordingAct.Equals(recordingAct);
      } else {
        return true;
      }
    }

    public FixedList<RecordingAct> GetRecordingActsTract() {
      return RecordingActsData.GetPropertyRecordingActList(this);
    }

    public FixedList<RecordingAct> GetRecordingActsTractUntil(RecordingAct breakAct, bool includeBreakAct) {
      return RecordingActsData.GetPropertyRecordingActListUntil(this, breakAct, includeBreakAct);
    }

    public RecordingAct GetAntecedent(RecordingAct baseRecordingAct) {
      FixedList<RecordingAct> tract = this.GetRecordingActsTract();

      int index = tract.IndexOf(baseRecordingAct);

      if (index == -1) {
        return InformationAct.Empty;
      } else if ((index + 1) < tract.Count) {
        return tract[index + 1];
      } else {
        return InformationAct.Empty; // No Antecedent
      }
    }

    public RecordingAct GetDomainAntecedent(RecordingAct baseRecordingAct) {
      FixedList<RecordingAct> tract = this.GetRecordingActsTractUntil(baseRecordingAct, false);

      if (tract.Count == 0) {         // Antecedent no registered
        return InformationAct.Empty;
      }
      RecordingAct antecedent = tract.FindLast((x) => x is DomainAct);
      if (antecedent != null) {
        return antecedent;
      } else if (tract[0].RecordingActType.Equals(RecordingActType.Empty)) {
        return tract[0];
      } else {
        return InformationAct.Empty;
      }
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.Location = Address.FromJson((string) row["LocationExtData"]);
      this.CadastralData = CadastralInfo.FromJson((string) row["CadastralExtData"]);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.AssignUID();
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      Assertion.Assert(this.UID.Length != 0, "Property UniqueIdentifier can't be an empty string.");
      PropertyData.WriteProperty(this);
    }

    private void AssignUID() {
      Assertion.Assert(this.UID.Length == 0, "Property has already assigned a UniqueIdentifier.");
      
      while (true) {
        string temp = TransactionData.GeneratePropertyKey();
        if (!PropertyData.ExistsPropertyUID(temp)) {
          this.UID = temp;
          break;
        }
      } // while
      Assertion.Assert(this.UID.Length != 0, "Property UniqueIdentifier has not been generated.");
    }

    #endregion Public methods

  } // class Property

} // namespace Empiria.Land.Registration
