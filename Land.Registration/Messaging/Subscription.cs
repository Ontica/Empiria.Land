/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Queue notification                      *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Messsage queue processor                *
*  Type     : LandMessenger                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a workflow status change of a land transaction.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Messaging;

using Empiria.Land.Registration;

namespace Empiria.Land.Messaging {


  public enum SubscriptionStatus {

    Pending = 'P',

    Confirmed = 'C',

    Unsubscribed = 'U'

  }

  public class Subscription : BaseObject {


    #region Constructors and parsers

    internal Subscription(SubscriptionType subscriptionType, string subscribedObjectUID, SendTo sendTo) {
      Assertion.AssertObject(subscribedObjectUID, "subscribedObjectUID");
      Assertion.AssertObject(sendTo, "sendTo");

      this.SubscriptionType = subscriptionType;
      this.SubscribedObjectUID = subscribedObjectUID;
      this.SendTo = sendTo;
    }


    static public Subscription TryParse(string subscriptionUID) {
      return BaseObject.TryParse<Subscription>($"UID = '{subscriptionUID}'");
    }


    #endregion Constructors and parsers


    #region Fields

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


    public SubscriptionStatus Status {
      get;
      private set;
    } = SubscriptionStatus.Pending;


    public DateTime PostingTime {
      get;
      private set;
    } = DateTime.Now;


    #endregion Fields

    #region Public methods

    protected override void OnSave() {
      MessagingData.WriteSubscription(this);
    }


    /// <summary>Confirms subscription.</summary>
    public void Confirm() {
      Assertion.Assert(this.Status == SubscriptionStatus.Pending,
                  "Subscriptions can be confirmed only whern they are in pending status.");
      this.Status = SubscriptionStatus.Confirmed;

      this.Save();
    }


    /// <summary>Unsubscribes for resource changes.</summary>
    public void Unsubscribe() {
      Assertion.Assert(this.Status != SubscriptionStatus.Unsubscribed,
                  "Subscription its already in Unsubscribed status.");

      this.Status = SubscriptionStatus.Unsubscribed;

      this.Save();
    }

    #endregion Public methods

    #region Private methods


    //private ObjectPosting TryGetPosting(Resource resource, SendTo sendTo) {
    //  var registered = ObjectPosting.GetList(SubscriptionMessageType.SubscribedForResourceChanges,
    //                                         resource.UID);

    //  foreach (ObjectPosting posting in registered) {
    //    if (found) {
    //      return posting;
    //    }
    //  }
    //  return null;
    //}


    //private void Store() {
    //  var posting = new ObjectPosting(SubscriptionMessageType.SubscribedForResourceChanges,
    //                                  this.Resource.UID, this.SendTo.ToJson());

    //  posting.Save();
    //}

    #endregion Private methods

  }  // class LandMessenger

}  // namespace Empiria.Land.Messaging
