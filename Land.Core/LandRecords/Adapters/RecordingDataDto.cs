/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingDataDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with recording data for a real property and other kinds of recordable subjects.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.DataTypes;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Output DTO with recording data for a real property and other
  /// kinds of recordable subjects.</summary>
  public class RecordingDataDto {

    internal RecordingDataDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string RecordingID {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public NamedEntityDto Office {
      get; internal set;
    }


    [JsonProperty(PropertyName = "BookEntry",
                  NullValueHandling = NullValueHandling.Include)]
    public VolumeEntryDataDto VolumeEntryData {
      get; internal set;
    }


    public DateTime RecordingTime {
      get; internal set;
    }


    public string RecordedBy {
      get; internal set;
    }


    public string AuthorizedBy {
      get; internal set;
    }


    public DateTime PresentationTime {
      get; internal set;
    }


    public string TransactionUID {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }


    public MediaData Media {
      get; internal set;
    }

  }  // class RecordingDataDto



  /// <summary>Contains data that describes a physical volume entry.</summary>
  public class VolumeEntryDataDto {

    internal VolumeEntryDataDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string RecordingBookUID {
      get; internal set;
    }


    public string InstrumentRecordingUID {
      get; internal set;
    }

  }  // class VolumeEntryDataDto


}  // namespace Empiria.Land.Registration.Adapters
