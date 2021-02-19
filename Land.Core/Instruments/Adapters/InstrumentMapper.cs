/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : InstrumentMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Adapters;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.PhysicalBooks.Adapters;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal partial class InstrumentMapper {

    static internal FixedList<InstrumentDto> Map(FixedList<Instrument> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<InstrumentDto>(mappedItems);
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
        Media = mediaFiles,
        Status = instrument.Status.ToString(),
        Registration = GetRegistrationDto(instrument),
        PhysicalRecordings = GetPhysicalRecordingListDto(instrument),
        Actions = GetControlDataDto(instrument)
      };

      return dto;
    }

    #region Private methods

    static private InstrumentControlDataDto GetControlDataDto(Instrument instrument) {
      InstrumentControlData controlData = instrument.ControlData;

      var dto = new InstrumentControlDataDto();

      dto.Can.Close = controlData.CanClose;
      dto.Can.CreatePhysicalRecordings = controlData.CanCreatePhysicalRecordings;
      dto.Can.Delete = controlData.CanDelete;
      dto.Can.DeletePhysicalRecordings = controlData.CanDeletePhysicalRecordings;
      dto.Can.Edit = controlData.CanEdit;
      dto.Can.EditPhysicalRecordingActs = controlData.CanEditPhysicalRecordingActs;
      dto.Can.EditRecordingActs = controlData.CanEditRecordingActs;
      dto.Can.LinkPhysicalRecordings = controlData.CanLinkPhysicalRecordings;
      dto.Can.Open = controlData.CanOpen;
      dto.Can.UploadFiles = controlData.CanUploadFiles;

      dto.Show.Files = controlData.ShowFiles;
      dto.Show.PhysicalRecordings = controlData.ShowPhysicalRecordings;
      dto.Show.RecordingActs = controlData.ShowRecordingActs;
      dto.Show.RegistrationStamps = controlData.ShowRegistrationStamps;

      return dto;
    }


    static private RegistrationDto GetRegistrationDto(Instrument instrument) {
      var dto = new RegistrationDto();

      dto.PhysicalRecordings = GetPhysicalRecordingListDto(instrument);

      var document = instrument.TryGetRecordingDocument();

      if (document == null) {
        return dto;
      }

      dto.UID = document.GUID;
      dto.RegistrationID = document.UID;

      var mediaBuilder = new LandMediaBuilder(LandMediaContent.PhysicalRegistrationStamp,
                                              instrument.GetTransaction());

      dto.StampMedia = mediaBuilder.GetMediaDto("-1");

      return dto;
    }


    static private PhysicalRecordingDto GetPhysicalRecording(PhysicalRecording recording,
                                                             LRSTransaction transaction) {
      var dto = new PhysicalRecordingDto();

      dto.UID = recording.UID;
      dto.RecordingTime = recording.RecordingTime;
      dto.RecorderOfficeName = recording.RecordingBook.RecorderOffice.Alias;
      dto.RecordingSectionName = recording.RecordingBook.RecordingSection.Name;
      dto.VolumeNo = recording.RecordingBook.BookNumber;
      dto.RecordingNo = recording.Number;
      dto.RecordedBy = recording.RecordedBy.Alias;

      var mediaBuilder = new LandMediaBuilder(LandMediaContent.PhysicalRegistrationStamp,
                                              transaction);

      dto.StampMedia = mediaBuilder.GetMediaDto(recording.Id.ToString());

      return dto;
    }


    static private FixedList<PhysicalRecordingDto> GetPhysicalRecordingListDto(Instrument instrument) {
      var list = instrument.PhysicalRecordings;

      var mappedItems = list.Select((x) => GetPhysicalRecording(x, instrument.GetTransaction()));

      return new FixedList<PhysicalRecordingDto>(mappedItems);
    }

    #endregion Private methods

  }  // class InstrumentMapper


}  // namespace Empiria.Land.Instruments.Adapters
