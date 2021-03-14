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

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal partial class InstrumentRecordingMapper {

    static internal InstrumentRecordingDto Map(RecordingDocument instrumentRecording,
                                               LRSTransaction transaction) {
      var dto = new InstrumentRecordingDto();

      Instrument instrument = Instrument.Parse(instrumentRecording.InstrumentId);

      var mediaBuilder = new LandMediaBuilder(LandMediaContent.BookEntryRegistrationStamp,
                                              transaction);

      var actions = new InstrumentRecordingControlData(instrument, transaction);

      dto.UID = instrumentRecording.GUID;
      dto.InstrumentRecordingID = instrumentRecording.UID;
      dto.Instrument = Map(instrument);
      dto.BookEntries = GetRecordingBookEntriesListDto(instrument);
      dto.StampMedia = mediaBuilder.GetMediaDto("-1");
      dto.TransactionUID = transaction.UID;
      dto.Actions = GetControlDataDto(actions);

      return dto;
    }


    static internal InstrumentDto Map(Instrument instrument) {
      var issuerDto = IssuerMapper.Map(instrument.Issuer);

      var mediaFiles = LandMediaFileMapper.Map(instrument.GetMediaFileSet());


      var dto = new InstrumentDto {
        UID = instrument.UID,
        Type = instrument.InstrumentType.ToInstrumentTypeEnum(),
        TypeName = instrument.InstrumentType.DisplayName,
        Kind = instrument.Kind,
        ControlID = instrument.ControlID,
        Issuer = issuerDto,
        IssueDate = instrument.IssueDate,
        Summary = instrument.Summary,
        AsText = instrument.AsText,
        InstrumentNo = instrument.InstrumentNo,
        BinderNo = instrument.BinderNo,
        Folio = instrument.Folio,
        EndFolio = instrument.EndFolio,
        SheetsCount = instrument.SheetsCount,
        Media = mediaFiles
      };

      return dto;
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


    static private RecordingBookEntryDto GetRecordingBookEntryDto(PhysicalRecording bookEntry,
                                                                  LRSTransaction transaction) {
      var dto = new RecordingBookEntryDto();

      dto.UID = bookEntry.UID;
      dto.RecordingTime = bookEntry.RecordingTime;
      dto.RecorderOfficeName = bookEntry.RecordingBook.RecorderOffice.Alias;
      dto.RecordingSectionName = bookEntry.RecordingBook.RecordingSection.Name;
      dto.VolumeNo = bookEntry.RecordingBook.BookNumber;
      dto.RecordingNo = bookEntry.Number;
      dto.RecordedBy = bookEntry.RecordedBy.Alias;

      var mediaBuilder = new LandMediaBuilder(LandMediaContent.BookEntryRegistrationStamp,
                                              transaction);

      dto.StampMedia = mediaBuilder.GetMediaDto(bookEntry.Id.ToString());

      return dto;
    }


    static private FixedList<RecordingBookEntryDto> GetRecordingBookEntriesListDto(Instrument instrument) {
      var list = instrument.RecordingBookEntries;

      var mappedItems = list.Select((x) => GetRecordingBookEntryDto(x, instrument.GetTransaction()));

      return new FixedList<RecordingBookEntryDto>(mappedItems);
    }

    #endregion Private methods

  }  // class InstrumentRecordingMapper

}  // namespace Empiria.Land.Registration.Adapters
