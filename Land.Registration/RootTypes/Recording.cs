/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Recording                                      Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a general recording in Land Registration System.                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
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

    internal Recording(RecordingBook recordingBook,
                       RecordingDocument mainDocument, string recordingNumber) {
      Assertion.AssertObject(recordingBook, "recordingBook");
      Assertion.AssertObject(mainDocument, "mainDocument");
      Assertion.AssertObject(recordingNumber, "recordingNumber");

      Assertion.Assert(!recordingBook.IsEmptyInstance, "recordingBook can't be the empty instance.");
      Assertion.Assert(!mainDocument.IsEmptyInstance, "mainDocument can't be the empty instance.");

      this.RecordingBook = recordingBook;
      this.MainDocument = mainDocument;
      this.Number = recordingNumber;
    }

    internal Recording(RecordingDTO dto) : this(dto?.RecordingBook, dto?.MainDocument, dto?.Number) {
      this.MainDocument = dto.MainDocument;

      if (this.MainDocument.IsNew) {
        this.MainDocument.PresentationTime = dto.PresentationTime;
        // this.MainDocument.AuthorizationTime = dto.AuthorizationTime;
      }
      this.StartImageIndex = dto.StartImageIndex;
      this.EndImageIndex = dto.EndImageIndex;

      // this.AuthorizationTime = dto.AuthorizationDate;
      this.AuthorizedBy = dto.AuthorizedBy;
      this.Notes = dto.Notes;
      this.Status = dto.Status;
    }

    protected override void OnInitialize() {
      recordingActList = new Lazy<FixedList<RecordingAct>>(() => RecordingActsData.GetPhysicalRecordingRecordedActs(this));
      payments = new Lazy<LRSPaymentList>(() => LRSPaymentList.Parse(this));
      this.ExtendedData = new RecordingExtData();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtendedData = RecordingExtData.Parse((string) row["RecordingExtData"]);
    }


    static public Recording Parse(int id) {
      return BaseObject.ParseId<Recording>(id);
    }

    static private readonly Recording _empty = BaseObject.ParseEmpty<Recording>();
    static public Recording Empty {
      get {
        return _empty.Clone<Recording>();
      }
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

    [DataField("MainDocumentId")]
    public RecordingDocument MainDocument {
      get;
      private set;
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

    public string Notes {
      get {
        return ExtendedData.Notes;
      }
      private set {
        ExtendedData.Notes = value;
      }
    }

    public string AsText {
      get {
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          return String.Format("Partida {0} en {1}", this.Number, this.RecordingBook.AsText);
        } else {
          return String.Format("Inscripción {0} en {1}", this.Number, this.RecordingBook.AsText);
        }
      }
    }

    internal RecordingExtData ExtendedData {
      get;
      private set;
    }

    public int StartImageIndex {
      get {
        return ExtendedData.StartImageIndex;
      }
      private set {
        ExtendedData.StartImageIndex = value;
      }
    }

    public int EndImageIndex {
      get {
        return ExtendedData.EndImageIndex;
      }
      private set {
        ExtendedData.EndImageIndex = value;
      }
    }

    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.Number, this.RecordingBook.AsText);
      }
    }

    public Contact ReviewedBy {
      get {
        return ExtendedData.ReviewedBy;
      }
      private set {
        ExtendedData.ReviewedBy = value;
      }
    }

    public Contact AuthorizedBy {
      get {
        return ExtendedData.AuthorizedBy;
      }
      private set {
        ExtendedData.AuthorizedBy = value;
      }
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

    public void AssertCanBeClosed() {

    }

    public void Delete() {
      Delete(true);
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

    public FixedList<Resource> GetResources() {
      return ResourceData.GetPhysicalRecordingResources(this);
    }

    public FixedList<RecordingAct> GetAnnotationActs() {
      return this.RecordingActs.FindAll((x) => x.IsAnnotation);
    }

    public FixedList<RecordingAct> GetNoAnnotationActs() {
      return this.RecordingActs.FindAll((x) => !x.IsAnnotation);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.RecordingTime = DateTime.Now;
        this.RecordedBy = Contact.Parse(ExecutionServer.CurrentUserId);

        if (this.MainDocument.IsNew) {
          this.MainDocument.Save();
        }
      }
      RecordingBooksData.WriteRecording(this);
    }

    public void Refresh() {
      this.recordingActList = null;
    }

    public void SortRecordingActs() {
      this.recordingActList = null;
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        RecordingActs[i].Index = i + 1;
        RecordingActs[i].Save();
      }
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
