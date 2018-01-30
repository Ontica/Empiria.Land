/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution : Empiria Land                                     System  : Land Application Services           *
*  Assembly : Empiria.Land.AppServices.dll                     Pattern : Application services                *
*  Type     : FilingServices                                   License : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Application services for Empiria Land filing transactions.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.AppServices {

  /// <summary>Application services for Empiria Land filing transactions.</summary>
  static public class FilingServices {

    #region Services

    static public IFiling GetFiling(string filingUID) {
      Assertion.AssertObject(filingUID, "filingUID");

      var filing = LRSTransaction.TryParse(filingUID);

      if (filing == null) {
        throw new ResourceNotFoundException("Land.Filing.NotFound",
                                            $"No tenemos registrado ningún trámite con número '{filingUID}'.");
      }

      return filing;
    }

    #endregion Services

  }  // class FilingServices

}  // namespace Empiria.Land.AppServices
