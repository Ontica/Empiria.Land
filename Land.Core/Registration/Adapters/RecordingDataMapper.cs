/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordingDataMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for recording data.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  static internal class RecordingDataMapper {

    #region Public methods

    static internal RecordingDataDto MapRecordingData(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      BookEntry bookEntry = recordingAct.HasBookEntry ?
                                            recordingAct.BookEntry : BookEntry.Empty;

      var transaction = recordingAct.Document.GetTransaction();

      return new RecordingDataDto {
        UID = document.GUID,
        Type = document.DocumentType.DisplayName,
        RecordingID = document.UID,
        Description = bookEntry.IsEmptyInstance ? document.UID : recordingAct.BookEntry.AsText,
        Office = document.RecorderOffice.MapToNamedEntity(),
        VolumeEntryData = MapVolumeEntry(bookEntry),
        RecordingTime = document.AuthorizationTime,
        RecordedBy = document.GetRecordingOfficials()[0].ShortName,
        AuthorizedBy = document.AuthorizedBy.ShortName,
        PresentationTime = document.PresentationTime,
        TransactionUID = transaction.UID,
        Status = document.Status.ToString(),
        Media = InstrumentRecordingMapper.MapStampMedia(document, document.GetTransaction())
      };
    }


    static private VolumeEntryDataDto MapVolumeEntry(BookEntry bookEntry) {
      if (bookEntry.IsEmptyInstance) {
        return null;
      }

      return new VolumeEntryDataDto {
        UID = bookEntry.UID,
        RecordingBookUID = bookEntry.RecordingBook.UID,
        InstrumentRecordingUID = bookEntry.MainDocument.GUID,
      };
    }

    #endregion Public methods

  }  // class RecordingDataMapper

}  // namespace Empiria.Land.Registration.Adapters
