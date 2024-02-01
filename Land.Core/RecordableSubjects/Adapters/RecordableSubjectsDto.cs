/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordableSubjectsDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTOs for all recordable subjects: real estate, associations and no-property subjects.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

namespace Empiria.Land.RecordableSubjects.Adapters {

  public enum RecordableSubjectType {

    None,

    RealEstate,

    Association,

    NoProperty

  }


  public class RecordingContextDto {

    internal RecordingContextDto() {
      // no-op
    }

    public RecordingContextDto(string landRecordUID, string recordingActUID) {
      Assertion.Require(landRecordUID, nameof(landRecordUID));
      Assertion.Require(recordingActUID, nameof(recordingActUID));

      InstrumentRecordingUID = landRecordUID;
      RecordingActUID = recordingActUID;
    }

    public string InstrumentRecordingUID {
      get; internal set;
    }

    public string RecordingActUID {
      get; internal set;
    }

  }  // class RecordingContextDto


  /// <summary>Output DTOs for all recordable subjects: real estate, associations
  /// and no-property subjects.</summary>
  public abstract class RecordableSubjectDto {

    public string UID {
      get; internal set;
    }

    public RecordableSubjectType Type {
      get; internal set;
    }

    public NamedEntityDto RecorderOffice {
      get; internal set;
    }

    public string ElectronicID {
      get; internal set;
    }

    public string Kind {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public RecordingContextDto RecordingContext {
      get; internal set;
    }

  }  // class RecordableSubjectDto


  /// <summary>Output DTO model for Real Estate.</summary>
  public class RealEstateDto : RecordableSubjectDto {

    public string CadastralID {
      get; internal set;
    }


    public DateTime CadastreLinkingDate {
      get; internal set;
    }


    public MediaData CadastralCardMedia {
      get; internal set;
    }


    public NamedEntityDto Municipality {
      get; internal set;
    }


    public decimal LotSize {
      get; internal set;
    }


    public NamedEntityDto LotSizeUnit {
      get; internal set;
    }


    public string MetesAndBounds {
      get; internal set;
    }


    public decimal BuildingArea {
      get; internal set;
    }


    public decimal UndividedPct {
      get; internal set;
    }


    public string Section {
      get; internal set;
    }


    public string Block {
      get; internal set;
    }


    public string Lot {
      get; internal set;
    }

  }  // class RealEstateDto



  /// <summary>Output DTO model for Assocations.</summary>
  public class AssociationDto : RecordableSubjectDto {


  }  // class AssociationDto


  /// <summary>Output DTO model for No-property resources.</summary>
  public class NoPropertyDto : RecordableSubjectDto {


  }  // class NoPropertyDto


}  // namespace Empiria.Land.RecordableSubjects.Adapters
