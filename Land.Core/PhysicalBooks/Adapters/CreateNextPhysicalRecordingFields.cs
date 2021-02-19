/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Physical Registration                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : CreateNextPhysicalRecordingFields          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to automatically create physical recordings in the next volume entry.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.PhysicalBooks.Adapters {

  /// <summary>Data structure used to automatically create physical
  /// recordings in the next volume entry.</summary>
  public class CreateNextPhysicalRecordingFields {

    public string RecorderOfficeUID {
      get; set;
    }

    public string SectionUID {
      get; set;
    }

  }  // class CreateNextPhysicalRecordingFields

}  // namespace Empiria.Land.PhysicalBooks.Adapters
