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

    static internal TractIndexDto Map(Resource recordableSubject,
                                      FixedList<RecordingAct> amendableActs) {
      return new TractIndexDto {
        RecordableSubject = RecordableSubjectsMapper.Map(recordableSubject),
        TractIndex = MapTractIndex(amendableActs)
      };
    }


    static private FixedList<TractIndexEntryDto> MapTractIndex(FixedList<RecordingAct> list) {
      return new FixedList<TractIndexEntryDto>(list.Select((x) => MapTractIndexEntry(x)));
    }


    static private TractIndexEntryDto MapTractIndexEntry(RecordingAct recordingAct) {
      return new TractIndexEntryDto {
        UID = recordingAct.UID,
        Type = "RecordingAct",
        Name = recordingAct.DisplayName,
        DocumentID = recordingAct.Document.UID,
        TransactionID = recordingAct.Document.TransactionID,
        RecordingTime = recordingAct.Document.AuthorizationTime,
        PresentationTime = recordingAct.Document.PresentationTime,
        RecordableSubject = RecordableSubjectsMapper.Map(recordingAct.Resource),
        StampMedia = InstrumentRecordingMapper.MapStampMedia(recordingAct.Document,
                                                             recordingAct.Document.GetTransaction()),
        InstrumentRecordingUID = recordingAct.Document.GUID,

        Status = recordingAct.StatusName
      };
    }

  }  // class TractIndexMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
