/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : RecorderExpert                                 Pattern  : Standard Class                      *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Performs the registry of recording acts based on a supplied recording task                    *
*              and a set of rules defined for each recording act type.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
    public RecorderExpert(RecordingTask task, RecordingAct recordingAct) {
      this.Task = task;
      this.RecordingAct = recordingAct;
    }

    #endregion Constructors and parsers

    #region Properties

    private bool ApplyOverNewProperty {
      get {
        return (Task.RecordingRule.AppliesTo == RecordingRuleApplication.Property &&
                Task.PropertyRecordingType == PropertyRecordingType.createProperty &&
                Task.TargetResource.IsEmptyInstance);
      }
    }

    private bool ExecuteCreatePrecedentRecording {
      get {
        return (Task.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                Task.PrecedentRecording.IsEmptyInstance &&
                Task.QuickAddRecordingNumber != -1);
      }
    }

    private bool ExecuteCreatePrecedentRecordingAct {
      get {
        return (Task.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !Task.PrecedentRecordingBook.IsEmptyInstance &&
                !Task.PrecedentRecording.IsEmptyInstance &&
                Task.TargetResource.IsNew);
      }
    }

    public RecordingTask Task {
      get;
      private set;
    }

    public RecordingAct RecordingAct {
      get;
      private set;
    }

    #endregion Properties

    #region Public methods

    public void AssertValidTask() {
      if (ExecuteCreatePrecedentRecordingAct) {
        if (!Task.PrecedentRecording.Transaction.IsEmptyInstance &&
            Task.PrecedentRecording.Transaction.PresentationTime > Task.Transaction.PresentationTime) {
          throw new LandRegistrationException(LandRegistrationException.Msg.PrecendentPresentationTimeIsAfterThisTransactionDate,
                                              Task.PrecedentRecording.FullNumber, 
                                              Task.PrecedentRecording.Transaction.PresentationTime,
                                              Task.Transaction.Key, Task.Transaction.PresentationTime);
        }
      }
    }

    internal RecordingAct DoRecording() {

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
          return this.DoStructureRecording();      // (e.g. fusión de predios [merge])
        }
      }

      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        if (Task.RecordingRule.IsMainRecording) {
          return this.DoRecordingActRecording();   // (e.g. fusión de predios [merge])
        }
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

    #endregion Public methods

    #region Recording methods

    private RecordingAct DoNoneResourceRecording() {
      Assertion.Assert(this.RecordingAct.TargetRecordingAct.IsEmptyInstance,
                      "TargetRecordingAct should be the empty instance.");
      Assertion.Assert(this.RecordingAct.TargetResource.IsEmptyInstance,
                       "TargetResource should be the empty instance.");

      RecordingBook book = this.GetRecordingBook();

      return this.RecordingAct.WriteOn(book);

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


      /// ORIGINAL CODE
      //this.GetRecordingBook());
      //RecordingBook book = this.GetRecordingBook(); 
      //Recording recording = book.CreateRecording();

      //return recording.CreateRecordingAct(Task.RecordingActType, Property.Empty);
    }

    private RecordingAct DoPropertyAnnotation() {
      if (this.ExecuteCreatePrecedentRecording) {
        this.CreatePrecedent();
      } else if (this.ExecuteCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      }
      var lastRecording = Task.TargetResource.LastRecordingAct.Recording;

      return lastRecording.CreateAnnotation(Task.Transaction, Task.RecordingActType,
                                            Task.TargetResource);
    }

    private RecordingAct DoPropertyRecording() {
      if (this.ExecuteCreatePrecedentRecording) {
        this.CreatePrecedent();
      } else if (this.ExecuteCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      }
      if (this.ApplyOverNewProperty) {
        Task.TargetResource = new Property();
      }

      RecordingBook book = this.GetRecordingBook();
      Recording recording = book.CreateRecording(Task.Transaction);

      return recording.CreateRecordingAct(Task.RecordingActType, Task.TargetResource);
    }

    private RecordingAct DoRecordingActAnnotation() {
      throw new NotImplementedException("DoRecordingActAnnotation");
    }

    private RecordingAct DoRecordingActCancelation() {
      throw new NotImplementedException("DoRecordingActCancelation");
    }

    private RecordingAct DoRecordingActRecording() {
      throw new NotImplementedException("DoRecordingActSectionRecording");
    }

    private RecordingAct DoStructureRecording() {
      return DoPropertyRecording();
    }

    #endregion Recording methods

    #region Private auxiliar methods

    private void CreatePrecedent() {
      if (!this.ExecuteCreatePrecedentRecording) {
        return;
      }
      Task.PrecedentRecording = Task.PrecedentRecordingBook.CreateQuickRecording(Task.QuickAddRecordingNumber,
                                                                                 Task.QuickAddBisRecordingSuffixTag);
      Task.TargetResource = Task.PrecedentRecording.RecordingActs[0].PropertiesEvents[0].Property;
      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        Task.TargetRecordingAct = Task.PrecedentRecording.RecordingActs[0];
      }
    }

    private void CreatePrecedentRecordingAct() {
      if (!this.ExecuteCreatePrecedentRecordingAct) {
        return;
      }
      var recordingAct = Task.PrecedentRecording.CreateRecordingAct(RecordingActType.Empty, new Property());
      Task.TargetResource = recordingAct.PropertiesEvents[0].Property;
      if (Task.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        Task.TargetRecordingAct = Task.PrecedentRecording.RecordingActs[0];
      }
    }
    private RecordingBook GetRecordingBook() {
      return RecordingBook.GetAssignedBookForRecording(this.Task.RecorderOffice, 
                                                       this.Task.RecordingRule.RecordingSection, 
                                                       this.Task.Document);
    }

    #endregion Private auxiliar methods

  }  // class RecorderExpert

}  // namespace Empiria.Land.Registration