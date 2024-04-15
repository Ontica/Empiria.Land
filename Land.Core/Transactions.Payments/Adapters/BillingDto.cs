/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : BillingDto                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that represents data about a transaction billing request.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Payments.Adapters {

  /// <summary>Output DTO that represents data about a transaction billing request.</summary>
  public class BillingDto {

    public string BillTo {
      get; internal set;
    }

    public string RFC {
      get; internal set;
    }

  }  // class BillingDto

} // namespace Empiria.Land.Transactions.Payments.Adapters
