/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : SubjectHistoryMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for recording acts history of a real property or other recordable subjects.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Mapping methods for recording acts history of a real
  /// property or other recordable subjects.</summary>
  static internal class SubjectHistoryMapper {

    static internal SubjectHistoryDto Map(Resource subject,
                                          FixedList<RecordingAct> baseEntries) {
      var builder = new SubjectHistoryBuilder(subject, baseEntries);

      SubjectHistory history = builder.Build();

      return new SubjectHistoryDto {
        RecordableSubject = RecordableSubjectsMapper.Map(subject),
        Entries = MapHistory(history),
        Actions = history.EditionRules
      };
    }


    static private FixedList<SubjectHistoryEntryDto> MapHistory(SubjectHistory history) {
      return history.Entries.Select(entry => MapHistoryEntry(history.Subject, entry))
                            .ToFixedList();
    }


    static private SubjectHistoryEntryDto MapHistoryEntry(Resource subject,
                                                          SubjectHistoryEntry historyEntry) {
      RecordingAct recordingAct = historyEntry.RecordingAct;

      return new SubjectHistoryEntryDto {
        UID = recordingAct.UID,
        EntryType = historyEntry.EntryType,
        Name = historyEntry.Name,
        Description = historyEntry.Description,
        Status = historyEntry.StatusName,
        RecordingData = RecordingDataMapper.MapRecordingData(recordingAct),
        SubjectSnapshot = RecordableSubjectsMapper.Map(subject,
                                                       historyEntry.SubjectSnapshot),
        Actions = historyEntry.EditionRules
      };
    }

  }  // class SubjectHistoryMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
