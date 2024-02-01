/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Resource                                       Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract type that represents a registrable resource. Typically a real estate property.       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Abstract type that represents a registrable resource. Typically a real estate property.</summary>
  abstract public class Resource : BaseObject, IProtected {

    #region Constructors and parsers

    protected Resource() {
      // Required by Empiria Framework.
    }


    static public Resource Parse(int id) {
      return BaseObject.ParseId<Resource>(id);
    }


    static public Resource Parse(int id, bool reload) {
      return BaseObject.ParseId<Resource>(id, reload);
    }


    static public Resource ParseGuid(string guid) {
      var resource = BaseObject.TryParse<Resource>($"PropertyGUID = '{guid}'");

      Assertion.Require(resource,
                             $"There is not registered a recordable subject resource with guid {guid}.");

      return resource;
    }


    static public FixedList<Resource> GetList(string filter, string orderBy, int pageSize) {
      return ResourceData.SearchResources(filter, orderBy, pageSize);
    }


    static public Resource TryParseWithUID(string propertyUID, bool reload = false) {
      Resource resource = ResourceData.TryGetResourceWithUID(propertyUID);

      if (resource == null) {
        return null;
      }

      if (reload) {
        return BaseObject.ParseId<Resource>(resource.Id, true);
      } else {
        return resource;
      }
    }


    static public bool MatchesWithUID(string resourceUID) {
      return (resourceUID.Length == 19 &&
             resourceUID.StartsWith("FR-ZS") &&
             resourceUID.Contains("-"));
    }


    static public Resource Empty {
      get {
        return RealEstate.Empty;
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyGUID", IsOptional = false)]
    public string GUID {
      get;
      private set;
    }


    [DataField("PropertyUID", IsOptional = false)]
    private string _propertyUID = String.Empty;

    public override string UID {
      get {
        return _propertyUID;
      }
    }


    [DataField("PropertyName")]
    public string Name {
      get;
      set;
    } = string.Empty;


    [DataField("PropertyDescription")]
    public string Description {
      get;
      set;
    }


    public virtual string AsText {
      get {
        if (this.Name.Length == 0 && this.Description.Length == 0) {
          return "Información disponible únicamente en documentos físicos.";
        }
        return this.Name.Length != 0 ? this.Name : this.Description;
      }
    }


    [DataField("PropertyKind")]
    public string Kind {
      get;
      set;
    } = string.Empty;


    [DataField("RecorderOfficeId")]
    public RecorderOffice RecorderOffice {
      get;
      set;
    } = RecorderOffice.Empty;


    internal protected virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID, this.Kind, this.Name,
                                           this.Description, this.RecorderOffice.ShortName);
      }
    }


    [DataField("PostedById")]
    private LazyInstance<Contact> _postedBy = LazyInstance<Contact>.Empty;
    public Contact PostedBy {
      get {
        return _postedBy.Value;
      }
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
      private set;
    }


    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "PropertyType", this.GetEmpiriaType().Id, "UID", this.UID, "PostedBy",
          this.PostedBy.Id, "PostingTime", this.PostingTime, "Status", (char) this.Status,
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

    public RecordableSubjectTract Tract {
      get {
        return RecordableSubjectTract.Parse(this);
      }
    }


    public bool IsCompleted {
      get {
        return (this.Status == RecordableObjectStatus.Registered ||
                this.Status == RecordableObjectStatus.Closed ||
                this.HasCompleteInformation());
      }
    }


    #endregion Public properties

    #region Public methods


    public virtual void AssertCanBeClosed() {

    }


    public void AssertCanBeAddedTo(RecordingDocument landRecord, RecordingActType newRecordingActType) {
      this.AssertIsLastInPrelationOrder(landRecord, newRecordingActType);
      this.AssertChainedRecordingAct(landRecord, newRecordingActType);
    }


    private void AssertChainedRecordingAct(RecordingDocument landRecord, RecordingActType newRecordingActType) {
      if (landRecord.IssueDate < DateTime.Parse("2014-01-01") ||
          landRecord.PresentationTime < DateTime.Parse("2016-01-01")) {
        return;
      }

      var chainedRecordingActType = newRecordingActType.RecordingRule.ChainedRecordingActType;

      if (chainedRecordingActType.Equals(RecordingActType.Empty)) {
        return;
      }

      // Lookup the tract for the last chained act
      var lastChainedActInstance = this.Tract.TryGetLastActiveChainedAct(chainedRecordingActType,
                                                                         landRecord);
      // If exists an active chained act, then the assertion meets
      if (lastChainedActInstance != null) {
        return;
      }

      // Try to assert that the act is in the very first land record
      var tract = this.Tract.GetClosedRecordingActsUntil(landRecord, true);

      // First check no real estates
      if (!(this is RealEstate) &&
          (tract.Count == 0 || tract[0].LandRecord.Equals(landRecord))) {
        return;
      }

      // For real estates, this rule apply for new no-partitions
      if (this is RealEstate && !((RealEstate) this).IsPartition &&
          (tract.Count == 0 || tract[0].LandRecord.Equals(landRecord))) {
        return;
      }

      // When the chained act rule applies to a modification act, then lookup in this
      // recorded document for an act applied to a partition of this real estate
      // with the same ChainedRecordingActType, if it is found then the assertion meets.
      // Ejemplo: Tanto CV como Rectificación de medidas requieren aviso preventivo.
      // Si en el documento hay una CV sobre una fracción F de P, y también hay una
      // rectificación de medidas del predio P, entonces basta con que el aviso preventivo
      // exista para la fraccion F de P.
      if (this is RealEstate && newRecordingActType.IsModificationActType) {
        foreach (RecordingAct recordingAct in landRecord.RecordingActs) {
          if (recordingAct.Equals(this)) {
            break;
          }
          if (recordingAct.Resource is RealEstate &&
              ((RealEstate) recordingAct.Resource).IsPartitionOf.Equals(this) &&
              recordingAct.RecordingActType.RecordingRule.ChainedRecordingActType.Equals(chainedRecordingActType)) {
              recordingAct.Validator.AssertChainedRecordingAct();
            return;
          }
        }
      }

      Assertion.RequireFail(
        $"El acto jurídico {newRecordingActType.DisplayName} no puede ser inscrito debido a que el folio real '{this.UID}' " +
        $"no tiene registrado previamente un acto de: '{chainedRecordingActType.DisplayName}'.\n\n" +
        "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
        "Favor de revisar la historia del folio real involucrado. Es posible que el trámite donde " +
        "viene el acto faltante aún no haya sido procesado o que el documento esté abierto.");
    }


    public void AssertIsLastInPrelationOrder(RecordingDocument landRecord, RecordingActType newRecordingActType) {
      // Cancelation acts don't follow prelation rules
      if (newRecordingActType.IsCancelationActType) {
        return;
      }

      var fullTract = this.Tract.GetRecordingActs();

      fullTract = fullTract.FindAll((x) => !x.RecordingActType.RecordingRule.SkipPrelation);

      var wrongPrelation = fullTract.Contains((x) => x.LandRecord.PresentationTime > landRecord.PresentationTime &&
                                                     x.LandRecord.IsClosed);

      //if (wrongPrelation) {
      //  Assertion.AssertFail("El folio real '{0}' tiene registrado cuando menos otro acto jurídico " +
      //                       "con una prelación posterior a la fecha de presentación de este documento.\n\n" +
      //                       "Por lo anterior, no es posible agregarlo en este documento.\n\n" +
      //                       "Favor de revisar la historia del predio.", this.UID);
      //}
    }


    internal void AssertIsStillAlive(DateTime presentationTime) {
      Assertion.Require(this.Status != RecordableObjectStatus.Deleted,
                       $"El folio real '{this.UID}' está marcado como eliminado.");

      var tract = this.Tract.GetRecordingActs();
      if (0 != tract.CountAll((x) => x.RecordingActType.RecordingRule.IsEndingAct &&
                                     x.LandRecord.PresentationTime < presentationTime)) {
        Assertion.RequireFail($"El folio real '{this.UID}' ya fue cancelado, fusionado o dividido en su totalidad. " +
                             "Ya no es posible agregarlo en este documento.");
      }
    }


    abstract protected string GenerateResourceUID();

    abstract public ResourceShapshotData GetSnapshotData();

    protected override void OnBeforeSave() {
      if (this.IsNew) {
        this.GUID = Guid.NewGuid().ToString().ToLower();
        this.AssignUID();
        this.PostedBy = ExecutionServer.CurrentContact;
        this.PostingTime = DateTime.Now;
        EnsureRecorderOfficeIsAssigned();
      }

      Assertion.Ensure(this.UID.Length != 0,
                       "Property UniqueIdentifier can't be an empty string.");
    }


    protected override void OnSave() {
      Assertion.EnsureNoReachThisCode();
    }


    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return Cryptographer.CreateHashCode(this.Id.ToString("00000000"), this.UID)
                            .Substring(0, 8)
                            .ToUpperInvariant();
      } else {
        return String.Empty;
      }
    }

    public void SetStatus(RecordableObjectStatus status) {
      this.Status = status;
    }

    internal void TryDelete() {
      var tract = this.Tract.GetRecordingActs();
      if (tract.Count == 0) {
        this.Status = RecordableObjectStatus.Deleted;
        this.Save();
      }
    }

    #endregion Public methods

    #region Private methods

    private void AssignUID() {
      Assertion.Require(this._propertyUID.Length == 0,
                        "Property has already assigned a UniqueIdentifier.");

      while (true) {
        string temp = this.GenerateResourceUID();
        if (!ResourceData.ExistsResourceUID(temp)) {
          this._propertyUID = temp;
          break;
        }
      } // while

      Assertion.Ensure(this._propertyUID.Length != 0,
                      "Property UniqueIdentifier has not been generated.");
    }

    private bool HasCompleteInformation() {
      return false;
    }


    private void EnsureRecorderOfficeIsAssigned() {
      if (!this.RecorderOffice.IsEmptyInstance) {
        return;
      }
      this.RecorderOffice = Permissions.GetUserRecorderOffice();
    }

    #endregion Private methods

  }  // abstract class Resource

} // namespace Empiria.Land.Registration
