/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Application Services                    *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Enumeration type                        *
*  Type     : SubscriptionRequestCommand                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a SubscriptionRequest command.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Describes a SubscriptionRequest command.</summary>
  public enum SubscriptionRequestCommand {

    Subscribe,

    ConfirmSubscription,

    Unsubscribe

  }  // SubscriptionRequestCommand

}  // namespace Empiria.Land.Messaging
