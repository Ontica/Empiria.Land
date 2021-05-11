/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Partial Methods                    *
*  Type     : InstrumentMedia (Partial)                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partial class with legal instrument media related methods.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media;

namespace Empiria.Land.Instruments {

  /// <summary>Partial class with legal instrument media related methods.</summary>
  public partial class Instrument {

    private LandMediaFileSet _mediaFileSet;

    internal LandMediaFileSet GetMediaFileSet() {
      if (_mediaFileSet == null) {
        _mediaFileSet = LandMediaFileSet.GetFor(this);
      }
      return _mediaFileSet;
    }

  }  // partial class Instrument/InstrumentMedia

}  // namespace Empiria.Land.Instruments
