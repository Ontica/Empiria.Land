﻿/* Empiria Land **********************************************************************************************
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
        Entries = MapTractIndex(amendableActs)
      };
    }


    static private FixedList<TractIndexEntryDto> MapTractIndex(FixedList<RecordingAct> list) {
      return new FixedList<TractIndexEntryDto>(list.Select((x) => MapTractIndexEntry(x)));
    }


    static private TractIndexEntryDto MapTractIndexEntry(RecordingAct recordingAct) {
      return new TractIndexEntryDto {
        UID = recordingAct.UID,
        EntryType = "RecordingAct",
        Description = recordingAct.DisplayName,
        RequestedTime = recordingAct.Document.PresentationTime,
        IssueTime = recordingAct.Document.AuthorizationTime,
        Status = recordingAct.StatusName,

        Transaction = MapTransaction(recordingAct.Document.GetTransaction()),
        OfficialDocument = MapToOfficialDocument(recordingAct),
        SubjectChanges = MapSubjectChanges(recordingAct)

        //OfficialDocument
        //Transaction
        //RecordingAct =
        //Certificate =


        //TransactionID = recordingAct.Document.TransactionID,
        //TransactionUID = recordingAct.Document.TransactionID,

        //RegisteredBy = recordingAct.RegisteredBy.Alias,


        //DocumentUID = recordingAct.Document.GUID,
        //DocumentNumber = recordingAct.Document.UID,
        //DocumentMedia = InstrumentRecordingMapper.MapStampMedia(recordingAct.Document,
        //                                                        recordingAct.Document.GetTransaction()),


        //RecordableSubject = RecordableSubjectsMapper.Map(recordingAct.Resource),
        //InstrumentRecordingUID = recordingAct.Document.GUID,


      };
    }


    static private OfficialDocumentDto MapToOfficialDocument(RecordingAct recordingAct) {
      RecordingDocument document = recordingAct.Document;

      return new OfficialDocumentDto {
        UID = document.GUID,
        Type = document.DocumentType.DisplayName,
        DocumentID = document.UID,
        Description = recordingAct.HasPhysicalRecording ?
                          recordingAct.PhysicalRecording.AsText : document.UID,
        Office = document.RecorderOffice.MapToNamedEntity(),
        IssueTime = document.AuthorizationTime,
        ElaboratedBy = document.GetRecordingOfficials()[0].Alias,
        AuthorizedBy = document.AuthorizedBy.Alias,
        Status = document.Status.ToString(),
        Media = InstrumentRecordingMapper.MapStampMedia(document, document.GetTransaction())
      };
    }



    static private RecordableSubjectChangesDto MapSubjectChanges(RecordingAct recordingAct) {
      return new RecordableSubjectChangesDto {
        Summary = "Totalidad / Sobre el predio ... / Creado a partir de ... / Fusionado en ... / Subdividido en ...",
        Snapshot = RecordableSubjectsMapper.Map(recordingAct.Resource),
        StructureChanges = new FixedList<StructureChangeDto>()
      };
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
