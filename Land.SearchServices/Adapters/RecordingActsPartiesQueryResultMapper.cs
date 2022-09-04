/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Mapper class                            *
*  Type     : RecordingActsPartiesQueryResultMapper      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map recording act parties to their related query results.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods used to map recording act parties to their related query results.</summary>
  static internal class RecordingActsPartiesQueryResultMapper {

    #region Mappers

    static internal FixedList<RecordingActPartyQueryResultDto> MapForInternalUse(FixedList<RecordingActParty> list) {
      return list.Select((x) => MapForInternalUse(x))
                 .ToFixedList();
    }

    #endregion Mappers

    #region Helpers

    static private RecordingActPartyQueryResultDto MapForInternalUse(RecordingActParty actParty) {
      Record record = LandRecordsSearcher.GetRecordingActPartyRecord(actParty);

      return new RecordingActPartyQueryResultDto {
        UID = actParty.UID,
        Type = actParty.RoleType.ToString(),
        Party = PartyMapper.Map(actParty.Party),
        Role = actParty.PartyRole.Name,
        RecordingActType = actParty.RecordingAct.DisplayName,
        Status = actParty.Status.ToString(),
        RecordableSubject = RecordableSubjectQueryResultMapper.MapForInternalUse(actParty.RecordingAct.Resource),
        Record = RecordMapper.Map(record)
      };
    }

    #endregion Helpers

  }  // class RecordingActsPartiesQueryResultMapper

}  // namespace Empiria.Land.SearchServices
