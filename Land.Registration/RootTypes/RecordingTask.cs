using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  public enum PropertyRecordingType {
    actAppliesToOtherRecordingAct,
    actNotApplyToProperty,
    actAppliesOnlyToSection,
    createProperty,
    selectProperty,
  }

  public class RecordingTask {

    #region Constructors and parsers

    public RecordingTask(int transactionId = -1, int documentId = -1,
                         int recordingActTypeCategoryId = -1, int recordingActTypeId = -1,
                         PropertyRecordingType propertyType = PropertyRecordingType.actNotApplyToProperty,
                         int recorderOfficeId = -1, int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int precedentPropertyId = -1,
                         int targetRecordingActId = -1, int quickAddRecordingNumber = -1,
                         string quickAddBisRecordingSuffixTag = "") {

      this.Transaction = LRSTransaction.Parse(transactionId);
      this.Document = RecordingDocument.Parse(documentId);
      this.RecorderOffice = RecorderOffice.Parse(recorderOfficeId);
      this.RecordingActTypeCategory = RecordingActTypeCategory.Parse(recordingActTypeCategoryId);
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);

      this.PropertyRecordingType = propertyType;
      this.PrecedentRecordingBook = RecordingBook.Parse(precedentRecordingBookId);
      this.PrecedentRecording = Recording.Parse(precedentRecordingId);
      if (precedentPropertyId == 0) {
        this.PrecedentProperty = new Property();
      } else if (precedentPropertyId == -1) {
        this.PrecedentProperty = Property.Empty;
      } else {
        this.PrecedentProperty = Property.Parse(precedentPropertyId);
      }
      this.TargetRecordingAct = RecordingAct.Parse(targetRecordingActId);
      this.QuickAddRecordingNumber = quickAddRecordingNumber;
      this.QuickAddBisRecordingSuffixTag = quickAddBisRecordingSuffixTag;

      this.RecordingRule = RecordingActType.RecordingRule;

      if (!RecordingRule.FixedRecorderOffice.IsEmptyInstance) {
        this.RecorderOffice = RecordingRule.FixedRecorderOffice;
      } else if (!this.PrecedentRecordingBook.IsEmptyInstance) {
        RecorderOffice = this.PrecedentRecordingBook.RecorderOffice;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public LRSTransaction Transaction { 
      get; 
      private set;
    }

    public RecordingDocument Document { 
      get; 
      private set;
    }
    
    public RecorderOffice RecorderOffice {
      get; 
      private set; 
    }

    public RecordingActTypeCategory RecordingActTypeCategory { 
      get; 
      private set;
    }
        
    public RecordingActType RecordingActType { 
      get; 
      private set;
    }

    public PropertyRecordingType PropertyRecordingType { 
      get; 
      private set;
    }
    
    public RecordingBook PrecedentRecordingBook { 
      get;
      private set;
    }
    
    public Recording PrecedentRecording {
      get;
      private set;
    }

    public Property PrecedentProperty { 
      get;
      private set;
    }

    public RecordingAct TargetRecordingAct { 
      get; 
      private set;
    }
    
    public int QuickAddRecordingNumber { 
      get; 
      private set;
    }

    public string QuickAddBisRecordingSuffixTag {
      get;
      private set;
    }

    public RecordingRule RecordingRule { 
      get;
      private set;
    }


    public bool ApplyOverNewProperty {
      get {
        return (this.RecordingRule.AppliesTo == RecordingRuleApplication.Property &&
                this.PropertyRecordingType == PropertyRecordingType.createProperty &&
                this.PrecedentProperty.IsEmptyInstance);
      }
    }

    public bool ExecuteCreatePrecedentRecording {
      get {
        return (this.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !this.PrecedentRecordingBook.IsEmptyInstance &&
                this.PrecedentRecording.IsEmptyInstance &&
                this.QuickAddRecordingNumber != -1);
      }
    }

    public bool ExecuteCreatePrecedentRecordingAct {
      get {
        return (this.PropertyRecordingType == PropertyRecordingType.selectProperty &&
                !this.PrecedentRecordingBook.IsEmptyInstance &&
                !this.PrecedentRecording.IsEmptyInstance &&
                this.PrecedentProperty.IsNew);
      }
    }

    #endregion Properties

    #region Public methods

    public void AssertValid() {
      if (ExecuteCreatePrecedentRecordingAct) {
        if (!this.PrecedentRecording.Transaction.IsEmptyInstance && 
            this.PrecedentRecording.Transaction.PresentationTime > this.Transaction.PresentationTime) {
              throw new LandRegistrationException(LandRegistrationException.Msg.PrecendentPresentationTimeIsAfterThisTransactionDate, 
                                                  this.PrecedentRecording.FullNumber, this.PrecedentRecording.Transaction.PresentationTime, 
                                                  this.Transaction.Key, this.Transaction.PresentationTime);
        }
      }
    }

    public RecordingAct DoRecording() {
      if (this.RecordingRule.IsAnnotation) {
        return DoAnnotation();
      } else if (this.RecordingRule.IsCancelation) {
        return DoCancelation();
      } else if (!this.RecordingRule.RecordingSection.IsEmptyInstance) {
        return DoSectionRecording();
      } else {
        throw new NotImplementedException("RecordingTask.DoRecording: recording act " +
                                          RecordingActType.Id + " has an undefined or wrong rule.");
      }
    }

    #endregion Public methods

    #region Private methods

    private void CreatePrecedent() {
      if (!this.ExecuteCreatePrecedentRecording) {
        return;
      }
      this.PrecedentRecording = this.PrecedentRecordingBook.CreateQuickRecording(this.QuickAddRecordingNumber, 
                                                                                 this.QuickAddBisRecordingSuffixTag);
      this.PrecedentProperty = this.PrecedentRecording.RecordingActs[0].PropertiesEvents[0].Property;
      if (this.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        this.TargetRecordingAct = this.PrecedentRecording.RecordingActs[0];
      }
    }

    private void CreatePrecedentRecordingAct() {
      if (!this.ExecuteCreatePrecedentRecordingAct) {
        return;
      }
      var recordingAct = this.PrecedentRecording.CreateRecordingAct(RecordingActType.Empty, new Property());
      this.PrecedentProperty = recordingAct.PropertiesEvents[0].Property;
      if (this.RecordingRule.AppliesTo == RecordingRuleApplication.RecordingAct) {
        this.TargetRecordingAct = this.PrecedentRecording.RecordingActs[0];
      }
    }

    private RecordingAct DoAnnotation() {
      switch (RecordingRule.AppliesTo) {
        case RecordingRuleApplication.RecordingAct:
          return DoRecordingActAnnotation();
        case RecordingRuleApplication.Property:
          return DoPropertyAnnotation();
        default:
          throw new NotImplementedException("RecordingTask.DoSectionRecording: recording act " +
                                            RecordingActType.Id + " has an undefined or wrong rule.");
      }
      throw new NotImplementedException("DoAnnotation");
    }

    private RecordingAct DoCancelation() {
      throw new NotImplementedException("DoCancelation");
    }

    private RecordingAct DoPropertyAnnotation() {
      if (this.ExecuteCreatePrecedentRecording) {
        this.CreatePrecedent();
      } else if (this.ExecuteCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      }
      var lastRecording = this.PrecedentProperty.LastRecordingAct.Recording;

      return lastRecording.CreateAnnotation(this.Transaction, this.RecordingActType, this.PrecedentProperty);
    }

    private RecordingAct DoRecordingActAnnotation() {
      throw new NotImplementedException("DoRecordingActAnnotation");
    }

    private RecordingAct DoPropertyRecording() {
      if (this.ExecuteCreatePrecedentRecording) {
        this.CreatePrecedent();
      } else if (this.ExecuteCreatePrecedentRecordingAct) {
        this.CreatePrecedentRecordingAct();
      }
      if (this.ApplyOverNewProperty) {
        this.PrecedentProperty = new Property();
      }
      var book = RecordingBook.GetAssignedBookForRecording(this.RecorderOffice,
                                                           this.RecordingRule.RecordingSection,
                                                           this.Transaction.Document);
      
      Recording recording = book.CreateRecording(this.Transaction);

      return recording.CreateRecordingAct(this.RecordingActType, this.PrecedentProperty);
    }

    private RecordingAct DoRecordingActRecording() {
      throw new NotImplementedException("DoRecordingActSectionRecording");
    }

    private RecordingAct DoSectionRecording() {
      switch (RecordingRule.AppliesTo) {
        case RecordingRuleApplication.None:
          return DoSimpleSectionRecording();
        case RecordingRuleApplication.RecordingAct:
          return DoRecordingActRecording();
        case RecordingRuleApplication.Property:
        case RecordingRuleApplication.Structure:
          return DoPropertyRecording();
        default:
          throw new NotImplementedException("RecordingTask.DoSectionRecording: recording act " +
                                            RecordingActType.Id + " has an undefined or a wrong rule.");
      }
    }

    private RecordingAct DoSimpleSectionRecording() {
      var book = RecordingBook.GetAssignedBookForRecording(this.RecorderOffice,
                                                           this.RecordingRule.RecordingSection,
                                                           this.Transaction.Document);
      Recording recording = book.CreateRecording(this.Transaction);
      return recording.CreateRecordingAct(this.RecordingActType, Property.Empty);
    }

    #endregion Private methods

  }  // class RecordingTask

}  // namespace Empiria.Land.Registration