/* Empiria Land **********************************************************************************************
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
using Empiria.Storage;

using Empiria.Land.Instruments;
using Empiria.Land.Media.Adapters;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Media {

  /// <summary>Data read and write services for Land media postings.</summary>
  static internal class LandMediaPostingsData {


    internal static FixedList<LandMediaPosting> GetFilePostings(StorageFile file) {
      var filter = $"StorageItemId = {file.Id}";

      return GetMediaPostings(filter);
    }


    static internal FixedList<LandMediaPosting> GetMediaPostings(BaseObject instance) {
      string filter = BuildFilter(instance);

      return GetMediaPostings(filter);
    }


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

    static private string BuildFilter(BaseObject instance) {
      if (instance.Id == -1) {
        return SearchExpression.NoRecordsFilter;
      }

      if (instance is LRSTransaction) {
        return $"TransactionId = {instance.Id}";

      } else if (instance is Instrument instrument) {
        LRSTransaction transaction = instrument.GetTransaction();

        if (!instrument.IsEmptyInstance && transaction != null && !transaction.IsEmptyInstance) {
          return $"(InstrumentId = {instrument.Id} OR TransactionId = {transaction.Id})";
        } else {
          return $"InstrumentId = {instrument.Id}";
        }

      } else if (instance is PhysicalRecording bookEntry) {
        return $"BookEntryId = {bookEntry.Id}";

      }

      throw Assertion.EnsureNoReachThisCode($"Unhandled instance type {instance.GetType()}.");
    }


    static private string BuildFilter(LandMediaContent mediaContent, BaseObject instance) {
      return $"([MediaContentType] = '{mediaContent}' AND {BuildFilter(instance)})";
    }


    static private FixedList<LandMediaPosting> GetMediaPostings(string filter) {
      string sql = "SELECT * FROM LRSMediaPostings " +
                   $"WHERE {filter} AND MediaPostingStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<LandMediaPosting>(op);
    }

    #endregion Helpers

  }  // class LandMediaPostingsData

}  // namespace Empiria.Land.Media
