/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Instruments Recording                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : PartyMapper                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods used to map recording act parties.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Measurement;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Contains methods used to map recording act parties.</summary>
  static public class PartyMapper {


    static public FixedList<RecordingActPartyDto> Map(FixedList<RecordingActParty> list) {
      return list.Select((x) => MapRecordingActParty(x))
                 .ToFixedList();
    }


    static public FixedList<PartyDto> Map(FixedList<Party> list) {
      return list.Select((x) => Map(x))
                 .ToFixedList();
    }


    static public PartyDto Map(Party party) {
      if (party.IsEmptyInstance) {
        return null;
      }

      return new PartyDto {
        UID = party.UID,
        FullName = party.FullName,
        Type = party is HumanParty ? PartyType.Person : PartyType.Organization,
        RFC = party.RFC,
        CURP = party is HumanParty ? (party as HumanParty).CURP : string.Empty,
        Notes = party.Notes
      };
    }

    #region Private methods

    static private RecordingActPartyDto MapRecordingActParty(RecordingActParty recordingActParty) {
      return new RecordingActPartyDto {
        UID = recordingActParty.UID,
        Type = recordingActParty.RoleType,
        Party = Map(recordingActParty.Party),
        Role = recordingActParty.PartyRole.MapToNamedEntity(),
        PartAmount = ToPartAmountText(recordingActParty.OwnershipPart),
        PartUnit = recordingActParty.OwnershipPart.Unit.MapToNamedEntity(),
        AssociatedWith = Map(recordingActParty.PartyOf),
        Notes = recordingActParty.Notes,
      };
    }

    static private string ToPartAmountText(Quantity ownershipPart) {
      if (ownershipPart.Unit.UID == "Unit.Fraction") {
        var fractionParts = ownershipPart.Amount.ToString().Split('.');

        return fractionParts[0] + "/" + fractionParts[1].TrimEnd('0');
      }

      return ownershipPart.Amount.ToString();
    }

    #endregion Private methods

  }  // class PartyMapper

}  // namespace Empiria.Land.Recording.UseCases
