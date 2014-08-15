/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionTask                             Pattern  : Association Class                   *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Workflow item or task inside a LRSTransaction processing time window.                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  public enum TrackStatus {
    Pending = 'P',
    OnDelivery = 'D',
    Closed = 'C',
    Deleted = 'X',
    Historic = 'H',
  }

  public enum TrackMode {
    Automatic = 'A',
    Manual = 'M',
  }

  /// <summary>Workflow item or task inside a LRSTransaction processing time window.</summary>
  public class LRSTransactionTask : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransactionTask";

    private LRSTransaction transaction = null;
    private int eventId = -1;
    private TrackMode mode = TrackMode.Manual;
    private Contact assignedBy = Person.Empty;
    private Contact responsible = Person.Empty;
    private Contact nextContact = Person.Empty;
    private TransactionStatus currentStatus = TransactionStatus.Undefined;
    private TransactionStatus nextStatus = TransactionStatus.EndPoint;
    private DateTime checkInTime = DateTime.Now;
    private DateTime endProcessTime = ExecutionServer.DateMaxValue;
    private DateTime checkOutTime = ExecutionServer.DateMaxValue;
    private string notes = String.Empty;
    private TrackStatus status = TrackStatus.Pending;
    private string integrityHashCode = String.Empty;

    private int previousTaskId = -1;
    private int nextTaskId = -1;
    private LRSTransactionTask previousTask = null;
    private LRSTransactionTask nextTask = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSTransactionTask(LRSTransaction transaction)
      : base(thisTypeName) {
      this.transaction = transaction;
    }

    protected LRSTransactionTask()
      : base(thisTypeName) {
      // Instance creation of this type may be invoked with ....
    }

    protected LRSTransactionTask(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSTransactionTask Parse(int id) {
      return BaseObject.Parse<LRSTransactionTask>(thisTypeName, id);
    }

    static internal LRSTransactionTask Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSTransactionTask>(thisTypeName, dataRow);
    }

    static public LRSTransactionTask Empty {
      get { return BaseObject.ParseEmpty<LRSTransactionTask>(thisTypeName); }
    }

    static internal LRSTransactionTask CreateFirst(LRSTransaction transaction) {
      LRSTransactionTask track = new LRSTransactionTask(transaction);

      track.AssignedBy = LRSTransaction.InterestedContact;
      track.CurrentStatus = transaction.Status;
      track.CheckInTime = DateTime.Now;
      track.EndProcessTime = track.CheckInTime;
      track.Responsible = Contact.Parse(ExecutionServer.CurrentUserId);
      track.previousTask = LRSTransactionTask.Empty;
      track.nextTask = LRSTransactionTask.Empty;
      track.Save();

      return track;
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get { return transaction; }
    }

    public int EventId {
      get { return eventId; }
      set { eventId = value; }
    }

    public TrackMode Mode {
      get { return mode; }
      set { mode = value; }
    }

    public Contact AssignedBy {
      get { return assignedBy; }
      set { assignedBy = value; }
    }

    public Contact Responsible {
      get { return responsible; }
      set { responsible = value; }
    }

    public Contact NextContact {
      get { return nextContact; }
      set { nextContact = value; }
    }

    public TransactionStatus CurrentStatus {
      get { return currentStatus; }
      set { currentStatus = value; }
    }

    public string CurrentStatusName {
      get { return LRSTransaction.StatusName(currentStatus); }
    }

    public TransactionStatus NextStatus {
      get { return nextStatus; }
      set { nextStatus = value; }
    }

    public string NextStatusName {
      get { return LRSTransaction.StatusName(nextStatus); }
    }

    public DateTime CheckInTime {
      get { return checkInTime; }
      set { checkInTime = value; }
    }

    public DateTime EndProcessTime {
      get { return endProcessTime; }
      set { endProcessTime = value; }
    }

    public DateTime CheckOutTime {
      get { return checkOutTime; }
      set { checkOutTime = value; }
    }

    public TimeSpan ElapsedTime {
      get {
        if (checkOutTime == ExecutionServer.DateMaxValue) {
          return DateTime.Now.Subtract(checkInTime);
        } else {
          return checkOutTime.Subtract(checkInTime);
        }
      }
    }

    public TimeSpan OfficeWorkElapsedTime {
      get {
        if (LRSTransaction.StatusIsOfficeWork(currentStatus)) {
          return this.ElapsedTime;
        } else {
          return TimeSpan.Zero;
        }
      }
    }

    public string Notes {
      get { return notes; }
      set { notes = EmpiriaString.TrimAll(value); }
    }

    public TrackStatus Status {
      get { return status; }
      set { status = value; }
    }

    public LRSTransactionTask PreviousTask {
      get {
        if (previousTask == null) {
          previousTask = LRSTransactionTask.Parse(previousTaskId);
        }
        return previousTask;
      }
    }

    public LRSTransactionTask NextTask {
      get {
        if (nextTask == null) {
          nextTask = LRSTransactionTask.Parse(nextTaskId);
        }
        return nextTask;
      }
    }

    public string StatusName {
      get {
        switch (status) {
          case TrackStatus.Pending:
            return "Pendiente";
          case TrackStatus.OnDelivery:
            return "Por entregar";
          case TrackStatus.Closed:
            return "Terminado";
          case TrackStatus.Deleted:
            return "Eliminado";
          default:
            return "No determinado";
        }
      } // get
    }

    #endregion Public properties

    #region Public method

    internal void Close() {
      if (this.EndProcessTime == ExecutionServer.DateMaxValue) {
        this.endProcessTime = DateTime.Now;
      }
      this.CheckOutTime = DateTime.Now;
      this.nextContact = LRSTransaction.InterestedContact;
      this.nextStatus = TransactionStatus.EndPoint;
      this.nextTaskId = -1;
      this.nextTask = LRSTransactionTask.Empty;
      this.Status = TrackStatus.Closed;

      this.Save();
    }

    private void ExecuteSpecialCase(LRSTransactionTask nextTrack) {
      if (nextTrack.IsNew) {
        if (nextTrack.CurrentStatus == TransactionStatus.ToDeliver || 
            nextTrack.CurrentStatus == TransactionStatus.ToReturn) {
          nextTrack.NextStatus = (nextTrack.CurrentStatus == TransactionStatus.ToDeliver) ? 
                    TransactionStatus.Delivered : TransactionStatus.Returned;
          nextTrack.EndProcessTime = nextTrack.CheckInTime;
          nextTrack.Status = TrackStatus.OnDelivery;
        }
      }
    }

    internal LRSTransactionTask CreateNext(string notes) {
      // Create next track
      LRSTransactionTask newTrack = new LRSTransactionTask(this.transaction);
      newTrack.previousTask = this;
      newTrack.CurrentStatus = this.NextStatus;
      newTrack.AssignedBy = this.Responsible;
      newTrack.Responsible = Contact.Parse(ExecutionServer.CurrentUserId);
      newTrack.CheckInTime = DateTime.Now;
      if (notes.Length != 0) {
        newTrack.Notes = notes;
      }
      newTrack.Status = TrackStatus.Pending;

      ExecuteSpecialCase(newTrack);

      newTrack.Save();

      // Close previous track
      if (this.EndProcessTime == ExecutionServer.DateMaxValue) {
        this.EndProcessTime = newTrack.checkInTime;
      }
      this.CheckOutTime = newTrack.checkInTime;
      if (this.nextContact.IsEmptyInstance) {
        this.nextContact = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.Status = TrackStatus.Closed;
      this.nextTask = newTrack;
      this.Save();

      return newTrack;
    }

    internal void SetNextStatus(TransactionStatus nextStatus, Contact nextContact, string notes) {
      this.nextStatus = nextStatus;
      this.nextContact = nextContact;
      this.endProcessTime = DateTime.Now;
      this.notes = notes;
      this.status = TrackStatus.OnDelivery;
      this.Save();
    }

    internal void SetPending() {
      this.EndProcessTime = ExecutionServer.DateMaxValue;
      this.CheckOutTime = ExecutionServer.DateMaxValue;
      this.NextStatus = TransactionStatus.EndPoint;
      this.nextTaskId = -1;
      this.nextTask = LRSTransactionTask.Empty;
      this.NextContact = Person.Empty;
      this.Status = TrackStatus.Pending;
      this.Save();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.eventId = (int) row["EventId"];

      this.mode = (TrackMode) Convert.ToChar(row["Mode"]);
      this.assignedBy = Contact.Parse((int) row["AssignedById"]);
      this.responsible = Contact.Parse((int) row["ResponsibleId"]);
      this.nextContact = Contact.Parse((int) row["NextContactId"]);
      this.currentStatus = (TransactionStatus) Convert.ToChar(row["CurrentTransactionStatus"]);
      this.nextStatus = (TransactionStatus) Convert.ToChar(row["NextTransactionStatus"]);
      this.checkInTime = (DateTime) row["CheckInTime"];
      this.endProcessTime = (DateTime) row["EndProcessTime"];
      this.checkOutTime = (DateTime) row["CheckOutTime"];
      this.notes = (string) row["TrackNotes"];
      this.previousTaskId = (int) row["PreviousTrackId"];
      this.nextTaskId = (int) row["NextTrackId"];
      this.status = (TrackStatus) Convert.ToChar(row["TrackStatus"]);
      this.integrityHashCode = (string) row["TrackDIF"];
    }

    protected override void OnSave() {
      if (base.IsNew) {
        //this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        //this.postingTime = DateTime.Now;
      }
      //this.currentStatus = this.Transaction.Status;
      TransactionData.WriteTransactionTask(this);
    }

    #endregion Public methods

  } // class LRSTransactionTask

} // namespace Empiria.Land.Registration.Transactions
