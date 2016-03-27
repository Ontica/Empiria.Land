﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderExpert                                 Pattern  : Standard Class                      *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Performs the registry of recording acts based on a supplied recording task                    *
*              and a set of rules defined for each recording act type.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Data;

namespace Empiria.Land.Registration {

  /// <summary>Performs the registry of recording acts based on a supplied recording task
  ///  and a set of rules defined for each recording act type.</summary>
  public class RecorderExpert {

    #region Constructors and parsers

    public RecorderExpert(RecordingTask task) {
      this.Task = task;
    }

    static public RecordingAct Execute(RecordingTask task) {
      Assertion.AssertObject(task, "task");

      var expert = new RecorderExpert(task);

      expert.AssertValidTask();

      return expert.ProcessTask();
    }

    #endregion Constructors and parsers

    #region Properties

    private bool CreateNewResource {
      get {
        return ((Task.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.RealEstate ||
                 Task.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.Association) &&
                 Task.RecordingTaskType == RecordingTaskType.createProperty &&
                 Task.PrecedentProperty.IsEmptyInstance);
      }
    }

    private bool NeedCreateResourceOnNewPhysicalRecording {
      get {
        return ((Task.RecordingTaskType != RecordingTaskType.createProperty) &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                Task.PrecedentRecording.IsEmptyInstance &&
                Task.QuickAddRecordingNumber != String.Empty);
      }
    }

    private bool NeedCreateAdditionalResourceOnPhysicalRecording {
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
      string sMsg = String.Empty;
      if (NeedCreateAdditionalResourceOnPhysicalRecording) {
        throw new NotImplementedException();
      }
      if (NeedCreateResourceOnNewPhysicalRecording &&
          Task.PrecedentRecordingBook.ExistsRecording(Task.QuickAddRecordingNumber)) {
          sMsg = "La partida indicada ya existe en el libro seleccionado,\n" +
                  "y ya no es posible generar más de un folio de predio\n" +
                  "en una misma partida o antecedente.\n\n" +
                  "Si se requiere registrar más de un predio en una partida,\n" +
                  "favor de consultarlo con el área de soporte. Gracias.";

        throw new NotImplementedException(sMsg);
      }
      //  if (Task.PrecedentRecording.PresentationTime > Task.Transaction.PresentationTime) {
      //    throw new LandRegistrationException(LandRegistrationException.Msg.PrecendentPresentationTimeIsAfterThisTransactionDate,
      //                                        Task.PrecedentRecording.FullNumber,
      //                                        Task.PrecedentRecording.PresentationTime,
      //                                        Task.Transaction.UID, Task.Transaction.PresentationTime);
      //  }
    }

    #endregion Public methods

    #region Recording methods

    private bool AppliesOverNewPartition {
      get {
        return (Task.PartitionInfo.PartitionType != PropertyPartitionType.None);
      }
    }

    private RecordingAct DoPropertyRecording() {
      if (this.NeedCreateResourceOnNewPhysicalRecording) {
        this.CreatePropertyOnPhysicalRecording();
      } else if (this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        this.CreateAdditionalResourceOnPhysicalRecording();
      } else if (this.CreateNewResource) {
        Task.PrecedentProperty = new RealEstate(Task.CadastralKey);
      } else if (this.AppliesOverNewPartition) {
        Task.PrecedentProperty = ((RealEstate)Task.PrecedentProperty).Subdivide(Task.PartitionInfo);
      }
      Assertion.Assert(this.Task.TargetActInfo.IsEmptyInstance,
                       "The target recording act should be the empty instance.");
      Assertion.Assert(!this.Task.PrecedentProperty.IsEmptyInstance,
                       "The target resource cannot be the Property.Empty instance.");

      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType,
                                                   this.Task.PrecedentProperty);
    }

