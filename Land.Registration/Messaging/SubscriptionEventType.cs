/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : SubscriptionEventType                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a workflow status change of a land transaction.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Describes a workflow status change of a land transaction.</summary>
  internal enum SubscriptionEventType {

    DocumentWasChanged,

    ResourceWasChanged,

    SubscribedForDocumentChanges,

    SubscribedForResourceChanges,

    UnsubscribedForDocumentChanges,

    UnsubscribedForResourceChanges

  }  // enum SubscriptionEventType

}  // namespace Empiria.Land.Messaging
