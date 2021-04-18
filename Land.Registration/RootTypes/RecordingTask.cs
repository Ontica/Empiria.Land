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


  public class RecordingTaskFields {

    public RecordingTaskType RecordingTaskType {
      get; set;
    } = RecordingTaskType.actNotApplyToProperty;


    public string RecordingDocumentUID {
      get; set;
    } = string.Empty;


    public string RecordingActTypeUID {
      get; set;
    } = string.Empty;


    public string RecordableSubjectUID {
      get; set;
    } = string.Empty;


    public string TargetRecordingActUID {
      get; set;
    } = string.Empty;


    public string RecordingBookUID {
      get; set;
    } = string.Empty;


    public string BookEntryUID {
      get; set;
    } = string.Empty;


    public string PartitionType {
      get; set;
    } = string.Empty;


    public string PartitionNo {
      get; set;
    } = string.Empty;

  }


  /// <summary>Contains data about a recording instruction. A recording task it's needed to create
  /// recording acts. RecordingTasks generally are filled by the user services.</summary>
  public class RecordingTask {

    #region Constructors and parsers

    public RecordingTask(RecordingTaskFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.RecordingTaskType = fields.RecordingTaskType;
      this.Document = RecordingDocument.ParseGuid(fields.RecordingDocumentUID);

      this.RecordingActType = RecordingActType.Parse(fields.RecordingActTypeUID);

      if (!String.IsNullOrWhiteSpace(fields.RecordableSubjectUID)) {
        this.PrecedentProperty = Resource.ParseGuid(fields.RecordableSubjectUID);
      }

      if (!String.IsNullOrWhiteSpace(fields.RecordingBookUID)) {
        this.PrecedentRecordingBook = RecordingBook.Parse(fields.RecordingBookUID);
      }

      if (!String.IsNullOrWhiteSpace(fields.BookEntryUID)) {
        this.PrecedentRecording = PhysicalRecording.Parse(fields.BookEntryUID);
      }

      if (this.RecordingTaskType == RecordingTaskType.createPartition) {
        this.PartitionInfo = new RealEstatePartitionDTO(fields.PartitionType, fields.PartitionNo, String.Empty);
      }
    }


    public RecordingTask(int documentId = -1,
                         int recordingActTypeId = -1,
                         RecordingTaskType recordingTaskType = RecordingTaskType.actNotApplyToProperty,
                         int precedentRecordingBookId = -1,
                         int precedentRecordingId = -1, int precedentResourceId = -1,
                         string quickAddRecordingNumber = "",
                         string resourceName = "", string cadastralKey = "",
                         RealEstatePartitionDTO partition = null, RecordingActInfoDTO targetActInfo = null) {
      this.Document = RecordingDocument.Parse(documentId);
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
    } = RecordingBook.Empty;


    public PhysicalRecording PrecedentRecording {
      get;
      internal set;
    } = PhysicalRecording.Empty;


    public string ResourceName {
      get;
      private set;
    } = string.Empty;


    public string CadastralKey {
      get;
      private set;
    } = string.Empty;


    public Resource PrecedentProperty {
      get;
      internal set;
    } = Resource.Empty;


    public string QuickAddRecordingNumber {
      get;
      private set;
    } = string.Empty;


    public RealEstatePartitionDTO PartitionInfo {
      get;
      private set;
    } = RealEstatePartitionDTO.Empty;


    internal RecordingActInfoDTO TargetActInfo {
      get;
      private set;
    } = RecordingActInfoDTO.Empty;


    #endregion Properties

    #region Public methods

    public void AssertValid() {
      var expert = new RecorderExpert(this);

      expert.AssertValidTask();
    }

    #endregion Public methods

  }  // class RecordingTask

}  // namespace Empiria.Land.Registration
