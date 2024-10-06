/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Output Data Transfer Object             *
*  Type     : BookEntryOutputDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTOs with book entry data (physical books entries).                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO for a book entry (physical book recording).</summary>
  public class BookEntryOutputDto {

    public string UID {
      get; internal set;
    }


    public string RecordingBookUID {
      get; internal set;
    }

    public DateTime RecordingTime {
      get; internal set;
    }


    public string RecorderOfficeName {
      get; internal set;
    }


    public string RecordingSectionName {
      get; internal set;
    }


    public string VolumeNo {
      get; internal set;
    }


    public string RecordingNo {
      get; internal set;
    }


    public DateTime PresentationTime {
      get; internal set;
    }


    public DateTime AuthorizationDate {
      get; internal set;
    }


    public string RecordedBy {
      get; internal set;
    }


    public LandRecordDescriptorDto InstrumentRecording {
      get; internal set;
    }


    public FixedList<RecordingActEntryDto> RecordingActs {
      get; internal set;
    } = new FixedList<RecordingActEntryDto>();


    public RecordableObjectStatus Status {
      get;
      internal set;
    }


    public FixedList<LandMediaFileDto> MediaFiles {
      get; internal set;
    } = new FixedList<LandMediaFileDto>();


    public MediaData StampMedia {
      get; internal set;
    } = MediaData.Empty;


  }  // class BookEntryOutputDto



  /// <summary>Minimal data for a book entry.</summary>
  public class BookEntryShortDto {

    public string UID {
      get; internal set;
    }

    public string RecordingNo {
      get; internal set;
    }

    public string InstrumentRecordingUID {
      get; internal set;
    } = string.Empty;


    public DateTime AuthorizationDate {
      get; internal set;
    }

  }  // BookEntryShortDto


}  // namespace Empiria.Land.Registration.Adapters
