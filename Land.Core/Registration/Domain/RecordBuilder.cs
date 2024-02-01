/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Service provider                        *
*  Type     : RecordBuilder                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds Record objects for Land entities.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Builds Record objects for Land entities.</summary>
  static public class RecordBuilder {

    static public Record GetLastDomainActRecord(Resource resource) {
      Assertion.Require(resource, nameof(resource));

      RecordingDocument landRecord = resource.Tract.LastRecordingAct.LandRecord;

      return new Record(landRecord);
    }


    static public Record GetRecordingActPartyRecord(RecordingActParty recordingActParty) {
      Assertion.Require(recordingActParty, nameof(recordingActParty));

      return new Record(recordingActParty.RecordingAct.LandRecord);
    }

  }  // class RecordBuilder

}  // namespace Empiria.Land.SearchServices
