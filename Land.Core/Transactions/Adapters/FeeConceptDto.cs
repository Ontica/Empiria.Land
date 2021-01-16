/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : FeeConceptDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data related to a transaction concept fee.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO with data related to a transaction concept fee.</summary>
  public class FeeConceptDto {

    public string UID {
      get; internal set;
    }

    public string LegalBasis {
      get; internal set;
    }

    public string FinancialCode {
      get; internal set;
    }

    public bool RequiresTaxableBase {
      get; internal set;
    }

  }  // public class FeeConceptDto

}  // namespace Empiria.Land.Transactions.Adapters
