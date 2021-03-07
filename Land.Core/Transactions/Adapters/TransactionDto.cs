/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TransactionDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that holds full data related to a transaction.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that holds full data related to a transaction.</summary>
  public class TransactionDto {

    public string UID {
      get; internal set;
    }

    public NamedEntityDto Type {
      get; internal set;
    }

    public NamedEntityDto Subtype {
      get; internal set;
    }

    public string TransactionID {
      get; internal set;
    }

    public RequestedByDto RequestedBy {
      get; internal set;
    }

    public DateTime PresentationTime {
      get; internal set;
    }

    public string InternalControlNo {
      get; internal set;
    }

    public NamedEntityDto Agency {
      get; internal set;
    }

    public NamedEntityDto RecorderOffice {
      get; internal set;
    }

    public string InstrumentDescriptor {
      get; internal set;
    }

    public RequestedServiceDto[] RequestedServices {
      get; internal set;
    }

    public PaymentOrderDto PaymentOrder {
      get; internal set;
    }

    public PaymentFields Payment {
      get; internal set;
    }

    public MediaData SubmissionReceipt {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

    public NamedEntityDto AssignedTo {
      get; internal set;
    }

    public string NextStatusName {
      get; internal set;
    }

    public NamedEntityDto NextAssignedTo {
      get; internal set;
    }

    public TransactionControlDataDto Actions {
      get; internal set;
    }

  }  // class TransactionDto

}  // namespace Empiria.Land.Transactions.Adapters
