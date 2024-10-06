/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : LandRecordMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to LandRecordDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Storage;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Transactions;

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Methods to map legal instruments to LandRecordDto objects. </summary>
  static internal partial class LandRecordMapper {

    static internal LandRecordDto Map(LandRecord landRecord) {
      return Map(landRecord.Transaction, landRecord);
    }


    internal static LandRecordDto Map(LRSTransaction transaction, LandRecord landRecord) {
      var dto = new LandRecordDto();

      var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      dto.UID = landRecord.GUID;
      dto.InstrumentRecordingID = landRecord.UID;
      dto.Instrument = InstrumentMapper.Map(landRecord.Instrument, transaction);

      if (bookEntries.Count > 0) {
        dto.BookEntries = RecordingBookMapper.MapBookEntriesListDto(bookEntries);
        dto.BookRecordingMode = true;

      } else {
        dto.RecordingActs = MapRecordingActsListDto(landRecord.RecordingActs);
      }

      dto.StampMedia = MapStampMedia(landRecord);

      dto.TransactionUID = transaction.UID;

      var actions = new LandRecordControlData(landRecord);

      dto.Actions = GetControlDataDto(transaction, actions);

      return dto;
    }


    static internal MediaData MapStampMedia(LandRecord landRecord) {
      var mediaBuilder = new LandMediaBuilder();

      var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      if (bookEntries.Count > 0) {
        return mediaBuilder.GetMediaDto(LandMediaContent.BookEntryRegistrationStamp,
                                        "-1", landRecord.Transaction.Id.ToString());
      } else {
        return mediaBuilder.GetMediaDto(LandMediaContent.RegistrationStamp,
                                        landRecord.UID);
      }
    }


    static internal LandRecordDescriptorDto MapToShort(LandRecord landRecord) {
      var dto = new LandRecordDescriptorDto() {
        UID = landRecord.GUID,
        ControlID = landRecord.UID,
        AsText = $"AsText {landRecord.Id}"
      };

      return dto;
    }


    static internal FixedList<RecordingActEntryDto> MapRecordingActsListDto(FixedList<RecordingAct> list) {
      return list.Select((x) => GetRecordingActEntryDto(x))
                 .ToFixedList();
    }

    static internal RecordingContextDto MapRecordingContext(RecordingAct recordingAct) {
      return new RecordingContextDto {
        InstrumentRecordingUID = recordingAct.LandRecord.GUID,
        RecordingActUID = recordingAct.UID
      };
    }


    #region Private methods

    static private LandRecordControlDataDto GetControlDataDto(LRSTransaction transaction,
                                                              LandRecordControlData controlData) {
      var dto = new LandRecordControlDataDto();

      dto.Can.EditInstrument = transaction.IsEmptyInstance ? controlData.CanEdit :
                                                             transaction.ControlData.CanEditInstrument;
      dto.Can.OpenInstrument = controlData.CanOpen;
      dto.Can.CloseInstrument = controlData.CanClose;
      dto.Can.DeleteInstrument = controlData.CanDelete;

      dto.Can.CreateNextRecordingBookEntries = controlData.CanCreateRecordingBookEntries;
      dto.Can.DeleteRecordingBookEntries = controlData.CanDeleteRecordingBookEntries;
      dto.Can.EditRecordingActs = controlData.CanEditRecordingActs;

      dto.Can.UploadInstrumentFiles = controlData.CanUploadFiles;

      dto.Show.Files = controlData.ShowFiles;
      dto.Show.RecordingActs = controlData.ShowRecordingActs;
      dto.Show.RecordingBookEntries = controlData.ShowRecordingBookEntries;
      dto.Show.RegistrationStamps = controlData.ShowRegistrationStamps;

      return dto;
    }


    static private RecordingActEntryDto GetRecordingActEntryDto(RecordingAct recordingAct) {
      var dto = new RecordingActEntryDto();

      dto.UID = recordingAct.UID;
      dto.Name = recordingAct.RecordingActType.DisplayName;
      dto.RecordableSubject = RecordableSubjectsMapper.Map(recordingAct.Resource);
      dto.RecordableSubject.RecordingContext = MapRecordingContext(recordingAct);

      if (recordingAct.IsAppliedOverNewPartition) {
        dto.RelatedSubject = RecordableSubjectsMapper.Map(recordingAct.RelatedResource);
        dto.RelatedSubject.RecordingContext = MapRecordingContext(recordingAct.RelatedResource.Tract.GetRecordingAntecedent());
      }

      dto.Antecedent = recordingAct.GetRecordingAntecedentText();

      return dto;
    }

    #endregion Private methods

  }  // class LandRecordMapper

}  // namespace Empiria.Land.Registration.Adapters
