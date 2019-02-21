/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Messaging services                    Component : Application Services                    *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service                                 *
*  Type     : SubscriptionServices                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Application services to subscribe and unsubscribe for resource and document changes.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Messaging {

  /// <summary>Application services to subscribe and unsubscribe for resource and document changes.</summary>
  static public class SubscriptionServices {

    #region Services


    static public Subscription ConfirmSubscription(SubscriptionRequest request) {
      EnsureIsValid(request);

      throw new NotImplementedException();
    }


    static public Subscription Subscribe(SubscriptionRequest request) {
      EnsureIsValid(request);

      if (ExistsSubscription(request)) {
        throw new ResourceConflictException("Land.Subscription.AlreadyExists",
                          $"El correo electrónico {0} ya está registrado para recibir avisos registrales " +
                           "sobre este recurso.");
      }

      var subscription = new Subscription(request.SubscriptionType, request.SubscribedObjectUID, request.SendTo);

      subscription.Save();

      LandMessenger.Notify(GetSubscriptionEventType(request), subscription);

      return subscription;
    }


    static public Subscription Unsubscribe(string subscriptionUID, SubscriptionRequest request) {
      EnsureIsValid(request);

      var subscription = Subscription.TryParse(subscriptionUID);

      if (subscription == null) {
        throw new ResourceConflictException("Land.Subscription.NotFound",
                          $"No tenemos registrada una subscripción a los servicios de alerta registral " +
                          $"con identificador {subscriptionUID}.");
      }

      subscription.Unsubscribe();

      subscription.Save();

      LandMessenger.Notify(GetSubscriptionEventType(request), subscription);

      return subscription;
    }


    #endregion Services

    #region Private methods


    static private void EnsureIsValid(SubscriptionRequest request) {

    }


    static private void EnsureNoRegistered() {

    }


    static private bool ExistsSubscription(SubscriptionRequest subscriptionRequest) {
      return false;
    }


    private static SubscriptionEventType GetSubscriptionEventType(SubscriptionRequest request) {
      SubscriptionRequestCommand command = request.Command;
      SubscriptionType type = request.SubscriptionType;

      if (command == SubscriptionRequestCommand.Subscribe) {

        switch (type) {
          case SubscriptionType.CertificateChangesSubscription:
            return SubscriptionEventType.SubscribedForCertificateChanges;

          case SubscriptionType.RecordingDocumentChangesSubscription:
            return SubscriptionEventType.SubscribedForRecordingDocumentChanges;

          case SubscriptionType.ResourceChangesSubscription:
            return SubscriptionEventType.SubscribedForResourceChanges;
        }

      } else if (command == SubscriptionRequestCommand.ConfirmSubscription) {

        switch (type) {
          case SubscriptionType.CertificateChangesSubscription:
            return SubscriptionEventType.ConfirmedForCertificateChanges;

          case SubscriptionType.RecordingDocumentChangesSubscription:
            return SubscriptionEventType.ConfirmedForRecordingDocumentChanges;

          case SubscriptionType.ResourceChangesSubscription:
            return SubscriptionEventType.ConfirmedForResourceChanges;
        }

      } else if (command == SubscriptionRequestCommand.Unsubscribe) {

        switch (type) {
          case SubscriptionType.CertificateChangesSubscription:
            return SubscriptionEventType.UnsubscribedForCertificateChanges;

          case SubscriptionType.RecordingDocumentChangesSubscription:
            return SubscriptionEventType.UnsubscribedForRecordingDocumentChanges;

          case SubscriptionType.ResourceChangesSubscription:
            return SubscriptionEventType.UnsubscribedForResourceChanges;
        }

      }  // else if

      throw Assertion.AssertNoReachThisCode();
    }

    #endregion Private methods

  }  // class SubscriptionServices

}  // namespace Empiria.Land.Messaging
