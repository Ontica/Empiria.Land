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
      dto.Parties = PartyMapper.Map(recordingAct.GetParties());
      dto.Status = recordingAct.Status;
      dto.Actions = MapControlData(recordingAct);

      return dto;
    }

    #region Private methods

    static private RecordingActControlDataDto MapControlData(RecordingAct recordingAct) {
      var dto = new RecordingActControlDataDto();

      dto.IsEditable = recordingAct.IsEditable;

      dto.EditableFields = MapEditableFields(recordingAct);

      if (!recordingAct.IsEditable) {
        return dto;
      }

      dto.EditionValues = MapEditionValues(recordingAct, dto.EditableFields);

      return dto;
    }


    static private FixedList<string> MapEditableFields(RecordingAct recordingAct) {
      var recordingRule = recordingAct.RecordingActType.RecordingRule;

      var list = new List<string>();

      list.AddRange(recordingRule.Fields);

      if (!recordingRule.AllowNoParties) {
        list.Add("Participants");
      }

      if (recordingRule.Kinds.Length != 0) {
        list.Add("Kinds");
      }

      if (recordingAct.IsHistoric && recordingRule.ReplaceableBy.Count != 0) {
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

      if (editableFields.Contains("RecordingActType")) {
        dto.RecordingActTypes = RecordingActTypeMapper.MapToNamedEntityList(recordingRule.ReplaceableBy);
      }

      if (!editableFields.Contains("Participants")) {
        return dto;
      }


      dto.PartUnits = recordingAct.RecordingActType.GetPartyPartUnits()
                                                   .Select(x => x.MapToNamedEntity())
                                                   .ToFixedList();


      dto.PrimaryPartyRoles = recordingAct.RecordingActType.GetPrimaryRoles()
                                                           .Select(x => x.MapToNamedEntity())
                                                           .ToFixedList();

      dto.SecondaryPartyRoles = SecondaryPartyRole.GetList()
                                                  .Select(x => x.MapToNamedEntity())
                                                  .ToFixedList();

      return dto;
    }


    #endregion Private methods

  }  // class RecordingActMapper

}  // namespace Empiria.Land.Recording.UseCases
