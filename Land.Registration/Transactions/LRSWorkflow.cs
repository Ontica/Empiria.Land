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


    public LRSTransactionStatus NextStatus {
      get {
        if (this._transaction.IsNew) {
          return LRSTransactionStatus.Undefined;
        } else {
          return this.GetCurrentTask().NextStatus;
        }
      }
    }


    public string CurrentStatusName {
      get {
        return LRSWorkflowRules.GetStatusName(this.CurrentStatus);
      }
    }


    public bool DeliveredOrReturned {
      get {
        return (this.CurrentStatus == LRSTransactionStatus.Delivered ||
                this.CurrentStatus == LRSTransactionStatus.Returned);
      }
    }


    public bool IsArchivable {
      get {
        return LRSWorkflowRules.IsArchivable(_transaction.TransactionType,
                                             _transaction.DocumentType);
      }
    }

    public bool IsEmptyItemsTransaction {
      get {
        return LRSWorkflowRules.IsEmptyItemsTransaction(_transaction);
      }
    }


    public bool IsFinished {
      get {
        return (this.DeliveredOrReturned ||
                this.CurrentStatus == LRSTransactionStatus.Archived);
      }
    }


    public bool IsReadyForDeliveryOrReturn {
      get {
        return (this.CurrentStatus == LRSTransactionStatus.ToDeliver ||
                this.CurrentStatus == LRSTransactionStatus.ToReturn);
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

    public bool CanBeDeleted {
      get {
        return (this.CurrentStatus == LRSTransactionStatus.Payment);
      }
    }

    #endregion Properties

    #region Public methods

    internal void Delete() {
      Assertion.Assert(this.CanBeDeleted, "This transaction can not be deleted.");

      this.Close(LRSTransactionStatus.Deleted,
            $"Deleted by user {ExecutionServer.CurrentIdentity.User.FullName} on {DateTime.Now}.");

    }


    public void DeliveredElectronicallyToAgency() {
      Assertion.Assert(this.IsReadyForDeliveryOrReturn,
        $"Transaction {_transaction.UID} is not ready to be electronically delivered to the agency.");

      if (this.CurrentStatus == LRSTransactionStatus.ToDeliver) {
        this.Close(LRSTransactionStatus.Delivered,
                   "Entregado a través del sistema de notarías.",
                   LRSWorkflowRules.InterestedContact, DateTime.Now);

      } else if (this.CurrentStatus == LRSTransactionStatus.ToReturn) {
        this.Close(LRSTransactionStatus.Returned,
                   "Devuelto a través del sistema de notarías.",
                   LRSWorkflowRules.InterestedContact, DateTime.Now);

      } else {
        throw Assertion.AssertNoReachThisCode();

      }
    }


    public void DeliverElectronically(string messageUID) {
      if (!IsReadyForElectronicDelivery(messageUID)) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NotReadyForElectronicalDelivery,
                                            _transaction.UID);
      }
      this.Close(LRSTransactionStatus.Delivered,
                 "Entregado al interesado a través del portal de consultas.",
                 LRSWorkflowRules.InterestedContact, DateTime.Now);
    }


    public bool IsReadyForElectronicDelivery(string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return false;
      }
      if (!this.IsReadyForDeliveryOrReturn) {
        return false;
      }
      if (LRSWorkflowRules.IsDigitalizable(_transaction.TransactionType,
                                            _transaction.DocumentType)) {
        return false;
      }
      return true;
    }


    public void Receive(string notes) {
      if (this.CurrentStatus != LRSTransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }
      if (_transaction.Payments.Count == 0 && !_transaction.IsFeeWaiverApplicable
          && !this.IsEmptyItemsTransaction) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }


      //    using (var context = StorageContext.Open()) {
      _transaction.SetInternalControlNo();
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

      LandMessenger.Notify(_transaction, TransactionEventType.TransactionReceived);

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


    public void Reentry() {
      if (!this.IsReadyForReentry) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }

      this.AssertGraceDaysForReentry();
      this.AssertRecordingActsPrelation();
      this.AssertDigitalizedDocument();

      this.CurrentStatus = LRSTransactionStatus.Reentry;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      _transaction.LastReentryTime = DateTime.Now;
      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = LRSTransactionStatus.Reentry;

      currentTask = currentTask.CreateNext("Trámite reingresado");
      currentTask.NextStatus = LRSTransactionStatus.Control;
      currentTask.Status = WorkflowTaskStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflowRules.InterestedContact;
      currentTask.Save();
      _transaction.Save();
      this.ResetTasksList();

      LandMessenger.Notify(_transaction, TransactionEventType.TransactionReentered);
    }


    public void SetNextStatus(LRSTransactionStatus nextStatus, Contact nextContact,
                              string notes, DateTime? date = null) {

      if (nextStatus == LRSTransactionStatus.Returned ||
          nextStatus == LRSTransactionStatus.Delivered ||
          nextStatus == LRSTransactionStatus.Archived) {
        if (date.HasValue) {
          this.Close(nextStatus, notes, nextContact, date.Value);
        } else {
          this.Close(nextStatus, notes);
        }
        return;
      }

      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetNextStatus(nextStatus, nextContact, notes, date);

      if (nextStatus == LRSTransactionStatus.OnSign || nextStatus == LRSTransactionStatus.Revision) {
        _transaction.Save();
      } else if (this.CurrentStatus == LRSTransactionStatus.OnSign &&
                (nextStatus == LRSTransactionStatus.ToDeliver || nextStatus == LRSTransactionStatus.Archived)) {
        _transaction.Save();
      }
    }

    public void Take(string notes) {
      var responsible = Contact.Parse(ExecutionServer.CurrentUserId);

      this.Take(notes, responsible, DateTime.Now);
    }


    public void Take(string notes, Contact responsible, DateTime date) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.NextStatus == LRSTransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }

      string s = LRSWorkflowRules.ValidateStatusChange(_transaction, currentTask.NextStatus);

      if (!String.IsNullOrWhiteSpace(s)) {
        Assertion.AssertFail(s);
        return;
      }

      this.CurrentStatus = currentTask.NextStatus;
      currentTask.CreateNext(notes, responsible, date);
      ResetTasksList();

      if (this.CurrentStatus == LRSTransactionStatus.ToDeliver ||
          this.CurrentStatus == LRSTransactionStatus.ToReturn ||
          this.CurrentStatus == LRSTransactionStatus.Archived) {
        _transaction.ClosingTime = currentTask.EndProcessTime;
      }
      _transaction.Save();
      if (this.CurrentStatus == LRSTransactionStatus.ToDeliver) {
        LandMessenger.Notify(_transaction, TransactionEventType.TransactionReadyToDelivery);
      } else if (this.CurrentStatus == LRSTransactionStatus.ToReturn) {
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


    private LRSWorkflowTask _currentTask = null;
    public LRSWorkflowTask GetCurrentTask() {
      if (_currentTask == null) {
        _currentTask = WorkflowData.GetWorkflowLastTask(_transaction);
      }
      return _currentTask;
    }


    public void PullToControlDesk(string notes) {
      if (notes.Length == 0) {
        notes = "Se trajo a la mesa de control";
      }
      this.SetNextStatus(LRSTransactionStatus.Control,
                         Person.Empty, notes);
      this.Take(String.Empty);
    }

    #endregion Public methods

    #region Private methods

    private void AssertDigitalizedDocument() {
      if (!LRSWorkflowRules.IsDigitalizable(_transaction.TransactionType,
                                            _transaction.DocumentType)) {
        return;
      }

      if (_transaction.Document.IsEmptyInstance) {
        return;
      }

      if (_transaction.Document.Imaging.HasImageSet) {
        return;
      }

      if (ExecutionServer.CurrentUserId == 2) {
        return;
      }

      // int graceDaysForImaging = ConfigurationData.GetInteger("GraceDaysForImaging");
      int graceDaysForImaging = 90;

      DateTime lastDate = _transaction.Document.AuthorizationTime;

      if (lastDate.AddDays(graceDaysForImaging) <= DateTime.Now) {
        Assertion.AssertFail("No es posible reingresar este trámite debido a que el documento " +
                             "que se registró aún no ha sido digitalizado y ya " +
                             "transcurrieron más de {0} días desde que éste se cerró.\n\n" +
                             "Favor de preguntar en la mesa de armado acerca de este documento.",
                             graceDaysForImaging);
      }
    }


    private void AssertGraceDaysForReentry() {
      // int graceDaysForReentry = ConfigurationData.GetInteger("GraceDaysForReentry");

      if (ExecutionServer.CurrentUserId == 2) {
        return;
      }

      int graceDaysForReentry = 90;

      DateTime lastDate = _transaction.PresentationTime;

      //if (_transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
      //  lastDate = _transaction.LastReentryTime;
      //}
      if (!_transaction.Document.IsEmptyInstance) {
        lastDate = _transaction.Document.AuthorizationTime;
      }

      if (lastDate.AddDays(graceDaysForReentry) <= DateTime.Now) {
        Assertion.AssertFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             "no es posible reingresar trámites después de {0} días contados " +
                             "a partir de su fecha de presentación original, de su fecha de registro, o bien, " +
                             "de la fecha del último reingreso.\n\n" +
                             "En su lugar se debe optar por registrar un nuevo trámite.",
                             graceDaysForReentry);
      }
    }


    private void AssertRecordingActsPrelation() {
      if (_transaction.Document.IsEmptyInstance || _transaction.Document.IsEmptyDocumentType) {
        return;
      }
      foreach (var recordingAct in _transaction.Document.RecordingActs) {
        recordingAct.AssertIsLastInPrelationOrder();
      }
    }


    private void Close(LRSTransactionStatus closeStatus, string notes) {
      var responsible = Contact.Parse(ExecutionServer.CurrentUserId);

      this.Close(closeStatus, notes, responsible, DateTime.Now);
    }


    private void Close(LRSTransactionStatus closeStatus, string notes,
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

      if (closeStatus == LRSTransactionStatus.Archived) {
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
