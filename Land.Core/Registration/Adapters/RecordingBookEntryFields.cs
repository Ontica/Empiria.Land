/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : RecordingBookEntryFields                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to create recording book entries.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Data structure used to create recording book entries.</summary>
  public class RecordingBookEntryFields {

    public string RecorderOfficeUID {
      get; set;
    }

    public string SectionUID {
      get; set;
    }

  }  // class RecordingBookEntryFields

}  // namespace Empiria.Land.Registration.Adapters
