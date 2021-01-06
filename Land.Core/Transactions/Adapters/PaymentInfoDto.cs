/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : PaymentInfoDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a transaction payment information.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO with a transaction payment information.</summary>
  public class PaymentInfoDto {

    public string ReceiptNo {
      get; internal set;
    }

    public decimal Total {
      get; internal set;
    }

    public string MediaUri {
      get; internal set;
    }

  }  // class PaymentInfoDto

}  // namespace Empiria.Land.Transactions.Adapters
