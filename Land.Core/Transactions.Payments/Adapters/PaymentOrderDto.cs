﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : PaymentOrderDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for a transaction payment order.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

namespace Empiria.Land.Transactions.Payments.Adapters {

  /// <summary>Output DTO for a transaction payment order.</summary>
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

}  // namespace Empiria.Land.Transactions.Payments.Adapters
