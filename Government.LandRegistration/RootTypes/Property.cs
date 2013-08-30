/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a property.                                                                        *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
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

    private RecordingAct recordingAct = InformationAct.Empty;
    private string uniqueCode = String.Empty;
    private PropertyType propertyType = PropertyType.Empty;
    private PropertyLandUse landUse = PropertyLandUse.Empty;
    private string commonName = String.Empty;

    private string recordingNotes = String.Empty;
    private string antecedent = String.Empty;

    private RecorderOffice cadastralOffice = RecorderOffice.Empty;
    private int cadastralObjectId = -1;
    private string cadastralCode = String.Empty;

    private GeographicRegionItem municipality = GeographicRegionItem.Empty;
    private GeographicRegionItem locality = GeographicRegionItem.Empty;
    private GeographicRegionItem settlement = GeographicRegionItem.Empty;
    private GeographicRegionItem postalCode = GeographicRegionItem.Empty;
    private GeographicPathItem street = GeographicPathItem.Empty;
    private string streetSegment = String.Empty;
    private GeographicPathItem fromStreet = GeographicPathItem.Empty;
    private GeographicPathItem toStreet = GeographicPathItem.Empty;
    private GeographicPathItem backStreet = GeographicPathItem.Empty;

    private string externalNo = String.Empty;
    private string internalNo = String.Empty;
    private string fractionTag = String.Empty;
    private string ubicationReference = String.Empty;
    private string keywords = String.Empty;

    private string metesAndBounds = String.Empty;
    private string geoPolygon = String.Empty;

    private Quantity totalArea = Quantity.Parse(Unit.Empty, 0m);
    private Quantity floorArea = Quantity.Parse(Unit.Empty, 0m);
    private Quantity commonArea = Quantity.Parse(Unit.Empty, 0m);

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

    public GeographicPathItem BackStreet {
      get { return backStreet; }
      set { backStreet = value; }
    }

    public string CadastralCode {
      get { return cadastralCode; }
      set { cadastralCode = EmpiriaString.TrimAll(value); }
    }

    public int CadastralObjectId {
      get { return cadastralObjectId; }
      set { cadastralObjectId = value; }
    }

    public RecorderOffice CadastralOffice {
      get { return cadastralOffice; }
      set { cadastralOffice = value; }
    }

    public Quantity CommonArea {
      get { return commonArea; }
      set { commonArea = value; }
    }

    public string CommonName {
      get { return commonName; }
      set { commonName = EmpiriaString.TrimAll(value); }
    }

    public string ExternalNo {
      get { return externalNo; }
      set { externalNo = EmpiriaString.TrimAll(value); }
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

    public Quantity FloorArea {
      get { return floorArea; }
      set { floorArea = value; }
    }

    public string FractionTag {
      get { return fractionTag; }
      set { fractionTag = value; }
    }

    public GeographicPathItem FromStreet {
      get { return fromStreet; }
      set { fromStreet = value; }
    }

    public string GeoPolygon {
      get { return geoPolygon; }
      set { geoPolygon = value; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public string InternalNo {
      get { return internalNo; }
      set { internalNo = EmpiriaString.TrimAll(value); }
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

    public GeographicRegionItem Locality {
      get { return locality; }
      set { locality = value; }
    }

    public GeographicRegionItem PostalCode {
      get { return postalCode; }
      set { postalCode = value; }
    }

    public PropertyType PropertyType {
      get { return propertyType; }
      set { propertyType = value; }
    }

    public RecordingAct RecordingAct {
      get { return recordingAct; }
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

    public string StreetSegment {
      get { return streetSegment; }
      set { streetSegment = value; }
    }

    public string MetesAndBounds {
      get { return metesAndBounds; }
      set { metesAndBounds = value; }
    }

    public string RecordingNotes {
      get { return recordingNotes; }
      set { recordingNotes = value; }
    }

    public GeographicPathItem ToStreet {
      get { return toStreet; }
      set { toStreet = value; }
    }

    public Quantity TotalArea {
      get { return totalArea; }
      set { totalArea = value; }
    }

    public string UbicationReference {
      get { return ubicationReference; }
      set { ubicationReference = EmpiriaString.TrimAll(value); }
    }

    public string UniqueCode {
      get { return uniqueCode; }
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

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.recordingAct = RecordingAct.Parse((int) row["RecordingActId"]);
      this.uniqueCode = (string) row["PropertyUniqueCode"];
      this.propertyType = PropertyType.Parse((int) row["PropertyTypeId"]);
      this.landUse = PropertyLandUse.Parse((int) row["PropertyLandUseId"]);
      this.commonName = (string) row["PropertyCommonName"];
      this.recordingNotes = (string) row["PropertyRecordingNotes"];
      this.antecedent = (string) row["Antecedent"];

      this.cadastralOffice = RecorderOffice.Parse((int) row["CadastralOfficeId"]);
      this.cadastralObjectId = (int) row["CadastralObjectId"];
      this.cadastralCode = (string) row["CadastralCode"];

      this.municipality = GeographicRegionItem.Parse((int) row["MunicipalityId"]);
      this.locality = GeographicRegionItem.Parse((int) row["LocalityId"]);
      this.settlement = GeographicRegionItem.Parse((int) row["SettlementId"]);
      this.postalCode = GeographicRegionItem.Parse((int) row["PostalCodeId"]);
      this.street = GeographicPathItem.Parse((int) row["StreetId"]);
      this.streetSegment = (string) row["StreetSegment"];
      this.fromStreet = GeographicPathItem.Parse((int) row["FromStreetId"]);
      this.toStreet = GeographicPathItem.Parse((int) row["ToStreetId"]);
      this.backStreet = GeographicPathItem.Parse((int) row["BackStreetId"]);
      this.externalNo = (string) row["ExternalNo"];
      this.internalNo = (string) row["InternalNo"];
      this.fractionTag = (string) row["FractionTag"];
      this.ubicationReference = (string) row["UbicationReference"];
      this.keywords = (string) row["PropertyKeywords"];

      this.metesAndBounds = (string) row["MetesAndBounds"];
      this.geoPolygon = (string) row["GeoPolygon"];
      this.totalArea = Quantity.Parse(Unit.Parse((int) row["TotalAreaUnitId"]), (decimal) row["TotalArea"]);
      this.floorArea = Quantity.Parse(Unit.Parse((int) row["FloorAreaUnitId"]), (decimal) row["FloorArea"]);
      this.commonArea = Quantity.Parse(Unit.Parse((int) row["CommonAreaUnitId"]), (decimal) row["CommonArea"]);

      this.status = (PropertyStatus) Convert.ToChar(row["PropertyStatus"]);
      this.integrityHashCode = (string) row["PropertyRIHC"];
    }

    protected override void ImplementsSave() {
      if (this.uniqueCode.Length == 0) {
        while (true) {
          string temp = TransactionData.GeneratePropertyKey();
          if (!PropertyData.ExistsPropertyTractKey(temp)) {
            this.uniqueCode = temp;
            break;
          }
        } // while
      }
      this.keywords = EmpiriaString.BuildKeywords(this.UniqueCode, this.CadastralCode, this.CommonName, this.UbicationReference,
                                                  this.Street.Name, this.ExternalNo, this.InternalNo, this.StreetSegment, this.FractionTag,
                                                  this.Settlement.Keywords, this.Municipality.Name, this.Locality.Name, this.PostalCode.Name, 
                                                  this.CadastralOffice.Alias, this.PropertyType.Name);
      PropertyData.WriteProperty(this);
    }

    #endregion Public methods

  } // class Property

} // namespace Empiria.Government.LandRegistration