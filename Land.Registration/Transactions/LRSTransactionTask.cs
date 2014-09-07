﻿/* Empiria Land 2014 *****************************************************************************************
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

    #region Constructors and parsers

    private LRSTransactionTask() {
      // Required by Empiria Framework.
    }
    
    internal LRSTransactionTask(LRSTransaction transaction) {
      this.Transaction = transaction;
    }

    static public LRSTransactionTask Parse(int id) {
      return BaseObject.ParseId<LRSTransactionTask>(id);
    }

    static internal LRSTransactionTask Parse(DataRow dataRow) {
      return BaseObject.ParseDataRow<LRSTransactionTask>(dataRow);
    }

    static public LRSTransactionTask Empty {
      get { return BaseObject.ParseEmpty<LRSTransactionTask>(); }
    }

    static internal LRSTransactionTask CreateFirst(LRSTransaction transaction) {
      LRSTransactionTask track = new LRSTransactionTask(transaction);

      track.AssignedBy = LRSTransaction.InterestedContact;
      track.CurrentStatus = transaction.Status;
      track.EndProcessTime = track.CheckInTime;
      track.Responsible = Contact.Parse(ExecutionServer.CurrentUserId);
      track.Save();

      return track;
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

    [DataField("Mode", Default = TrackMode.Manual)]
    public TrackMode Mode {
      get;
      set;
    }

    [DataField("AssignedById", Default = "Contacts.Person.Empty")]
    public Contact AssignedBy {
      get;
      set;
    }

    [DataField("ResponsibleId", Default = "Contacts.Person.Empty")]
    public Contact Responsible {
      get;
      set;
    }

    [DataField("NextContactId", Default = "Contacts.Person.Empty")]
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
      get { return LRSTransaction.StatusName(this.CurrentStatus); }
    }

    [DataField("NextTransactionStatus", Default = TransactionStatus.EndPoint)]
    public TransactionStatus NextStatus {
      get;
      set;
    }

    public string NextStatusName {
      get { return LRSTransaction.StatusName(this.NextStatus); }
    }

    [DataField("CheckInTime", Default = "DateTime.Now")]
    public DateTime CheckInTime {
      get;
      set;
    }

    [DataField("EndProcessTime")]
    public DateTime EndProcessTime {
      get;
      set;
    }

    [DataField("CheckOutTime")]
    public DateTime CheckOutTime {
      get;
      set;
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

    public TimeSpan OfficeWorkElapsedTime {
      get {
        if (LRSTransaction.StatusIsOfficeWork(this.CurrentStatus)) {
          return this.ElapsedTime;
        } else {
          return TimeSpan.Zero;
        }
      }
    }

    [DataField("TrackNotes")]
    public string Notes {
      get;
      set;
    }

    [DataField("TrackStatus", Default = TrackStatus.Pending)]
    public TrackStatus Status {
      get;
      internal set;
    }

    [DataField("PreviousTrackId")]
    private LazyObject<LRSTransactionTask> _previousTask = LazyObject<LRSTransactionTask>.Empty;
    public LRSTransactionTask PreviousTask {
      get { return _previousTask.Instance; }
      private set { _previousTask.Instance = value; }
    }

    [DataField("NextTrackId")]
    private LazyObject<LRSTransactionTask> _nextTask = LazyObject<LRSTransactionTask>.Empty;
    public LRSTransactionTask NextTask {
      get { return _nextTask.Instance; }
      private set { _nextTask.Instance = value; }
    }

    public string StatusName {
      get {
        switch (this.Status) {
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
        this.EndProcessTime = DateTime.Now;
      }
      this.CheckOutTime = DateTime.Now;
      this.NextContact = LRSTransaction.InterestedContact;
      this.NextStatus = TransactionStatus.EndPoint;
      this.NextTask = LRSTransactionTask.Empty;
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
      LRSTransactionTask newTrack = new LRSTransactionTask(this.Transaction);
      newTrack.PreviousTask = this;
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
        this.EndProcessTime = newTrack.CheckInTime;
      }
      this.CheckOutTime = newTrack.CheckInTime;
      if (this.NextContact.IsEmptyInstance) {
        this.NextContact = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.Status = TrackStatus.Closed;
      this.NextTask = newTrack;
      this.Save();

      return newTrack;
    }

    internal void SetNextStatus(TransactionStatus nextStatus, Contact nextContact, string notes) {
      this.NextStatus = nextStatus;
      this.NextContact = nextContact;
      this.EndProcessTime = DateTime.Now;
      this.Notes = notes;
      this.Status = TrackStatus.OnDelivery;
      this.Save();
    }

    internal void SetPending() {
      this.EndProcessTime = ExecutionServer.DateMaxValue;
      this.CheckOutTime = ExecutionServer.DateMaxValue;
      this.NextStatus = TransactionStatus.EndPoint;
      this.NextTask = LRSTransactionTask.Empty;
      this.NextContact = Person.Empty;
      this.Status = TrackStatus.Pending;
      this.Save();
    }

    protected override void OnSave() {
      TransactionData.WriteTransactionTask(this);
    }

    #endregion Public methods

  } // class LRSTransactionTask

} // namespace Empiria.Land.Registration.Transactions
