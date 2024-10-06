/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : LandRecordDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data representing a legal instrument registration.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Storage;

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO with data representing a legal instrument registration.</summary>
  public class LandRecordDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string InstrumentRecordingID {
      get; internal set;
    } = string.Empty;


    public InstrumentDto Instrument {
      get; internal set;
    } = null;


    public bool BookRecordingMode {
      get; internal set;
    } = false;


    public MediaData StampMedia {
      get; internal set;
    } = MediaData.Empty;


    public string TransactionUID {
      get; internal set;
    } = string.Empty;


    public LandRecordControlDataDto Actions {
      get; internal set;
    } = new LandRecordControlDataDto();


    public FixedList<RecordingActEntryDto> RecordingActs {
      get; internal set;
    } = new FixedList<RecordingActEntryDto>();


    public FixedList<BookEntryOutputDto> BookEntries {
      get; internal set;
    } = new FixedList<BookEntryOutputDto>();


  }  // class LandRecordDto



  /// <summary>Short DTO descriptor for land records.</summary>
  public class LandRecordDescriptorDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string ControlID {
      get; internal set;
    } = string.Empty;


    public string AsText {
      get; internal set;
    } = string.Empty;

  }  // class LandRecordDescriptorDto

}  // namespace Empiria.Land.Registration.Adapters
