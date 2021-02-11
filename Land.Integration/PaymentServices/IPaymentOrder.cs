/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Data integration interface              *
*  Type     : IPaymentOrder                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Interface that defines a payment order.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>Interface that defines a payment order.</summary>
  public interface IPaymentOrder {

    string UID { get; }

    DateTime IssueTime { get; }

    DateTime DueDate { get; }

    decimal Total { get; }

    string Status { get; }

    IDictionary<string, object> Attributes { get; }

  }  // interface IPaymentOrder

} // namespace Empiria.Land.Integration.PaymentServices
