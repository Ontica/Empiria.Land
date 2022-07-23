/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordingActTypeMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for recording act types.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  static internal class RecordingActTypeMapper {

    #region Public methods


    static internal FixedList<RecordingActTypeGroupDto> Map(ApplicableRecordingActTypeList list) {
      FixedList<INamedEntity> groups = list.GetGroups();

      return groups.Select(group => Map(group, list.GetGroupItems(group)))
                   .ToFixedList();
    }


    static internal FixedList<NamedEntityDto> MapToNamedEntityList(FixedList<RecordingActType> list) {
      return list.Select((x) => new NamedEntityDto(x.UID, x.DisplayName))
                 .ToFixedList();
    }

    #endregion Public methods

    #region Helpers

    static private RecordingActTypeGroupDto Map(INamedEntity group,
                                                FixedList<ApplicableRecordingActType> applicableActs) {
      return new RecordingActTypeGroupDto {
        UID = group.UID,
        Name = group.Name,
        RecordingActTypes = applicableActs.Select(applicableActType => Map(applicableActType))
                                          .ToFixedList()
      };
    }

    static private RecordingActTypeDto Map(ApplicableRecordingActType actType) {
      return new RecordingActTypeDto {
        UID = actType.RecordingActType.UID,
        Name = actType.RecordingActType.DisplayName,
        RegistrationCommands = actType.Commands.Select(command => Map(command))
                                               .ToFixedList()
      };
    }

    static private RegistrationCommandDto Map(RegistrationCommandType commandType) {
      return new RegistrationCommandDto {
        UID = commandType.ToString(),
        Name = commandType.Name(),
        Rules = commandType.Rules()
      };
    }

    #endregion Helpers

  }  // class RecordingActTypeMapper

}  // namespace Empiria.Land.Registration.Adapters
