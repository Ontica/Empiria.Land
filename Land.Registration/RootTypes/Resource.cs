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
using System.Data;

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

    static public Resource TryParseWithUID(string propertyUID, bool reload = false) {
      DataRow row = ResourceData.GetResourceWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Resource>(row, reload);
      } else {
        return null;
      }
    }

    static public Resource Empty {
      get {
        return RealEstate.Empty;
      }
    }

    static public bool IsCancelationRole(ResourceRole resourceRole) {
      return (resourceRole == ResourceRole.Canceled ||
              resourceRole == ResourceRole.MergedInto ||
              resourceRole == ResourceRole.Split);
    }

    static public bool IsCreationalRole(ResourceRole resourceRole) {
      return (resourceRole == ResourceRole.Created ||
              resourceRole == ResourceRole.DivisionOf ||
              resourceRole == ResourceRole.Extended ||
              resourceRole == ResourceRole.PartitionOf);
    }

    static public bool IsInformativeRole(ResourceRole resourceRole) {
      return (resourceRole == ResourceRole.Edited ||
              resourceRole == ResourceRole.Informative);
    }

    static public bool IsResourceEditingRole(ResourceRole resourceRole) {
      return (IsCreationalRole(resourceRole) || resourceRole == ResourceRole.Edited);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("PropertyUID", IsOptional = false)]
    private string _propertyUID = String.Empty;

    public override string UID {
      get {
        return _propertyUID;
      }
    }

    internal protected virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID);
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

    public ResourceTract Tract {
      get {
        return ResourceTract.Parse(this);
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

    public bool IsCompleted {
      get {
        return (this.Status == RecordableObjectStatus.Registered ||
                this.Status == RecordableObjectStatus.Closed ||
                this.HasCompleteInformation());
      }
    }

    #endregion Public properties

    #region Public methods

    public bool AllowHistoricChanges() {
      return (!this.Tract.FirstRecordingAct.PhysicalRecording.IsEmptyInstance);
    }

    public virtual void AssertCanBeClosed() {

    }

    public void AssertCanBeAddedTo(RecordingDocument document, RecordingActType newRecordingActType) {
      this.AssertIsLastInPrelationOrder(document, newRecordingActType);
      this.AssertChainedRecordingAct(document, newRecordingActType);
    }

    private void AssertChainedRecordingAct(RecordingDocument document, RecordingActType newRecordingActType) {
      if (document.IssueDate < DateTime.Parse("2014-01-01") ||
          document.PresentationTime < DateTime.Parse("2016-01-01")) {
        return;
      }

      var chainedRecordingActType = newRecordingActType.RecordingRule.ChainedRecordingActType;
      if (chainedRecordingActType.Equals(RecordingActType.Empty)) {
        return;
      }

      // Lookup the tract for the last chained act
      var lastChainedActInstance = this.Tract.TryGetLastActiveChainedAct(chainedRecordingActType,
                                                                         document);
      // If exists an active chained act, then the assertion meets
      if (lastChainedActInstance != null) {
        return;
      }

      // Try to assert that the act is in the very first recorded document
      var tract = this.Tract.GetClosedRecordingActsUntil(document, true);

      // First check no real estates
      if (!(this is RealEstate) &&
          (tract.Count == 0 || tract[0].Document.Equals(document))) {
        return;
      }

      // For real estates, this rule apply for new no-partitions
      if (this is RealEstate && !((RealEstate) this).IsPartition &&
          (tract.Count == 0 || tract[0].Document.Equals(document))) {
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
        foreach (RecordingAct recordingAct in document.RecordingActs) {
          if (recordingAct.Equals(this)) {
            break;
          }
          if (recordingAct.Resource is RealEstate &&
              ((RealEstate) recordingAct.Resource).IsPartitionOf.Equals(this) &&
              recordingAct.RecordingActType.RecordingRule.ChainedRecordingActType.Equals(chainedRecordingActType)) {
              recordingAct.AssertChainedRecordingAct();
            return;
          }
        }
      }

      Assertion.AssertFail("El acto jurídico {0} no puede ser inscrito debido a que el folio real '{1}' " +
                            "no tiene registrado previamente un acto de: '{2}'.\n\n" +
                            "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                            "Favor de revisar la historia del folio real involucrado. Es posible que el trámite donde " +
                            "viene el acto faltante aún no haya sido procesado o que el documento esté abierto.",
                            newRecordingActType.DisplayName, this.UID, chainedRecordingActType.DisplayName);
    }

    public void AssertIsLastInPrelationOrder(RecordingDocument document, RecordingActType newRecordingActType) {
      // Cancelation acts don't follow prelation rules
      if (newRecordingActType.IsCancelationActType) {
        return;
      }

      var fullTract = this.Tract.GetRecordingActs();

      fullTract = fullTract.FindAll((x) => !x.RecordingActType.RecordingRule.SkipPrelation);

      var wrongPrelation = fullTract.Contains((x) => x.Document.PresentationTime > document.PresentationTime &&
                                                     x.Document.IsClosed);

      if (wrongPrelation) {
        Assertion.AssertFail("El folio real '{0}' tiene registrado cuando menos otro acto jurídico " +
                             "con una prelación posterior a la fecha de presentación de este documento.\n\n" +
                             "Por lo anterior, no es posible agregarlo en este documento.\n\n" +
                             "Favor de revisar la historia del predio.", this.UID);
      }
    }

    internal void AssertIsStillAlive(RecordingDocument document) {
      Assertion.Assert(this.Status != RecordableObjectStatus.Deleted,
                       "El folio real '{0}' está marcado como eliminado.", this.UID);

      var tract = this.Tract.GetRecordingActs();
      if (0 != tract.CountAll((x) => x.RecordingActType.RecordingRule.IsEndingAct &&
                                     x.Document.PresentationTime < document.PresentationTime)) {
        Assertion.AssertFail("El folio real '{0}' ya fue cancelado, fusionado o dividido en su totalidad. " +
                             "Ya no es posible agregarlo en este documento.", this.UID);
      }
    }

    abstract protected string GenerateResourceUID();

    protected override void OnBeforeSave() {
      if (this.IsNew) {
        this.AssignUID();
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      Assertion.Assert(this.UID.Length != 0, "Property UniqueIdentifier can't be an empty string.");
    }

    protected override void OnSave() {
      Assertion.AssertNoReachThisCode();
    }

    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return FormerCryptographer.CreateHashCode(this.Id.ToString("00000000"), this.UID)
                                  .Substring(0, 8)
                                  .ToUpperInvariant();
      } else {
        return String.Empty;
      }
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
      Assertion.Assert(this._propertyUID.Length == 0, "Property has already assigned a UniqueIdentifier.");

      while (true) {
        string temp = this.GenerateResourceUID();
        if (!ResourceData.ExistsResourceUID(temp)) {
          this._propertyUID = temp;
          break;
        }
      } // while
      Assertion.Assert(this._propertyUID.Length != 0, "Property UniqueIdentifier has not been generated.");
    }

    private bool HasCompleteInformation() {
      return false;
    }

    #endregion Private methods

  }  // abstract class Resource

} // namespace Empiria.Land.Registration
