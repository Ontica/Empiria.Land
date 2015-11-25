/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Services                       *
*  Namespace : Empiria.Land.Services                          Assembly : Empiria.Land.Services.dll           *
*  Type      : ExternalTransactionBase                        Pattern  : External Interfacer                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract class that holds an external transaction request to be integrated into Empiria Land. *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.WebApi.Models {

  /// <summary>Abstract class that holds an external transaction request
  ///  to be integrated into Empiria Land.</summary>
  public abstract class ExternalTransactionBase {

    /// <summary>The external transaction number or unique identifier.</summary>
    public string TransactionNo {
      get;
      set;
    } = String.Empty;

    /// <summary>The requested transaction type from the external system.</summary>
    public ExternalTransactionType TransactionType {
      get;
      set;
    } = ExternalTransactionType.Undefined;

    /// <summary>Date and time of the requested transaction in the external system.</summary>
    public DateTime TransactionTime {
      get;
      set;
    } = ExecutionServer.DateMaxValue;

    /// <summary>The amount payed for the transaction controlled by the external system.</summary>
    public decimal PaymentAmount {
      get;
      set;
    } = 0m;

    /// <summary>The payment receipt number controlled by the external system.
    /// This number should be unique among each external system.</summary>
    public string PaymentReceiptNo {
      get;
      set;
    } = String.Empty;

    /// <summary>The full name of the person or company that request the transaction.</summary>
    public string RequestedBy {
      get;
      set;
    } = String.Empty;

  }  // class ExternalTransactionBase

}  // namespace Empiria.Land.Services
