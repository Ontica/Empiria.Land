/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionShortModel                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds minimal transaction data to be used as list items.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that holds minimal transaction data to be used as list items.</summary>
  public class TransactionShortModel {

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

    public string InternalControlNo {
      get; internal set;
    }

    public string Stage {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

    public string NextStatus {
      get; internal set;
    }

    public string NextStatusName {
      get; internal set;
    }

    public string AssignedToUID {
      get; internal set;
    }

    public string AssignedToName {
      get; internal set;
    }

    public string NextAssignedToName {
      get; internal set;
    }

  }  // class TransactionShortModel

}  // namespace Empiria.Land.Transactions.Adapters
