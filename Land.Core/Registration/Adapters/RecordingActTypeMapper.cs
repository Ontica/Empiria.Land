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


    static internal FixedList<RecordingActTypeGroupDto> Map(FixedList<RecordingActTypeCategory> list,
                                                            bool forUseInTractIndexEdition) {
      return new FixedList<RecordingActTypeGroupDto>(list.Select((x) => Map(x, forUseInTractIndexEdition)));
    }


    static internal FixedList<NamedEntityDto> MapToNamedEntityList(FixedList<RecordingActType> list) {
      return new FixedList<NamedEntityDto>(list.Select((x) => new NamedEntityDto(x.UID, x.DisplayName)));

    }

    #endregion Public methods

    #region Helpers

    static private RecordingActTypeGroupDto Map(RecordingActTypeCategory group,
                                                bool forUseInTractIndexEdition) {

      return new RecordingActTypeGroupDto {
        UID = group.UID,
        Name = group.Name,
        RecordingActTypes = group.RecordingActTypes.Select(x => Map(x, forUseInTractIndexEdition))
                                                   .ToFixedList()
      };
    }


    static private RecordingActTypeDto Map(RecordingActType recordingActType,
                                           bool forUseInTractIndexEdition) {
      return new RecordingActTypeDto {
        UID = recordingActType.UID,
        Name = recordingActType.DisplayName,
        RegistrationCommands = MapRegistrationCommandTypes(recordingActType, forUseInTractIndexEdition)
      };
    }


    static private FixedList<RegistrationCommandDto> MapRegistrationCommandTypes(RecordingActType recordingActType,
                                                                                 bool forUseInTractIndexEdition) {

      FixedList<RegistrationCommandType> commandTypes;

      if (forUseInTractIndexEdition) {
        commandTypes = recordingActType.TractIndexRegistrationCommandTypes();
      } else {
        commandTypes = recordingActType.RegistrationCommandTypes();
      }

      return commandTypes.Select((x) => MapRegistrationCommandType(x))
                         .ToFixedList();
    }


    static private RegistrationCommandDto MapRegistrationCommandType(RegistrationCommandType commandType) {
      return new RegistrationCommandDto {
        UID = commandType.ToString(),
        Name = commandType.Name(),
        Rules = commandType.Rules()
      };
    }

    #endregion Helpers

  }  // class RecordingActTypeMapper

}  // namespace Empiria.Land.Registration.Adapters
