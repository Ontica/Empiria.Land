/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LRSWorkflowTask                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a land registration workflow task.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Transactions.Workflow.Data;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Enumerates the different workflow statuses that a workflow task can have.</summary>
  public enum WorkflowTaskStatus {
    Pending = 'P',
    OnDelivery = 'D',
    Closed = 'C',
    Deleted = 'X',
    Historic = 'H',
  }

  /// <summary>Indicates if the workflow task assignment is automatic or assigned manually.</summary>
  public enum WorkflowTrackMode {
    Automatic = 'A',
    Manual = 'M',
  }

  /// <summary>Represents a land registration workflow task.</summary>
  public class LRSWorkflowTask : BaseObject {

    #region Constructors and parsers

    private LRSWorkflowTask() {
      // Required by Empiria Framework.
    }

    internal LRSWorkflowTask(LRSTransaction transaction) {
      this.Transaction = transaction;
    }

    static public LRSWorkflowTask Parse(int id) {
      return BaseObject.ParseId<LRSWorkflowTask>(id);
    }

    static public LRSWorkflowTask Empty {
      get { return BaseObject.ParseEmpty<LRSWorkflowTask>(); }
    }

    static internal LRSWorkflowTask CreateFirst(LRSTransaction transaction) {
      LRSWorkflowTask task = new LRSWorkflowTask(transaction);

      task.AssignedBy = LRSWorkflowRules.InterestedContact;
      task.CurrentStatus = transaction.Workflow.CurrentStatus;
      task.EndProcessTime = task.CheckInTime;
      task.Responsible = ExecutionServer.CurrentContact;
      task.Save();

      return task;
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TransactionId")]
    public LRSTransaction Transaction {
      get;
      private set;
    }

    [DataField("EventId", Default = -1)]
    public int EventId {
      get;
      set;
    }

    [DataField("Mode", Default = WorkflowTrackMode.Manual)]
    public WorkflowTrackMode Mode {
      get;
      set;
    }

    [DataField("AssignedById")]
    public Contact AssignedBy {
      get;
      set;
    }

    [DataField("ResponsibleId")]
    public Contact Responsible {
      get;
      set;
    }

    [DataField("NextContactId")]
    public Contact NextContact {
      get;
      set;
    }

    [DataField("CurrentTransactionStatus", Default = TransactionStatus.Undefined)]
    public TransactionStatus CurrentStatus {
      get;
      set;
    }

    public string CurrentStatusName {
      get { return this.CurrentStatus.GetStatusName(); }
    }

    [DataField("NextTransactionStatus", Default = TransactionStatus.EndPoint)]
    public TransactionStatus NextStatus {
      get;
      set;
    }

    public string NextStatusName {
      get { return this.NextStatus.GetStatusName(); }
    }

    [DataField("CheckInTime", Default = "DateTime.Now")]
    public DateTime CheckInTime {
      get;
      private set;
    }

    [DataField("EndProcessTime")]
    public DateTime EndProcessTime {
      get;
      internal set;
    }

    [DataField("CheckOutTime")]
    public DateTime CheckOutTime {
      get;
      private set;
    }

    public TimeSpan ElapsedTime {
      get {
        if (this.CheckOutTime == ExecutionServer.DateMaxValue) {
          return DateTime.Now.Subtract(this.CheckInTime);
        } else {
          return this.CheckOutTime.Subtract(this.CheckInTime);
        }
      }
    }


    [DataField("TrackNotes")]
    public string Notes {
      get;
      set;
    }

    [DataField("TrackStatus", Default = WorkflowTaskStatus.Pending)]
    public WorkflowTaskStatus Status {
      get;
      internal set;
    }

    [DataField("PreviousTrackId")]
    private LazyInstance<LRSWorkflowTask> _previousTask = LazyInstance<LRSWorkflowTask>.Empty;
    public LRSWorkflowTask PreviousTask {
      get { return _previousTask.Value; }
      private set {
        _previousTask = LazyInstance<LRSWorkflowTask>.Parse(value);
      }
    }

    [DataField("NextTrackId")]
    private LazyInstance<LRSWorkflowTask> _nextTask = LazyInstance<LRSWorkflowTask>.Empty;
    public LRSWorkflowTask NextTask {
      get { return _nextTask.Value; }
      private set {
        _nextTask = LazyInstance<LRSWorkflowTask>.Parse(value);
      }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case WorkflowTaskStatus.Pending:
            return "Pendiente";
          case WorkflowTaskStatus.OnDelivery:
            return "Por entregar";
          case WorkflowTaskStatus.Closed:
            return "Terminado";
          case WorkflowTaskStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      } // get
    }

    #endregion Public properties

    #region Public method

    internal void Close() {
      DateTime closingDate = ExecutionServer.NowWithoutSeconds;

      if (this.EndProcessTime == ExecutionServer.DateMaxValue) {
        this.EndProcessTime = closingDate;
      }
      this.CheckOutTime = closingDate;
      this.NextContact = LRSWorkflowRules.InterestedContact;
      this.NextStatus = TransactionStatus.EndPoint;
      this.NextTask = LRSWorkflowTask.Empty;
      this.Status = WorkflowTaskStatus.Closed;

      this.Save();
    }

    private void ExecuteSpecialCase(LRSWorkflowTask nextTrack) {
      if (!nextTrack.IsNew) {
        return;
      }
      if (nextTrack.CurrentStatus == TransactionStatus.ToDeliver ||
          nextTrack.CurrentStatus == TransactionStatus.ToReturn) {
        nextTrack.NextStatus = (nextTrack.CurrentStatus == TransactionStatus.ToDeliver) ?
                  TransactionStatus.Delivered : TransactionStatus.Returned;
        nextTrack.EndProcessTime = nextTrack.CheckInTime;
        nextTrack.Status = WorkflowTaskStatus.OnDelivery;
      }
    }

    internal LRSWorkflowTask CreateNext(string notes) {
      return this.CreateNext(notes, ExecutionServer.CurrentContact, ExecutionServer.NowWithoutSeconds);
    }

    internal LRSWorkflowTask CreateNext(string notes, Contact responsible, DateTime date) {
      // Create next track
      LRSWorkflowTask newTrack = new LRSWorkflowTask(this.Transaction);
      newTrack.PreviousTask = this;
      newTrack.NextTask = LRSWorkflowTask.Empty;
      newTrack.CurrentStatus = this.NextStatus;
      newTrack.AssignedBy = this.Responsible;
      newTrack.Responsible = responsible;
      newTrack.CheckInTime = date;
      if (notes.Length != 0) {
        newTrack.Notes = notes;
      }
      newTrack.Status = WorkflowTaskStatus.Pending;

      ExecuteSpecialCase(newTrack);

      newTrack.Save();

      // Close previous track
      if (this.EndProcessTime == ExecutionServer.DateMaxValue) {
        this.EndProcessTime = newTrack.CheckInTime;
      }
      this.CheckOutTime = newTrack.CheckInTime;
      if (this.NextContact.IsEmptyInstance) {
        this.NextContact = responsible;
      }
      this.Status = WorkflowTaskStatus.Closed;
      this.NextTask = newTrack;
      this.Save();

      return newTrack;
    }

    internal void SetNextStatus(TransactionStatus nextStatus, Contact nextContact,
                                string notes) {
      this.NextStatus = nextStatus;
      this.NextContact = nextContact;
      this.EndProcessTime = ExecutionServer.NowWithoutSeconds;
      this.Notes = notes;
      this.Status = WorkflowTaskStatus.OnDelivery;
      this.Save();
    }

    internal void SetPending() {
      this.EndProcessTime = ExecutionServer.DateMaxValue;
      this.CheckOutTime = ExecutionServer.DateMaxValue;
      this.NextStatus = TransactionStatus.EndPoint;
      this.NextTask = LRSWorkflowTask.Empty;
      this.NextContact = Person.Empty;
      this.Status = WorkflowTaskStatus.Pending;
      this.Save();
    }

    protected override void OnSave() {
      WorkflowData.WriteWorkflowTask(this);
    }

    #endregion Public methods

  } // class LRSWorkflowTask

} // namespace Empiria.Land.Transactions.Workflow
