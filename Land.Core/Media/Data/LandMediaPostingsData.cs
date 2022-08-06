﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Services                           *
*  Type     : LandMediaPostingsData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read and write services for Land media postings.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Media {

  /// <summary>Data read and write services for Land media postings.</summary>
  static internal class LandMediaPostingsData {


    static internal FixedList<LandMediaPosting> GetMediaPostings(LandMediaContent mediaContent,
                                                                 BaseObject instance) {
      string filter = BuildFilter(mediaContent, instance);

      return GetMediaPostings(filter);
    }


    static internal void WriteMediaPosting(LandMediaPosting o) {
      var op = DataOperation.Parse("writeLRSMediaPosting",
               o.Id, o.UID, o.StorageItem.Id, o.MediaContent.ToString(), o.ImagingControlID,
               o.Keywords, o.ExtensionData.ToString(), o.Transaction.Id, o.Instrument.Id,
               o.InstrumentRecording.Id, o.RecordingBook.Id, o.BookEntry.Id,
               o.BookEntryNo, o.ExternalTransactionId, o.PostingTime, o.PostedBy.Id,
               (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    #region Helpers

    static private string BuildFilter(LandMediaContent mediaContent, BaseObject instance) {
      return $"[PhysicalRecordingId] = {instance.Id}";
    }


    static private FixedList<LandMediaPosting> GetMediaPostings(string filter) {
      string sql = "SELECT * FROM LRSMediaPostings " +
                   $"WHERE ({filter}) AND MediaPostingStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LandMediaPosting>(op);
    }

    #endregion Helpers

  }  // class LandMediaPostingsData

}  // namespace Empiria.Land.Media
