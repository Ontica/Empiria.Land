/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingTask                                  Pattern  : Standard Class                      *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains data about a recording instruction. A recording task it's needed to create           *
*              recording acts. RecordingTasks generally are filled by the user services.                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Empiria.DataTypes;
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
                         int precedentRecordingId = -1, int precedentResourceId = -1,
                         int quickAddRecordingNumber = -1,
                         string quickAddRecordingSubnumber = "", string quickAddRecordingSuffixTag = "",
                         PropertyPartition partition = null, RecordingActInfo targetActInfo = null) {
      this.Transaction = LRSTransaction.Parse(transactionId);
      this.Document = RecordingDocument.Parse(documentId);
      this.RecorderOffice = RecorderOffice.Parse(recorderOfficeId);
      this.RecordingActTypeCategory = RecordingActTypeCategory.Parse(recordingActTypeCategoryId);
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.PropertyRecordingType = propertyType;
      this.PrecedentRecordingBook = RecordingBook.Parse(precedentRecordingBookId);
      this.PrecedentRecording = Recording.Parse(precedentRecordingId);
      if (precedentResourceId == 0) {
        this.PrecedentProperty = new Property();
      } else if (precedentResourceId == -1) {
        this.PrecedentProperty = Property.Empty;
      } else {
        this.PrecedentProperty = Property.Parse(precedentResourceId);
      }
      this.QuickAddRecordingNumber = quickAddRecordingNumber;
      this.QuickAddRecordingSubNumber = quickAddRecordingSubnumber;
      this.QuickAddRecordingSuffixTag = quickAddRecordingSuffixTag;

      this.RecordingRule = RecordingActType.RecordingRule;

      if (!RecordingRule.FixedRecorderOffice.IsEmptyInstance) {
        this.RecorderOffice = RecordingRule.FixedRecorderOffice;
      } else if (!this.PrecedentRecordingBook.IsEmptyInstance) {
        RecorderOffice = this.PrecedentRecordingBook.RecorderOffice;
      }
      if (partition != null) {
        this.PartitionInfo = partition;
      } else {
        this.PartitionInfo = new PropertyPartition();
      }
      if (targetActInfo != null) {
        this.TargetActInfo = targetActInfo;
      } else {
        this.TargetActInfo = RecordingActInfo.Empty;
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

    public Property PrecedentProperty {
      get;
      internal set;
    }

    public int QuickAddRecordingNumber {
      get;
      private set;
    }

    public string QuickAddRecordingSubNumber {
      get;
      private set;
    }

    public string QuickAddRecordingSuffixTag {
      get;
      private set;
    }

    public RecordingRule RecordingRule {
      get;
      private set;
    }

    public PropertyPartition PartitionInfo {
      get;
      private set;
    }
    
    public RecordingActInfo TargetActInfo {
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
