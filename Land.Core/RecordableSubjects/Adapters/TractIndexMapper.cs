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
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Methods used to map the tract index of a real estate or recordable
  /// subjects of other kinds.</summary>
  static internal class TractIndexMapper {

    static internal TractIndexDto Map(Resource recordableSubject,
                                      FixedList<RecordingAct> amendableActs) {
      return new TractIndexDto {
        RecordableSubject = RecordableSubjectsMapper.Map(recordableSubject),
        Entries = MapTractIndex(recordableSubject, amendableActs),
        Structure = new FixedList<TractIndexEntryDto>(),
        Actions = MapActions(recordableSubject, amendableActs)
      };
    }

    static private TractIndexActions MapActions(Resource recordableSubject,
                                                FixedList<RecordingAct> amendableActs) {
      return new TractIndexActions {
        CanBeClosed = true,
        CanBeOpened = false,
        CanBeUpdated = true
      };
    }

    static private TrantIndexEntryActions MapActions(RecordingAct recordingAct) {
      bool isHistoric = recordingAct.Document.IsHistoricDocument;
      bool isClosed = recordingAct.Document.IsClosed;

      return new TrantIndexEntryActions {
        CanBeDeleted = isHistoric && !isClosed,
        CanBeClosed = isHistoric && !isClosed,
        CanBeOpened = isHistoric && isClosed,
        CanBeUpdated = isHistoric && !isClosed &&  recordingAct.IsEditable,
      };
    }

    static private FixedList<TractIndexEntryDto> MapTractIndex(Resource recordableSubject,
                                                               FixedList<RecordingAct> list) {
      return new FixedList<TractIndexEntryDto>(list.Select((x) => MapTractIndexEntry(recordableSubject, x)));
    }


    static private TractIndexEntryDto MapTractIndexEntry(Resource recordableSubject,
                                                         RecordingAct recordingAct) {
      return new TractIndexEntryDto {
        UID = recordingAct.UID,
        EntryType = "RecordingAct",
        Description = recordingAct.DisplayName,
        RequestedTime = recordingAct.Document.PresentationTime,
        IssueTime = recordingAct.Document.AuthorizationTime,
        Status = recordingAct.StatusName,

        Transaction = MapTransaction(recordingAct.Document.GetTransaction()),
        OfficialDocument = MapToOfficialDocument(recordingAct),
        SubjectChanges = MapSubjectChanges(recordableSubject, recordingAct),
        Actions = MapActions(recordingAct)
      };
    }



    static private OfficialDocumentDto MapToOfficialDocument(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      PhysicalRecording bookEntry = recordingAct.HasPhysicalRecording ?
                                          recordingAct.PhysicalRecording : PhysicalRecording.Empty;

      return new OfficialDocumentDto {
        UID = document.GUID,
        Type = document.DocumentType.DisplayName,
        DocumentID = document.UID,
        Description = bookEntry.IsEmptyInstance ? document.UID  : recordingAct.PhysicalRecording.AsText,
        Office = document.RecorderOffice.MapToNamedEntity(),
        BookEntry = bookEntry.IsEmptyInstance ? null : MapBookEntry(bookEntry),
        IssueTime = document.AuthorizationTime,
        ElaboratedBy = document.GetRecordingOfficials()[0].Alias,
        AuthorizedBy = document.AuthorizedBy.Alias,
        Status = document.Status.ToString(),
        Media = InstrumentRecordingMapper.MapStampMedia(document, document.GetTransaction())
      };
    }

    static private BookEntryIdentifiersDto MapBookEntry(PhysicalRecording bookEntry) {
      return new BookEntryIdentifiersDto {
         UID = bookEntry.UID,
         RecordingBookUID = bookEntry.RecordingBook.UID,
         InstrumentRecordingUID = bookEntry.MainDocument.GUID,
      };
    }

    static private RecordableSubjectChangesDto MapSubjectChanges(Resource recordableSubject,
                                                                 RecordingAct recordingAct) {
      if (!(recordableSubject is RealEstate)) {
        return CreateSubjectChangesDto(recordingAct, string.Empty);
      }

      if (!Resource.IsCreationalRole(recordingAct.ResourceRole)) {
        return CreateSubjectChangesDto(recordingAct, string.Empty);
      }

      var realEstate = (RealEstate) recordingAct.Resource;

      string summary = String.Empty;

      if (recordingAct.ResourceRole == ResourceRole.Created) {
        summary = "Predio inscrito por primera vez (no es fusión ni se subdividió de otro).";

      } else if (realEstate.Equals(recordableSubject)) {
        summary = $"Creado a partir del predio " +
                  $"{recordingAct.RelatedResource.UID} como {PartitionText(realEstate)}.";

      } else {
        summary = $"Subdividido en {PartitionText(realEstate)} con folio real " +
                  $"{realEstate.UID}. Superficie: {realEstate.LotSize}.";

      }

      return CreateSubjectChangesDto(recordingAct, summary);
    }


    static private RecordableSubjectChangesDto CreateSubjectChangesDto(RecordingAct recordingAct,
                                                                       string summary) {
      return new RecordableSubjectChangesDto {
        Summary = summary,
        Snapshot = RecordableSubjectsMapper.Map(recordingAct.Resource,
                                                recordingAct.GetResourceSnapshotData()),
        StructureChanges = new FixedList<StructureChangeDto>()
      };
    }


    static private string PartitionText(RealEstate newPartition) {
      if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length == 0) {
        return $"FRACCIÓN O PARTE SIN IDENTIFICAR";

      } else if (newPartition.Kind.Length != 0 && newPartition.PartitionNo.Length == 0) {
        return $"{newPartition.Kind} SIN IDENTIFICAR";

      } else if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length != 0) {
        return $"FRACCIÓN O PARTE {newPartition.PartitionNo}";

      } else {
        return $"{newPartition.Kind} {newPartition.PartitionNo}";
      }
    }


    static private TransactionInfoDto MapTransaction(LRSTransaction transaction) {
      return new TransactionInfoDto {
         UID = transaction.GUID,
         TransactionID = transaction.UID,
         RequestedBy = transaction.RequestedBy,
         Agency = transaction.Agency.MapToNamedEntity(),
         FilingOffice = transaction.RecorderOffice.MapToNamedEntity(),
         PresentationTime = transaction.PresentationTime,
         CompletedTime  = transaction.ClosingTime,
         Status = transaction.Workflow.CurrentStatusName
      };
    }

  }  // class TractIndexMapper

}  // namespace Empiria.Land.RecordableSubjects.Adapters
