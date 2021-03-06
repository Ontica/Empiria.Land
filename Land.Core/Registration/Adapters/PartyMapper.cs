﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Instruments Recording                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : PartyMapper                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods used to map recording act parties.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Contains methods used to map recording act parties.</summary>
  static internal class PartyMapper {


    static public FixedList<RecordingActPartyDto> Map(FixedList<RecordingActParty> list) {
      return new FixedList<RecordingActPartyDto>(list.Select((x) => MapRecordingActParty(x)));
    }


    static public FixedList<PartyDto> Map(FixedList<Party> list) {
      return new FixedList<PartyDto>(list.Select((x) => MapParty(x)));
    }


    #region Private methods

    static private RecordingActPartyDto MapRecordingActParty(RecordingActParty recordingActParty) {
      return new RecordingActPartyDto {
        UID = recordingActParty.UID,
        Type = recordingActParty.RoleType,
        Party = MapParty(recordingActParty.Party),
        Role = recordingActParty.PartyRole.MapToNamedEntity(),
        PartAmount = recordingActParty.OwnershipPart.Amount,
        PartUnit = recordingActParty.OwnershipPart.Unit.MapToNamedEntity(),
        AssociatedWith = MapParty(recordingActParty.PartyOf),
        Notes = recordingActParty.Notes,
      };
    }


    static private PartyDto MapParty(Party party) {
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

    #endregion Private methods

  }  // class PartyMapper

}  // namespace Empiria.Land.Recording.UseCases
