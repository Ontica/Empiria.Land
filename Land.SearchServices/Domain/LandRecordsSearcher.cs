/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Service provider                        *
*  Type     : LandRecordsSearcher                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides search services for land records.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Provides search services for land records.</summary>
  static internal class LandRecordsSearcher {

    static internal Record GetLastDomainActRecord(Resource resource) {
      Assertion.Require(resource, nameof(resource));

      RecordingDocument lastRecordingDocument = resource.Tract.LastRecordingAct.Document;

      return new Record(lastRecordingDocument);
    }


    static internal Record GetRecordingActPartyRecord(RecordingActParty recordingActParty) {
      Assertion.Require(recordingActParty, nameof(recordingActParty));

      return new Record(recordingActParty.RecordingAct.Document);
    }

  }  // class LandRecordsSearcher

}  // namespace Empiria.Land.SearchServices
