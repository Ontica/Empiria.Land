/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderExpert                                 Pattern  : Standard Class                      *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
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

    private bool AppliesOverNewProperty {
      get {
        return (Task.RecordingRule.AppliesTo == RecordingRuleApplication.Property &&
                Task.PropertyRecordingType == PropertyRecordingType.createProperty &&
                Task.PrecedentProperty.IsEmptyInstance);
      }
    }

    private bool NeedCreateResourceOnNewPhysicalRecording {
      get {
        return ((Task.PropertyRecordingType != PropertyRecordingType.createProperty) &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                Task.PrecedentRecording.IsEmptyInstance &&
                Task.QuickAddRecordingNumber != -1);
      }
    }

    private bool NeedCreateAdditionalResourceOnPhysicalRecording {
      get {
        return ((Task.PropertyRecordingType != PropertyRecordingType.createProperty) &&
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
          Task.PrecedentRecordingBook.ExistsRecording(Task.QuickAddRecordingNumber, 
                                                      Task.QuickAddRecordingSubNumber,
                                                      Task.QuickAddRecordingSuffixTag)) {
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

    private RecordingAct DoDocumentRecording() {
      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType);
    }

    private RecordingAct DoCreateAssociation() {
      Task.PrecedentProperty = new Association(Task.ResourceName);

      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType,
                                                   this.Task.PrecedentProperty);
    }

    private RecordingAct DoCreateRecordingOnAssociation() {
      if (this.NeedCreateResourceOnNewPhysicalRecording) {
        this.CreateAssociationOnNewPhysicalRecording();
      }
      Assertion.Assert(!this.Task.PrecedentProperty.IsEmptyInstance,
                       "The target resource cannot be the Resource.Empty instance.");
      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType,
                                                   this.Task.PrecedentProperty);
    }

    private RecordingAct DoPropertyRecording() {
      if (this.NeedCreateResourceOnNewPhysicalRecording) {
        this.CreateResourceOnNewPhysicalRecording();
      } else if (this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        this.CreateAdditionalResourceOnPhysicalRecording();
      } else if (this.AppliesOverNewProperty) {
        Task.PrecedentProperty = new Property();
      } else if (this.AppliesOverNewPartition) {
        Task.PrecedentProperty = ((Property)Task.PrecedentProperty).Subdivide(Task.PartitionInfo);
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
        this.CreateResourceOnNewPhysicalRecording();
      } else if (this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        this.CreateAdditionalResourceOnPhysicalRecording();
      }
      Assertion.Assert(!this.Task.TargetActInfo.IsEmptyInstance,
                       "The target recording act should not be the empty instance.");
      Assertion.Assert(!this.Task.PrecedentProperty.IsEmptyInstance,
                       "The target resource cannot be the Property.Empty instance.");

      RecordingAct amendmendOf;
      if (this.Task.TargetActInfo.RecordingActId == -1) {
        amendmendOf = CreateAmendmendOfRecordingAct(this.Task.PrecedentProperty);
      } else {
        amendmendOf = RecordingAct.Parse(this.Task.TargetActInfo.RecordingActId);
      }

      return this.Task.Document.AppendRecordingAct(this.Task.RecordingActType,
                                                   this.Task.PrecedentProperty, amendmendOf);
    }

    #endregion Recording methods

    #region Private auxiliar methods

    private void CreateAssociationOnNewPhysicalRecording() {
      if (!this.NeedCreateResourceOnNewPhysicalRecording) {
        return;
      }

      var association = new Association(Task.ResourceName);

      var document = new RecordingDocument(RecordingDocumentType.Empty);
      Recording recording = Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber,
                                                                             Task.QuickAddRecordingSubNumber,
                                                                             Task.QuickAddRecordingSuffixTag);

      RecordingAct recordingAct = document.AppendRecordingAct(RecordingActType.Parse(2750), association,
                                                              physicalRecording: recording);

      Task.PrecedentProperty = association;
      Task.PrecedentRecording = recording;
    }

    private void CreateResourceOnNewPhysicalRecording() {
      if (!this.NeedCreateResourceOnNewPhysicalRecording) {
        return;
      }

      Property property = new Property();

      var document = new RecordingDocument(RecordingDocumentType.Empty);
      Recording recording = Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber,
                                                                             Task.QuickAddRecordingSubNumber,
                                                                             Task.QuickAddRecordingSuffixTag);

      RecordingAct recordingAct = document.AppendRecordingAct(RecordingActType.Empty, property,
                                                              physicalRecording: recording);

      if (Task.PartitionInfo.PartitionType != PropertyPartitionType.None) {
        Task.PrecedentProperty = property.Subdivide(Task.PartitionInfo);
      } else {
        Task.PrecedentProperty = property;
      }
      Task.PrecedentRecording = recording;
    }

    private RecordingAct CreateAmendmendOfRecordingAct(Resource resource) {
      var document = new RecordingDocument(RecordingDocumentType.Empty);

      Recording recording = Task.TargetActInfo.PhysicalRecording;

      return document.AppendRecordingAct(Task.TargetActInfo.RecordingActType,
                                         resource, physicalRecording: recording);
    }

    private void CreateAdditionalResourceOnPhysicalRecording() {
      if (!this.NeedCreateAdditionalResourceOnPhysicalRecording) {
        return;
      }

      var property = new Property();
      var document = Task.PrecedentRecording.Document;
      var recordingAct = document.AppendRecordingAct(RecordingActType.Empty, property,
                                                     physicalRecording: Task.PrecedentRecording);
      Task.PrecedentProperty = property;
    }

    //private RecordingBook GetOpenedRecordingBook() {
    //  return RecordingBook.GetAssignedBookForRecording(this.Task.RecorderOffice,
    //                                                   this.Task.RecordingRule.RecordingSection,
    //                                                   this.Task.Document);
    //}

    private RecordingAct ProcessTask() {
      switch (Task.RecordingRule.AppliesTo) {
        case RecordingRuleApplication.Association:
          if (Task.RecordingRule.PropertyRecordingStatus == PropertyRecordingStatus.Unregistered) {
            return this.DoCreateAssociation();       // (e.g. Alta de sociedad civil)
          } else {
            return this.DoCreateRecordingOnAssociation();       // (e.g. Alta de sociedad civil)
          }
        case RecordingRuleApplication.Property:
          if (Task.RecordingRule.IsMainRecording) {
            return this.DoPropertyRecording();       // (e.g. Título de propiedad o compra-venta)
          }
          if (Task.RecordingRule.IsAnnotation) {
            //return this.DoPropertyAnnotation();    // (e.g. Aviso preventivo)
            return this.DoPropertyRecording();
          }
          break;
        case RecordingRuleApplication.Structure:
          if (Task.RecordingRule.IsMainRecording) {
            return this.DoPropertyRecording();      // (e.g. Fusión de predios [merge])
          }
          break;
        case RecordingRuleApplication.RecordingAct:
          //if (Task.RecordingRule.IsAnnotation) {
          //  return this.DoRecordingActAnnotation();  // (e.g. Nombramiento de albacea)
          //}
          if (Task.RecordingRule.IsCancelation) {
            return this.DoRecordingActCancelation(); // (e.g. Cancelación de crédito hipotecario)
          }
          break;
        case RecordingRuleApplication.Document:
          return this.DoDocumentRecording();      // (e.g. Testamento, Nombramiento de albacea o Capitulaciones matrimoniales)
        default:
          break;
      }
      throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
                                        Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
    }

    #endregion Private auxiliar methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
