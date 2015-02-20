/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyEvent                                  Pattern  : Association Class                   *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording act/property association.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
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

    internal TractIndexItem(Resource resource, RecordingAct recordingAct) {
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(recordingAct, "recordingAct");

      this.Resource = resource;
      this.RecordingAct = recordingAct;
    }

    static public TractIndexItem Parse(int id) {
      return BaseObject.ParseId<TractIndexItem>(id);
    }

    static public TractIndexItem Empty {
      get {
        return BaseObject.ParseEmpty<TractIndexItem>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyId", Default = "Empiria.Land.Registration.Property.Empty", IsOptional = false)]
    public Resource Resource {
      get;
      private set;
    }

    [DataField("RecordingActId", Default = "Empiria.Land.Registration.InformationAct.Empty", IsOptional = false)]
    public RecordingAct RecordingAct {
      get;
      private set;
    }

    public TractIndexItemExtData ExtensionData {
      get;
      private set;
    }

    //[DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    //[DataField("PostingTime", Default = "DateTime.Now")]
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
          "Property", this.Resource.Id, "ExtensionData", this.ExtensionData.ToJson(),
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

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
    }

    protected override void OnInitialize() {
      this.ExtensionData = new TractIndexItemExtData();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = TractIndexItemExtData.Parse((string) row["TractItemExtData"]);
    }

    protected override void OnSave() {
      if (this.Resource.IsNew) {
        this.Resource.Save();
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
