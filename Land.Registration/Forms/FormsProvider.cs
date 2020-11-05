/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                            Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : FormsProvider                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data forms for transactions.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.OnePoint.EFiling;

using Empiria.Land.Integration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Provides data forms for transactions.</summary>
  static internal class FormsProvider {

    static internal IForm GetForm(LRSTransaction transaction) {
      var eFilingProvider = ExternalProviders.GetEFilingProvider();

      EFilingRequest request = eFilingProvider.GetEFilingRequest(transaction.ExternalTransactionNo);

      switch (request.Procedure.NamedKey) {
        case "AvisoPreventivo":
          return PreventiveNoteForm.Parse(request);

        case "SegundoAvisoDefinitivo":
          return DefinitiveNoteForm.Parse(request);

        default:
          throw Assertion.AssertNoReachThisCode($"Unrecognized external procedure: '{request.Procedure.NamedKey}'.");
      }
    }

  }  // class FormsProvider

}  // namespace Empiria.Land.Registration.Forms
