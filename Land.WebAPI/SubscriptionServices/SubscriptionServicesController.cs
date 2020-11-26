/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Subscription services                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : SubscriptionServicesController               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Subscription services used to follow changes on resources and documents.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.Land.Messaging;

namespace Empiria.Land.WebApi {

  /// <summary>Subscription services used to follow changes on resources and documents.</summary>
  public class SubscriptionServicesController : WebApiController {

    #region Public APIs


    [HttpPost, AllowAnonymous]
    [Route("v1/online-services/subscriptions/{subscriptionUID}/confirm")]
    public SingleObjectModel ConfirmSubscription([FromUri] string subscriptionUID,
                                                 [FromBody] object body) {
      try {
        SubscriptionRequest subscriptionRequest =
                    this.BuildSubscriptionRequestFromBody(SubscriptionRequestCommand.ConfirmSubscription, body);

        Subscription subscription = SubscriptionServices.ConfirmSubscription(subscriptionRequest);

        return new SingleObjectModel(this.Request, subscription.ToResponse(),
                                     typeof(Subscription).ToString());

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v1/online-services/subscriptions")]
    public SingleObjectModel Subscribe([FromBody] object body) {
      try {
        SubscriptionRequest subscriptionRequest =
                    this.BuildSubscriptionRequestFromBody(SubscriptionRequestCommand.Subscribe, body);

        Subscription subscription = SubscriptionServices.Subscribe(subscriptionRequest);

        return new SingleObjectModel(this.Request, subscription.ToResponse(),
                                     typeof(Subscription).ToString());

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v1/online-services/subscriptions/{subscriptionUID}/unsubscribe")]
    public SingleObjectModel Unsubscribe([FromUri] string subscriptionUID,
                                         [FromBody] object body) {
      try {
        base.RequireResource(subscriptionUID, "subscriptionUID");

        SubscriptionRequest subscriptionRequest =
                    this.BuildSubscriptionRequestFromBody(SubscriptionRequestCommand.Unsubscribe, body);

        Subscription subscription = SubscriptionServices.Unsubscribe(subscriptionUID, subscriptionRequest);

        return new SingleObjectModel(this.Request, subscription.ToResponse(),
                                     typeof(Subscription).ToString());

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    #endregion Public APIs

    #region Private methods


    private SubscriptionRequest BuildSubscriptionRequestFromBody(SubscriptionRequestCommand command,
                                                                 object body) {
      base.RequireBody(body);

      var bodyAsJson = JsonObject.Parse(body);

      bodyAsJson.Add("command", command.ToString());

      return SubscriptionRequest.Parse(bodyAsJson);
    }


    #endregion Private methods

  }  // class SubscriptionServicesController

}  // namespace Empiria.Land.WebApi
