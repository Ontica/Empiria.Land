/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Physical Registration                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : PhysicalRecordingDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a recording registered in a physical book.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

namespace Empiria.Land.PhysicalBooks.Adapters {

  public class PhysicalRecordingDto {

    public string UID {
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


    public string RecordedBy {
      get; internal set;
    }

    public MediaData StampMedia {
      get; internal set;
    } = MediaData.Empty;


  }  // class PhysicalRecordingDto

}  // namespace Empiria.Land.PhysicalBooks.Adapters
