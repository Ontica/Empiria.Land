/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : CreateNextBookEntryFields                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure used to automatically create the next recording book entry.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Data structure used to automatically create the next recording book entry.</summary>
  public class CreateNextBookEntryFields {

    public string RecorderOfficeUID {
      get; set;
    }

    public string SectionUID {
      get; set;
    }

  }  // class CreateNextBookEntryFields

}  // namespace Empiria.Land.Registration.Adapters
