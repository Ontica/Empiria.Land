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

using Empiria.Json;
using Empiria.Messaging;
using Empiria.StateEnums;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Messaging {

  public class LandMessenger {

    #region Fields

    private static readonly MessageQueue MESSAGE_QUEUE = MessageQueue.Parse("Land.SendDocumentsToRequester");

    #endregion Fields


    #region Public static methods


    /// <summary>Executes pending messages using a synchronous execution mode.</summary>
    static public void Execute() {

      EmpiriaLog.Info("LandMessenger was started.");

      var messages = MESSAGE_QUEUE.GetNextMessages();

      foreach (var message in messages) {
        if (message.IsReadyToProcess()) {
          ProcessMessage(message);
        }
      }

      EmpiriaLog.Info("LandMessenger was stopped. All messages were processed.");
    }


    /// <summary>Notifies the messenger about a workflow status change of a land transaction.</summary>
    static internal void Notify(LRSTransaction transaction,
                                NotificationType notificationType) {
      Assertion.AssertObject(transaction, "transaction");

      SendTo sendTo = transaction.ExtensionData.SendTo;

      if (!sendTo.IsEmptyInstance) {
        Notify(sendTo, transaction, notificationType);
      }

      sendTo = transaction.Agency.ExtendedData.Get<SendTo>("land.sendCompletedFilingsTo", SendTo.Empty);

      if (!sendTo.IsEmptyInstance) {
        Notify(sendTo, transaction, notificationType);
      }
    }


    /// <summary>Starts the execution engine for pending messages in asyncronous mode.</summary>
    static internal void Start() {

    }


    /// <summary>Stops the execution engine.</summary>
    static internal void Stop() {

    }

    #endregion Public static methods


    #region Private methods

    static private Resource GetResource(Message message) {
      var resource = Resource.TryParseWithUID(message.UnitOfWorkUID);

      Assertion.AssertObject(resource,
                            $"Unrecognized resource with UID {message.UnitOfWorkUID}.");

      return resource;
    }


    static private LRSTransaction GetTransaction(Message message) {
      var transaction = LRSTransaction.TryParse(message.UnitOfWorkUID);

      Assertion.AssertObject(transaction,
                            $"Unrecognized transaction with UID {message.UnitOfWorkUID}.");

      return transaction;
    }


    static private void Notify(SendTo sendTo, LRSTransaction transaction,
                               NotificationType notificationType) {
      var data = new JsonObject();

      data.Add("NotificationType", notificationType.ToString());
      data.Add("SendTo", sendTo.ToJson());

      var newMessage = new Message(data);

      MESSAGE_QUEUE.AddMessage(newMessage, transaction.UID);
    }


    static private void ProcessMessage(Message message) {
      var json = new JsonObject();

      try {
        var notificationType = message.MessageData.Get<NotificationType>("NotificationType");

        SendEmail(message);

        json.Add("Result", "E-mail was sent.");

        MESSAGE_QUEUE.MarkAsProcessed(message, json, ExecutionStatus.Completed);

      } catch (Exception e) {
        json.Add("FailedReason", $"E-mail was not sent because: {e.Message}.");

        MESSAGE_QUEUE.MarkAsProcessed(message, json, ExecutionStatus.Failed);

      }
    }


    static private void SendEmail(Message message) {
      var notificationType = message.MessageData.Get<NotificationType>("NotificationType");

      var emailContentBuilder = new LandEMailContentBuilder();

      EMailContent content = null;

      switch (notificationType) {

        case NotificationType.RegisterForResourceChanges:
          content = emailContentBuilder.BuildForRegisterForResourceChanges(GetResource(message));
          break;

        case NotificationType.ResourceWasChanged:
          content = emailContentBuilder.BuildForResourceChanged(GetResource(message));
          break;

        case NotificationType.TransactionDelayed:
          content = emailContentBuilder.BuildForTransactionDelayed(GetTransaction(message));
          break;

        case NotificationType.TransactionFinished:
          content = emailContentBuilder.BuildForTransactionFinished(message);
          break;

        case NotificationType.TransactionReceived:
          content = emailContentBuilder.BuildForTransactionReceived(GetTransaction(message));
          break;

        case NotificationType.TransactionReentered:
          content = emailContentBuilder.BuildForTransactionReentered(message);
          break;

        case NotificationType.TransactionReturned:
          content = emailContentBuilder.BuildForTransactionReturned(GetTransaction(message));
          break;

        case NotificationType.TransactionArchived:
          content = emailContentBuilder.BuildForTransactionFinished(message);
          break;

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled notificationType {notificationType.ToString()}.");

      }

      var sendTo = message.MessageData.Get<SendTo>("SendTo");

      EMail.Send(sendTo, content);
    }

    #endregion Private methods


  }  // class LandMessenger

}  // namespace Empiria.Land.Messaging
