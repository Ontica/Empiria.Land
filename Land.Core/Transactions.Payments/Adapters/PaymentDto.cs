/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Tranfer Object                     *
*  Type     : PaymentDto                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input/output DTO that serves to update and return payment data for a transaction.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Payments.Adapters {

  /// <summary>Input/output DTO that serves to update and return payment data for a transaction.</summary>
  public class PaymentDto {

    public string ReceiptNo {
      get; set;
    } = string.Empty;


    public decimal Total {
      get; set;
    }


    public string Status {
      get; set;
    }


    internal void AssertValid() {
      Assertion.Require(this.ReceiptNo, "ReceiptNo");
      Assertion.Require(this.Total >= 0, "Total must be a non-negative amount.");

      this.Status = this.Status ?? "Pendiente";
    }

  }  // public class PaymentDto

}  // namespace Empiria.Land.Transactions.Payments.Adapters
