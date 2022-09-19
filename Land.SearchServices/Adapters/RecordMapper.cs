﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Mapper class                            *
*  Type     : RecordMapper                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map land electronic records.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods used to map land electronic records.</summary>
  static internal class RecordMapper {

    static internal RecordDto Map(Record record) {
      return new RecordDto {
        UID = record.UID,
        RecordID = record.RecordingID,
        RecorderOffice = record.RecorderOffice.Alias,
        RecordingTime = record.RecordingTime,
        PresentationTime = record.PresentationTime,
        RecordedBy = record.RecordedBy.Alias,
        AuthorizedBy = record.AuthorizedBy.Alias,
        Instrument = record.Instrument.AsText,
        BookEntry = record.HasBookEntry ? record.BookEntry.AsText : string.Empty,
        Transaction = MapTransaction(record)
      };
    }


    static private RecordTransactionDto MapTransaction(Record record) {
      var transaction = record.HasTransaction ?
                              record.Transaction :
                              Registration.Transactions.LRSTransaction.Empty;

      return new RecordTransactionDto {
        UID = transaction.GUID,
        TransactionID = transaction.UID,
        InternalControlNo = transaction.InternalControlNo
      };
    }

  }  // class RecordMapper

}  // namespace Empiria.Land.SearchServices
