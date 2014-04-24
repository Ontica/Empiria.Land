/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyEvent                                  Pattern  : Association Class                   *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a recording act/property association.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a recording act/property association.</summary>
  public class TractIndexItem : BaseObject, IExtensible<TractIndexItemExtData>, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.TractIndexItem";

    #endregion Fields

    #region Constructors and parsers

    internal TractIndexItem(RecordingAct recordingAct, Property property) : base(thisTypeName) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.AssertObject(property, "property");

      Initialize();

      this.RecordingAct = recordingAct;
      this.Property = property;
    }

    protected TractIndexItem(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
      Initialize();
    }

    private void Initialize() {
      this.RecordingAct = InformationAct.Empty;
      this.Property = Property.Empty;
      this.ExtensionData = TractIndexItemExtData.Empty;
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.PostingTime = DateTime.Now;
      this.Status = RecordableObjectStatus.Pending;
    }

    static public TractIndexItem Parse(int id) {
      return BaseObject.Parse<TractIndexItem>(thisTypeName, id);
    }

    static internal TractIndexItem Parse(DataRow dataRow) {
      return BaseObject.Parse<TractIndexItem>(thisTypeName, dataRow);
    }
    static public TractIndexItem Empty {
      get {
        return BaseObject.ParseEmpty<TractIndexItem>(thisTypeName);
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingAct RecordingAct {
      get;
      private set;
    }

    public Property Property {
      get;
      private set;
    }

    public TractIndexItemExtData ExtensionData {
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

    public RecordableObjectStatus Status {
      get;
      set;  // OOJJOO Set the status using a special method and replace this public set accesor by a private one.
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

    int IProtected.CurrentDataIntegrityVersion {
      get { return 1; }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "RecordingAct", this.RecordingAct.Id, 
          "Property", this.Property.Id, "ExtensionData", this.ExtensionData.ToJson(),        
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

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.Property = Property.Parse((int) row["PropertyId"]);
      this.RecordingAct = RecordingAct.Parse((int) row["RecordingActId"]);
      this.ExtensionData = TractIndexItemExtData.Parse((string) row["TractItemExtensionData"]);
      this.PostedBy = Contact.Parse((int) row["PostedById"]);
      this.PostingTime = (DateTime) row["PostingTime"];
      this.Status = (RecordableObjectStatus) Convert.ToChar(row["TractIndexItemStatus"]);

      Integrity.Assert((string) row["TractIndexItemDIF"]);
    }

    protected override void ImplementsSave() {
      if (this.Property.IsNew) {
        this.Property.Save();
      }
      PropertyData.WriteTractIndexItem(this);
    }

    #endregion Public methods

  } // class TractIndexItem

} // namespace Empiria.Land.Registration