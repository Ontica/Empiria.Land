/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : TractItem                                      Pattern  : Association Class                   *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act with another entity that can be a resource (property or        *
*              association), a document, a party (person or organization) or another recording act.          *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Application of a recording act with another entity that can be a resource (property
  /// or association), a document, a party (person or organization) or another recording act.</summary>
  public class TractItem : BaseObject, IExtensible<TractItemExtData>, IProtected {

    #region Constructors and parsers

    protected TractItem() {
      // Required by Empiria Framework.
    }

    public TractItem(RecordingAct recordingAct, Resource resource,
                     ResourceRole resourceRole = ResourceRole.Informative,
                     decimal recordingActPercentage = 1.0m) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.Assert(!recordingAct.IsEmptyInstance, "recordingAct can't be the empty instance.");
      Assertion.AssertObject(resource, "resource");
      Assertion.Assert(!resource.IsEmptyInstance, "resource can't be the empty instance.");
      Assertion.Assert(decimal.Zero < recordingActPercentage && recordingActPercentage <= decimal.One,
        "Recording act percentage must be a number greater than zero and less or equal than one.");

      this.RecordingAct = recordingAct;
      this.Resource = resource;
      this.ResourceRole = resourceRole;
    }

    static public TractItem Empty {
      get {
        return BaseObject.ParseEmpty<TractItem>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("RecordingActId")]
    public RecordingAct RecordingAct {
      get;
      private set;
    }

    [DataField("ResourceId", Default = "Empiria.Land.Registration.RealEstate.Empty")]
    public Resource Resource {
      get;
      private set;
    }

    [DataField("ResourceRole", Default = ResourceRole.Informative)]
    public ResourceRole ResourceRole {
      get;
      private set;
    }

    [DataField("RecordingActPercentage", Default = 1.0)]
    public decimal RecordingActPercentage {
      get;
      private set;
    }

    [DataField("LastAmendedById")]
    public RecordingAct LastAmendedBy {
      get;
      private set;
    }

    public bool WasCanceled {
      get {
        return this.LastAmendedBy.RecordingActType.IsCancelationActType;
      }
    }

    public bool WasModified {
      get {
        return this.LastAmendedBy.RecordingActType.IsModificationActType;
      }
    }

    public TractItemExtData ExtensionData {
      get;
      protected set;
    }

    [DataField("RegisteredById")]
    public Contact RegisteredBy {
      get;
      private set;
    }

    [DataField("TractItemStatus", Default = RecordableObjectStatus.Pending)]
    public RecordableObjectStatus Status {
      get;
      private set;
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
          "ExtensionData", this.ExtensionData.ToJson(),
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

    internal virtual void Delete() {
      this.Status = RecordableObjectStatus.Deleted;
      base.Save();
      this.Resource.TryDelete();
    }

    protected override void OnInitialize() {
      this.ExtensionData = new TractItemExtData();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = TractItemExtData.Parse((string) row["TractItemExtData"]);
    }

    protected override void OnSave() {
      if (this.Resource.IsNew) {
        this.RegisteredBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.Resource.Save();
      }
      RecordingActsData.WriteTractItem(this);
    }

    #endregion Public methods

  } // class TractItem

} // namespace Empiria.Land.Registration
