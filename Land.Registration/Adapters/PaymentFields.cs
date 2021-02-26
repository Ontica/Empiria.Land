/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : PaymentFields                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves to update payment data for a transaction.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Data structure that serves to update payment data for a transaction.</summary>
  public class PaymentFields {

    public string ReceiptNo {
      get; set;
    } = string.Empty;


    public decimal Total {
      get; set;
    }


    public string Status {
      get; set;   // ToDo: internal set when PaymentFields will moved to Core
    }


    public void AssertValid() {
      Assertion.AssertObject(this.ReceiptNo, "ReceiptNo");
      Assertion.Assert(this.Total >= 0, "Total must be a non-negative amount.");

      this.Status = this.Status ?? "Pendiente";
    }

  }  // public class PaymentFields

}  // namespace Empiria.Land.Transactions.Adapters
