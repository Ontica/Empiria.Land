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
        BookEntries = MapBookEntriesListDto(book.BookEntries)
      };
    }


    static internal FixedList<BookEntryOutputDto> MapBookEntriesListDto(FixedList<BookEntry> list) {
      var mappedItems = list.Select((x) => MapBookEntry(x));

      return new FixedList<BookEntryOutputDto>(mappedItems);
    }


    static internal FixedList<BookEntryShortDto> MapBookEntriesListShortDto(FixedList<BookEntry> list) {
      var mappedItems = list.Select((x) => MapBookEntryToShortDto(x));

      return new FixedList<BookEntryShortDto>(mappedItems);
    }

    #endregion Public methods

    #region Helper methods

    static private BookEntryOutputDto MapBookEntry(BookEntry bookEntry) {
      var dto = new BookEntryOutputDto();

      dto.UID = bookEntry.UID;
      dto.RecordingBookUID = bookEntry.RecordingBook.UID;
      dto.RecordingTime = bookEntry.RecordingTime;
      dto.RecorderOfficeName = bookEntry.RecordingBook.RecorderOffice.ShortName;
      dto.RecordingSectionName = bookEntry.RecordingBook.RecordingSection.Name;
      dto.VolumeNo = bookEntry.RecordingBook.BookNumber;
      dto.RecordingNo = bookEntry.Number;
      dto.PresentationTime = bookEntry.LandRecord.PresentationTime;
      dto.AuthorizationDate = bookEntry.LandRecord.AuthorizationTime;
      dto.RecordedBy = bookEntry.RecordedBy.ShortName;
      dto.InstrumentRecording = LandRecordMapper.MapToShort(bookEntry.LandRecord);
      dto.RecordingActs = LandRecordMapper.MapRecordingActsListDto(bookEntry.RecordingActs);
      dto.Status = bookEntry.Status;

      dto.MediaFiles = MapBookEntryMediaFiles(bookEntry);

      var mediaBuilder = new LandMediaBuilder();

      dto.StampMedia = mediaBuilder.GetMediaDto(LandMediaContent.BookEntryRegistrationStamp,
                                                bookEntry.Id.ToString(), "-1");

      return dto;
    }


    static private FixedList<LandMediaFileDto> MapBookEntryMediaFiles(BookEntry bookEntry) {
      var mediaBuilder = new LandMediaBuilder();

      var files = mediaBuilder.GetLandMediaPostings(LandMediaContent.BookEntryMediaFiles, bookEntry);

      return files.Select(x => LandMediaFileMapper.Map(x))
                  .ToFixedList();
    }


    static private BookEntryShortDto MapBookEntryToShortDto(BookEntry bookEntry) {
      return new BookEntryShortDto {
        UID = bookEntry.UID,
        RecordingNo = bookEntry.Number,
        InstrumentRecordingUID = bookEntry.LandRecord.GUID,
        AuthorizationDate = bookEntry.LandRecord.AuthorizationTime
      };
    }


    #endregion Helper methods

  }  // class RecordingBookMapper

}  // namespace Empiria.Land.Registration.Adapters
