/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyEvent                                  Pattern  : Association Class                   *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
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

    #region Constructors and parsers

    private TractIndexItem() {
      // Required by Empiria Framework.
    }

    internal TractIndexItem(Property property, RecordingAct recordingAct) {
      Assertion.AssertObject(property, "property");
      Assertion.AssertObject(recordingAct, "recordingAct");

      this.Property = property;
      this.RecordingAct = recordingAct;
    }

    static public TractIndexItem Parse(int id) {
      return BaseObject.ParseId<TractIndexItem>(id);
    }

    static internal TractIndexItem Parse(DataRow dataRow) {
      return BaseObject.ParseDataRow<TractIndexItem>(dataRow);
    }
    static public TractIndexItem Empty {
      get {
        return BaseObject.ParseEmpty<TractIndexItem>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyId", IsOptional = false)]
    public Property Property {
      get;
      private set;
    }

    [DataField("RecordingActId", Default = "Land.Registration.InformationAct.Empty", IsOptional = false)]
    public RecordingAct RecordingAct {
      get;
      private set;
    }

    public TractIndexItemExtData ExtensionData {
      get;
      private set;
    }

    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("TractIndexItemStatus", Default = RecordableObjectStatus.Pending)]
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

    internal void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = TractIndexItemExtData.Parse((string) row["TractItemExtData"]);
    }

    protected override void OnSave() {
      if (this.Property.IsNew) {
        this.Property.Save();
      }
      if (this.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      PropertyData.WriteTractIndexItem(this);
    }

    #endregion Public methods

  } // class TractIndexItem

} // namespace Empiria.Land.Registration
