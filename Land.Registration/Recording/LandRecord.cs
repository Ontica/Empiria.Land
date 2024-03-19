﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LandRecord                                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents a land instrument record with one or more recording acts.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Empiria.Contacts;

using Empiria.Land.Data;
using Empiria.Land.Providers;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.Instruments;

namespace Empiria.Land.Registration {

  /// <summary>Represents a land instrument record with one or more recording acts.</summary>
  public class LandRecord : BaseObject {

    #region Fields

    private Lazy<List<RecordingAct>> _recordingActs = new Lazy<List<RecordingAct>>();

    #endregion Fields

    #region Constructors and parsers

    private LandRecord() {
      // Required by Empiria Framework.
    }

    public LandRecord(Instrument instrument) {
      Assertion.Require(instrument, nameof(instrument));
      Assertion.Require(!instrument.IsEmptyInstance, "instrument can't be the empty instance.");

      Instrument = instrument;
    }

    public LandRecord(Instrument instrument, LRSTransaction transaction) : this(instrument) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(!transaction.IsEmptyInstance, "transaction can't be the empty instance.");

      this.Transaction = transaction;
      this.PresentationTime = this.Transaction.PresentationTime;
    }

    static internal LandRecord Parse(int id) {
      return BaseObject.ParseId<LandRecord>(id);
    }

    static public LandRecord ParseGuid(string guid) {
      var landRecord = BaseObject.TryParse<LandRecord>($"LandRecordGuid = '{guid}'");

      Assertion.Require(landRecord,
                        $"There is not registered a land record with guid '{guid}'.");

      return landRecord;
    }


    static public LandRecord TryParse(string landRecordUID) {
      return BaseObject.TryParse<LandRecord>($"LandRecordUID = '{landRecordUID}'");
    }


    static public LandRecord Empty {
      get { return BaseObject.ParseEmpty<LandRecord>(); }
    }


    #endregion Constructors and parsers

    #region Public properties

    [DataField("LandRecordGuid")]
    public string GUID {
      get;
      private set;
    }


    [DataField("InstrumentId")]
    public Instrument Instrument {
      get;
      private set;
    }


    [DataField("LandRecordUID", IsOptional = false)]
    private string _documentUID = String.Empty;

    public override string UID {
      get {
        return _documentUID;
      }
    }


    public bool IsHistoricRecord {
      get {
        if (this.IsEmptyInstance) {
          return false;
        }
        if (this.Transaction.IsEmptyInstance) {
          return true;
        }
        if (this.IsRegisteredInRecordingBook) {
          return true;
        }
        return false;
      }
    }


