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

using Empiria.Storage;

namespace Empiria.Land.Media.Adapters {

  /// <summary>Methods used to map Land media files to LandMediaFileDto objects.</summary>
  static internal class LandMediaFileMapper {

    static internal FixedList<LandMediaFileDto> MapTests() {
      return new FixedList<LandMediaFileDto>();
    }

    static internal LandMediaFileDto[] Map(LandMediaFileSet mediaFileSet) {
      FixedList<LandMediaPosting> mediaFiles = mediaFileSet.GetFiles();

      LandMediaFileDto[] array = new LandMediaFileDto[mediaFiles.Count];

      for (int i = 0; i < mediaFiles.Count; i++) {
        array[i] = Map(mediaFiles[i]);
      }

      return array;
    }


    static public LandMediaFileDto Map(LandMediaPosting mediaPosting) {
      StorageFile file = (StorageFile) mediaPosting.StorageItem;

      return new LandMediaFileDto {
        UID = mediaPosting.UID,
        Type = FileType.Pdf.ToString(),
        Content = file.AppContentType,
        Name = file.OriginalFileName,
        Url = file.Url,
        Size = file.Size
      };
    }

  }  // class LandMediaFileMapper

}  // namespace Empiria.Land.Media.Adapters
