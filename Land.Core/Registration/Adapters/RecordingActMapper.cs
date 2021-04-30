﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Instruments Recording                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordingActMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods used to map recording acts.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.Land.Registration.Adapters {


/// <summary>Contains methods used to map recording acts.</summary>
static internal class RecordingActMapper {


    static internal RecordingActDto Map(RecordingAct recordingAct) {
      var dto = new RecordingActDto();

      dto.UID = recordingAct.UID;
      dto.Type = recordingAct.RecordingActType.Name;
      dto.Name = recordingAct.DisplayName;
      dto.Kind = recordingAct.Kind;
      dto.Description = recordingAct.Summary;
      dto.OperationAmount = recordingAct.OperationAmount;
      dto.CurrencyUID = recordingAct.OperationCurrency.UID;
      dto.RecordableSubject = new NamedEntityDto(recordingAct.Resource.UID,
                                                 recordingAct.Resource.Description);
      dto.Parties = MapParticipants(recordingAct.GetParties());
      dto.Status = recordingAct.Status;
      dto.Actions = MapControlData(recordingAct);

      return dto;
    }

    #region Private methods

    static private RecordingActControlDataDto MapControlData(RecordingAct recordingAct) {
      var dto = new RecordingActControlDataDto();

      dto.IsEditable = recordingAct.IsEditable;

      if (!recordingAct.IsEditable) {
        return dto;
      }

      dto.EditableFields = MapEditableFields(recordingAct.RecordingActType);
      dto.EditionValues = MapEditionValues(recordingAct, dto.EditableFields);

      return dto;
    }


    static private FixedList<string> MapEditableFields(RecordingActType recordingActType) {
      var recordingRule = recordingActType.RecordingRule;

      var list = new List<string>();

      list.AddRange(recordingRule.Fields);

      if (!recordingRule.AllowNoParties) {
        list.Add("Participants");
      }

      if (recordingRule.Kinds.Length != 0) {
        list.Add("Kinds");
      }

      if (recordingRule.ReplaceableBy.Count != 0) {
        list.Add("RecordingActType");
      }

      return list.ToFixedList();
    }


    static private RecordingActEditionValuesDto MapEditionValues(RecordingAct recordingAct,
                                                                 FixedList<string> editableFields) {
      var dto = new RecordingActEditionValuesDto();

      var recordingRule = recordingAct.RecordingActType.RecordingRule;

      if (editableFields.Contains("OperationAmount")) {
        dto.Currencies = recordingRule.Currencies.MapToNamedEntityList();
      }

      if (editableFields.Contains("Kinds")) {
        dto.Kinds = new FixedList<string>(recordingRule.Kinds);
        dto.Kinds.Sort((x, y)  => x.CompareTo(y));
      }

      if (recordingRule.ReplaceableBy.Count != 0) {
        dto.RecordingActTypes = RecordingActTypeMapper.MapToNamedEntityList(recordingRule.ReplaceableBy);
      }

      if (!editableFields.Contains("Participants")) {
        return dto;
      }


      dto.PartUnits = recordingAct.RecordingActType.GetPartyPartUnits()
                                                   .Select(x => x.MapToNamedEntity())
                                                   .ToList()
                                                   .ToFixedList();


      dto.PrimaryPartyRoles = recordingAct.RecordingActType.GetPrimaryRoles()
                                                           .Select(x => x.MapToNamedEntity())
                                                           .ToList()
                                                           .ToFixedList();

      dto.SecondaryPartyRoles = SecondaryPartyRole.GetList()
                                                  .Select(x => x.MapToNamedEntity())
                                                  .ToList()
                                                  .ToFixedList();

      return dto;
    }


    static private FixedList<RecordingActPartyDto> MapParticipants(FixedList<RecordingActParty> list) {
      return new FixedList<RecordingActPartyDto>(list.Select((x) => MapParticipant(x)));
    }


    static private RecordingActPartyDto MapParticipant(RecordingActParty recordingActParty) {
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

    private static PartyDto MapParty(Party party) {
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

  }  // class RecordingActMapper

}  // namespace Empiria.Land.Recording.UseCases