    [DataField("PresentationTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime PresentationTime {
      get;
      private set;
    }

    [DataField("AuthorizationTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime AuthorizationTime {
      get;
      private set;
    }

    [DataField("ImagingControlID")]
    public string ImagingControlID {
      get;
      internal set;
    } = string.Empty;


    [DataField("TransactionId")]
    public LRSTransaction Transaction {
      get;
      private set;
    }

    public bool HasTransaction {
      get {
        return !Transaction.IsEmptyInstance;
      }
    }

    public RecorderOffice RecorderOffice {
      get {
        if (HasTransaction) {
          return Transaction.RecorderOffice;
        } else {
          return RecorderOffice.Empty;
        }
      }
    }

    [DataField("AuthorizedById")]
    public Person AuthorizedBy {
      get;
      private set;
    }

    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(UID, Instrument.Keywords, ImagingControlID);
      }
    }


    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("LandRecordStatus", Default = RecordableObjectStatus.Incomplete)]
    public RecordableObjectStatus Status {
      get;
      private set;
    }

    public FixedList<RecordingAct> RecordingActs {
      get {
        return _recordingActs.Value.FindAll(x => x.Status != RecordableObjectStatus.Deleted)
                                   .ToFixedList();
      }
    }


    public bool HasRecordingActs {
      get {
        return (this.RecordingActs.Count > 0);
      }
    }


    public bool IsRegisteredInRecordingBook {
      get {
        return (BookEntry.GetBookEntriesForLandRecord(this).Count != 0);
      }
    }


    public bool IsClosed {
      get {
        return (this.Status == RecordableObjectStatus.Closed);
      }
    }

    public LandRecordSecurity Security {
      get {
        return new LandRecordSecurity(this);
      }
    }

    [DataObject]
    public LandRecordSecurityData SecurityData {
      get;
      private set;
    }

    [DataField("LandRecordDIF")]
    private string IntegrityField {
      get;
      set;
    }

    #endregion Public properties

    #region Public methods

    /// <summary>Adds a recording act to the document's recording acts collection.</summary>
    /// <param name="recordingAct">The item to be added at the end of the RecordingActs collection.</param>
    /// <returns> The recording act's index inside the RecordingActs collection.</returns>
    internal RecordingAct AppendRecordingAct(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, nameof(recordingAct));

      Assertion.Require(this.IsHistoricRecord || !this.IsClosed,
                       "Recording acts can't be added to closed documents.");

      _recordingActs.Value.Add(recordingAct);

      recordingAct.Index = _recordingActs.Value.Count - 1;

      return recordingAct;
    }


    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Resource resource,
                                           BookEntry bookEntry) {

      Assertion.Require(recordingActType, nameof(recordingActType));
      Assertion.Require(resource, nameof(resource));
      Assertion.Require(bookEntry, nameof(bookEntry));

      Assertion.Require(!this.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Require(!resource.IsEmptyInstance, "Resource can't be an empty instance.");
      Assertion.Require(!bookEntry.IsEmptyInstance, "BookEntry can't be an empty instance.");

      Assertion.Require(this.IsHistoricRecord || !this.IsClosed,
                       "Recording acts can't be added to closed documents");

      if (this.IsNew) {
        this.Save();
      }

      var recordingAct = RecordingAct.Create(recordingActType, this, resource, bookEntry);

      _recordingActs.Value.Add(recordingAct);

      recordingAct.Index = _recordingActs.Value.Count - 1;

      return recordingAct;
    }


    public void SetDates(DateTime presentationTime, DateTime authorizationTime) {
      Assertion.Require(this.IsNew || this.IsHistoricRecord,
        "Autorization and Presentation dates can be set only over new or historic documents.");
      Assertion.Require(!this.IsClosed,
        "Autorization and Presentation dates can be set only over opened documents.");

      if (!this.HasTransaction) {
        this.PresentationTime = presentationTime;
      }

      this.AuthorizationTime = authorizationTime;
    }


    public void Close() {
      this.Security.AssertCanBeClosed();

      if (!this.IsHistoricRecord) {
        this.AuthorizationTime = DateTime.Now;
        this.AuthorizedBy = ExecutionServer.CurrentContact as Person;
      }

      this.Status = RecordableObjectStatus.Closed;

      this.Save();
    }


    public void Open() {
      this.Security.AssertCanBeOpened();

      if (!this.IsHistoricRecord) {
        this.AuthorizationTime = ExecutionServer.DateMinValue;
        this.AuthorizedBy = Person.Empty;
      }

      this.Status = RecordableObjectStatus.Incomplete;

      this.Save();
    }


    public RecordingAct GetRecordingAct(string recordingActUID) {
      RecordingAct recordingAct = this.RecordingActs.Find(x => x.UID == recordingActUID);

      Assertion.Require(recordingAct, nameof(recordingAct));

      return recordingAct;
    }


    public List<Contact> GetRecordingOfficials() {
      var recordingOfficials = new List<Contact>();

      var recordingActs = this.RecordingActs;
      for (int i = 0; i < recordingActs.Count; i++) {
        if (!recordingOfficials.Contains(recordingActs[i].RegisteredBy)) {
          recordingOfficials.Add(recordingActs[i].RegisteredBy);
        }
      }
      return recordingOfficials;
    }

    public Resource GetUniqueInvolvedResource() {
      var recordingActs = this.RecordingActs;

      if (recordingActs.Count == 0) {
        return Resource.Empty;
      } else if (recordingActs.Count == 1) {
        return recordingActs[0].Resource;
      }

      var distinctResources = recordingActs.Select((x) => x.Resource).GroupBy((x) => x.Id).ToList();

      if (distinctResources.Count == 1) {
        return recordingActs[0].Resource;
      } else {
        return Resource.Empty;
      }
    }


    protected override void OnLoadObjectData(DataRow row) {
      //Assertion.Require(
      //  this.Security.Integrity.GetUpdatedHashCode() == (string) row["LandRecordDIF"],
      //        $"PROBLEMA GRAVE DE SEGURIDAD: La inscripción {this.UID} " +
      //        $"fue indebidamente modificada directamente en la base de datos.");

      RefreshRecordingActs();
    }

    public void RefreshRecordingActs() {
      _recordingActs = new Lazy<List<RecordingAct>>(() => LandRecordsData.GetLandRecordRecordingActs(this));
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.GUID = Guid.NewGuid().ToString().ToLower();

        IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

        _documentUID = provider.GenerateRecordID();

        this.PostingTime = DateTime.Now;
        this.PostedBy = ExecutionServer.CurrentContact;
      }

      LandRecordsData.WriteLandRecord(this);
    }

    public void RemoveRecordingAct(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, nameof(recordingAct));

      Assertion.Require(this.IsHistoricRecord || !this.IsClosed,
                       "Recording acts can't be removed from closed documents.");

      Assertion.Require(recordingAct.LandRecord.Equals(this),
                        "The recording act doesn't belong to this document.");

      recordingAct.Delete();
      _recordingActs.Value.Remove(recordingAct);
    }


    public BookEntry TryGetBookEntry() {
      if (!this.IsHistoricRecord) {
        return null;
      }
      BookEntry bookEntry = this.RecordingActs[0].BookEntry;

      Assertion.Require(!bookEntry.IsEmptyInstance,
                        "bookEntry can't be the empty instance.");

      return bookEntry;
    }

    public void EnsureIntegrity() {
      Assertion.Require(this.Security.Integrity.GetUpdatedHashCode() == IntegrityField,
                        $"PROBLEMA GRAVE DE SEGURIDAD: La inscripción {this.UID} " +
                        $"fue indebidamente modificada directamente en la base de datos.");
    }

    #endregion Public methods

  } // class LandRecord

} // namespace Empiria.Land.Registration
