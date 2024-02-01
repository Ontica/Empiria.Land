/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information holder                    *
*  Type     : BookEntry                                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : DTO that holds data about a physical book recording entry.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>DTO that holds data about a physical book recording entry.</summary>
  public class BookEntry : BaseObject, IProtected {

    #region Fields

    private Lazy<FixedList<RecordingAct>> recordingActList = new Lazy<FixedList<RecordingAct>>();

    #endregion Fields

    #region Constructors and parsers

    private BookEntry() {
      // Required by Empiria Framework.
      recordingActList = GetNewRecordingActListLazyInstance();
    }

    internal BookEntry(RecordingBook recordingBook,
                       RecordingDocument landRecord, string recordingNumber) {
      Assertion.Require(recordingBook, nameof(recordingBook));
      Assertion.Require(landRecord, nameof(landRecord));
      Assertion.Require(recordingNumber, nameof(recordingNumber));

      Assertion.Require(!recordingBook.IsEmptyInstance, "recordingBook can't be the empty instance.");
      Assertion.Require(!landRecord.IsEmptyInstance, "landRecord can't be the empty instance.");

      this.RecordingBook = recordingBook;
      this.LandRecord = landRecord;
      this.Number = recordingNumber;

      recordingActList = GetNewRecordingActListLazyInstance();
    }

    internal BookEntry(BookEntryDto dto) : this(dto?.RecordingBook, dto?.LandRecord, dto?.Number) {
      LoadData(dto);
      recordingActList = GetNewRecordingActListLazyInstance();
    }

    protected override void OnLoadObjectData(DataRow row) {
      recordingActList = GetNewRecordingActListLazyInstance();
      this.ExtendedData = BookEntryExtData.Parse((string) row["RecordingExtData"]);
    }


    static public BookEntry Parse(int id) {
      return BaseObject.ParseId<BookEntry>(id);
    }


    static public BookEntry Parse(string uid) {
      return BaseObject.ParseKey<BookEntry>(uid);
    }


    static public FixedList<BookEntry> GetBookEntriesForLandRecord(RecordingDocument landRecord) {
      return RecordingBooksData.GetBookEntriesForLandRecord(landRecord);
    }


    static private readonly BookEntry _empty = BaseObject.ParseEmpty<BookEntry>();
    static public BookEntry Empty {
      get {
        return _empty.Clone<BookEntry>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("MainDocumentId")]
    public RecordingDocument LandRecord {
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
        if (!this.IsEmptyInstance) {
          return String.Format("Inscripción {0} del {1}", this.Number, this.RecordingBook.AsText);
        } else {
          return "Empty";
        }
      }
    }

    internal BookEntryExtData ExtendedData {
      get;
      private set;
    } = new BookEntryExtData();


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

    public FixedList<RecordingAct> RecordingActs {
      get { return recordingActList.Value; }
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

    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Resource resource,
                                           RecordingAct amendmentOf = null) {
      Assertion.Require(recordingActType, "recordingActType");
      Assertion.Require(resource, "resource");

      Assertion.Require(!resource.IsEmptyInstance, "Resource can't be an empty instance.");

      amendmentOf = amendmentOf ?? RecordingAct.Empty;

      return this.LandRecord.AppendRecordingAct(recordingActType, resource, amendmentOf, this);
    }

    public void AssertCanBeClosed() {

    }

    public void Delete() {
      Assertion.Require(this.RecordingActs.Count == 0,
                            "This recording can't be deleted because it has recording acts.");
      Assertion.Require(this.RecordingBook.IsAvailableForManualEditing ||
                        this.RecordingBook.Status == RecordingBookStatus.Opened,
                       "This recording can't be deleted because its recording book is not available for manual editing.");

      this.Status = RecordableObjectStatus.Deleted;

      this.Save();
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

    protected override void OnSave() {
      if (this.IsNew) {
        this.RecordingTime = DateTime.Now;
        this.RecordedBy = ExecutionServer.CurrentContact;

        if (this.LandRecord.IsNew) {
          this.LandRecord.Save();
        }
      }
      RecordingBooksData.WriteBookEntry(this);
    }

    public void Refresh() {
      this.recordingActList = GetNewRecordingActListLazyInstance();
    }


    public void Update(BookEntryDto data) {
      this.LoadData(data);
      data.LandRecord.Save();
      this.Save();
    }

    #endregion Public methods

    #region Private methods

    private Lazy<FixedList<RecordingAct>> GetNewRecordingActListLazyInstance() {
      return new Lazy<FixedList<RecordingAct>>(() => RecordingActsData.GetBookEntryRecordedActs(this));
    }


    private void LoadData(BookEntryDto dto) {
      if (this.IsNew) {
        this.LandRecord = dto.LandRecord;
      }

      this.LandRecord.PresentationTime = dto.PresentationTime;
      this.LandRecord.SetAuthorizationTime(dto.AuthorizationDate);

      this.StartImageIndex = dto.StartImageIndex;
      this.EndImageIndex = dto.EndImageIndex;

      this.AuthorizedBy = dto.AuthorizedBy;
      this.Notes = dto.Notes;
      this.Status = dto.Status;
    }

    #endregion Private methods

  } // class BookEntry

} // namespace Empiria.Land.Registration
