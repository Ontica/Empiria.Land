﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : RecordingBookMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for recording books.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Registration.Adapters {

  static internal class RecordingBookMapper {

    #region Public methods

    static internal RecordingBookDto Map(RecordingBook book) {
      return new RecordingBookDto {
        UID = book.UID,
        VolumeNo = book.BookNumber,
        RecorderOffice = book.RecorderOffice.MapToNamedEntity(),
        RecordingSection = book.RecordingSection.MapToNamedEntity(),
        Status = book.Status,
        BookEntries = MapRecordingBookEntriesListDto(book.Recordings)
      };
    }


    static internal FixedList<RecordingBookEntryDto> MapRecordingBookEntriesListDto(FixedList<PhysicalRecording> list) {
      var mappedItems = list.Select((x) => GetRecordingBookEntryDto(x));

      return new FixedList<RecordingBookEntryDto>(mappedItems);
    }


    #endregion Public methods

    #region Helper methods


    static private RecordingBookEntryDto GetRecordingBookEntryDto(PhysicalRecording bookEntry) {
      var dto = new RecordingBookEntryDto();

      dto.UID = bookEntry.UID;
      dto.RecordingTime = bookEntry.RecordingTime;
      dto.RecorderOfficeName = bookEntry.RecordingBook.RecorderOffice.Alias;
      dto.RecordingSectionName = bookEntry.RecordingBook.RecordingSection.Name;
      dto.VolumeNo = bookEntry.RecordingBook.BookNumber;
      dto.RecordingNo = bookEntry.Number;
      dto.RecordedBy = bookEntry.RecordedBy.Alias;
      dto.InstrumentRecording = InstrumentRecordingMapper.MapToShort(bookEntry.MainDocument);
      dto.Status = bookEntry.Status;

      var mediaBuilder = new LandMediaBuilder(LandMediaContent.BookEntryRegistrationStamp);

      dto.StampMedia = mediaBuilder.GetMediaDto(bookEntry.Id.ToString(), "-1");

      return dto;
    }


    #endregion Helper methods

  }  // class RecordingBookMapper

}  // namespace Empiria.Land.Registration.Adapters