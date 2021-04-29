/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingActDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for a recording act with it associated subject and its participants.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO for a recording act with it associated subject and its participants.</summary>
  public class RecordingActDto {

    internal RecordingActDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Kind {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

    public decimal OperationAmount {
      get; internal set;
    }

    public string CurrencyUID {
      get; internal set;
    }

    public NamedEntityDto RecordableSubject {
      get; internal set;
    }

    public FixedList<RecordingActPartyDto> Parties {
      get; internal set;
    }

    public RecordableObjectStatus Status {
      get; internal set;
    }

    public RecordingActControlDataDto Actions {
      get; internal set;
    }

  }  // class RecordingActDto



  /// <summary>Output DTO with control data for a recording act.</summary>
  public class RecordingActControlDataDto {

    internal RecordingActControlDataDto() {
      // no-op
    }

    public bool IsEditable {
      get; internal set;
    }

    public FixedList<string> EditableFields {
      get; internal set;
    } = new FixedList<string>();


    public RecordingActEditionValuesDto EditionValues {
      get; internal set;
    } = new RecordingActEditionValuesDto();

  }  // class RecordingActControlDataDto



  /// <summary>Output DTO with values used for recording act edition.</summary>
  public class RecordingActEditionValuesDto {

    internal RecordingActEditionValuesDto() {
      // no-op
    }

    public FixedList<NamedEntityDto> RecordingActTypes {
      get; internal set;
    }

    public FixedList<string> Kinds {
      get; internal set;
    }

    public FixedList<NamedEntityDto> Currencies {
      get; internal set;
    }

    public FixedList<NamedEntityDto> PrimaryPartyRoles {
      get; internal set;
    }

    public FixedList<NamedEntityDto> SecondaryPartyRoles {
      get; internal set;
    }

  }  // class RecordingActEditionValuesDto



  /// <summary>Output DTO for a recording act with minimal information.</summary>
  public class RecordingActEntryDto {

    internal RecordingActEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Antecedent {
      get; internal set;
    }

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }

  }  // class RecordingActEntryDto

}  // namespace Empiria.Land.Registration.Adapters