    private RecordingAct DoRecordingActCancelation() {
      if (this.NeedCreateResourceOnNewPhysicalRecording) {
        this.CreatePropertyOnPhysicalRecording();
      } else if (this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        this.CreateAdditionalResourceOnPhysicalRecording();
      }
      Assertion.Assert(!this.Task.TargetActInfo.IsEmptyInstance,
                       "The target recording act should not be the empty instance.");
      Assertion.Assert(!this.Task.PrecedentProperty.IsEmptyInstance,
                       "The target resource cannot be the Property.Empty instance.");

      RecordingAct amendmendOf;
      if (this.Task.TargetActInfo.RecordingActId == -1) {
        amendmendOf = this.CreateAmendmendOfRecordingAct(this.Task.PrecedentProperty);
      } else {
        amendmendOf = RecordingAct.Parse(this.Task.TargetActInfo.RecordingActId);
      }

      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType,
                                                   this.Task.PrecedentProperty, amendmendOf);
    }

    #endregion Recording methods

    #region Private auxiliar methods

    private DomainAct CreateDomainAct() {
      /// ToDo: Fist validate if the resource is compatible with the act
      RealEstate resource = (RealEstate) this.GetResource();

      var domainAct = new DomainAct(this.Task.RecordingActType,
                                    this.Task.Document, resource, this.Task.RecordingActPercentage);
      domainAct.Save();

      return domainAct;
    }

    private InformationAct CreateInformationAct() {
      InformationAct informationAct = null;

      switch (this.Task.RecordingActType.AppliesTo) {
        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.RealEstate:
          Resource resource = this.GetResource();
          informationAct = new InformationAct(this.Task.RecordingActType,
                                              this.Task.Document, resource);
          break;
        case RecordingRuleApplication.NoProperty:
          resource = new NoPropertyResource();
          informationAct = new InformationAct(this.Task.RecordingActType,
                                              this.Task.Document, resource);
          //RecordingDocument targetDocument = this.GetTargetDocument();

          break;
        //case RecordingRuleApplication.RecordingAct:
        //  RecordingAct targetRecordingAct = this.GetTargetRecordingAct();
        //  informationAct = new InformationAct(this.Task.RecordingActType,
        //                                      this.Task.Document, targetRecordingAct);
        //  break;
        default:
          throw Assertion.AssertNoReachThisCode();
      }

      informationAct.Save();

      return informationAct;
    }

    private RecordingAct GetTargetRecordingAct() {
      throw new NotImplementedException();
    }

    private RecordingDocument GetTargetDocument() {
      throw new NotImplementedException();
    }

    private LimitationAct CreateLimitationAct() {
      RealEstate property = this.GetProperty();

      var limitationAct = new LimitationAct(this.Task.RecordingActType,
                                            this.Task.Document, property);
      limitationAct.Save();

      return limitationAct;
    }

    //private RecordingAct CreateDocumentAct() {
    //  var documentAct = new DocumentAct(this.Task.RecordingActType,
    //                                    this.Task.Document);
    //  documentAct.Save();

    //  return documentAct;
    //}

    private Resource GetResource() {
      switch (this.Task.RecordingActType.RecordingRule.AppliesTo) {
        case RecordingRuleApplication.Association:
          return this.GetAssociation();
        case RecordingRuleApplication.RealEstate:
          return this.GetProperty();
        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private Association GetAssociation() {
      if (this.CreateNewResource) {
        return new Association(Task.ResourceName);
      } else if (this.NeedCreateResourceOnNewPhysicalRecording) {
        return this.CreateAssociationOnPhysicalRecording();
      } else {
        return (Association) this.Task.PrecedentProperty;
      }
    }

    private RealEstate GetProperty() {
      if (this.CreateNewResource) {
        return new RealEstate(Task.CadastralKey);
      }

      RealEstate property = null;

      if (this.NeedCreateResourceOnNewPhysicalRecording) {
        property = this.CreatePropertyOnPhysicalRecording();
      } else if (this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        property = this.CreateAdditionalResourceOnPhysicalRecording();
      } else {
        property = (RealEstate) this.Task.PrecedentProperty;
      }

      if (this.AppliesOverNewPartition) {
        return property.Subdivide(Task.PartitionInfo);
      } else {
        return property;
      }
    }

    private Association CreateAssociationOnPhysicalRecording() {
      Assertion.Assert(this.NeedCreateResourceOnNewPhysicalRecording,
                       "Association resource was already created on physical recording.");

      var association = new Association(Task.ResourceName);

      var document = new RecordingDocument(RecordingDocumentType.Empty);
      Recording physicalRecording =
            Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber);
      var precedentAct = new InformationAct(RecordingActType.Empty, document,
                                            association, physicalRecording);
      precedentAct.Save();

      return association;
    }

    private RealEstate CreatePropertyOnPhysicalRecording() {
      Assertion.Assert(this.NeedCreateResourceOnNewPhysicalRecording,
                       "Wrong RecordingTask values to execute this method.");

      RealEstate property = new RealEstate(Task.CadastralKey);

      var document = new RecordingDocument(RecordingDocumentType.Empty);
      Recording physicalRecording =
            Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber);
      var precedentAct = new DomainAct(RecordingActType.Empty, document,
                                       property, physicalRecording);

      precedentAct.Save();

      return property;
    }

    private RealEstate CreateAdditionalResourceOnPhysicalRecording() {
      Assertion.Assert(this.NeedCreateAdditionalResourceOnPhysicalRecording,
                       "Wrong RecordingTask values to execute this method.");


      var property = new RealEstate(Task.CadastralKey);
      var document = Task.PrecedentRecording.Document;
      var recordingAct = document.AppendRecordingAct(RecordingActType.Empty, property,
                                                     physicalRecording: Task.PrecedentRecording);
      return property;
    }

    private RecordingAct CreateAmendmendOfRecordingAct(Resource resource) {
      var document = new RecordingDocument(RecordingDocumentType.Empty);

      Recording recording = Task.TargetActInfo.PhysicalRecording;

      return document.AppendRecordingAct(Task.TargetActInfo.RecordingActType,
                                         resource, physicalRecording: recording);
    }

    //private RecordingBook GetOpenedRecordingBook() {
    //  return RecordingBook.GetAssignedBookForRecording(this.Task.RecorderOffice,
    //                                                   this.Task.RecordingRule.RecordingSection,
    //                                                   this.Task.Document);
    //}

    private RecordingAct ProcessTask() {
      var recordingActType = this.Task.RecordingActType;

      if (recordingActType.IsDomainActType) {
        return this.CreateDomainAct();          // CV, Donación, Título de propiedad, Constitución SC,
      } else if (recordingActType.IsLimitationActType) {
        return this.CreateLimitationAct();     // Hipoteca, Embargo, Inmovilización, Aviso preventivo
      } else if (recordingActType.IsInformationActType) {
        return this.CreateInformationAct();     // Testamento, Cap matrim, Anotación marginal
      //} else if (recordingActType.IsModificationActType) {
        //return this.CreateInformationAct();     // Testamento, Cap matrim, Anotación marginal
      } else {
        throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
                                          Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
      }

      //if (recordingActType.IsSubtypeOf<DocumentAct>()) {
      //  return this.CreateDocumentAct();        // Testamento, Nombr. de albacea, Capit. matrimoniales.
      ////} else if (recordingActType.IsSubtypeOf<InformationAct>()) {
      ////} else if (recordingActType.IsSubtypeOf<ModificationAct>()) {
      ////} else if (recordingActType.IsSubtypeOf<CancelationAct>()) {
      ////} else if (recordingActType.IsSubtypeOf<StructureAct>()) {
      //} else {
      //  throw Assertion.AssertNoReachThisCode();
      //}

      //switch (Task.RecordingRule.AppliesTo) {
      //  case RecordingRuleApplication.Property:
      //    if (Task.RecordingRule.IsMainRecording) {
      //      return this.DoPropertyRecording();       // (e.g. Título de propiedad o compra-venta)
      //    }
      //    if (Task.RecordingRule.IsAnnotation) {
      //      //return this.DoPropertyAnnotation();    // (e.g. Aviso preventivo)
      //      return this.DoPropertyRecording();
      //    }
      //    break;
      //  case RecordingRuleApplication.Structure:
      //    if (Task.RecordingRule.IsMainRecording) {
      //      return this.DoPropertyRecording();      // (e.g. Fusión de predios [merge])
      //    }
      //    break;
      //  case RecordingRuleApplication.RecordingAct:
      //    //if (Task.RecordingRule.IsAnnotation) {
      //    //  return this.DoRecordingActAnnotation();  // (e.g. Nombramiento de albacea)
      //    //}
      //    if (Task.RecordingRule.IsCancelation) {
      //      return this.DoRecordingActCancelation(); // (e.g. Cancelación de crédito hipotecario)
      //    }
      //    break;
      //  default:
      //    break;
      //}
      //throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
      //                                  Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
    }

    #endregion Private auxiliar methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
