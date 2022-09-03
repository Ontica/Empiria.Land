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

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods used to map land electronic records.</summary>
  static internal class RecordMapper {

    static internal RecordDto Map(RecordingDocument document) {
      PhysicalRecording physicalRec = document.TryGetHistoricRecording();

      string bookEntry = physicalRec != null ? physicalRec.AsText : String.Empty;

      string instrument = $"{document.DocumentType.DisplayName} Número {document.Id}";

      return new RecordDto {
        UID = document.GUID,
        RecordID = document.UID,
        RecorderOffice = document.RecorderOffice.Alias,
        RecordingTime = document.AuthorizationTime,
        PresentationTime = document.PresentationTime,
        RecordedBy = document.PostedBy.Alias,
        AuthorizedBy = document.AuthorizedBy.Alias,
        Instrument = instrument,
        BookEntry = bookEntry,
        Transaction = MapTransaction(document)
      };
    }


    static private RecordTransactionDto MapTransaction(RecordingDocument document) {
      var transaction = document.HasTransaction ?
                              document.GetTransaction() :
                              Registration.Transactions.LRSTransaction.Empty;

      return new RecordTransactionDto {
        UID = transaction.GUID,
        TransactionID = transaction.UID,
        InternalControlNo = transaction.InternalControlNo
      };
    }

  }  // class RecordMapper

}  // namespace Empiria.Land.SearchServices
