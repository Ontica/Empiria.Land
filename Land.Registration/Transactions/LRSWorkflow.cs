/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSWorkflow                                    Pattern  : Micro-workflow                      *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Micro-workflow for the Land Registration System.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Data;
using Empiria.Land.Messaging;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Micro-workflow for the Land Registration System.</summary>
  public class LRSWorkflow {

    #region Fields

    private LRSTransaction _transaction = null;
    private Lazy<LRSWorkflowTaskList> taskList = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSWorkflow(LRSTransaction transaction) {
      _transaction = transaction;
      this.CurrentStatus = TransactionStatus.Payment;
      this.taskList = new Lazy<LRSWorkflowTaskList>(() => new LRSWorkflowTaskList());
    }

    internal static LRSWorkflow Create(LRSTransaction transaction) {
      var workflow = new LRSWorkflow(transaction);

      workflow.CurrentStatus = TransactionStatus.Payment;
      workflow.Tasks.Add(LRSWorkflowTask.CreateFirst(transaction));

      return workflow;
    }

    static internal LRSWorkflow Parse(LRSTransaction transaction,
                                      TransactionStatus onLoadStatus = TransactionStatus.Undefined) {
      var workflow = new LRSWorkflow(transaction);

      if (onLoadStatus != TransactionStatus.Undefined) {
        workflow.CurrentStatus = onLoadStatus;
      } else {
        workflow.CurrentStatus = workflow.GetCurrentTask().CurrentStatus;
      }
      workflow.taskList = new Lazy<LRSWorkflowTaskList>(() => LRSWorkflowTaskList.Parse(transaction));

      return workflow;
    }

    #endregion Constructors and parsers

    #region Properties

    public TransactionStatus CurrentStatus {
      get;
      private set;
    } = TransactionStatus.Payment;


    public TransactionStatus NextStatus {
      get {
        if (this._transaction.IsNew) {
          return TransactionStatus.Undefined;
        } else {
          return this.GetCurrentTask().NextStatus;
        }
      }
    }


    public LRSWorkflowTaskList Tasks {
      get {
        return taskList.Value;
      }
    }


    #endregion Properties

    #region Public methods

    internal void Delete() {

      Assertion.Require(LRSWorkflowRules.CanBeDeleted(_transaction),
                        "This transaction workflow cannot be deleted.");

      this.Close(TransactionStatus.Deleted,
            $"Deleted by user {ExecutionServer.CurrentContact.FullName} on {DateTime.Now}.");

    }


    public void DeliverElectronicallyToAgency() {

      Assertion.Require(LRSWorkflowRules.IsReadyForDeliveryOrReturn(_transaction),
        $"Transaction {_transaction.UID} is not ready to be electronically delivered to the agency.");

      if (this.CurrentStatus == TransactionStatus.ToDeliver) {
        this.Close(TransactionStatus.Delivered,
                   "Entregado a través del sistema de notarías.",
                   LRSWorkflowRules.InterestedContact, DateTime.Now);

      } else if (this.CurrentStatus == TransactionStatus.ToReturn) {
        this.Close(TransactionStatus.Returned,
                   "Devuelto a través del sistema de notarías.",
                   LRSWorkflowRules.InterestedContact, DateTime.Now);

      } else {
        throw Assertion.EnsureNoReachThisCode();

      }
    }


    public void DeliverElectronicallyToRequester(string messageUID) {

      if (!LRSWorkflowRules.IsReadyForElectronicDelivery(_transaction, messageUID)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotReadyForElectronicalDelivery,
                                            _transaction.UID);
      }

      this.Close(TransactionStatus.Delivered,
                 "Entregado al interesado a través del portal de consultas.",
                 LRSWorkflowRules.InterestedContact, DateTime.Now);
    }


    private LRSWorkflowTask _currentTask = null;
    public LRSWorkflowTask GetCurrentTask() {
      if (_currentTask == null) {
        _currentTask = WorkflowData.GetWorkflowLastTask(_transaction);
      }
      return _currentTask;
    }


    internal Contact GetPostedBy() {
      if (this.Tasks.Count != 0) {
        return this.Tasks[0].Responsible;
      } else {
        return Person.Empty;
      }
    }


    internal DateTime GetPostingTime() {
      if (this.Tasks.Count != 0) {
        return this.Tasks[0].CheckInTime;
      } else {
        return ExecutionServer.DateMaxValue;
      }
    }


    internal Contact GetReceivedBy() {
      var task = this.Tasks.Find((x) => x.CurrentStatus == TransactionStatus.Received);

      if (task != null) {
        return task.Responsible;
      } else {
        return Person.Empty;
      }
    }


    public void PullToControlDesk(string notes) {
      if (notes.Length == 0) {
        notes = "Se trajo a la mesa de control";
      }

      this.SetNextStatus(TransactionStatus.Control, Person.Empty, notes);

      this.Take(String.Empty);
    }


    public void Receive(string notes) {
      LRSWorkflowRules.CanBeReceived(_transaction);


      _transaction.SetInternalControlNumber();
      _transaction.PresentationTime = DateTime.Now;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;

      this.CurrentStatus = TransactionStatus.Received;

      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Received;
      currentTask.NextContact = LRSWorkflowRules.InterestedContact;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = LRSWorkflowRules.GetNextStatusAfterReceive(_transaction);
      currentTask.Status = WorkflowTaskStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflowRules.InterestedContact;
      currentTask.Save();

      _transaction.Save();

      this.ResetTasksList();

      LandMessenger.Notify(_transaction, TransactionEventType.TransactionReceived);
    }


    public void Reentry() {
      if (!LRSWorkflowRules.IsReadyForReentry(_transaction)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }

      // LRSWorkflowRules.AssertGraceDaysForReentry(_transaction);
      // LRSWorkflowRules.AssertRecordingActsPrelation(_transaction);
      // LRSWorkflowRules.AssertDigitalizedDocument(_transaction);

      this.CurrentStatus = TransactionStatus.Reentry;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      _transaction.LastReentryTime = DateTime.Now;
      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Reentry;

      currentTask = currentTask.CreateNext("Trámite reingresado");
      currentTask.NextStatus = TransactionStatus.Control;
      currentTask.Status = WorkflowTaskStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflowRules.InterestedContact;
      currentTask.Save();
      _transaction.Save();
      this.ResetTasksList();

      LandMessenger.Notify(_transaction, TransactionEventType.TransactionReentered);
    }


    public void ReturnToMe() {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetPending();

      _transaction.Save();
      ResetTasksList();
    }


    public void SetNextStatus(TransactionStatus nextStatus, Contact nextContact,
                              string notes, DateTime? date = null) {

      if (nextStatus == TransactionStatus.Returned ||
          nextStatus == TransactionStatus.Delivered ||
          nextStatus == TransactionStatus.Archived) {
        if (date.HasValue) {
          this.Close(nextStatus, notes, nextContact, date.Value);
        } else {
          this.Close(nextStatus, notes);
        }
        return;
      }

      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetNextStatus(nextStatus, nextContact, notes, date);

      if (nextStatus == TransactionStatus.OnSign || nextStatus == TransactionStatus.Revision) {
        _transaction.Save();
      } else if (this.CurrentStatus == TransactionStatus.OnSign &&
                (nextStatus == TransactionStatus.ToDeliver || nextStatus == TransactionStatus.Archived)) {
        _transaction.Save();
      }
    }


    public void Take(string notes) {
      var responsible = ExecutionServer.CurrentContact;

      this.Take(notes, responsible, DateTime.Now);
    }


    public void Take(string notes, Contact responsible, DateTime date) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.NextStatus == TransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }

      string s = LRSWorkflowRules.ValidateStatusChange(_transaction, currentTask.NextStatus);

      if (!String.IsNullOrWhiteSpace(s)) {
        Assertion.RequireFail(s);
        return;
      }

      this.CurrentStatus = currentTask.NextStatus;

      currentTask.CreateNext(notes, responsible, date);

      ResetTasksList();

      if (this.CurrentStatus == TransactionStatus.ToDeliver ||
          this.CurrentStatus == TransactionStatus.ToReturn ||
          this.CurrentStatus == TransactionStatus.Archived) {
        _transaction.ClosingTime = currentTask.EndProcessTime;
      }

      _transaction.Save();

      if (this.CurrentStatus == TransactionStatus.ToDeliver) {
        LandMessenger.Notify(_transaction, TransactionEventType.TransactionReadyToDelivery);
      } else if (this.CurrentStatus == TransactionStatus.ToReturn) {
        LandMessenger.Notify(_transaction, TransactionEventType.TransactionReturned);
      }
    }


    public void Undelete() {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.Status == WorkflowTaskStatus.OnDelivery) {
        this.CurrentStatus = currentTask.CurrentStatus;
      } else if (currentTask.Status == WorkflowTaskStatus.Pending) {
        this.CurrentStatus = currentTask.CurrentStatus;
      } else if (currentTask.Status == WorkflowTaskStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }

      _transaction.Save();
    }

    #endregion Public methods

    #region Private methods

    private void Close(TransactionStatus closeStatus, string notes) {
      var responsible = ExecutionServer.CurrentContact;

      this.Close(closeStatus, notes, responsible, DateTime.Now);
    }


    private void Close(TransactionStatus closeStatus, string notes,
                       Contact responsible, DateTime date) {

      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.NextStatus = closeStatus;
      currentTask = currentTask.CreateNext(notes, responsible, date);

      ResetTasksList();

      currentTask.Notes = notes;

      currentTask.Close(date);

      _transaction.LastDeliveryTime = currentTask.EndProcessTime;
      this.CurrentStatus = closeStatus;

      _transaction.Save();

      if (closeStatus == TransactionStatus.Archived) {
        LandMessenger.Notify(_transaction, TransactionEventType.TransactionArchived);
      }
    }

    private void ResetTasksList() {
      taskList = new Lazy<LRSWorkflowTaskList>(() => LRSWorkflowTaskList.Parse(_transaction));

      _currentTask = null;
    }

    #endregion Private methods

  }  // class LRSWorkflow

}  // namespace Empiria.Land.Registration.Transactions
