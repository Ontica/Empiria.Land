/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingActPartyDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for a recording act party.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO for a recording act party.</summary>
  public class RecordingActPartyDto {

    internal RecordingActPartyDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public RecordingActPartyType Type {
      get; internal set;
    }

    public PartyDto Party {
      get; internal set;
    }

    public NamedEntityDto Role {
      get; internal set;
    }


    public string PartAmount {
      get; internal set;
    } = "1";


    public NamedEntityDto PartUnit {
      get; internal set;
    }


    public PartyDto AssociatedWith {
      get; internal set;
    } = new PartyDto();


    public string Notes {
      get; internal set;
    }

  }  // class RecordingActPartyDto



  /// <summary>Output DTO for a party (person or organization) associated with a recording act.</summary>
  public class PartyDto {

    internal PartyDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public PartyType Type {
      get; set;
    }

    public string FullName {
      get; internal set;
    }

    public string CURP {
      get; internal set;
    }

    public string RFC {
      get; internal set;
    }

    public string Notes {
      get; internal set;
    }

  }  // PartyDto

}  // namespace Empiria.Land.UseCases
