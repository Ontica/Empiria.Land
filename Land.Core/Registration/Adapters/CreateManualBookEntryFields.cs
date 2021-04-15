/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : CreateManualBookEntryFields                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to manually create a recording book entry.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Data structure used to manually create a recording book entry.</summary>
  public class CreateManualBookEntryFields {

    public string RecordingNo {
      get; set;
    }

    public InstrumentFields Instrument {
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


    public RecordableObjectStatus Status {
      get; set;
    } = RecordableObjectStatus.Incomplete;


    internal void EnsureIsValid() {
      Assertion.AssertObject(RecordingNo, "fields.RecordingNo");
      Assertion.AssertObject(Instrument, "Instrument value is required.");
      Assertion.Assert(Instrument.Type.HasValue, "Instrument.Type value is required.");
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

  }  // class CreateManualBookEntryFields

}  // namespace Empiria.Land.Registration.Adapters
