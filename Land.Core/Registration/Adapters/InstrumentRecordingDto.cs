/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : InstrumentRecordingDto                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument registration.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO with data representing a legal instrument registration.</summary>
  public class InstrumentRecordingDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string InstrumentRecordingID {
      get; internal set;
    } = string.Empty;


    public InstrumentDto Instrument {
      get; internal set;
    } = null;


    public FixedList<RecordingBookEntryDto> BookEntries {
      get; internal set;
    } = new FixedList<RecordingBookEntryDto>();


    public MediaData StampMedia {
      get; internal set;
    } = MediaData.Empty;


    public string TransactionUID {
      get; internal set;
    } = string.Empty;


    public InstrumentRecordingControlDataDto Actions {
      get; internal set;
    } = new InstrumentRecordingControlDataDto();


  }  // class InstrumentRecordingDto

}  // namespace Empiria.Land.Registration.Adapters
