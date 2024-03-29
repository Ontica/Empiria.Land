﻿/* Empiria Land **********************************************************************************************
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
  static public class LandMediaFileMapper {


    static public FixedList<LandMediaFileDto> Map(FixedList<LandMediaPosting> list) {
      return list.Select(x => Map(x))
                 .ToFixedList();
    }


    static internal LandMediaFileDto Map(LandMediaPosting mediaPosting) {
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
