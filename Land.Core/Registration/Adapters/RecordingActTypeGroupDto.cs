/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingActTypeGroupDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO of a group of recording act types.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Adapters {

  /// <summary> Output DTO of a group of recording act types.</summary>
  public class RecordingActTypeGroupDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public FixedList<RecordingActTypeDto> RecordingActTypes {
      get; internal set;
    }

  }  // class RecordingActTypeGroupDto

}  // namespace Empiria.Land.Registration.Adapters
