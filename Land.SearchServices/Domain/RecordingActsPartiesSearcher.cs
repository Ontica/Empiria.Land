/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Service provider                        *
*  Type     : RecordingActsPartiesSearcher               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs searches over recording acts parties.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

using Empiria.Land.SearchServices.Data;

namespace Empiria.Land.SearchServices {

  /// <summary>Performs searches over recording acts parties.</summary>
  internal class RecordingActsPartiesSearcher {

    internal RecordingActsPartiesSearcher() {
      // no-op
    }

    internal FixedList<RecordingActParty> Search(string filter, string sort, int pageSize) {
      Assertion.Require(filter, nameof(filter));
      Assertion.Require(sort, nameof(sort));
      Assertion.Require(pageSize > 0, "pageSize must be greater than zero.");

      return SearchServicesData.GetRecordingActParties(filter, sort, pageSize);
    }

  }  // class RecordingActsPartiesSearcher

}  // namespace Empiria.Land.SearchServices
