/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holders                      *
*  Type     : BookEntryFields                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTOs used to hold book entry fields and create and update them manually.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Input DTO used to hold book entry fields.</summary>
  public class BookEntryFields {

    public string RecordingNo {
      get; set;
    }

    public int StartImageIndex {
      get; set;
    } = -1;


    public int EndImageIndex {
      get; set;
    } = -1;


    public DateTime PresentationTime {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime AuthorizationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    internal void EnsureIsValid() {
      Assertion.Require(RecordingNo, "fields.RecordingNo");
      if (PresentationTime != ExecutionServer.DateMinValue &&
          AuthorizationDate != ExecutionServer.DateMinValue) {
        Assertion.Require(PresentationTime <= AuthorizationDate,
                        "La fecha de presentación debe ser anterior o igual a la fecha de registro.");
      }
    }


    internal RecordingDTO MapToRecordingDTO(RecordingBook book,
                                            RecordingDocument mainDocument) {
      var dto = new RecordingDTO(book, this.RecordingNo);

      dto.MainDocument = mainDocument;
      dto.PresentationTime = this.PresentationTime;
      dto.AuthorizationDate = this.AuthorizationDate;
      dto.EndImageIndex = this.EndImageIndex;
      dto.StartImageIndex = this.StartImageIndex;

      return dto;
    }

  }  // class BookEntryFields



  /// <summary>Data structure used to automatically create the next recording book entry.</summary>
  public class CreateNextBookEntryFields {

    public string RecorderOfficeUID {
      get; set;
    }

    public string SectionUID {
      get; set;
    }

  }  // class CreateNextBookEntryFields



  /// <summary>Data structure used to manually create and update recording book entries.</summary>
  public class ManualEditBookEntryFields {

    public BookEntryFields BookEntry {
      get; set;
    } = new BookEntryFields();


    public InstrumentFields Instrument {
      get; set;
    }

    public RecordableObjectStatus Status {
      get; set;
    } = RecordableObjectStatus.Incomplete;


    internal void EnsureIsValid() {
      Assertion.Require(BookEntry, "BookEntry value is required.");
      Assertion.Require(Instrument, "Instrument value is required.");
      Assertion.Require(Instrument.Type.HasValue, "Instrument.Type value is required.");

      BookEntry.EnsureIsValid();
    }


    internal RecordingDTO MapToRecordingDTO(RecordingBook book,
                                            RecordingDocument mainDocument) {
      var dto = new RecordingDTO(book, this.BookEntry.RecordingNo);

      dto.MainDocument = mainDocument;
      dto.PresentationTime = this.BookEntry.PresentationTime;
      dto.AuthorizationDate = this.BookEntry.AuthorizationDate;
      dto.EndImageIndex = this.BookEntry.EndImageIndex;
      dto.StartImageIndex = this.BookEntry.StartImageIndex;

      return dto;
    }

  }  // class ManualBookEntryFields


}  // namespace Empiria.Land.Registration.Adapters
