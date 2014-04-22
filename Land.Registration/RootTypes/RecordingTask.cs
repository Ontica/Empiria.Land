/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingTask                                  Pattern  : Standard Class                      *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains data about a recording instruction. A recording task it's needed to create           *
*              recording acts. RecordingTasks generally are filled by the user services.                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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

  /// <summary>Contains data about a recording instruction. A recording task it's needed to create 
  /// recording acts. RecordingTasks generally are filled by the user services.</summary>
  public class RecordingTask {

    #region Constructors and parsers

    public RecordingTask(int transactionId = -1, int documentId = -1,
                         int recordingActTypeCategoryId = -1, int recordingActTypeId = -1,
                         PropertyRecordingType propertyType = PropertyRecordingType.actNotApplyToProperty,
                         int recorderOfficeId = -1, int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int targetResourceId = -1,
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
      if (targetResourceId == 0) {
        this.TargetProperty = new Property();
      } else if (targetResourceId == -1) {
        this.TargetProperty = Property.Empty;
      } else {
        this.TargetProperty = Property.Parse(targetResourceId);
      }
      if (targetRecordingActId != -1) {
        this.TargetRecordingAct = RecordingAct.Parse(targetRecordingActId);
      } else {
        this.TargetRecordingAct = InformationAct.Empty;
      }

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
      internal set;
    }

    public Property TargetProperty { 
      get;
      internal set;
    }

    public RecordingAct TargetRecordingAct { 
      get;
      internal set;
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

    #endregion Properties

    #region Public methods

    public void AssertValid() {
      var expert = new RecorderExpert(this);

      expert.AssertValidTask();
    }

    #endregion Public methods

  }  // class RecordingTask

}  // namespace Empiria.Land.Registration