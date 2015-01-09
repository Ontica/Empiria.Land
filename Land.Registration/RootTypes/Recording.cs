/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a general recording in Land Registration System.                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Security;
using Empiria.Land.Registration.Data;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Represents a general recording in Land Registration System.</summary>
  public class Recording : BaseObject, IProtected {

    #region Fields
    
    private Lazy<FixedList<RecordingAct>> recordingActList = null;
    private Lazy<LRSPaymentList> payments = null;

    #endregion Fields

    #region Constructors and parsers

    private Recording() {
      // Required by Empiria Framework.
    }

    internal Recording(RecordingBook recordingBook, string number) {
      Assertion.AssertObject(recordingBook, "recordingBook");
      Assertion.AssertObject(number, "number");

      Assertion.Assert(!recordingBook.IsEmptyInstance, "recordingBook can't be the empty instance.");

      this.RecordingBook = recordingBook;
      this.Number = number;
    }

    protected override void OnInitialize() {
      recordingActList = new Lazy<FixedList<RecordingAct>>(() => RecordingActsData.GetRecordingActs(this));
      payments = new Lazy<LRSPaymentList>(() => LRSPaymentList.Parse(this));
      this.ExtendedData = new RecordingExtData();
    }

    static public Recording Parse(int id) {
      return BaseObject.ParseId<Recording>(id);
    }

    static public Recording Empty {
      get { return BaseObject.ParseEmpty<Recording>(); }
    }

    static public int SplitRecordingNumber(string fullRecordingNumber, out string bisSuffixTag) {
      if (EmpiriaString.IsInteger(fullRecordingNumber)) {
        bisSuffixTag = String.Empty;
        return int.Parse(fullRecordingNumber);
      }
      if (fullRecordingNumber.Contains("-")) {
        int index = fullRecordingNumber.IndexOf("-");
        bisSuffixTag = fullRecordingNumber.Substring(index);
        return int.Parse(fullRecordingNumber.Substring(0, index));
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingNumber,
                                            fullRecordingNumber);
      }
    }

    #endregion Constructors and parsers

    #region Public properties

   
    public RecordingDocument Document {
      get {
        if (recordingActList.Value.Count == 0) {
          return RecordingDocument.Empty;
        } else {
          return recordingActList.Value[0].Document;
        }
      }
    }

    [DataField("PhysicalBookId")]
    public RecordingBook RecordingBook {
      get;
      private set;
    }

    [DataField("RecordingNo")]
    public string Number {
      get;
      private set;
    }

    [DataField("RecordingNotes")]
    public string Notes {
      get;
      set;
    }
    
    public string AsText {
      get {
        return String.Format("Partida {0} en {1}", this.Number, this.RecordingBook.AsText);
      }
    }

    public RecordingExtData ExtendedData {
      get;
      private set;
    }

    public int StartImageIndex {
      get { return -1; }
    }

    public int EndImageIndex {
      get { return -1; }
    }

    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.Number, this.RecordingBook.AsText);
      }
    }

    [DataField("RecordingAuthorizationTime")]
    public DateTime AuthorizationTime {
      get;
      private set;
    }

    [DataField("ReviewedById")]
    public Contact ReviewedBy {
      get;
      private set;
    }

    [DataField("AuthorizedById")]
    public Contact AuthorizedBy {
      get;
      private set;
    }

    [DataField("RecordedById")]
    public Contact RecordedBy {
      get;
      private set;
    }

    [DataField("RecordingTime")]
    public DateTime RecordingTime {
      get;
      private set;
    }

    [DataField("RecordingStatus", Default = RecordableObjectStatus.Incomplete)]
    public RecordableObjectStatus Status {
      get;
      private set;
    }

    public string FullNumber {
      get {
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          return "Partida " + this.Number + " en " + this.RecordingBook.AsText;
        } else {
          return "Inscripción " + this.Number + " en " + this.RecordingBook.AsText;
        }
      }
    }

    public LRSPaymentList Payments {
      get {
        return payments.Value;
      }
    }

    public FixedList<RecordingAct> RecordingActs {
      get { return recordingActList.Value; }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.NoLegible:
            return "No legible";
          case RecordableObjectStatus.Incomplete:
            return "Incompleta";
          case RecordableObjectStatus.Pending:
            return "Pendiente";
          case RecordableObjectStatus.Registered:
            return "Registrada";
          case RecordableObjectStatus.Closed:
            return "Cerrada";
          case RecordableObjectStatus.Deleted:
            return "Eliminada";
          default:
            return "No determinado";
        }
      }
    }


    #endregion Public properties

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id,
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #region Public methods

    public void Delete() {
      Delete(true);
    }

    //public RecordingAct CreateAnnotation(LRSTransaction transaction,
    //                                     RecordingActType recordingActType, Property property) {
    //  Assertion.AssertObject(transaction, "transaction");
    //  Assertion.AssertObject(transaction.Document, "document");
    //  Assertion.Assert(!transaction.Document.IsEmptyInstance && !transaction.Document.IsNew,
    //                    "Transaction document can not be neither an empty or a new document instance");
    //  Assertion.Assert(!property.IsNew && !property.IsEmptyInstance,
    //                    "Property can not be empty or a new instance");
    //  Assertion.Assert(!this.IsEmptyInstance && !this.IsNew,
    //                    "Can not create an annotation using an empty or new recording");

    //  var recordingAct = RecordingAct.Create(recordingActType, Recording.Empty, property);

    //  this.Refresh();
    //  this.RecordingBook.Refresh();

    //  return recordingAct;
    //}

    //public RecordingAct CreateRecordingAct(RecordingActType recordingActType, Property property) {
    //  if (this.IsNew) {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.NotSavedRecording, "CreateRecordingAct");
    //  }
    //  if (this.Status == RecordableObjectStatus.Closed) {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
    //  }
    //  if (this.Status == RecordableObjectStatus.Obsolete) {
    //    this.Status = RecordableObjectStatus.Incomplete;
    //    this.Save();
    //  }

    //  var recordingAct = RecordingAct.Create(recordingActType, this, property);

    //  this.Refresh();
    //  this.RecordingBook.Refresh();

    //  return recordingAct;
    //}

    //public void DeleteRecordingAct(RecordingAct recordingAct) {
    //  if (this.Status == RecordableObjectStatus.Closed) {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterRecordingActOnClosedRecording, this.Id);
    //  }
    //  if (!this.RecordingActs.Contains(recordingAct)) {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording, 
    //                                        recordingAct.Id, this.Id);
    //  }
    //  if (recordingAct.Status == RecordableObjectStatus.Closed) {
    //    throw new LandRegistrationException(LandRegistrationException.Msg.CantAlterClosedRecordingAct, recordingAct.Id);
    //  }
    //  recordingAct.Delete();
    //  SortRecordingActs();
    //  this.DeleteMeIfNecessary();
    //  this.Refresh();
    //  this.RecordingBook.Refresh();
    //}

    public IList<Property> GetProperties() {
      return PropertyData.GetRecordingProperties(this);
    }

    public void SortRecordingActs() {
      this.recordingActList = null;
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        RecordingActs[i].Index = i + 1;
        RecordingActs[i].Save();
      }
      this.recordingActList = null;
    }

    public RecordingAct GetRecordingAct(int recordingActId) {
      RecordingAct recordingAct = RecordingAct.Parse(recordingActId);
      if (this.RecordingActs.Contains(recordingAct)) {
        return recordingAct;
      } else {
        throw new LandRegistrationException(LandRegistrationException.Msg.RecordingActNotBelongsToRecording,
                                            recordingActId, this.Id);
      }
    }

    public FixedList<RecordingAct> GetAnnotationActs() {
      return new FixedList<RecordingAct>(this.RecordingActs.FindAll((x) => x.IsAnnotation));
    }

    public FixedList<RecordingAct> GetNoAnnotationActs() {
      return new FixedList<RecordingAct>(this.RecordingActs.FindAll((x) => !x.IsAnnotation));
    }

    public FixedList<TractIndexItem> GetPropertiesAnnotationsList() {
      return PropertyData.GetRecordingPropertiesAnnotationsList(this);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.RecordingTime = DateTime.Now;
        this.RecordedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecording(this);
    }

    public void Refresh() {
      this.recordingActList = null;
    }

    #endregion Public methods

    #region Private methods

    private void Delete(bool publicCall) {
      Assertion.Assert(this.RecordingActs.Count == 0,
                       "This recording can't be deleted because it has recording acts.");
      Assertion.Assert(!publicCall || this.RecordingBook.IsAvailableForManualEditing,
                       "This recording can't be deleted because its recording book is not available for manual editing.");
      this.Status = RecordableObjectStatus.Deleted;
      //this.canceledBy = Contact.Parse(ExecutionServer.CurrentUserId);
      //this.canceledTime = DateTime.Now;
      this.Save();
    }

    private void DeleteMeIfNecessary() {
      if (this.RecordingBook.IsAvailableForManualEditing) {
        return;
      }
      if (this.RecordingActs.Count != 0) {
        return;
      }
      this.Delete(false);
    }

    #endregion Private methods

  } // class Recording

} // namespace Empiria.Land.Registration
