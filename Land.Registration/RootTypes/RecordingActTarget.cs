/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActTarget                             Pattern  : Association Class                   *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act with another entity that can be a resource (property or        *
*              association), a document, a party (person or organization) or another recording act.          *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Application of a recording act with another entity that can be a resource (property
  /// or association), a document, a party (person or organization) or another recording act.</summary>
  public abstract class RecordingActTarget : BaseObject, IExtensible<RecordingActTargetExtData>, IProtected {

    #region Constructors and parsers

    protected RecordingActTarget() {
      // Required by Empiria Framework.
    }

    protected RecordingActTarget(RecordingAct recordingAct) {
      Assertion.AssertObject(recordingAct, "recordingAct");
      Assertion.Assert(!recordingAct.IsEmptyInstance, "recordingAct can't be the empty instance");

      this.RecordingAct = recordingAct;
    }

    static public RecordingActTarget Parse(int id) {
      return BaseObject.ParseId<RecordingActTarget>(id);
    }

    static public RecordingActTarget Empty {
      get {
        return BaseObject.ParseEmpty<RecordingActTarget>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("RecordingActId", Default = "Empiria.Land.Registration.InformationAct.Empty")]
    public RecordingAct RecordingAct {
      get;
      private set;
    }

    [DataField("TargetRecordingActId", Default = "Empiria.Land.Registration.InformationAct.Empty")]
    public RecordingAct TargetAct {
      get;
      private set;
    }

    public RecordingActTargetExtData ExtensionData {
      get;
      private set;
    }

    [DataField("RecordingActTargetStatus", Default = RecordableObjectStatus.Pending)]
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
    }

    protected override void OnInitialize() {
      this.ExtensionData = new RecordingActTargetExtData();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtensionData = RecordingActTargetExtData.Parse((string) row["RecordingActTargetExtData"]);
    }

    #endregion Public methods

  } // class RecordingActTarget

} // namespace Empiria.Land.Registration
