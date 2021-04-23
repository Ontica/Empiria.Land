/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordableSubjectsMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map recordable subjects like real estate, associations and no property subjects.    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Methods to map recordable subjects like real estate, associations
  /// and no property subjects.</summary>
  static internal class RecordableSubjectTractIndexMapper {

    static internal RecordableSubjectTractIndexDto Map(Resource recordableSubject,
                                                       FixedList<RecordingAct> amendableActs) {
      return new RecordableSubjectTractIndexDto {
        RecordableSubject = RecordableSubjectsMapper.Map(recordableSubject),
        TractIndex = MapTractIndex(amendableActs)
      };
    }


    static private FixedList<RecordableSubjectTractIndexEntryDto> MapTractIndex(FixedList<RecordingAct> list) {
      return new FixedList<RecordableSubjectTractIndexEntryDto>(list.Select((x) => MapTractIndexEntry(x)));
    }


    static private RecordableSubjectTractIndexEntryDto MapTractIndexEntry(RecordingAct recordingAct) {
      return new RecordableSubjectTractIndexEntryDto {
        RecordingActUID = recordingAct.UID,
        RecordingActName = recordingAct.DisplayName,
        Antecedent = "Antecedente",
        RecordingDate = recordingAct.Document.AuthorizationTime,
        PresentationTime = recordingAct.Document.PresentationTime,
      };
    }

  }  // class RecordableSubjectsMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
