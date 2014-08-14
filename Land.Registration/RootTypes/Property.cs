/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a property.                                                                        *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Security;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property.</summary>
  public class Property : BaseObject, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.Property";

    private LazyObject<Property> _partitionOf = LazyObject<Property>.Empty;
    private LazyObject<Property> _mergedInto = LazyObject<Property>.Empty;

    #endregion Fields

    #region Constructors and parsers

    public Property() : base(thisTypeName) {
      Initialize();
    }

    protected Property(string typeName) : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
    }

    static public Property Parse(int id) {
      return BaseObject.Parse<Property>(thisTypeName, id);
    }

    static internal Property Parse(DataRow dataRow) {
      return BaseObject.Parse<Property>(thisTypeName, dataRow);
    }

    static public Property ParseWithUniqueCode(string propertyKey) {
      DataRow row = PropertyData.GetPropertyWithUniqueCode(propertyKey);

      if (row != null) {
        return Property.Parse(row);
      } else {
        return null;
      }
    }

    static public Property Empty {
      get { return BaseObject.ParseEmpty<Property>(thisTypeName); }
    }

    private void Initialize() {
      this.UniqueCode = String.Empty;
      this.Name = String.Empty;
      this.PropertyKind = PropertyKind.Empty;
      this.RecordingNotes = String.Empty;
      this.AntecedentNotes = String.Empty;
      this.Location = Address.Empty;
      this.CadastralData = CadastralInfo.Empty;
      this.PartitionNo = String.Empty;
      this.PostedBy = Person.Empty;
      this.PostingTime = DateTime.Now;
      this.Status = RecordableObjectStatus.Incomplete;
    }

    #endregion Constructors and parsers

    #region Public properties

    public string UniqueCode {
      get;
      private set;
    }

    public string Name {
      get;
      set;
    }

    public PropertyKind PropertyKind {
      get;
      set;
    }

    public string RecordingNotes {
      get;
      set;
    }

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
        return EmpiriaString.BuildKeywords(this.UniqueCode, this.CadastralData.CadastralCode, this.Name, 
                                           this.PartitionNo, this.Location.Keywords, this.PropertyKind.Name);
      }
    }

    public Property PartitionOf {
      get {
        return _partitionOf;
      }
    }

    public string PartitionNo {
      get;
      private set;
    }

    public Property MergedInto {
      get {
        return _mergedInto;
      }
    }

    public Contact PostedBy {
      get;
      private set;
    }

    public DateTime PostingTime {
      get;
      private set;
    }

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
          1, "Id", this.Id, "PropertyType", this.ObjectTypeInfo.Id,
          "PostedBy", this.PostedBy.Id, "PostingTime", this.PostingTime, 
          "PartitionOf", _partitionOf.Id,
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
                                              this.UniqueCode);
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

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.UniqueCode = (string) row["PropertyUniqueCode"];
      this.Name = (string) row["PropertyName"];
      this.PropertyKind = PropertyKind.Parse((int) row["PropertyKindId"]);
      this.RecordingNotes = (string) row["PropertyRecordingNotes"];
      this.AntecedentNotes = (string) row["AntecedentNotes"];
      this.Location = Address.FromJson((string) row["LocationExtData"]);
      this.CadastralData = CadastralInfo.FromJson((string) row["CadastralExtData"]);
      _partitionOf = LazyObject<Property>.Parse((int) row["PartitionOfId"]);
      this.PartitionNo = (string) row["PartitionNo"];
      _mergedInto = LazyObject<Property>.Parse((int) row["MergedIntoId"]);
      this.PostedBy = Contact.Parse((int) row["PostedById"]);
      this.PostingTime = (DateTime) row["PostingTime"];
      this.Status = (RecordableObjectStatus) Convert.ToChar(row["PropertyStatus"]);

      Integrity.Assert((string) row["PropertyDIF"]);
    }

    protected override void ImplementsSave() {
      if (this.IsNew) {
        this.AssignUniqueCode();
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      Assertion.Assert(this.UniqueCode.Length != 0, "Property UniqueCode can't be an empty string.");
      PropertyData.WriteProperty(this);
    }

    private void AssignUniqueCode() {
      Assertion.Assert(this.UniqueCode.Length == 0, "Property has already assigned a UniqueCode.");
      
      while (true) {
        string temp = TransactionData.GeneratePropertyKey();
        if (!PropertyData.ExistsPropertyUniqueCode(temp)) {
          this.UniqueCode = temp;
          break;
        }
      } // while
      Assertion.Assert(this.UniqueCode.Length != 0, "Property UniqueCode has not been generated.");
    }

    #endregion Public methods

  } // class Property

} // namespace Empiria.Land.Registration
