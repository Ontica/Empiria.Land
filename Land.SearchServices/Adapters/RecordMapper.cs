/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Mapper class                            *
*  Type     : RecordMapper                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map land electronic records.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Methods used to map land electronic records.</summary>
  static public class RecordMapper {

    static public RecordDto Map(Record record) {
      return new RecordDto {
        UID = record.UID,
        RecordID = record.RecordingID,
        RecorderOffice = record.RecorderOffice.ShortName,
        RecordingTime = record.RecordingTime,
        PresentationTime = record.PresentationTime,
        RecordedBy = record.RecordedBy.ShortName,
        AuthorizedBy = record.AuthorizedBy.ShortName,
        Instrument = record.Instrument.AsText,
        BookEntry = record.HasBookEntry ? record.BookEntry.AsText : string.Empty,
        Transaction = MapTransaction(record)
      };
    }


    static private RecordTransactionDto MapTransaction(Record record) {
      var transaction = record.HasTransaction ?
                              record.Transaction :
                              LRSTransaction.Empty;

      return new RecordTransactionDto {
        UID = transaction.GUID,
        TransactionID = transaction.UID,
        InternalControlNo = transaction.InternalControlNumber
      };
    }

  }  // class RecordMapper

}  // namespace Empiria.Land.Registration.Adapters
