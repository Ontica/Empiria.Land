/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : InstrumentRecordingMapper                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Instruments;
using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.DataTypes;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal partial class InstrumentRecordingMapper {

    static internal InstrumentRecordingDto Map(RecordingDocument instrumentRecording) {
      return Map(instrumentRecording, instrumentRecording.GetTransaction());
    }


    static internal InstrumentRecordingDto Map(RecordingDocument instrumentRecording,
                                               LRSTransaction transaction) {
      var dto = new InstrumentRecordingDto();

      Instrument instrument = Instrument.Parse(instrumentRecording.InstrumentId);



      var actions = new InstrumentRecordingControlData(instrumentRecording, instrument, transaction);

      var bookEntries = instrument.RecordingBookEntries;

      dto.UID = instrumentRecording.GUID;
      dto.InstrumentRecordingID = instrumentRecording.UID;
      dto.Instrument = InstrumentMapper.Map(instrument);

      if (bookEntries.Count > 0) {
        dto.BookEntries = RecordingBookMapper.MapBookEntriesListDto(bookEntries);
        dto.BookRecordingMode = true;

      } else {
        dto.RecordingActs = MapRecordingActsListDto(instrumentRecording.RecordingActs);
      }

      dto.StampMedia = MapStampMedia(instrumentRecording, transaction);

      dto.TransactionUID = transaction.UID;
      dto.Actions = GetControlDataDto(actions);

      return dto;
    }


    static internal MediaData MapStampMedia(RecordingDocument instrumentRecording,
                                            LRSTransaction transaction) {
      var mediaBuilder = new LandMediaBuilder();

      Instrument instrument = Instrument.Parse(instrumentRecording.InstrumentId);

      if (instrument.RecordingBookEntries.Count > 0) {
        return mediaBuilder.GetMediaDto(LandMediaContent.BookEntryRegistrationStamp,
                                        "-1", transaction.Id.ToString());
      } else {
        return mediaBuilder.GetMediaDto(LandMediaContent.RegistrationStamp,
                                        instrumentRecording.UID);
      }
    }


    static internal InstrumentRecordingShortDto MapToShort(RecordingDocument instrumentRecording) {
      var dto = new InstrumentRecordingShortDto() {
        UID = instrumentRecording.GUID,
        ControlID = instrumentRecording.UID,
        AsText = $"AsText {instrumentRecording.Id}"
      };

      return dto;
    }


    static internal FixedList<RecordingActEntryDto> MapRecordingActsListDto(FixedList<RecordingAct> list) {
      var mappedItems = list.Select((x) => GetRecordingActEntryDto(x));

      return new FixedList<RecordingActEntryDto>(mappedItems);
    }


    #region Private methods

    static private InstrumentRecordingControlDataDto GetControlDataDto(InstrumentRecordingControlData controlData) {
      var dto = new InstrumentRecordingControlDataDto();

      dto.Can.EditInstrument = controlData.CanEdit;
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
      dto.RecordableSubject.RecordingContext = GetRecordingContext(recordingAct);

      dto.Antecedent = recordingAct.GetRecordingAntecedentText();

      return dto;
    }


    static private RecordingContextDto GetRecordingContext(RecordingAct recordingAct) {
      return new RecordingContextDto {
        InstrumentUID = recordingAct.Document.UID,
        RecordingActUID = recordingAct.UID
      };
    }

    #endregion Private methods

  }  // class InstrumentRecordingMapper

}  // namespace Empiria.Land.Registration.Adapters
