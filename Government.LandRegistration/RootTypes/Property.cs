/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a property.                                                                         *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;

using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration {

  public enum PropertyStatus {
    Obsolete = 'S',
    NoLegible = 'L',
    Incomplete = 'I',
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Represents a property.</summary>
  public class Property : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.Property";

    private PropertyType propertyType = PropertyType.Empty;
    private string tractKey = String.Empty;
    private RecorderOffice cadastralOffice = RecorderOffice.Empty;
    private int cadastralObjectId = -1;
    private string cadastralKey = String.Empty;
    private PropertyLandUse landUse = PropertyLandUse.Empty;
    private string commonName = String.Empty;
    private string antecedent = String.Empty;
    private GeographicRegionItem municipality = GeographicRegionItem.Empty;
    private GeographicRegionItem settlement = GeographicRegionItem.Empty;
    private GeographicPathItem street = GeographicPathItem.Empty;
    private GeographicRegionItem postalCode = GeographicRegionItem.Empty;
    private string externalNumber = String.Empty;
    private string internalNumber = String.Empty;
    private string buildingTag = String.Empty;
    private string floorTag = String.Empty;
    private string fractionTag = String.Empty;
    private string batchTag = String.Empty;
    private string blockTag = String.Empty;
    private string sectionTag = String.Empty;
    private string superSectionTag = String.Empty;
    private GeographicPathItem fromStreet = GeographicPathItem.Empty;
    private GeographicPathItem toStreet = GeographicPathItem.Empty;
    private string ubication = String.Empty;
    private string firstKnownOwner = String.Empty;
    private string notes = String.Empty;
    private string keywords = String.Empty;
    private Contact postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    private PropertyStatus status = PropertyStatus.Incomplete;
    private string integrityHashCode = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    public Property()
      : base(thisTypeName) {

    }

    protected Property(string typeName)
      : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
    }

    static public Property Parse(int id) {
      return BaseObject.Parse<Property>(thisTypeName, id);
    }

    static internal Property Parse(DataRow dataRow) {
      return BaseObject.Parse<Property>(thisTypeName, dataRow);
    }

    static public Property ParseWithTractKey(string propertyKey) {
      DataRow row = PropertyData.GetPropertyWithTractKey(propertyKey);

      if (row != null) {
        return Property.Parse(row);
      } else {
        return null;
      }
    }

    static public Property Empty {
      get { return BaseObject.ParseEmpty<Property>(thisTypeName); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public ObjectList<RecordingAct> Annotations {
      get {
        return RecordingBooksData.GetPropertyAnnotationList(this);
      }
    }

    public string Antecedent {
      get { return antecedent; }
      set { antecedent = value; }
    }

    public string BatchTag {
      get { return batchTag; }
      set { batchTag = EmpiriaString.TrimAll(value); }
    }

    public string BlockTag {
      get { return blockTag; }
      set { blockTag = EmpiriaString.TrimAll(value); }
    }

    public string BuildingTag {
      get { return buildingTag; }
      set { buildingTag = EmpiriaString.TrimAll(value); }
    }

    public string CadastralKey {
      get { return cadastralKey; }
      set { cadastralKey = EmpiriaString.TrimAll(value); }
    }

    public int CadastralObjectId {
      get { return cadastralObjectId; }
      set { cadastralObjectId = value; }
    }

    public RecorderOffice CadastralOffice {
      get { return cadastralOffice; }
      set { cadastralOffice = value; }
    }

    public string CommonName {
      get { return commonName; }
      set { commonName = EmpiriaString.TrimAll(value); }
    }

    public RecordingAct FirstRecordingAct {
      get {
        ObjectList<RecordingAct> domainActs = this.GetRecordingActsTract();
        if (domainActs.Count != 0) {
          return domainActs[domainActs.Count - 1];
        } else {
          return InformationAct.Empty;
        }
      }
    }

    public RecordingAct LastRecordingAct {
      get {
        ObjectList<RecordingAct> domainActs = this.GetRecordingActsTract();
        if (domainActs.Count != 0) {
          return domainActs[0];
        } else {
          return InformationAct.Empty;
        }
      }
    }

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

    public ObjectList<RecordingAct> GetRecordingActsTract() {
      return RecordingBooksData.GetPropertyRecordingActList(this);
    }

    public RecordingAct GetAntecedent(RecordingAct baseRecordingAct) {
      ObjectList<RecordingAct> tract = this.GetRecordingActsTract();

      int index = tract.IndexOf(baseRecordingAct);

      if (index == -1) {
        return InformationAct.Empty;
      } else if ((index + 1) < tract.Count) {
        return tract[index + 1];
      } else {
        return tract[index];
      }
    }

    public string ExternalNumber {
      get { return externalNumber; }
      set { externalNumber = EmpiriaString.TrimAll(value); }
    }

    public string FirstKnownOwner {
      get { return firstKnownOwner; }
      set { firstKnownOwner = EmpiriaString.TrimAll(value); }
    }

    public string FloorTag {
      get { return floorTag; }
      set { floorTag = EmpiriaString.TrimAll(value); }
    }

    public string FractionTag {
      get { return fractionTag; }
      set { fractionTag = value; }
    }

    public GeographicPathItem FromStreet {
      get { return fromStreet; }
      set { fromStreet = value; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public string InternalNumber {
      get { return internalNumber; }
      set { internalNumber = EmpiriaString.TrimAll(value); }
    }

    public string Keywords {
      get { return keywords; }
    }

    public PropertyLandUse LandUse {
      get { return landUse; }
      set { landUse = value; }
    }

    public GeographicRegionItem Municipality {
      get { return municipality; }
      set { municipality = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = EmpiriaString.TrimAll(value); }
    }

    public GeographicRegionItem PostalCode {
      get { return postalCode; }
      set { postalCode = value; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public PropertyType PropertyType {
      get { return propertyType; }
      set { propertyType = value; }
    }

    public string SectionTag {
      get { return sectionTag; }
      set { sectionTag = EmpiriaString.TrimAll(value); }
    }

    public GeographicRegionItem Settlement {
      get { return settlement; }
      set { settlement = value; }
    }

    public PropertyStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
          case PropertyStatus.Obsolete:
            return "No vigente";
          case PropertyStatus.NoLegible:
            return "No legible";
          case PropertyStatus.Incomplete:
            return "Incompleto";
          case PropertyStatus.Pending:
            return "Pendiente";
          case PropertyStatus.Registered:
            return "Registrado";
          case PropertyStatus.Closed:
            return "Cerrado";
          case PropertyStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      } // get
    }

    public GeographicPathItem Street {
      get { return street; }
      set { street = value; }
    }

    public string SuperSectionTag {
      get { return superSectionTag; }
      set { superSectionTag = EmpiriaString.TrimAll(value); }
    }

    public string TractKey {
      get { return tractKey; }
    }

    public GeographicPathItem ToStreet {
      get { return toStreet; }
      set { toStreet = value; }
    }

    public string Ubication {
      get { return ubication; }
      set { ubication = EmpiriaString.TrimAll(value); }
    }

    #endregion Public properties

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.propertyType = PropertyType.Parse((int) row["PropertyTypeId"]);
      this.tractKey = (string) row["PropertyTractKey"];
      this.cadastralOffice = RecorderOffice.Parse((int) row["CadastralOfficeId"]);
      this.cadastralObjectId = (int) row["CadastralObjectId"];
      this.cadastralKey = (string) row["CadastralKey"];
      this.landUse = PropertyLandUse.Parse((int) row["PropertyLandUseId"]);
      this.commonName = (string) row["PropertyCommonName"];
      this.antecedent = (string) row["Antecedent"];
      this.municipality = GeographicRegionItem.Parse((int) row["MunicipalityId"]);
      this.settlement = GeographicRegionItem.Parse((int) row["SettlementId"]);
      this.street = GeographicPathItem.Parse((int) row["StreetId"]);
      this.postalCode = GeographicRegionItem.Parse((int) row["PostalCodeId"]);
      this.externalNumber = (string) row["ExternalNumber"];
      this.internalNumber = (string) row["InternalNumber"];
      this.buildingTag = (string) row["BuildingTag"];
      this.floorTag = (string) row["FloorTag"];
      this.fractionTag = (string) row["FractionTag"];
      this.batchTag = (string) row["BatchTag"];
      this.blockTag = (string) row["BlockTag"];
      this.sectionTag = (string) row["SectionTag"];
      this.superSectionTag = (string) row["SuperSectionTag"];
      this.fromStreet = GeographicPathItem.Parse((int) row["FromStreetId"]);
      this.toStreet = GeographicPathItem.Parse((int) row["ToStreetId"]);
      this.ubication = (string) row["Ubication"];
      this.firstKnownOwner = (string) row["FirstKnownOwner"];
      this.notes = (string) row["PropertyNotes"];
      this.keywords = (string) row["PropertyKeywords"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.status = (PropertyStatus) Convert.ToChar(row["PropertyStatus"]);
      this.integrityHashCode = (string) row["PropertyRIHC"];
    }

    private string GenerateTractKey() {
      const string literals = "ABCDEFHJKMNPQRTUWYZ" +
                              "012345678901234567890123456789";

      Random random = new Random();
      string temp = String.Empty;
      int hashCode = 0;
      for (int i = 0; i < 8; i++) {
        int position = random.Next(literals.Length);
        temp += literals[position];
        hashCode += (Math.Abs(temp.GetHashCode()) % (i + 1) ^ 2);
      }
      return temp + "-" + (hashCode % 10);
    }

    protected override void ImplementsSave() {
      if (this.tractKey.Length == 0) {
        while (true) {
          string temp = GenerateTractKey();
          if (!PropertyData.ExistsPropertyTractKey(temp)) {
            this.tractKey = temp;
            break;
          }
        } // while
      }
      if (base.IsNew) {
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.keywords = EmpiriaString.BuildKeywords(this.cadastralKey, this.tractKey, this.commonName, this.ubication, this.firstKnownOwner,
                                                  this.street.Name, this.externalNumber, this.internalNumber, this.batchTag,
                                                  this.blockTag, this.sectionTag, this.superSectionTag, this.buildingTag,
                                                  this.fractionTag, this.floorTag, this.settlement.Keywords, this.cadastralOffice.Alias,
                                                  this.municipality.Name, this.postalCode.Name, this.propertyType.Name);
      PropertyData.WriteProperty(this);
    }

    #endregion Public methods

  } // class Property

} // namespace Empiria.Government.LandRegistration