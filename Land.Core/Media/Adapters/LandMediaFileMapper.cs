/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : LandMediaFileMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map Land media files to LandMediaFileDto objects.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Media.Adapters {

  /// <summary>Methods used to map Land media files to LandMediaFileDto objects.</summary>
  static internal class LandMediaFileMapper {

    static internal LandMediaFileDto[] Map(LandMediaFileSet mediaFileSet) {
      FixedList<LandMediaFile> mediaFiles = mediaFileSet.GetFiles();

      LandMediaFileDto[] array = new LandMediaFileDto[mediaFiles.Count];

      for (int i = 0; i < mediaFiles.Count; i++) {
        array[i] = Map(mediaFiles[i]);
      }

      return array;
    }


    static public LandMediaFileDto Map(LandMediaFile mediaFile) {
      return new LandMediaFileDto {
        UID = mediaFile.UID,
        Type = mediaFile.MediaType,
        Content = mediaFile.MediaContent,
        Name = mediaFile.OriginalFileName,
        Url = mediaFile.Url,
        Size = mediaFile.Length
      };
    }

  }  // class LandMediaFileMapper

}  // namespace Empiria.Land.Media.Adapters
