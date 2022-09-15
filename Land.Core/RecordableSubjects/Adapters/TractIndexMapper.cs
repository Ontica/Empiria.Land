/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TractIndexMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map the tract index of a real estate or recordable subjects of other kinds.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Methods used to map the tract index of a real estate or recordable
  /// subjects of other kinds.</summary>
  static internal class TractIndexMapper {

    static internal TractIndexDto Map(Resource subject,
                                      FixedList<RecordingAct> baseEntries) {
      var builder = new SubjectHistoryBuilder(subject, baseEntries);

      SubjectHistory history = builder.Build();

      return new TractIndexDto {
        RecordableSubject = RecordableSubjectsMapper.Map(subject),
        Entries = MapHistory(history),
        Actions = history.EditionRules
      };
    }


    static private FixedList<TractIndexEntryDto> MapHistory(SubjectHistory history) {
      return history.Entries.Select(entry => MapHistoryEntry(history.Subject, entry))
                            .ToFixedList();
    }


    static private TractIndexEntryDto MapHistoryEntry(Resource subject, SubjectHistoryEntry historyEntry) {
      RecordingAct recordingAct = historyEntry.RecordingAct;

      return new TractIndexEntryDto {
        UID = recordingAct.UID,
        EntryType = historyEntry.EntryType,
        Name = historyEntry.Name,
        Description = historyEntry.Description,
        Status = historyEntry.StatusName,
        RecordingData = MapRecordingData(recordingAct),
        SubjectSnapshot = RecordableSubjectsMapper.Map(subject,
                                                       historyEntry.SubjectSnapshot),
        LotChange = new object(),
        Actions = historyEntry.EditionRules
      };
    }


    static private RecordingDataDto MapRecordingData(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      PhysicalRecording volumeEntry = recordingAct.HasPhysicalRecording ?
                                            recordingAct.PhysicalRecording : PhysicalRecording.Empty;

      var transaction = recordingAct.Document.GetTransaction();

      return new RecordingDataDto {
        UID = document.GUID,
        Type = document.DocumentType.DisplayName,
        RecordingID = document.UID,
        Description = volumeEntry.IsEmptyInstance ? document.UID  : recordingAct.PhysicalRecording.AsText,
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

  }  // class TractIndexMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
