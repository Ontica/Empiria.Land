/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderExpert                                 Pattern  : Standard Class                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Performs the registry of recording acts based on a supplied recording task                    *
*              and a set of rules defined for each recording act type.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Performs the registry of recording acts based on a supplied recording task
  ///  and a set of rules defined for each recording act type.</summary>
  public class RecorderExpert {

    #region Constructors and parsers

    public RecorderExpert(RecordingTask task) {
      this.Task = task;
    }

    static public RecordingAct[] Execute(RecordingTask task) {
      Assertion.AssertObject(task, "task");

      var expert = new RecorderExpert(task);

      expert.AssertValidTask();

      return expert.ProcessTask();
    }

    #endregion Constructors and parsers

    #region Properties

    private bool AppliesOverNewPartition {
      get {
        return (this.Task.RecordingTaskType == RecordingTaskType.createPartition);
      }
    }

    private bool CreateNewResource {
      get {
        return ((this.Task.RecordingTaskType == RecordingTaskType.actAppliesToDocument ||
                 Task.RecordingTaskType == RecordingTaskType.createProperty) &&
                 Task.PrecedentProperty.IsEmptyInstance);
      }
    }

    private bool CreateResourceOnNewPhysicalRecording {
      get {
        return ((Task.RecordingTaskType != RecordingTaskType.createProperty) &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                Task.PrecedentRecording.IsEmptyInstance &&
                Task.QuickAddRecordingNumber != String.Empty);
      }
    }

    private bool CreateResourceOnExistingPhysicalRecording {
      get {
        return ((Task.RecordingTaskType != RecordingTaskType.createProperty) &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                !Task.PrecedentRecording.IsEmptyInstance &&
                Task.PrecedentProperty.IsNew);
      }
    }

    public RecordingTask Task {
      get;
      private set;
    }

    #endregion Properties

    #region Public methods

    public void AssertValidTask() {
      if (!Task.PrecedentProperty.IsEmptyInstance) {

        this.AssertIsApplicableResource(Task.PrecedentProperty);

        Task.PrecedentProperty.AssertIsStillAlive();

        if (TlaxcalaOperationalCondition(Task.Document)) {
          return;
        }

        if (this.AppliesOverNewPartition && Task.RecordingActType.RecordingRule.HasChainedRule) {
          var msg = "Este acto no puede aplicar a una nueva fracción ya que requiere " +
                    "previamente un acto de " + Task.RecordingActType.RecordingRule.ChainedRecordingActType.DisplayName + ".";
          Assertion.AssertFail(msg);
        }
        Task.PrecedentProperty.AssertCanBeAddedTo(Task.Document, Task.RecordingActType);
      }

      if ((this.CreateResourceOnNewPhysicalRecording || this.CreateResourceOnExistingPhysicalRecording) &&
           Task.RecordingActType.RecordingRule.HasChainedRule) {
        if (TlaxcalaOperationalCondition(Task.Document)) {
          return;
        }
        var msg = "Este acto no puede aplicar a una nueva fracción ya que requiere " +
                    "previamente un acto de " + Task.RecordingActType.RecordingRule.ChainedRecordingActType.DisplayName + ".\n\n" +
                    "Sabemos que es posible que dicho acto se encuentre registrado en la partida, pero el sistema no tiene esa información.\n\n" +
                    "Si este es el caso, favor de agregar primero el acto que falta en este documento aclarando dicho asunto en las observaciones.";
          Assertion.AssertFail(msg);
      }
      string sMsg = String.Empty;
      if (CreateResourceOnExistingPhysicalRecording) {
        throw new NotImplementedException();
      }
      if (CreateResourceOnNewPhysicalRecording &&
          Task.PrecedentRecordingBook.ExistsRecording(Task.QuickAddRecordingNumber)) {
        sMsg = "La partida indicada ya existe en el libro seleccionado,\n" +
                "y no es posible generar más de un folio de predio\n" +
                "en una misma partida o antecedente.\n\n" +
                "Si se requiere registrar más de un predio en una partida,\n" +
                "favor de consultarlo con el área de soporte. Gracias.";

        throw new NotImplementedException(sMsg);
      }
    }

    #endregion Public methods

    #region Recording methods

    private void AssertIsApplicableResource(Resource resourceToApply) {
      Assertion.AssertObject(resourceToApply, "resourceToApply");

      switch (Task.RecordingActType.RecordingRule.AppliesTo) {
        case RecordingRuleApplication.Association:
          Assertion.Assert(resourceToApply is Association,
            "Este acto sólo es aplicable a asociaciones. El folio real corresponde a un predio.");
          return;
        case RecordingRuleApplication.RealEstate:
          Assertion.Assert(resourceToApply is RealEstate,
            "Este acto sólo es aplicable a predios. El folio real corresponde a una asociación.");
          return;
      }

    }

    private RecordingAct[] ProcessTask() {
      var recordingActType = this.Task.RecordingActType;

      if (recordingActType.IsDomainActType) {
        return this.CreateDomainAct();          // CV, Donación, Título de propiedad, Constitución SC,

      } else if (recordingActType.IsLimitationActType) {
        return this.CreateLimitationAct();     // Hipoteca, Embargo, Inmovilización, Aviso preventivo

      } else if (recordingActType.IsInformationActType) {
        return this.CreateInformationAct();     // Testamento, Cap matrim, Anotación marginal

      } else if (recordingActType.IsCancelationActType) {
        return this.CreateCancelationAct();

      } else if (recordingActType.IsModificationActType) {
        return this.CreateModificationAct();

      } else {
        throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
                                          Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
      }
    }

    private DomainAct[] CreateDomainAct() {
      // Cast because limitation acts supposed to be applicable only to real estates
      RealEstate[] realEstates = (RealEstate[]) this.GetResources();

      var domainActs = new DomainAct[realEstates.Length];
      for (int i = 0; i < realEstates.Length; i++) {
        domainActs[i] = new DomainAct(this.Task.RecordingActType,
                                      this.Task.Document, realEstates[i],
                                      this.Task.RecordingActPercentage);
        domainActs[i].Save();
      }
      return domainActs;
    }

    private InformationAct[] CreateInformationAct() {
      Resource[] resources = this.GetResources();

      var informationActs = new InformationAct[resources.Length];
      for (int i = 0; i < resources.Length; i++) {
        informationActs[i] = new InformationAct(this.Task.RecordingActType,
                                                this.Task.Document, resources[i]);
        informationActs[i].Save();
      }
      return informationActs;
    }

    private LimitationAct[] CreateLimitationAct() {
      // Cast because limitation acts supposed to be applicable only to real estates
      RealEstate[] realEstates = (RealEstate[]) this.GetResources();

      var recordingActs = new LimitationAct[realEstates.Length];
      for (int i = 0; i < realEstates.Length; i++) {
        recordingActs[i] = new LimitationAct(this.Task.RecordingActType,
                                             this.Task.Document, realEstates[i]);
        recordingActs[i].Save();
      }
      return recordingActs;
    }

    private CancelationAct[] CreateCancelationAct() {
      switch (this.Task.RecordingActType.AppliesTo) {
        case RecordingRuleApplication.RecordingAct:
          return CreateRecordingActCancelationAct();
        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          return this.CreateResourceCancelationAct();
        //case RecordingRuleApplication.Document:
        //  return this.CreateDocumentCancelationAct();
        case RecordingRuleApplication.Party:
          return this.CreatePartyCancelationAct();
        case RecordingRuleApplication.Structure:
          return this.CreateStructureCancelationAct();

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private ModificationAct[] CreateModificationAct() {
      switch (this.Task.RecordingActType.AppliesTo) {
        case RecordingRuleApplication.RecordingAct:
          return CreateRecordingActModificationAct();
        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          return this.CreateResourceModificationAct();
        //case RecordingRuleApplication.Document:
        //  return this.CreateDocumentModificationAct();
        case RecordingRuleApplication.Party:
          return this.CreatePartyModificationAct();
        case RecordingRuleApplication.Structure:
          return this.CreateStructureModificationAct();

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    #endregion Recording methods

    #region Cancelation methods

    private CancelationAct[] CreateDocumentCancelationAct() {
      throw new NotImplementedException();
    }

    private CancelationAct[] CreatePartyCancelationAct() {
      throw new NotImplementedException();
    }

    private CancelationAct[] CreateRecordingActCancelationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      return new[] { new CancelationAct(this.Task.RecordingActType,
                                        this.Task.Document, resource, targetAct) };
    }

    private CancelationAct[] CreateResourceCancelationAct() {
      var resource = this.GetOneResource();

      return new[] { new CancelationAct(this.Task.RecordingActType,
                                        this.Task.Document, resource) };
    }

    private CancelationAct[] CreateStructureCancelationAct() {
      throw new NotImplementedException();
    }

    private RecordingAct CreateTargetRecordingAct(Resource resource) {
      var document = new RecordingDocument(RecordingDocumentType.Empty);

      Recording recording = Task.TargetActInfo.PhysicalRecording;

      return document.AppendRecordingAct(Task.TargetActInfo.RecordingActType,
                                         resource, physicalRecording: recording);
    }

    private RecordingAct GetTargetRecordingAct(Resource resource) {
      if (this.Task.TargetActInfo.RecordingActId != -1) {
        return RecordingAct.Parse(this.Task.TargetActInfo.RecordingActId);
      } else {
        return this.CreateTargetRecordingAct(resource);
      }
    }

    private RecordingDocument GetTargetDocument() {
      throw new NotImplementedException();
    }

    #endregion Cancelation methods

    #region Modification methods

    private ModificationAct[] CreatePartyModificationAct() {
      throw new NotImplementedException();
    }

    private ModificationAct[] CreateRecordingActModificationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      return new[] { new ModificationAct(this.Task.RecordingActType,
                                         this.Task.Document, resource, targetAct) };
    }

    private ModificationAct[] CreateResourceModificationAct() {
      var resource = this.GetOneResource();

      return new[] { new ModificationAct(this.Task.RecordingActType,
                                         this.Task.Document, resource) };
    }

    private ModificationAct[] CreateStructureModificationAct() {
      throw new NotImplementedException();
    }


    #endregion Modification methods

    #region Get resources methods

    private Resource GetOneResource() {
      var resources = this.GetResources();

      Assertion.Assert(resources.Length == 1,
                      "Operation failed, too many resources returned by GetOneResource().");

      return resources[0];
    }

    private Resource[] GetResources() {
      RecordingRuleApplication appliesTo = this.Task.RecordingActType.RecordingRule.AppliesTo;

      if (appliesTo == RecordingRuleApplication.RecordingAct) {
        appliesTo = Task.TargetActInfo.RecordingActType.RecordingRule.AppliesTo;
      }
      switch (appliesTo) {
        case RecordingRuleApplication.Association:
          return this.GetAssociations();
        case RecordingRuleApplication.RealEstate:
          return this.GetRealEstates();
        case RecordingRuleApplication.NoProperty:
          return this.GetNoPropertyResources();
        default:
          throw Assertion.AssertNoReachThisCode(appliesTo + " application for " + this.Task.RecordingActType.DisplayName);
      }
    }

    // Don't call directly. Please use it only in GetResources()
    private Association[] GetAssociations() {
      if (this.CreateNewResource) {
        var newAssociationData = new AssociationExtData(Task.ResourceName);

        return new Association[] { new Association(newAssociationData) };
      } else if (this.CreateResourceOnNewPhysicalRecording) {
        var newAssociationData = new AssociationExtData(Task.ResourceName);

        var association = new Association(newAssociationData);

        this.AttachResourceToNewPhysicalRecording(association);
        return new Association[] { association };
      } else {
        return new Association[] { (Association) this.Task.PrecedentProperty };
      }
    }

    // Don't call directly. Please use it only in GetResources()
    private NoPropertyResource[] GetNoPropertyResources() {
      if (this.CreateNewResource) {
        return new NoPropertyResource[] { new NoPropertyResource() };

      } else if (this.CreateResourceOnNewPhysicalRecording) {
        var noPropertyResource = new NoPropertyResource();

        this.AttachResourceToNewPhysicalRecording(new NoPropertyResource());

        return new NoPropertyResource[] { noPropertyResource };
      } else {
        return new NoPropertyResource[] { (NoPropertyResource) this.Task.PrecedentProperty };
      }
    }

    // Call it only in GetResources()
    private RealEstate[] GetRealEstates() {
      if (this.CreateNewResource) {
        var data = new RealEstateExtData() { CadastralKey = Task.CadastralKey };

        return new RealEstate[] { new RealEstate(data) };
      }

      RealEstate property = null;

      if (this.CreateResourceOnNewPhysicalRecording) {
        var data = new RealEstateExtData() { CadastralKey = Task.CadastralKey };

        property = new RealEstate(data);
        this.AttachResourceToNewPhysicalRecording(property);
      } else if (this.CreateResourceOnExistingPhysicalRecording) {
        var data = new RealEstateExtData() { CadastralKey = Task.CadastralKey };

        property = new RealEstate(data);
        this.AttachResourceToExistingPhysicalRecording(property);
      } else {
        property = (RealEstate) this.Task.PrecedentProperty;
      }

      if (this.AppliesOverNewPartition) {
        return property.Subdivide(Task.PartitionInfo);
      } else {
        return new RealEstate[] { property };
      }
    }

    #endregion Get resources methods

    #region Physical recording methods

    private void AttachResourceToExistingPhysicalRecording(Resource resource) {
      Assertion.Assert(this.CreateResourceOnExistingPhysicalRecording,
                       "Wrong RecordingTask values to execute this method.");

      var document = Task.PrecedentRecording.Document;

      var precedentAct = new InformationAct(RecordingActType.Empty, document,
                                            resource, Task.PrecedentRecording);
      precedentAct.Save();
    }

    private Recording AttachResourceToNewPhysicalRecording(Resource resource) {
      Assertion.Assert(this.CreateResourceOnNewPhysicalRecording,
                       "Resource was already created on physical recording.");

      var document = new RecordingDocument(RecordingDocumentType.Empty);

      RecordingBook recordingBook = Task.PrecedentRecordingBook;

      Recording newPhysicalRecording = recordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber);
      var precedentAct = new InformationAct(RecordingActType.Empty, document,
                                            resource, newPhysicalRecording);
      precedentAct.Save();

      return newPhysicalRecording;
    }

    private bool TlaxcalaOperationalCondition(RecordingDocument document) {
      // Temporarily rule, based on Tlaxcala Recording Office operation
      if (document.PresentationTime < DateTime.Parse("2016-09-26") && DateTime.Today < DateTime.Parse("2016-10-03")) {
        return true;
      }
      return false;
    }

    #endregion Physical recording methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
