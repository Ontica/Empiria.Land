/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Application Services                    *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : SubscriptionType                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents the type of a subscription.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  public enum SubscriptionType {

    CertificateChangesSubscription,

    RecordingDocumentChangesSubscription,

    ResourceChangesSubscription,

  }

}  // namespace Empiria.Land.Messaging
