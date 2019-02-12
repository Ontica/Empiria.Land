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
using System.Threading;

using Empiria.Json;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;
using Empiria.Messaging;
using Empiria.StateEnums;

namespace Empiria.Land.Messaging {

  public class LandMessenger {

    #region Fields

    private static readonly MessageQueue MESSAGE_QUEUE = MessageQueue.Parse("Land.SendDocumentsToRequester");

    private static volatile bool isRunning = false;
    private static volatile Timer timer = null;

    #endregion Fields


    #region Public methods


    /// <summary>Notifies messenger about a workflow status change of a land transaction.</summary>
    static internal void Notify(LRSTransaction transaction,
                                NotificationType notificationType) {
      Assertion.AssertObject(transaction, "transaction");

      NotifyInterested(transaction, notificationType);
      NotifyAgency(transaction, notificationType);
      NotifyRegistered(transaction, notificationType);
    }


    /// <summary>Starts the execution engine for pending messages in asyncronous mode.</summary>
    static public void Start() {
      if (isRunning) {
        return;
      }

      int MESSAGE_ENGINE_EXECUTION_MINUTES = ConfigurationData.Get("MessageEngine.Execution.Minutes", 1);

      timer = new Timer(SendQueuedMessages, null, 10 * 1000,
                        MESSAGE_ENGINE_EXECUTION_MINUTES * 60 * 1000);

      isRunning = true;
      EmpiriaLog.Info("LandMessenger was started.");
    }


    /// <summary>Stops the execution engine.</summary>
    static public void Stop() {
      if (!isRunning) {
        return;
      }
      timer.Dispose();
      timer = null;
      isRunning = false;

      EmpiriaLog.Info("LandMessenger was stopped.");
    }


    #endregion Public methods


    #region Message queue execution methods


    private static bool IsMessageReadyToProcess(Message message) {
      if (!message.IsInProcessStatus) {
        return false;
      }

      int waitMinutes = WaitMinutesToProcessMessage(message);

      return (message.PostingTime.AddMinutes(waitMinutes) < DateTime.Now);
    }


    static private void ProcessQueuedMessage(Message message) {
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
          content = emailContentBuilder.BuildForTransactionReceived(message);
          break;

        case NotificationType.TransactionReentered:
          content = emailContentBuilder.BuildForTransactionReentered(message);
          break;

        case NotificationType.TransactionReturned:
          content = emailContentBuilder.BuildForTransactionReturned(message);
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


    /// <summary>Executes pending messages using a synchronous execution mode.</summary>
    static private void SendQueuedMessages(object stateInfo) {
      var messages = MESSAGE_QUEUE.GetNextMessages();

      int count = 0;
      foreach (var message in messages) {
        if (IsMessageReadyToProcess(message)) {
          ProcessQueuedMessage(message);
          count++;
        }
      }

      EmpiriaLog.Info($"LandMessenger was executed with {count} messages.");
    }


    static private int WaitMinutesToProcessMessage(Message message) {
      var notificationType = message.MessageData.Get<NotificationType>("NotificationType");

      int IMMEDIATELY = ConfigurationData.Get("Immediately.Execution.Minutes", 0);
      int SOME_WAIT = ConfigurationData.Get("SomeWait.Execution.Minutes", 5);

      switch (notificationType) {

        case NotificationType.RegisterForResourceChanges:
        case NotificationType.TransactionReceived:
        case NotificationType.TransactionReentered:
          return IMMEDIATELY;


        case NotificationType.ResourceWasChanged:
        case NotificationType.TransactionDelayed:
        case NotificationType.TransactionFinished:
        case NotificationType.TransactionReturned:
        case NotificationType.TransactionArchived:
          return SOME_WAIT;

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled notificationType {notificationType.ToString()}.");

      }
    }


    #endregion Message queue execution methods


    #region Queue notification methods


    static private void EnqueueNotification(SendTo sendTo, LRSTransaction transaction,
                                            NotificationType notificationType) {
      var data = new JsonObject();

      data.Add("NotificationType", notificationType.ToString());
      data.Add("SendTo", sendTo.ToJson());

      var newMessage = new Message(data);

      MESSAGE_QUEUE.AddMessage(newMessage, transaction.UID);
    }


    static private void NotifyAgency(LRSTransaction transaction, NotificationType notificationType) {
      if (notificationType == NotificationType.RegisterForResourceChanges ||
          notificationType == NotificationType.ResourceWasChanged ||
          notificationType == NotificationType.TransactionReceived) {
        return;
      }

      SendTo sendTo = transaction.Agency.ExtendedData.Get<SendTo>("land.sendCompletedFilingsTo", SendTo.Empty);

      if (!sendTo.IsEmptyInstance) {
        EnqueueNotification(sendTo, transaction, notificationType);
      }
    }


    static private void NotifyInterested(LRSTransaction transaction, NotificationType notificationType) {
      SendTo sendTo = transaction.ExtensionData.SendTo;

      if (!sendTo.IsEmptyInstance) {
        EnqueueNotification(sendTo, transaction, notificationType);
      }
    }


    static private void NotifyRegistered(LRSTransaction transaction, NotificationType notificationType) {
      if (notificationType != NotificationType.ResourceWasChanged) {
        return;
      }
    }


    #endregion Queue notification methods


    #region Utility methods


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


    #endregion Utility methods


  }  // class LandMessenger

}  // namespace Empiria.Land.Messaging
