/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Documents Recording                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Data Transfer Object                    *
*  Type     : RecordedDocumentDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data related to a recorded document.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Recording.UseCases {

  /// <summary>Holds data related to a recorded document.</summary>
  public class RecordedDocumentDto {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get;
      internal set;
    }

    public string Summary {
      get; internal set;
    }

    public FixedList<RecordingActDto> RecordingActs {
      get; internal set;
    }

  }  // class RecordedDocumentDto

}  // namespace Empiria.Land.Recording.UseCases
