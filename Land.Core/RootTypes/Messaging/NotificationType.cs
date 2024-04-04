/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : NotificationType                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a land messaging notification. This enum must be a union between                     *
*             SubscriptionEventType and TransactionEventType.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Describes a land messaging notification. This enum must be a union between
  /// SubscriptionEventType and TransactionEventType.</summary>
  internal enum NotificationType {

    // values from SubscriptionEventType enum

    DocumentWasChanged,

    ResourceWasChanged,

    SubscribedForDocumentChanges,

    SubscribedForResourceChanges,

    UnsubscribedForDocumentChanges,

    UnsubscribedForResourceChanges,


    // values from TransactionEventType enum

    TransactionReceived,

    TransactionDelayed,

    TransactionReadyToDelivery,

    TransactionReturned,

    TransactionReentered,

    TransactionArchived

  }  // enum NotificationType

}  // namespace Empiria.Land.Messaging
