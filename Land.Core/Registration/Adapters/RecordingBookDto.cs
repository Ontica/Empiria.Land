/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingBookDto                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for recording books and their book entries.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  public class RecordingBookDto {

    public string UID {
      get; internal set;
    }

    public NamedEntityDto RecorderOffice {
      get; internal set;
    }


    public NamedEntityDto RecordingSection {
      get; internal set;
    }


    public string VolumeNo {
      get; internal set;
    }

    public RecordingBookStatus Status {
      get;
      internal set;
    }

    public FixedList<RecordingBookEntryDto> BookEntries {
      get;
      internal set;
    }

  }  // class RecordingBookDto

}  // namespace Empiria.Land.Registration.Adapters
