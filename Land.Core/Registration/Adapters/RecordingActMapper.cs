/* Empiria Land **********************************************************************************************
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
      dto.Participants = MapParticipants(recordingAct.GetParties());
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

      dto.MainParticipantRoles = recordingAct.RecordingActType.GetPrimaryRoles()
                                                              .Select(x => new NamedEntityDto(x.UID, x.Name))
                                                              .ToList()
                                                              .ToFixedList();

      dto.SecondaryParticipantRoles = SecondaryPartyRole.GetList()
                                                        .Select(x => new NamedEntityDto(x.UID, x.Name))
                                                        .ToList()
                                                        .ToFixedList();

      return dto;
    }


    static private FixedList<ParticipantDto> MapParticipants(FixedList<RecordingActParty> list) {
      return new FixedList<ParticipantDto>(list.Select((x) => MapParticipant(x)));
    }


    static private ParticipantDto MapParticipant(RecordingActParty participant) {
      return new ParticipantDto {
        UID = participant.UID,
        //CURP = participant.Party.CURP,
        //Kind = participant.Party.Kind,
        //RFC = participant.Party.RFC,
        AssociatedWith = new NamedEntityDto(participant.PartyOf.UID, participant.PartyOf.FullName),
        RoleType = participant.PartyRole.UID,
        Role = participant.PartyRole.Name,
        FullName = participant.Party.FullName
      };
    }

    #endregion Private methods

  }  // class RecordingActMapper

}  // namespace Empiria.Land.Recording.UseCases
