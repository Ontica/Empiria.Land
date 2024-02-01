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


  /// <summary>Used to control the user interface. Describes how the recording act
  /// applies to resources.</summary>
  public enum RecordingTaskType {
    actAppliesToOtherRecordingAct,
    actNotApplyToProperty,
    actAppliesToDocument,
    createProperty,
    createPropertyOnAntecedent,
    createPartitionAndPropertyOnAntecedent,
    createPartition,
    selectProperty,
  }


  public class RecordingTaskFields {

    public RecordingTaskType RecordingTaskType {
      get; set;
    } = RecordingTaskType.actNotApplyToProperty;


    public string LandRecordUID {
      get; set;
    } = string.Empty;


    public string RecordingBookEntryUID {
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


    public string PrecedentBookEntryUID {
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
      Assertion.Require(fields, nameof(fields));

      this.RecordingTaskType = fields.RecordingTaskType;
      this.LandRecord = RecordingDocument.ParseGuid(fields.LandRecordUID);

      if (!String.IsNullOrWhiteSpace(fields.RecordingBookEntryUID)) {
        this.BookEntry = BookEntry.Parse(fields.RecordingBookEntryUID);
      }

      this.RecordingActType = RecordingActType.Parse(fields.RecordingActTypeUID);

      if (!String.IsNullOrWhiteSpace(fields.RecordableSubjectUID)) {
        this.PrecedentProperty = Resource.ParseGuid(fields.RecordableSubjectUID);
      }

      if (!String.IsNullOrWhiteSpace(fields.PrecedentBookEntryUID)) {
        this.PrecedentBookEntry = BookEntry.Parse(fields.PrecedentBookEntryUID);
      }

      if (this.RecordingTaskType == RecordingTaskType.createPartition ||
          this.RecordingTaskType == RecordingTaskType.createPartitionAndPropertyOnAntecedent) {
        this.PartitionInfo = new RealEstatePartitionDTO(fields.PartitionType, fields.PartitionNo, String.Empty);
      }

      if (!String.IsNullOrWhiteSpace(fields.TargetRecordingActUID)) {
        this.TargetActInfo = new RecordingActInfoDTO(fields.TargetRecordingActUID);
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public RecordingDocument LandRecord {
      get;
      private set;
    }

    public BookEntry BookEntry {
      get;
      internal set;
    } = BookEntry.Empty;


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


    public BookEntry PrecedentBookEntry {
      get;
      internal set;
    } = BookEntry.Empty;


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


    public RealEstatePartitionDTO PartitionInfo {
      get;
      private set;
    } = RealEstatePartitionDTO.Empty;


    internal RecordingActInfoDTO TargetActInfo {
      get;
      private set;
    } = RecordingActInfoDTO.Empty;


    #endregion Properties

  }  // class RecordingTask

}  // namespace Empiria.Land.Registration
