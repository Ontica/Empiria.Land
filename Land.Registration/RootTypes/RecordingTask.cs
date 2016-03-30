/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingTask                                  Pattern  : Standard Class                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains data about a recording instruction. A recording task it's needed to create           *
*              recording acts. RecordingTasks generally are filled by the user services.                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  public enum RecordingTaskType {
    actAppliesToOtherRecordingAct,
    actNotApplyToProperty,
    actAppliesToDocument,
    createProperty,
    createPartition,
    selectProperty,
  }

  /// <summary>Contains data about a recording instruction. A recording task it's needed to create
  /// recording acts. RecordingTasks generally are filled by the user services.</summary>
  public class RecordingTask {

    #region Constructors and parsers

    public RecordingTask(int transactionId = -1, int documentId = -1,
                         int recordingActTypeId = -1,
                         RecordingTaskType recordingTaskType = RecordingTaskType.actNotApplyToProperty,
                         int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int precedentResourceId = -1,
                         string quickAddRecordingNumber = "",
                         string resourceName = "", string cadastralKey = "",
                         RealEstatePartition partition = null, RecordingActInfo targetActInfo = null) {
      this.Transaction = LRSTransaction.Parse(transactionId);
      this.Document = RecordingDocument.Parse(documentId);
      //this.RecordingActTypeCategory = RecordingActTypeCategory.Parse(recordingActTypeCategoryId);
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.RecordingTaskType = recordingTaskType;
      this.ResourceName = EmpiriaString.TrimAll(resourceName);
      this.CadastralKey = cadastralKey;
      this.PrecedentRecordingBook = RecordingBook.Parse(precedentRecordingBookId);
      this.PrecedentRecording = Recording.Parse(precedentRecordingId);
      if (precedentResourceId == 0) {
        this.PrecedentProperty = new RealEstate(cadastralKey);
      } else if (precedentResourceId == -1) {
        this.PrecedentProperty = RealEstate.Empty;
      } else {
        this.PrecedentProperty = Resource.Parse(precedentResourceId);
      }

      this.QuickAddRecordingNumber = quickAddRecordingNumber;

      if (partition != null) {
        this.PartitionInfo = partition;
      } else {
        this.PartitionInfo = RealEstatePartition.Empty;
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

    //public RecordingActTypeCategory RecordingActTypeCategory {
    //  get;
    //  private set;
    //}

    public RecordingActType RecordingActType {
      get;
      private set;
    }

    public decimal RecordingActPercentage {
      get;
      private set;
    } = decimal.One;

    public RecordingTaskType RecordingTaskType {
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

    public string ResourceName {
      get;
      private set;
    }

    public string CadastralKey {
      get;
      private set;
    }

    public Resource PrecedentProperty {
      get;
      internal set;
    }

    public string QuickAddRecordingNumber {
      get;
      private set;
    }

    public RealEstatePartition PartitionInfo {
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
