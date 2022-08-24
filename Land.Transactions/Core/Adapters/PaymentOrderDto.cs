/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Data Transfer Object                    *
*  Type     : PaymentOrderDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for a payment order.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO for a payment order.</summary>
  public class PaymentOrderDto {

    public string UID {
      get; internal set;
    }

    public DateTime IssueTime {
      get; internal set;
    }

    public DateTime DueDate {
      get; internal set;
    }

    public decimal Total {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public MediaData Media {
      get; internal set;
    } = MediaData.Empty;

  }  // class PaymentOrderDto

}  // namespace Empiria.Land.Transactions.Adapters
