/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Interface adapters                      *
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


    static internal FixedList<RecordingActTypeGroupDto> Map(FixedList<RecordingActTypeCategory> list) {
      return new FixedList<RecordingActTypeGroupDto>(list.Select((x) => Map(x)));
    }


    static internal RecordingActTypeGroupDto Map(RecordingActTypeCategory group) {
      return new RecordingActTypeGroupDto {
        UID = group.UID,
        Name = group.Name,
        RecordingActTypes = Map(group.RecordingActTypes)
      };
    }


    static internal FixedList<RecordingActTypeDto> Map(FixedList<RecordingActType> list) {
      return new FixedList<RecordingActTypeDto>(list.Select((x) => Map(x)));
    }


    static internal RecordingActTypeDto Map(RecordingActType recordingActType) {
      return new RecordingActTypeDto {
        UID = recordingActType.UID,
        Name = recordingActType.DisplayName,
        SubjectCommands = MapSubjectCommands(recordingActType.SubjectCommandsList())
      };
    }


    #endregion Public methods

    #region Helper methods

    static private FixedList<NamedEntityDto> MapSubjectCommands(FixedList<RegistrationCommandType> list) {
      return new FixedList<NamedEntityDto>(list.Select((x) => MapSubjectCommand(x)));
    }


    static private NamedEntityDto MapSubjectCommand(RegistrationCommandType commandType) {
      return new NamedEntityDto(commandType.ToString(), commandType.Name());
    }

    #endregion Helper methods

  }  // class RecordingActTypeMapper

}  // namespace Empiria.Land.Registration.Adapters
