/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Application Services                    *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Interface adapter                       *
*  Type     : SubscriptionRequest                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a subscription request used to subscribe, unsubscribe or confirm subscriptions.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Messaging.EMailDelivery;

namespace Empiria.Land.Messaging {

  /// <summary>Describes a subscription request used to subscribe,
  /// unsubscribe or confirm subscriptions.</summary>
  public class SubscriptionRequest {

    #region Constructors and parsers

    private SubscriptionRequest(JsonObject json) {
      this.LoadData(json);
    }


    static public SubscriptionRequest Parse(JsonObject json) {
      EnsureIsValid(json);

      return new SubscriptionRequest(json);
    }


    static public void EnsureIsValid(JsonObject json) {
      Assertion.Require(json, "json");

      Assertion.Require(json.HasValue("command"),
                       "Subscription request must have a 'command' value.");

      Assertion.Require(json.HasValue("subscriptionType"),
                       "Subscription request must have a 'subscriptionType' value.");

      Assertion.Require(json.HasValue("subscribedObjectUID"),
                       "Subscription request must have a 'subscribedObjectUID' that refers to a resource, " +
                       "certificate or recording document.");

      Assertion.Require(json.HasValue("sendTo"),
                       "Subscription request must have a 'sendTo' value.");

    }

    #endregion Constructors and parsers


    #region Properties


    public SubscriptionRequestCommand Command {
      get;
      private set;
    }



    public SubscriptionType SubscriptionType {
      get;
      private set;
    }


    public string SubscribedObjectUID {
      get;
      private set;
    } = String.Empty;


    public SendTo SendTo {
      get;
      private set;
    }


    public string HashCode {
      get;
      private set;
    } = String.Empty;


    #endregion Properties

    #region Methods

    private void LoadData(JsonObject json) {
      this.Command = json.Get<SubscriptionRequestCommand>("command");
      this.SubscriptionType = json.Get<SubscriptionType>("subscriptionType");
      this.SubscribedObjectUID = json.Get<string>("subscribedObjectUID");
      this.SendTo = SendTo.Parse(json.Slice("sendTo"));
      this.HashCode = json.Get<string>("hashCode", String.Empty);
    }

    #endregion Methods

  }  // class SubscriptionRequest

}  // namespace Empiria.Land.Messaging
