﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Data Transfer Object                    *
*  Type     : TransactionDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data related to a transaction.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Holds data related to a transaction list item.</summary>
  public class TransactionDto {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get; internal set;
    }

    public string TransactionID {
      get;
      internal set;
    }

    public string RequestedBy {
      get; internal set;
    }

    public DateTime PresentationTime {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

  }  // class TransactionListItemDto

}  // namespace Empiria.Land.Transactions.UseCases
