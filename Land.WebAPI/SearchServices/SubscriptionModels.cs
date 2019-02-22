/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                      Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern : Response methods                        *
*  Type     : SubscriptionModels                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Models for Subscription responses.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Messaging;

namespace Empiria.Land.WebApi {

  /// <summary>Response models for SignEvent entities.</summary>
  static internal class SubscriptionModels {

    #region Response models

    static internal object ToResponse(this Subscription subscription) {
      return new {
        uid = subscription.UID,
        type = subscription.SubscriptionType,
        subscribedObjectUID = subscription.SubscribedObjectUID,
        sendTo = new {
          name = subscription.SendTo.Name,
          address = subscription.SendTo.Address,
        },
        status = subscription.Status
      };
    }

    #endregion Response models

  }  // class SubscriptionModels

}  // namespace Empiria.OnePoint.WebApi

