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
      RecordingDocument landRecord = recordingAct.LandRecord;

      BookEntry bookEntry = recordingAct.HasBookEntry ?
                                            recordingAct.BookEntry : BookEntry.Empty;

      return new RecordingDataDto {
        UID = landRecord.GUID,
        Type = landRecord.DocumentType.DisplayName,
        RecordingID = landRecord.UID,
        Description = bookEntry.IsEmptyInstance ? landRecord.UID : recordingAct.BookEntry.AsText,
        Office = landRecord.RecorderOffice.MapToNamedEntity(),
        VolumeEntryData = MapVolumeEntry(bookEntry),
        RecordingTime = landRecord.AuthorizationTime,
        RecordedBy = landRecord.GetRecordingOfficials()[0].ShortName,
        AuthorizedBy = landRecord.AuthorizedBy.ShortName,
        PresentationTime = landRecord.PresentationTime,
        TransactionUID = recordingAct.LandRecord.GetTransaction().UID,
        Status = landRecord.Status.ToString(),
        Media = LandRecordMapper.MapStampMedia(landRecord)
      };
    }


    static private VolumeEntryDataDto MapVolumeEntry(BookEntry bookEntry) {
      if (bookEntry.IsEmptyInstance) {
        return null;
      }

      return new VolumeEntryDataDto {
        UID = bookEntry.UID,
        RecordingBookUID = bookEntry.RecordingBook.UID,
        InstrumentRecordingUID = bookEntry.LandRecord.GUID,
      };
    }

    #endregion Public methods

  }  // class RecordingDataMapper

}  // namespace Empiria.Land.Registration.Adapters
