/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderExpert                                 Pattern  : Standard Class                      *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Performs the registry of recording acts based on a supplied recording task                    *
*              and a set of rules defined for each recording act type.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.Land.Registration {

  /// <summary>Performs the registry of recording acts based on a supplied recording task
  ///  and a set of rules defined for each recording act type.</summary>
  public class RecorderExpert {

    #region Constructors and parsers

    public RecorderExpert(RecordingTask task) {
      this.Task = task;
    }

    static public RecordingAct[] Execute(RecordingTask task) {
      Assertion.Require(task, nameof(task));

      var expert = new RecorderExpert(task);

      expert.AssertValidTask();

      return expert.ProcessTask();
    }

    #endregion Constructors and parsers

    #region Properties

    private bool AppliesOverNewPartition {
      get {
        return (this.Task.RecordingTaskType == RecordingTaskType.createPartition ||
                this.Task.RecordingTaskType == RecordingTaskType.createPartitionAndPropertyOnAntecedent);
      }
    }


    private bool CreateNewResource {
      get {
        return (Task.RecordingTaskType == RecordingTaskType.createProperty &&
                Task.PrecedentProperty.IsEmptyInstance);
      }
    }


    private bool CreateResourceOnBookEntry {
      get {
        return ((Task.RecordingTaskType == RecordingTaskType.createPropertyOnAntecedent ||
                 Task.RecordingTaskType == RecordingTaskType.createPartitionAndPropertyOnAntecedent) &&
                !Task.PrecedentBookEntry.IsEmptyInstance);
      }
    }


    private bool SelectResource {
      get {
        return (Task.RecordingTaskType == RecordingTaskType.selectProperty &&
                !Task.PrecedentProperty.IsEmptyInstance);
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

        Task.PrecedentProperty.AssertIsStillAlive(Task.LandRecord.PresentationTime);

        if (this.AppliesOverNewPartition && Task.RecordingActType.RecordingRule.HasChainedRule) {
          if (!OperationalCondition(Task.LandRecord)) {
            var msg = "Este acto no puede aplicarse a una nueva fracción ya que requiere " +
                      "previamente un acto de: '" +
                      Task.RecordingActType.RecordingRule.ChainedRecordingActType.DisplayName + "'.";
            Assertion.RequireFail(msg);
          }
        }
        Task.PrecedentProperty.AssertCanBeAddedTo(Task.LandRecord, Task.RecordingActType);
      }

      if ((this.CreateResourceOnBookEntry) &&
           Task.RecordingActType.RecordingRule.HasChainedRule) {
        if (OperationalCondition(Task.LandRecord)) {
          return;
        }
        var msg = "Este acto no puede aplicarse a una nueva fracción ya que requiere " +
                  "previamente un acto de: '" + Task.RecordingActType.RecordingRule.ChainedRecordingActType.DisplayName + "'.\n\n" +
                  "Es posible que dicho acto se encuentre registrado en la partida, " +
                  "pero el sistema no tiene esa información.\n\n" +
                  "Si este es el caso, favor de agregar primero el acto que falta en este documento " +
                  "aclarando dicho asunto en las observaciones.";
        Assertion.RequireFail(msg);
      }


      if (CreateResourceOnBookEntry) {
        var resources = Task.PrecedentBookEntry.RecordingActs.Select(x => x.Resource)
                                                             .Distinct()
                                                             .ToFixedList();

        if (resources.Count == 1) {
          Assertion.RequireFail("En esta inscripción ya está registrado un predio, " +
                                $"al cual se le asignó el folio real {resources[0].UID}.\n\n" +
                                "Para agregar múltiples predios a una inscripción debe utilizarse la " +
                                "herramienta de captura histórica.");

        } else if (resources.Count > 1) {
          Assertion.RequireFail($"La inscripción tiene registrados {resources.Count} predios con folio real, " +
                                "y no es posible agregarle otros por este medio.\n\n" +
                                "Para agregar múltiples predios a una inscripción debe utilizarse la " +
                                "herramienta de captura histórica.");
        }

      }
    }


    #endregion Public methods

    #region Recording methods

    private void AssertIsApplicableResource(Resource resourceToApply) {
      Assertion.Require(resourceToApply, "resourceToApply");

      switch (Task.RecordingActType.RecordingRule.AppliesTo) {
        case RecordingRuleApplication.Association:
          Assertion.Require(resourceToApply is Association,
            "Este acto sólo es aplicable a asociaciones. El folio electrónico corresponde a un predio.");
          return;

        case RecordingRuleApplication.RealEstate:
          Assertion.Require(resourceToApply is RealEstate,
            "Este acto sólo es aplicable a predios. El folio electrónico corresponde a una asociación.");
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
      // Cast because domain acts supposed to be applicable only to real estates
      RealEstate[] realEstates = (RealEstate[]) this.GetResources();

      var domainActs = new DomainAct[realEstates.Length];
      for (int i = 0; i < realEstates.Length; i++) {
        domainActs[i] = new DomainAct(this.Task.RecordingActType,
                                      this.Task.LandRecord, realEstates[i],
                                      this.Task.BookEntry,
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
                                                this.Task.LandRecord, resources[i],
                                                this.Task.BookEntry);
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
                                             this.Task.LandRecord, realEstates[i],
                                             this.Task.BookEntry,
                                             this.Task.RecordingActPercentage);
        recordingActs[i].Save();
      }
      return recordingActs;
    }


    private CancelationAct[] CreateCancelationAct() {
      switch (this.Task.RecordingActType.AppliesTo) {

        case RecordingRuleApplication.AssociationAct:
        case RecordingRuleApplication.NoPropertyAct:
        case RecordingRuleApplication.RealEstateAct:
          return CreateRecordingActCancelationAct();

        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          return this.CreateResourceCancelationAct();

        case RecordingRuleApplication.Party:
          return this.CreatePartyCancelationAct();

        case RecordingRuleApplication.Structure:
          return this.CreateStructureCancelationAct();

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private ModificationAct[] CreateModificationAct() {
      switch (this.Task.RecordingActType.AppliesTo) {
        case RecordingRuleApplication.AssociationAct:
        case RecordingRuleApplication.NoPropertyAct:
        case RecordingRuleApplication.RealEstateAct:
          return CreateRecordingActModificationAct();

        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          return this.CreateResourceModificationAct();

        case RecordingRuleApplication.Party:
          return this.CreatePartyModificationAct();

        case RecordingRuleApplication.Structure:
          return this.CreateStructureModificationAct();

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }

    #endregion Recording methods

    #region Cancelation methods


    private CancelationAct[] CreatePartyCancelationAct() {
      throw new NotImplementedException();
    }


    private CancelationAct[] CreateRecordingActCancelationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      return new[] { new CancelationAct(this.Task.RecordingActType,
                                        this.Task.LandRecord, resource, targetAct) };
    }


    private CancelationAct[] CreateResourceCancelationAct() {
      var resource = this.GetOneResource();

      return new[] { new CancelationAct(this.Task.RecordingActType,
                                        this.Task.LandRecord, resource) };
    }


    private CancelationAct[] CreateStructureCancelationAct() {
      throw new NotImplementedException();
    }


    private RecordingAct CreateTargetRecordingAct(Resource resource) {
      if (!Task.TargetActInfo.BookEntryWasCreated) {
        throw Assertion.EnsureNoReachThisCode("Invalid option in CreateTargetRecordingAct. BookEntryWasCreated must be true.");
      }

      BookEntry bookEntry = Task.TargetActInfo.BookEntry;

      LandRecord landRecord = bookEntry.LandRecord;

      return landRecord.AppendRecordingAct(Task.TargetActInfo.RecordingActType,
                                           resource, bookEntry: bookEntry);
    }


    private RecordingAct GetTargetRecordingAct(Resource resource) {
      if (this.Task.TargetActInfo.RecordingActId != -1) {
        return RecordingAct.Parse(this.Task.TargetActInfo.RecordingActId);
      } else {
        return this.CreateTargetRecordingAct(resource);
      }
    }


    #endregion Cancelation methods

    #region Modification methods

    private ModificationAct[] CreatePartyModificationAct() {
      throw new NotImplementedException();
    }


    private ModificationAct[] CreateRecordingActModificationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      var act = new[] { new ModificationAct(this.Task.RecordingActType,
                                            this.Task.LandRecord, resource, targetAct) };

      if (!this.Task.BookEntry.IsEmptyInstance) {
        act[0].SetBookEntry(this.Task.BookEntry);
      }

      return act;
    }


    private ModificationAct[] CreateResourceModificationAct() {
      var resource = this.GetOneResource();

      var act = new[] { new ModificationAct(this.Task.RecordingActType,
                                            this.Task.LandRecord, resource) };

      if (!this.Task.BookEntry.IsEmptyInstance) {
        act[0].SetBookEntry(this.Task.BookEntry);
      }

      return act;
    }


    private ModificationAct[] CreateStructureModificationAct() {
      throw new NotImplementedException();
    }


    #endregion Modification methods

    #region Get resources methods

    private Resource GetOneResource() {
      var resources = this.GetResources();

      Assertion.Require(resources.Length == 1,
                       "Operation failed, too many resources returned by GetOneResource().");

      return resources[0];
    }


    private Resource[] GetResources() {
      RecordingRuleApplication appliesTo = this.Task.RecordingActType.RecordingRule.AppliesTo;

      switch (appliesTo) {
        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.AssociationAct:
          return this.GetAssociations();

        case RecordingRuleApplication.RealEstate:
        case RecordingRuleApplication.RealEstateAct:
          return this.GetRealEstates();

        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.NoPropertyAct:
          return this.GetNoPropertyResources();

        default:
          throw Assertion.EnsureNoReachThisCode($"{appliesTo} application for {this.Task.RecordingActType.DisplayName}.");
      }
    }


    // Don't call directly. Please use it only in GetResources()
    private Association[] GetAssociations() {
      if (this.CreateNewResource) {
        return new Association[] { new Association() };

      } else if (this.CreateResourceOnBookEntry) {
        var association = new Association();

        this.AttachResourceToBookEntry(association);

        return new Association[] { association };

       } else if (this.SelectResource) {

        return new Association[] { (Association) this.Task.PrecedentProperty };

      } else {

        return new Association[] { new Association() };

      }
    }


    // Don't call directly. Please use it only in GetResources()
    private NoPropertyResource[] GetNoPropertyResources() {
      if (this.CreateNewResource) {
        return new NoPropertyResource[] { new NoPropertyResource() };

      } else if (this.CreateResourceOnBookEntry) {
        var noPropertyResource = new NoPropertyResource();

        this.AttachResourceToBookEntry(noPropertyResource);

        return new NoPropertyResource[] { noPropertyResource };

      } else if (this.SelectResource) {

        return new NoPropertyResource[] { (NoPropertyResource) this.Task.PrecedentProperty };

      } else {
        return new NoPropertyResource[] { new NoPropertyResource() };
      }
    }


    // Call it only in GetResources()
    private RealEstate[] GetRealEstates() {
      if (this.CreateNewResource) {
        var data = new RealEstateExtData() { CadastralKey = Task.CadastralKey };

        return new RealEstate[] { new RealEstate(data) };
      }

      RealEstate realEstate;

      if (this.CreateResourceOnBookEntry) {
        var data = new RealEstateExtData();

        realEstate = new RealEstate(data);

        this.AttachResourceToBookEntry(realEstate);

      } else if (this.SelectResource) {
        realEstate = (RealEstate) this.Task.PrecedentProperty;

      } else {
        realEstate = (RealEstate) this.Task.PrecedentProperty;
      }

      if (this.AppliesOverNewPartition) {
        return realEstate.Subdivide(Task.PartitionInfo);
      } else {
        return new RealEstate[] { realEstate };
      }
    }


    #endregion Get resources methods

    #region Book entry recording methods

    private void AttachResourceToBookEntry(Resource resource) {
      Assertion.Require(this.CreateResourceOnBookEntry,
                       "Wrong RecordingTask values to execute this method.");

      var document = Task.PrecedentBookEntry.LandRecord;

      var precedentAct = new InformationAct(RecordingActType.Empty, document,
                                            resource, Task.PrecedentBookEntry);
      precedentAct.Save();
    }


    private bool OperationalCondition(LandRecord landRecord) {
      // Fixed rule, based on law
      if (landRecord.Instrument.IssueDate < DateTime.Parse("2014-01-01")) {
        return true;
      }

      // Temporarily rule, based on customer's Recording Office operation
      if (landRecord.PresentationTime < DateTime.Parse("2016-10-01")) {
        return true;
      }

      // Temporarily rule, based on customer's Recording Office operation
      if (landRecord.PresentationTime < DateTime.Parse("2016-09-26") && DateTime.Today < DateTime.Parse("2016-10-01")) {
        return true;
      }
      return false;
    }


    #endregion Book entry recording methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
