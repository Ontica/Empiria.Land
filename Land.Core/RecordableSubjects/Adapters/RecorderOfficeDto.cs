/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecorderOfficeDto                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with information about a recorder office used for recordable subjects registration. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO with information about a recorder office used
  /// for recordable subjects registration.</summary>
  public class RecorderOfficeDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string Name {
      get; internal set;
    } = string.Empty;


    public FixedList<NamedEntityDto> Municipalities {
      get; internal set;
    } = new FixedList<NamedEntityDto>();


    public FixedList<NamedEntityDto> RecordingSections {
      get; internal set;
    } = new FixedList<NamedEntityDto>();


  }  // class RecorderOfficeDto

}  // namespace Empiria.Land.RecordableSubjects.Adapters
