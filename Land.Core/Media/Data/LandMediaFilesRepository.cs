/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Repository                              *
*  Type     : LandMediaFilesRepository                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Serves as a repository of media files for Empiria Land entities.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Media {

  /// <summary>Serves as a repository of media files for Empiria Land entities.</summary>
  internal class LandMediaFilesRepository {

    public LandMediaFilesRepository() {
      // no-op
    }

    internal FixedList<LandMediaFile> GetFiles(LandMediaContent mediaContent, BaseObject instance) {
      string filter = BuildFilter(mediaContent, instance);

      return GetMediaFiles(filter);
    }

    private string BuildFilter(LandMediaContent mediaContent, BaseObject instance) {
      return $"[PhysicalRecordingId] = {instance.Id}";
    }


    private FixedList<LandMediaFile> GetMediaFiles(string filter) {
      string sql = "SELECT * FROM LRSMediaPostings " +
                   $"WHERE ({filter}) AND MediaPostingStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LandMediaFile>(op);
    }

  }  // class LandMediaFilesRepository

}  // namespace Empiria.Land.Media
