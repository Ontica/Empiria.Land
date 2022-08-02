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

    internal FixedList<LandMediaPosting> GetFiles(LandMediaContent mediaContent, BaseObject instance) {
      string filter = BuildFilter(mediaContent, instance);

      return GetMediaFiles(filter);
    }

    private string BuildFilter(LandMediaContent mediaContent, BaseObject instance) {
      return $"[PhysicalRecordingId] = {instance.Id}";
    }


    private FixedList<LandMediaPosting> GetMediaFiles(string filter) {
      string sql = "SELECT * FROM LRSMediaPostings " +
                   $"WHERE ({filter}) AND MediaPostingStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LandMediaPosting>(op);
    }


    static internal void WriteMediaPosting(LandMediaPosting o) {
      var op = DataOperation.Parse("writeLRSMediaPosting",
               o.Id, o.UID, o.StorageItem.Id, o.ImagingControlID, o.Keywords,
               o.ExtensionData.ToString(), o.Transaction.Id, o.Instrument.Id,
               o.InstrumentRecording.Id, o.RecordingBook.Id, o.BookEntry.Id,
               o.BookEntryNo, o.ExternalTransactionId, o.PostingTime, o.PostedBy.Id,
               (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

  }  // class LandMediaFilesRepository

}  // namespace Empiria.Land.Media
