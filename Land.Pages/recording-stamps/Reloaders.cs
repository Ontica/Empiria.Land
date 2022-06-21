/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : Reloaders                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Temporal instances reload methods.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApp {

  /// <summary>Temporal instances reload methods.</summary>
  static internal class Reloaders {

    static internal RecordingActParty Reload(RecordingActParty party) {
      return RecordingActParty.Parse(party.Id, true);
    }

    static internal Party Reload(Party party) {
      return Party.Parse(party.Id, true);
    }


    static internal RecordingAct Reload(RecordingAct act) {
      return RecordingAct.Parse(act.Id, true);
    }


    static internal Resource Reload(Resource resource) {
      return Resource.Parse(resource.Id, true);
    }

  }  // class Reloaders

}  // namespace Empiria.Land.WebApp
