/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Data Layer                              *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Service                            *
*  Type     : ESignEngineData                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for ESign.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;
using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.Domain;
using Empiria.OnePoint.ESign;

namespace Empiria.Land.ESign.Data {

  /// <summary>Provides data read methods for ESign.</summary>
  static internal class ESignEngineData {


    internal static FixedList<SignRequestEntry> GetPendingESigns(string esignStatus) {
      Assertion.Require(esignStatus, nameof(esignStatus));

      //var eSignUseCases = ESignUseCases();

      return new FixedList<SignRequestEntry>();
    }


    static internal FixedList<SignRequestEntry> GetSignRequests(ESignQuery query) {
      Assertion.Require(query, nameof(query));

      return new FixedList<SignRequestEntry>();
    }


  } // class ESignDataService

} // namespace Empiria.Land.ESign.Data
