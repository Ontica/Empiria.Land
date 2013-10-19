/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PropertyEvent                                  Pattern  : Association Class                   *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a roled association between a property and a recording act.                        *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Data;

using Empiria.DataTypes;

using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration {

  public enum PropertyEventStatus {
    Obsolete = 'S',
    NoLegible = 'L',
    Incomplete = 'I',
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Represents a roled association between a property and a recording act.</summary>
  public class PropertyEvent : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.PropertyEvent";

    private Property property = null;
    private RecordingAct recordingAct = null;
    private string metesAndBounds = String.Empty;
    private string notes = String.Empty;
    private Quantity totalArea = Quantity.Parse(Unit.Empty, 0m);
    private Quantity floorArea = Quantity.Parse(Unit.Empty, 0m);
    private Quantity commonArea = Quantity.Parse(Unit.Empty, 0m);
    private PropertyEventStatus status = PropertyEventStatus.Pending;

    #endregion Fields

    #region Constructors and parsers

    private PropertyEvent()
      : base(thisTypeName) {

    }

    protected PropertyEvent(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    internal PropertyEvent(Property property, RecordingAct recordingAct)
      : base(thisTypeName) {
      this.property = property;
      this.recordingAct = recordingAct;
    }

    static public PropertyEvent Parse(int id) {
      return BaseObject.Parse<PropertyEvent>(thisTypeName, id);
    }

    static internal PropertyEvent Parse(DataRow dataRow) {
      return BaseObject.Parse<PropertyEvent>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public Quantity CommonArea {
      get { return commonArea; }
      set { commonArea = value; }
    }

    public Quantity FloorArea {
      get { return floorArea; }
      set { floorArea = value; }
    }

    public string MetesAndBounds {
      get { return metesAndBounds; }
      set { metesAndBounds = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public Property Property {
      get { return property; }
      set { property = value; }
    }

    public RecordingAct RecordingAct {
      get { return recordingAct; }
      set { recordingAct = value; }
    }

    public PropertyEventStatus Status {
      get { return status; }
      set { status = value; }
    }

    public string StatusName {
      get {
        switch (status) {
          case PropertyEventStatus.Obsolete:
            return "No vigente";
          case PropertyEventStatus.NoLegible:
            return "No legible";
          case PropertyEventStatus.Incomplete:
            return "Incompleto";
          case PropertyEventStatus.Pending:
            return "Pendiente";
          case PropertyEventStatus.Registered:
            return "Registrado";
          case PropertyEventStatus.Closed:
            return "Cerrado";
          case PropertyEventStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      } // get
    }

    public Quantity TotalArea {
      get { return totalArea; }
      set { totalArea = value; }
    }

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = PropertyEventStatus.Deleted;
      base.Save();
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.property = Property.Parse((int) row["PropertyId"]);
      this.recordingAct = RecordingAct.Parse((int) row["RecordingActId"]);
      this.metesAndBounds = (string) row["MetesAndBounds"];
      this.notes = (string) row["PropertyEventNotes"];
      this.totalArea = Quantity.Parse(Unit.Parse((int) row["TotalAreaUnitId"]), (decimal) row["TotalArea"]);
      this.floorArea = Quantity.Parse(Unit.Parse((int) row["FloorAreaUnitId"]), (decimal) row["FloorArea"]);
      this.commonArea = Quantity.Parse(Unit.Parse((int) row["CommonAreaUnitId"]), (decimal) row["CommonArea"]);
      this.status = (PropertyEventStatus) Convert.ToChar(row["PropertyEventStatus"]);
    }

    protected override void ImplementsSave() {
      PropertyData.WritePropertyEvent(this);
    }

    #endregion Public methods

  } // class PropertyEvent

} // namespace Empiria.Government.LandRegistration