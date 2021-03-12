/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LandMediaFile                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : A media file related to an Empiria Land entity like instrument, transaction or recording book. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Media {

  /// <summary>A media file related to an Empiria Land entity like instrument, transaction or book.</summary>
  internal class LandMediaFile : MediaFile {

    protected LandMediaFile() {
      //  no-op
    }


    public new LandMediaContent MediaContent {
      get {
        return LandMediaFileFields.ConvertMediaContent(base.MediaContent);
      }
    }


  }  // class LandMediaFile

}  // namespace Empiria.Land.Media
