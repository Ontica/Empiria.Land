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

      PhysicalRecording volumeEntry = recordingAct.HasPhysicalRecording ?
                                            recordingAct.PhysicalRecording : PhysicalRecording.Empty;

      var transaction = recordingAct.Document.GetTransaction();

      return new RecordingDataDto {
        UID = document.GUID,
        Type = document.DocumentType.DisplayName,
        RecordingID = document.UID,
        Description = volumeEntry.IsEmptyInstance ? document.UID : recordingAct.PhysicalRecording.AsText,
        Office = document.RecorderOffice.MapToNamedEntity(),
        VolumeEntryData = MapVolumeEntry(volumeEntry),
        RecordingTime = document.AuthorizationTime,
        RecordedBy = document.GetRecordingOfficials()[0].Alias,
        AuthorizedBy = document.AuthorizedBy.Alias,
        PresentationTime = document.PresentationTime,
        TransactionUID = transaction.UID,
        Status = document.Status.ToString(),
        Media = InstrumentRecordingMapper.MapStampMedia(document, document.GetTransaction())
      };
    }


    static private VolumeEntryDataDto MapVolumeEntry(PhysicalRecording volumeEntry) {
      if (volumeEntry.IsEmptyInstance) {
        return null;
      }

      return new VolumeEntryDataDto {
        UID = volumeEntry.UID,
        RecordingBookUID = volumeEntry.RecordingBook.UID,
        InstrumentRecordingUID = volumeEntry.MainDocument.GUID,
      };
    }

    #endregion Public methods

  }  // class RecordingDataMapper

}  // namespace Empiria.Land.Registration.Adapters
