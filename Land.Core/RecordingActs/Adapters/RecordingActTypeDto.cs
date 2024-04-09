/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingActTypeDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with information about a recording act type.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Adapters {

  public class RecordingActTypeDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public FixedList<RegistrationCommandDto> RegistrationCommands {
      get; internal set;
    }

  }  // class RecordingActTypeDto

}  // namespace Empiria.Land.Registration.Adapters
