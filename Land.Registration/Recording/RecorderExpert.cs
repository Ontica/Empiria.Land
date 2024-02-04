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

    private RecorderExpert(RecordingTask task) {
      this.Task = task;
    }

    static public void Execute(RecordingTask task) {
      Assertion.Require(task, nameof(task));

      var expert = new RecorderExpert(task);

      expert.AssertValidTask();

      expert.ProcessTask();
    }

    #endregion Constructors and parsers

    #region Properties

    private bool AppliesOverNewPartition {
      get {
        return (Task.RecordingTaskType == RecordingTaskType.createRealEstatePartition ||
                Task.RecordingTaskType == RecordingTaskType.createRealEstateOnBookEntryAndANewPartition);
      }
    }


    private bool ApplyOverExistingRecordableSubject {
      get {
        return (Task.RecordingTaskType == RecordingTaskType.applyOverExistingRecordableSubject &&
                !Task.RecordableSubject.IsEmptyInstance);
      }
    }


    private bool CreateNewRecordableSubject {
      get {
        return (Task.RecordingTaskType == RecordingTaskType.createNewRecordableSubject &&
                Task.RecordableSubject.IsEmptyInstance);
      }
    }


    private bool CreateRecordableSubjectOnBookEntry {
      get {
        return ((Task.RecordingTaskType == RecordingTaskType.createRecordableSubjectOnBookEntry ||
                 Task.RecordingTaskType == RecordingTaskType.createRealEstateOnBookEntryAndANewPartition) &&
                !Task.PrecedentBookEntry.IsEmptyInstance);
      }
    }


    public RecordingTask Task {
      get;
      private set;
    }


    #endregion Properties

    #region Public methods

    public void AssertValidTask() {
      if (!Task.RecordableSubject.IsEmptyInstance) {

        this.AssertIsApplicableResource(Task.RecordableSubject);

        Task.RecordableSubject.AssertIsStillAlive(Task.LandRecord.PresentationTime);

        if (this.AppliesOverNewPartition && Task.RecordingActType.RecordingRule.HasChainedRule) {
          if (!OperationalCondition(Task.LandRecord)) {
            var msg = "Este acto no puede aplicarse a una nueva fracción ya que requiere " +
                      "previamente un acto de: '" +
                      Task.RecordingActType.RecordingRule.ChainedRecordingActType.DisplayName + "'.";
            Assertion.RequireFail(msg);
          }
        }
        Task.RecordableSubject.AssertCanBeAddedTo(Task.LandRecord, Task.RecordingActType);
      }

      if ((this.CreateRecordableSubjectOnBookEntry) &&
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


      if (!CreateRecordableSubjectOnBookEntry) {
        return;
      }

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


    #endregion Public methods

    #region Recording methods

    private void AssertIsApplicableResource(Resource resourceToApply) {
      Assertion.Require(resourceToApply, nameof(resourceToApply));

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


    private void ProcessTask() {
      var recordingActType = Task.RecordingActType;

      if (recordingActType.IsDomainActType) {
        this.CreateDomainAct();          // CV, Donación, Título de propiedad, Constitución SC,

      } else if (recordingActType.IsLimitationActType) {
        CreateLimitationAct();     // Hipoteca, Embargo, Inmovilización, Aviso preventivo

      } else if (recordingActType.IsInformationActType) {
        this.CreateInformationAct();     // Testamento, Cap matrim, Anotación marginal

      } else if (recordingActType.IsCancelationActType) {
        this.CreateCancelationAct();

      } else if (recordingActType.IsModificationActType) {
        this.CreateModificationAct();

      } else {
        throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
                                          Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
      }
    }


    private void CreateDomainAct() {
      // Cast because domain acts supposed to be applicable only to real estates
      RealEstate[] realEstates = (RealEstate[]) this.GetRecordableSubjects();

      var domainActs = new DomainAct[realEstates.Length];
      for (int i = 0; i < realEstates.Length; i++) {
        domainActs[i] = new DomainAct(Task.RecordingActType,
                                      Task.LandRecord, realEstates[i],
                                      Task.BookEntry,
                                      Task.RecordingActPercentage);
        domainActs[i].Save();
      }
    }


    private void CreateInformationAct() {
      Resource[] resources = this.GetRecordableSubjects();

      var informationActs = new InformationAct[resources.Length];
      for (int i = 0; i < resources.Length; i++) {
        informationActs[i] = new InformationAct(Task.RecordingActType,
                                                Task.LandRecord, resources[i],
                                                Task.BookEntry);
        informationActs[i].Save();
      }
    }


    private void CreateLimitationAct() {
      // Cast because limitation acts supposed to be applicable only to real estates
      RealEstate[] realEstates = (RealEstate[]) this.GetRecordableSubjects();

      var recordingActs = new LimitationAct[realEstates.Length];
      for (int i = 0; i < realEstates.Length; i++) {
        recordingActs[i] = new LimitationAct(Task.RecordingActType,
                                             Task.LandRecord, realEstates[i],
                                             Task.BookEntry,
                                             Task.RecordingActPercentage);
        recordingActs[i].Save();
      }
    }


    private void CreateCancelationAct() {
      switch (Task.RecordingActType.AppliesTo) {

        case RecordingRuleApplication.AssociationAct:
        case RecordingRuleApplication.NoPropertyAct:
        case RecordingRuleApplication.RealEstateAct:
          CreateRecordingActCancelationAct();
          return;

        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          CreateResourceCancelationAct();
          return;

        case RecordingRuleApplication.Party:
          this.CreatePartyCancelationAct();
          return;

        case RecordingRuleApplication.Structure:
          CreateStructureCancelationAct();
          return;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void CreateModificationAct() {
      switch (this.Task.RecordingActType.AppliesTo) {
        case RecordingRuleApplication.AssociationAct:
        case RecordingRuleApplication.NoPropertyAct:
        case RecordingRuleApplication.RealEstateAct:
          CreateRecordingActModificationAct();
          return;

        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.RealEstate:
          CreateResourceModificationAct();
          return;

        case RecordingRuleApplication.Party:
          CreatePartyModificationAct();
          return;

        case RecordingRuleApplication.Structure:
          CreateStructureModificationAct();
          return;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }

    #endregion Recording methods

    #region Cancelation methods


    private void CreatePartyCancelationAct() {
      throw new NotImplementedException();
    }


    private void CreateRecordingActCancelationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      var newAct = new CancelationAct(Task.RecordingActType,
                                      Task.LandRecord, resource, targetAct);
      newAct.Save();
    }


    private void CreateResourceCancelationAct() {
      var resource = this.GetOneResource();

      var newAct = new CancelationAct(Task.RecordingActType,
                                      Task.LandRecord, resource);
      newAct.Save();
    }


    private void CreateStructureCancelationAct() {
      throw new NotImplementedException();
    }

    #endregion Cancelation methods

    #region Modification methods

    private void CreatePartyModificationAct() {
      throw new NotImplementedException();
    }


    private void CreateRecordingActModificationAct() {
      var resource = this.GetOneResource();

      RecordingAct targetAct = this.GetTargetRecordingAct(resource);

      var act = new ModificationAct(Task.RecordingActType,
                                    Task.LandRecord, resource, targetAct);

      if (!Task.BookEntry.IsEmptyInstance) {
        act.SetBookEntry(Task.BookEntry);
      }
    }


    private void CreateResourceModificationAct() {
      var resource = this.GetOneResource();

      var act = new ModificationAct(Task.RecordingActType,
                                    Task.LandRecord, resource);

      if (!Task.BookEntry.IsEmptyInstance) {
        act.SetBookEntry(Task.BookEntry);
      }
    }


    private void CreateStructureModificationAct() {
      throw new NotImplementedException();
    }


    #endregion Modification methods

    #region Get resources methods

    private Resource GetOneResource() {
      var resources = this.GetRecordableSubjects();

      Assertion.Require(resources.Length == 1,
                       "Operation failed, too many resources returned by GetOneResource().");

      return resources[0];
    }


    private Resource[] GetRecordableSubjects() {
      RecordingRuleApplication appliesTo = Task.RecordingActType.RecordingRule.AppliesTo;

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
          throw Assertion.EnsureNoReachThisCode($"{appliesTo} application for {Task.RecordingActType.DisplayName}.");
      }
    }


    // Don't call directly. Please use it only in GetResources()
    private Association[] GetAssociations() {
      if (this.CreateNewRecordableSubject) {
        return new Association[] { new Association() };

      } else if (this.CreateRecordableSubjectOnBookEntry) {
        var association = new Association();

        this.AttachResourceToBookEntry(association);

        return new Association[] { association };

       } else if (this.ApplyOverExistingRecordableSubject) {

        return new Association[] { (Association) Task.RecordableSubject };

      } else {

        return new Association[] { new Association() };

      }
    }


    // Don't call directly. Please use it only in GetResources()
    private NoPropertyResource[] GetNoPropertyResources() {
      if (this.CreateNewRecordableSubject) {
        return new NoPropertyResource[] { new NoPropertyResource() };

      } else if (this.CreateRecordableSubjectOnBookEntry) {
        var noPropertyResource = new NoPropertyResource();

        this.AttachResourceToBookEntry(noPropertyResource);

        return new NoPropertyResource[] { noPropertyResource };

      } else if (this.ApplyOverExistingRecordableSubject) {

        return new NoPropertyResource[] { (NoPropertyResource) Task.RecordableSubject };

      } else {
        return new NoPropertyResource[] { new NoPropertyResource() };
      }
    }


    // Call it only in GetResources()
    private RealEstate[] GetRealEstates() {
      if (this.CreateNewRecordableSubject) {
        var data = new RealEstateExtData() { CadastralKey = Task.CadastralKey };

        return new RealEstate[] { new RealEstate(data) };
      }

      RealEstate realEstate;

      if (this.CreateRecordableSubjectOnBookEntry) {
        var data = new RealEstateExtData();

        realEstate = new RealEstate(data);

        this.AttachResourceToBookEntry(realEstate);

      } else if (this.ApplyOverExistingRecordableSubject) {
        realEstate = (RealEstate) Task.RecordableSubject;

      } else {
        realEstate = (RealEstate) Task.RecordableSubject;
      }

      if (this.AppliesOverNewPartition) {
        return realEstate.Subdivide(Task.RealEstatePartitionInfo);
      } else {
        return new RealEstate[] { realEstate };
      }
    }


    #endregion Get resources methods

    #region Helpers

    private void AttachResourceToBookEntry(Resource resource) {
      Assertion.Require(this.CreateRecordableSubjectOnBookEntry,
                       "Wrong RecordingTask values to execute this method.");

      var document = Task.PrecedentBookEntry.LandRecord;

      var precedentAct = new InformationAct(RecordingActType.Empty, document,
                                            resource, Task.PrecedentBookEntry);
      precedentAct.Save();
    }


    private RecordingAct CreateTargetRecordingAct(Resource resource) {
      if (!Task.TargetActInfo.BookEntryWasCreated) {
        throw Assertion.EnsureNoReachThisCode("Invalid option in CreateTargetRecordingAct. BookEntryWasCreated must be true.");
      }

      BookEntry bookEntry = Task.TargetActInfo.BookEntry;

      LandRecord landRecord = bookEntry.LandRecord;

      return landRecord.AppendRecordingAct(Task.TargetActInfo.RecordingActType,
                                           resource, bookEntry);
    }


    private RecordingAct GetTargetRecordingAct(Resource resource) {
      if (Task.TargetActInfo.RecordingActId != -1) {
        return RecordingAct.Parse(Task.TargetActInfo.RecordingActId);
      } else {
        return this.CreateTargetRecordingAct(resource);
      }
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

    #endregion Helpers

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
