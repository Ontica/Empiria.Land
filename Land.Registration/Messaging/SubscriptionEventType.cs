/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : SubscriptionEventType                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Event type used to send notifications to document and resource subscribers.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Event type used to send notifications to document and resource subscribers.</summary>
  internal enum SubscriptionEventType {

    CertificateWasChanged,

    LandRecordWasChanged,

    ResourceWasChanged,

    SubscribedForCertificateChanges,

    SubscribedForLandRecordChanges,

    SubscribedForResourceChanges,

    ConfirmedForCertificateChanges,

    ConfirmedForLandRecordChanges,

    ConfirmedForResourceChanges,

    UnsubscribedForCertificateChanges,

    UnsubscribedForLandRecordChanges,

    UnsubscribedForResourceChanges

  }  // enum SubscriptionEventType

}  // namespace Empiria.Land.Messaging
