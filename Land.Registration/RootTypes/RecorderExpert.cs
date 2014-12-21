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
                Task.TargetProperty.IsEmptyInstance);
      }
    }

    private bool NeedCreatePrecedentRecording {
      get {
        return (Task.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                Task.PrecedentRecording.IsEmptyInstance &&
                Task.QuickAddRecordingNumber != -1);
      }
    }

    private bool NeedCreatePrecedentRecordingAct {
      get {
        return (Task.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                !Task.PrecedentRecording.IsEmptyInstance &&
                Task.TargetProperty.IsNew);
      }
    }

    public RecordingTask Task {
      get;
      private set;
    }

    #endregion Properties

    #region Public methods

    public void AssertValidTask() {
      if (NeedCreatePrecedentRecordingAct) {
        if (Task.PrecedentRecording.PresentationTime > Task.Transaction.PresentationTime) {
          throw new LandRegistrationException(LandRegistrationException.Msg.PrecendentPresentationTimeIsAfterThisTransactionDate,
                                              Task.PrecedentRecording.FullNumber, 
                                              Task.PrecedentRecording.PresentationTime,
                                              Task.Transaction.UniqueCode, Task.Transaction.PresentationTime);
        }
      }
    }

    #endregion Public methods

    #region Recording methods

    private RecordingAct DoNoneResourceRecording() {
      Assertion.Assert(this.Task.TargetRecordingAct.IsEmptyInstance,
                       "TargetRecordingAct should be the empty instance.");
      Assertion.Assert(this.Task.TargetProperty.IsEmptyInstance,
                       "TargetResource should be the empty instance.");

      var recordingAct = RecordingAct.Parse(this.Task);

      return recordingAct.WriteOn(this.Task.RecorderOffice,
                                  this.Task.RecordingRule.RecordingSection);
      
      /// Original Code
      //RecordingBook book = this.GetRecordingBook(); 
      //Recording recording = book.CreateRecording();
      //return recording.CreateRecordingAct(Task.RecordingActType, Property.Empty);

      // Current-form
      //using (var context = new StorageContext()) {
      //  RecordingBook book = this.GetRecordingBook();
      //  context.Save(book);
      //  this.RecordingAct.WriteOn(book);
      //  context.Save(this.RecordingAct);
      //  context.Update;
      //}

      /// Ugly-form
      //using (var context = StorageContext.Open()) {
      //  RecordingBook book = this.GetRecordingBook();
      //  context.Attach(book);
      //  context.Attach(this.RecordingAct);
      //  this.RecordingAct.WriteOn(book);
      //  context.Update();
      //}

      /// Thread-based
      //using (var context = new StorageContext()) {
      //  RecordingBook book = this.GetRecordingBook();
      //  this.RecordingAct.WriteOn(book);
      //  context.SaveChanges();
      //}

    }

    private RecordingAct DoPropertyAnnotation() {
      if (this.NeedCreatePrecedentRecording) {
        this.CreatePrecedentRecording();
      } else if (this.NeedCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      }
      var lastRecording = Task.TargetProperty.LastRecordingAct.Recording;

      return lastRecording.CreateAnnotation(Task.Transaction, Task.RecordingActType,
                                            Task.TargetProperty);
    }

    private RecordingAct DoPropertyRecording() {
      if (this.NeedCreatePrecedentRecording) {
        this.CreatePrecedentRecording();
      } else if (this.NeedCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      } else if (this.AppliesOverNewProperty) {
        Task.TargetProperty = new Property();
      }
      Assertion.Assert(this.Task.TargetProperty.IsEmptyInstance == false, 
                       "The target resource cannot be the Property.Empty instance.");

      var recordingAct = RecordingAct.Parse(this.Task);

      return recordingAct.WriteOn(this.Task.RecorderOffice,
                                  this.Task.RecordingRule.RecordingSection);

      //ORIGINAL CODE
      //RecordingBook book = this.GetOpenedRecordingBook();
      //Recording recording = book.CreateRecording(Task.Transaction);

      //return recording.CreateRecordingAct(Task.RecordingActType, Task.TargetResource);
    }

    private RecordingAct DoRecordingActAnnotation() {
      throw new NotImplementedException("DoRecordingActAnnotation");
    }

    private RecordingAct DoRecordingActCancelation() {
      throw new NotImplementedException("DoRecordingActCancelation");
    }

    private RecordingAct DoStructureRecording() {
      return DoPropertyRecording();
    }

    #endregion Recording methods

    #region Private auxiliar methods

    private void CreatePrecedentRecording() {
      if (!this.NeedCreatePrecedentRecording) {
        return;
      }

      Recording recording = Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber,
                                                                             Task.QuickAddBisRecordingSuffixTag);
      Property property = new Property();
      RecordingAct recordingAct = recording.CreateRecordingAct(RecordingActType.Empty, property);

      Task.TargetProperty = property;
      Task.PrecedentRecording = recording;
      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        Task.TargetRecordingAct = recordingAct;
      }
    }

    private void CreatePrecedentRecordingAct() {
      if (!this.NeedCreatePrecedentRecordingAct) {
        return;
      }

      var property = new Property();
      var recordingAct = Task.PrecedentRecording.CreateRecordingAct(RecordingActType.Empty, property);
      Task.TargetProperty = property;
      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        Task.TargetRecordingAct = recordingAct;
      }
    }

    private RecordingBook GetOpenedRecordingBook() {
      return RecordingBook.GetAssignedBookForRecording(this.Task.RecorderOffice,
                                                       this.Task.RecordingRule.RecordingSection,
                                                       this.Task.Document);
    }

    private RecordingAct ProcessTask() {
      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.Property) {
        if (Task.RecordingRule.IsMainRecording) {
          return this.DoPropertyRecording();       // (e.g. Título de propiedad o compra-venta)
        }
        if (Task.RecordingRule.IsAnnotation) {
          return this.DoPropertyAnnotation();      // (e.g. Aviso preventivo)
        }
      }

      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.Structure) {
        if (Task.RecordingRule.IsMainRecording) {
          return this.DoStructureRecording();      // (e.g. Fusión de predios [merge])
        }
      }

      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        if (Task.RecordingRule.IsAnnotation) {
          return this.DoRecordingActAnnotation();  // (e.g. Nombramiento de albacea)
        }
        if (Task.RecordingRule.IsCancelation) {
          return this.DoRecordingActCancelation(); // (e.g. Cancelación de crédito hipotecario)
        }
      }

      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.None) {
        if (Task.RecordingRule.IsMainRecording) {
          return this.DoNoneResourceRecording();  // (e.g. contrato entre particulares, cap. matrimoniales)
        }
      }

      throw new NotImplementedException("RecordingExpert.DoRecording: Recording act '" +
                                        Task.RecordingActType.DisplayName + "' has an undefined or wrong rule.");
    }

    #endregion Private auxiliar methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration
