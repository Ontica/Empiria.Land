/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingTask                                  Pattern  : Standard Class                      *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains data about a recording instruction. A recording task it's needed to create           *
*              recording acts. RecordingTasks generally are filled by the user services.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {


  /// <summary>Used to control the user interface. Describes how the recording act applies to resources.</summary>
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

    public RecordingTask(int documentId = -1,
                         int recordingActTypeId = -1,
                         RecordingTaskType recordingTaskType = RecordingTaskType.actNotApplyToProperty,
                         int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int precedentResourceId = -1,
                         string quickAddRecordingNumber = "",
                         string resourceName = "", string cadastralKey = "",
                         RealEstatePartitionDTO partition = null, RecordingActInfoDTO targetActInfo = null) {
      this.Document = RecordingDocument.Parse(documentId);
      //this.RecordingActTypeCategory = RecordingActTypeCategory.Parse(recordingActTypeCategoryId);
      this.RecordingActType = RecordingActType.Parse(recordingActTypeId);
      this.RecordingTaskType = recordingTaskType;
      this.ResourceName = EmpiriaString.TrimAll(resourceName);
      this.CadastralKey = cadastralKey;
      this.PrecedentRecordingBook = RecordingBook.Parse(precedentRecordingBookId);
      this.PrecedentRecording = PhysicalRecording.Parse(precedentRecordingId);

      if (precedentResourceId == 0) {
        var data = new RealEstateExtData() { CadastralKey = cadastralKey };

        this.PrecedentProperty = new RealEstate(data);
      } else if (precedentResourceId == -1) {
        this.PrecedentProperty = Resource.Empty;
      } else {
        this.PrecedentProperty = Resource.Parse(precedentResourceId);
      }

      this.QuickAddRecordingNumber = quickAddRecordingNumber;

      if (partition != null) {
        this.PartitionInfo = partition;
      } else {
        this.PartitionInfo = RealEstatePartitionDTO.Empty;
      }
      if (targetActInfo != null) {
        this.TargetActInfo = targetActInfo;
      } else {
        this.TargetActInfo = RecordingActInfoDTO.Empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

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

    public PhysicalRecording PrecedentRecording {
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

    public RealEstatePartitionDTO PartitionInfo {
      get;
      private set;
    }

    internal RecordingActInfoDTO TargetActInfo {
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
