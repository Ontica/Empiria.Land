/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                            Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : FormsProvider                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data forms for transactions.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint.EFiling;

using Empiria.Land.Integration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Provides data forms for transactions.</summary>
  internal class FormsProvider {

    internal static IForm GetForm(LRSTransaction transaction, LandSystemFormType formType) {
      var eFilingProvider = ExternalProviders.GetEFilingProvider();

      EFilingRequest request = eFilingProvider.GetEFilingRequest(transaction.ExternalTransactionNo);

      if (request.Procedure.NamedKey == "AvisoPreventivo") {
        return PreventiveNoteForm.Parse(request);
      } else {
        throw new NotImplementedException();
      }
    }

  }  // class FormsProvider

}  // namespace Empiria.Land.Registration.Forms
