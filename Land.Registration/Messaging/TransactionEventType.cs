/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : TransactionEventType                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a transaction workflow event used to send notifications.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Describes a transaction workflow event used to send notifications.</summary>
  internal enum TransactionEventType {

    TransactionReceived,

    TransactionDelayed,

    TransactionReadyToDelivery,

    TransactionReturned,

    TransactionReentered,

    TransactionArchived

  }  // enum TransactionEventType

}  // namespace Empiria.Land.Messaging
