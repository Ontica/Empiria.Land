﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSWorkflow                                    Pattern  : Micro-workflow                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Micro-workflow for the Land Registration System.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;

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
      this.CurrentStatus = LRSTransactionStatus.Payment;
      this.taskList = new Lazy<LRSWorkflowTaskList>(() => new LRSWorkflowTaskList());
    }

    internal static LRSWorkflow Create(LRSTransaction transaction) {
      var workflow = new LRSWorkflow(transaction);
      workflow.CurrentStatus = LRSTransactionStatus.Payment;
      workflow.Tasks.Add(LRSWorkflowTask.CreateFirst(transaction));

      return workflow;
    }

    static internal LRSWorkflow Parse(LRSTransaction transaction,
                                      LRSTransactionStatus onLoadStatus = LRSTransactionStatus.Undefined) {
      var workflow = new LRSWorkflow(transaction);

      if (onLoadStatus != LRSTransactionStatus.Undefined) {
        workflow.CurrentStatus = onLoadStatus;
      } else {
        workflow.CurrentStatus = workflow.GetCurrentTask().CurrentStatus;
      }
      workflow.taskList = new Lazy<LRSWorkflowTaskList>(() => LRSWorkflowTaskList.Parse(transaction));

      return workflow;
    }

    #endregion Constructors and parsers

    #region Properties

    public LRSTransactionStatus CurrentStatus {
      get;
      private set;
    } = LRSTransactionStatus.Payment;


    public string CurrentStatusName {
      get {
        return LRSWorkflowRules.GetStatusName(this.CurrentStatus);
      }
    }


    public bool IsEmptyItemsTransaction {
      get {
        return LRSWorkflowRules.IsEmptyItemsTransaction(_transaction);
      }
    }


    public bool IsReadyForReentry {
      get {
        return LRSWorkflowRules.IsReadyForReentry(_transaction);
      }
    }

    public LRSWorkflowTaskList Tasks {
      get {
        return taskList.Value;
      }
    }

    #endregion Properties

    #region Public methods

    public void Receive(string notes) {
      if (this.CurrentStatus != LRSTransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }
      if (_transaction.Payments.Count == 0 && !this.IsEmptyItemsTransaction) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }

      //    using (var context = StorageContext.Open()) {
      _transaction.PresentationTime = DateTime.Now;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      this.CurrentStatus = LRSTransactionStatus.Received;

      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = LRSTransactionStatus.Received;
      currentTask.NextContact = LRSWorkflowRules.InterestedContact;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = LRSWorkflowRules.GetNextStatusAfterReceive(_transaction);
      currentTask.Status = WorkflowTaskStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflowRules.InterestedContact;
      currentTask.Save();

      _transaction.Save();
      this.ResetTasksList();
      //  }
    }

    public void ReturnToMe() {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetPending();

      _transaction.Save();
      ResetTasksList();
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
      var task = this.Tasks.Find((x) => x.CurrentStatus == LRSTransactionStatus.Received);

      if (task != null) {
        return task.Responsible;
      } else {
        return Person.Empty;
      }
    }

    public void SetNextStatus(LRSTransactionStatus nextStatus, Contact nextContact, string notes) {
      if (nextStatus == LRSTransactionStatus.Returned || nextStatus == LRSTransactionStatus.Delivered ||
          nextStatus == LRSTransactionStatus.Archived) {
        this.Close(nextStatus, notes);
        return;
      }
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetNextStatus(nextStatus, nextContact, notes);

      if (nextStatus == LRSTransactionStatus.OnSign || nextStatus == LRSTransactionStatus.Revision) {
        _transaction.Save();
      } else if (this.CurrentStatus == LRSTransactionStatus.OnSign &&
                (nextStatus == LRSTransactionStatus.ToDeliver || nextStatus == LRSTransactionStatus.Archived)) {
        _transaction.Save();
      }
    }

    public void Take(string notes) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.NextStatus == LRSTransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }
      this.CurrentStatus = currentTask.NextStatus;
      currentTask.CreateNext(notes);
      ResetTasksList();

      if (this.CurrentStatus == LRSTransactionStatus.ToDeliver ||
          this.CurrentStatus == LRSTransactionStatus.ToReturn || this.CurrentStatus == LRSTransactionStatus.Archived) {
        _transaction.ClosingTime = currentTask.EndProcessTime;
      }
      _transaction.Save();
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

    public void DoReentry(string notes) {
      if (!this.IsReadyForReentry) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }
      this.CurrentStatus = LRSTransactionStatus.Reentry;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      _transaction.LastReentryTime = DateTime.Now;
      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = LRSTransactionStatus.Reentry;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = LRSTransactionStatus.Control;
      currentTask.Status = WorkflowTaskStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflowRules.InterestedContact;
      currentTask.Save();
      _transaction.Save();
      this.ResetTasksList();
    }

    public LRSWorkflowTask GetCurrentTask() {
      return WorkflowData.GetWorkflowLastTask(_transaction);
    }

    public void PullToControlDesk(string notes) {
      if (notes.Length == 0) {
        notes = "Se jaló a la mesa de control";
      }
      this.SetNextStatus(LRSTransactionStatus.Control, Person.Empty, notes);
      this.Take(String.Empty);
    }

    public void Unarchive(string notes) {
      Assertion.Assert(this.CurrentStatus == LRSTransactionStatus.Archived,
                       "Transaction current status should be Archived.");
      this.SetNextStatus(LRSTransactionStatus.Control, Person.Empty, notes);
      this.Take(String.Empty);
    }

    #endregion Public methods

    #region Private methods

    private void Close(LRSTransactionStatus closeStatus, string notes) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.NextStatus = closeStatus;
      currentTask = currentTask.CreateNext(notes);

      ResetTasksList();

      currentTask.Notes = notes;
      currentTask.Close();

      _transaction.LastDeliveryTime = currentTask.EndProcessTime;
      this.CurrentStatus = closeStatus;
      _transaction.Save();
    }

    private void ResetTasksList() {
      taskList = new Lazy<LRSWorkflowTaskList>(() => LRSWorkflowTaskList.Parse(_transaction));
    }

    #endregion Private methods

  }  // class LRSWorkflow

}  // namespace Empiria.Land.Registration.Transactions
