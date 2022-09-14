/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Commons                                    Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : EditionRules                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Flags holder with object's edition rules.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land {

  /// <summary>Flags holder with object's edition rules.</summary>
  public class EditionRules {

    internal EditionRules() {
      // no-op
    }

    public bool CanBeClosed {
      get; internal set;
    }


    public bool CanBeOpened {
      get; internal set;
    }


    public bool CanBeUpdated {
      get; internal set;
    }


    public bool CanBeDeleted {
      get; internal set;
    }

  }  // class EditionRules

}  // namespace Empiria.Land
