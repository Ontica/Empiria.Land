/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : TransactionEventType                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Event type used to send notifications when a transaction's status changes.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Event type used to send notifications when a transaction's status changes.</summary>
  internal enum TransactionEventType {

    TransactionReceived,

    TransactionDelayed,

    TransactionReadyToDelivery,

    TransactionReturned,

    TransactionReentered,

    TransactionArchived

  }  // enum TransactionEventType

}  // namespace Empiria.Land.Messaging
