/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution : Empiria Land                                     System  : Land Web API                        *
*  Assembly : Empiria.Land.WebApi.dll                          Pattern : Response methods                    *
*  Type     : FilingResponseModel                              License : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Response models for Filing objects.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint;

namespace Empiria.Land.WebApi.Filings {

  /// <summary>Response models for Filing objects.</summary>
  static internal class FilingResponseModel {

    static internal object ToResponse(this IFiling filing) {
      return new {
        uid = filing.UID
      };
    }

  }  // class FilingResponseModel

}  // namespace Empiria.Land.WebApi.Filings
