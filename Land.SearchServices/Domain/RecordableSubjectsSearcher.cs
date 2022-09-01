/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Service provider                        *
*  Type     : RecordableSubjectsSearcher                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs searches for recordable subjects (real estate, associations and no-property).         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Performs searches for recordable subjects (real estate, associations and no-property).</summary>
  internal class RecordableSubjectsSearcher {

    internal RecordableSubjectsSearcher() {
      // no-op
    }

    internal FixedList<Resource> Search(string filter, string sort, int pageSize) {
      Assertion.Require(filter, nameof(filter));
      Assertion.Require(sort, nameof(sort));
      Assertion.Require(pageSize > 0, "pageSize must be greater than zero.");

      return Resource.GetList(filter, sort, pageSize);
    }

  }  // class RecordableSubjectsSearcher

}  // namespace Empiria.Land.SearchServices
