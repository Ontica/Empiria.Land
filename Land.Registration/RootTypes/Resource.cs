﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Resource                                       Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract type that represents a registrable resource. Typically a real estate property.       *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Registration.Data;

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

    static public Resource TryParseWithUID(string propertyUID) {
      DataRow row = ResourceData.GetResourceWithUID(propertyUID);

      if (row != null) {
        return BaseObject.ParseDataRow<Resource>(row);
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
    public string UID {
      get;
      private set;
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

    public FixedList<RecordingAct> Annotations {
      get {
        throw new NotImplementedException();
        //return RecordingBooksData.GetPropertyAnnotationList(this);
      }
    }

    public RecordingAct FirstRecordingAct {
      get {
        FixedList<RecordingAct> recordingActs = this.GetRecordingActsTract();
        if (recordingActs.Count != 0) {
          return recordingActs[0];
        } else {
          throw new LandRegistrationException(LandRegistrationException.Msg.PropertyDoesNotHaveAnyRecordingActs,
                                              this.UID);
        }
      }
    }

    public RecordingAct LastRecordingAct {
      get {
        FixedList<RecordingAct> domainActs = this.GetRecordingActsTract();
        if (domainActs.Count != 0) {
          return domainActs[domainActs.Count - 1];
        } else {
          return RecordingAct.Empty;
        }
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
      return (!this.FirstRecordingAct.PhysicalRecording.IsEmptyInstance);
    }

    public virtual void AssertCanBeClosed() {

    }

    public void AssertCanBeAddedTo(RecordingDocument document, RecordingActType newRecordingActType) {
      this.AssertIsLastInPrelationOrder(document);
      this.AssertChainedRecordingAct(document, newRecordingActType);
    }

    private void AssertChainedRecordingAct(RecordingDocument document, RecordingActType newRecordingActType) {
      if (document.IssueDate < DateTime.Parse("2014-01-01") ||
          document.PresentationTime < DateTime.Parse("2016-01-01")) {
        return;
      }

      var chainedRecordingAct = newRecordingActType.RecordingRule.ChainedRecordingActType;

      if (chainedRecordingAct.Equals(RecordingActType.Empty)) {
        return;
      }


      var tract = this.GetRecordingActsTract();

      // This rule doesn't apply to new registered resources
      if (tract.Count == 1 && this is RealEstate && !((RealEstate) this).IsPartition) {
        return;
      }

      var lastAct = tract.FindLast((x) => (x.WasAliveOn(document.PresentationTime) &&
                                           !x.RecordingActType.RecordingRule.IsAnnotation &&
                                           !x.RecordingActType.IsCancelationActType &&
                                           !x.RecordingActType.IsModificationActType &&
                                           x.Document.PresentationTime < document.PresentationTime &&
                                           x.Document.IsClosed
                                           ));

      if (lastAct == null || !lastAct.RecordingActType.Equals(chainedRecordingAct)) {
        Assertion.AssertFail("El acto jurídico {0} no puede ser inscrito debido a que el folio real '{1}' " +
                             "no tiene registrado previamente un acto de: '{2}'.\n\n" +
                             "Por lo anterior, esta operación no puede ser ejecutada.\n\n" +
                             "Favor de revisar la historia del folio real involucrado. Es posible que el trámite donde " +
                             "viene el acto faltante aún no haya sido procesado o que el documento esté abierto.",
                             newRecordingActType.DisplayName, this.UID, chainedRecordingAct.DisplayName);
      }
    }

    public void AssertIsLastInPrelationOrder(RecordingDocument document) {
      var fullTract = this.GetRecordingActsTract();

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

      var tract = this.GetRecordingActsTract();
      if (0 != tract.CountAll((x) => x.RecordingActType.RecordingRule.IsEndingAct &&
                                     x.Document.PresentationTime < document.PresentationTime)) {
        Assertion.AssertFail("El folio real '{0}' ya fue cancelado, fusionado o dividido en su totalidad. " +
                             "Ya no es posible agregarlo en este documento.", this.UID);
      }
    }

    abstract protected string GenerateResourceUID();

    public bool IsFirstRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.IsAnnotation) {
        return false;
      }

      RecordingAct firstRecordingAct = this.FirstRecordingAct;
      if (firstRecordingAct != RecordingAct.Empty) {
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
      if (lastRecordingAct != RecordingAct.Empty) {
        return lastRecordingAct.Equals(recordingAct);
      } else {
        return true;
      }
    }

    public FixedList<Certification.Certificate> GetEmittedCerificates() {
      return Empiria.Land.Data.CertificatesData.ResourceEmittedCertificates(this);
    }

    public FixedList<RecordingAct> GetFullRecordingActsTract() {
      return RecordingActsData.GetResourceFullTractIndex(this);
    }

    public FixedList<IResourceTractItem> GetFullRecordingActsTractWithCertificates() {
      return RecordingActsData.GetResourceFullTractIndexWithCertificates(this);
    }

    public FixedList<RecordingAct> GetRecordingActsTract() {
      return RecordingActsData.GetResourceRecordingActList(this);
    }

    public FixedList<RecordingAct> GetRecordingActsTractUntil(RecordingAct breakAct,
                                                              bool includeBreakAct) {
      return RecordingActsData.GetResourceRecordingActListUntil(this, breakAct, includeBreakAct);
    }

    public RecordingAct GetRecordingAntecedent() {
      return this.GetRecordingAntecedent(RecordingAct.Empty, false);
    }

    public RecordingAct GetRecordingAntecedent(DateTime presentationTime) {
      FixedList<RecordingAct> tract = this.GetRecordingActsTract();

      if (tract.Count == 0) {
        return RecordingAct.Empty;
      }

      /// For no real estate, return always the first act that is the creational act.
      /// Resources different than real estates don't have domain or structure acts.
      if (!(this is RealEstate)) {
        return tract[0];
      }

      /// Returns the last domain or structure act if founded, otherwise
      /// return the very first act of the real estate.
      var antecedent = tract.FindLast((x) => x.Document.PresentationTime <= presentationTime &&
                                      (x.RecordingActType.IsDomainActType || x.RecordingActType.IsStructureActType));

      if (antecedent != null) {
        return antecedent;
      } else {
        return tract[0];
      }
    }

    public RecordingAct GetRecordingAntecedent(RecordingAct beforeRecordingAct,
                                               bool returnAmendmentActs = false) {
      /// For amendment acts, this method returns the amendmentOf act
      /// when returnAmendmentActs flag is true
      if (returnAmendmentActs && beforeRecordingAct.RecordingActType.IsAmendmentActType) {
        return beforeRecordingAct.AmendmentOf;
      }

      var tract = this.GetRecordingActsTractUntil(beforeRecordingAct, false);

      /// Returns the empty recording act if there are not antecedents.
      if (tract.Count == 0) {

        if (!(this is RealEstate)) {
          return RecordingAct.Empty;
        }

        /// If no antecedent, then look if the real estate is a new partition.
        /// If it is then return the antecedent of the partitioned or parent real estate.
        var parentRealEstate = ((RealEstate) this).IsPartitionOf;
        if (!parentRealEstate.IsEmptyInstance) {
          return parentRealEstate.GetRecordingAntecedent(beforeRecordingAct.Document.PresentationTime);
        } else {
          return RecordingAct.Empty;
        }

      }

      if (this is RealEstate) {
        /// Returns the last domain or structure act if founded, otherwise
        /// return the very first act of the real estate.

        return tract.FindLast((x) => x.RecordingActType.IsDomainActType ||
                                     x.RecordingActType.IsStructureActType) ?? tract[0];
      } else {
        /// For no real estate, return always the first act that is the creational act.
        /// Resources different than real estates don't have domain or structure acts.
        return tract[0];
      }
    }

    public bool IsFirstAct(RecordingAct recordingAct) {
      return this.FirstRecordingAct.Equals(recordingAct);
    }

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

    internal void TryDelete() {
      var tract = this.GetRecordingActsTract();
      if (tract.Count == 0) {
        this.Status = RecordableObjectStatus.Deleted;
        this.Save();
      }
    }

    #endregion Public methods

    #region Private methods

    private void AssignUID() {
      Assertion.Assert(this.UID.Length == 0, "Property has already assigned a UniqueIdentifier.");

      while (true) {
        string temp = this.GenerateResourceUID();
        if (!ResourceData.ExistsResourceUID(temp)) {
          this.UID = temp;
          break;
        }
      } // while
      Assertion.Assert(this.UID.Length != 0, "Property UniqueIdentifier has not been generated.");
    }

    private bool HasCompleteInformation() {
      return false;
    }

    #endregion Private methods

  }  // abstract class Resource

} // namespace Empiria.Land.Registration
