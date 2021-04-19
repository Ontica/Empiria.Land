/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : LandMediaFileDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing an Empiria Land system's media file.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Media.Adapters {

  /// <summary>Output DTO with data representing an Empiria Land system's media file.</summary>
  public class LandMediaFileDto {

    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Content {
      get; internal set;
    }


    public string Url {
      get; internal set;
    }


    public int Size {
      get; internal set;
    }


  }  // class LandMediaFileDto

}  // namespace Empiria.Land.Media.Adapters
