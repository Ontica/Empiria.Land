/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PhysicalRecording                              Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a general recording in Land Registration System.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a general recording in Land Registration System.</summary>
  public class PhysicalRecording : BaseObject, IProtected {

    #region Fields

    private Lazy<FixedList<RecordingAct>> recordingActList = null;

    #endregion Fields

    #region Constructors and parsers

    private PhysicalRecording() {
      // Required by Empiria Framework.
    }

    internal PhysicalRecording(RecordingBook recordingBook,
                               RecordingDocument mainDocument, string recordingNumber) {
      Assertion.Require(recordingBook, "recordingBook");
      Assertion.Require(mainDocument, "mainDocument");
      Assertion.Require(recordingNumber, "recordingNumber");

      Assertion.Require(!recordingBook.IsEmptyInstance, "recordingBook can't be the empty instance.");
      Assertion.Require(!mainDocument.IsEmptyInstance, "mainDocument can't be the empty instance.");

      this.RecordingBook = recordingBook;
      this.MainDocument = mainDocument;
      this.Number = recordingNumber;
    }

    internal PhysicalRecording(RecordingDTO dto) : this(dto?.RecordingBook, dto?.MainDocument, dto?.Number) {
      LoadData(dto);
    }

    protected override void OnInitialize() {
      recordingActList = GetNewRecordingActListLazyInstance();
      this.ExtendedData = new RecordingExtData();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ExtendedData = RecordingExtData.Parse((string) row["RecordingExtData"]);
    }


    static public PhysicalRecording Parse(int id) {
      return BaseObject.ParseId<PhysicalRecording>(id);
    }


    static public PhysicalRecording Parse(string uid) {
      return BaseObject.ParseKey<PhysicalRecording>(uid);
    }


    static public FixedList<PhysicalRecording> GetDocumentRecordings(int documentId) {
      return RecordingBooksData.GetPhysicalRecordingsForDocument(documentId);
    }


    static private readonly PhysicalRecording _empty = BaseObject.ParseEmpty<PhysicalRecording>();
    static public PhysicalRecording Empty {
      get {
        return _empty.Clone<PhysicalRecording>();
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
        return String.Format("Inscripción {0} en {1}", this.Number, this.RecordingBook.AsText);
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

    public FixedList<RecordingAct> RecordingActs {
      get { return recordingActList.Value; }
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case RecordableObjectStatus.Obsolete:
            return "No vigente";
          case RecordableObjectStatus.NotLegible:
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

    public RecordingAct AppendRecordingAct(RecordingActType recordingActType, Resource resource,
                                           RecordingAct amendmentOf = null) {
      Assertion.Require(recordingActType, "recordingActType");
      Assertion.Require(resource, "resource");

      Assertion.Require(!resource.IsEmptyInstance, "Resource can't be an empty instance.");

      amendmentOf = amendmentOf ?? RecordingAct.Empty;

      return this.MainDocument.AppendRecordingAct(recordingActType, resource, amendmentOf, this);
    }

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
      this.recordingActList = GetNewRecordingActListLazyInstance();
    }

    public void SortRecordingActs() {
      this.recordingActList = GetNewRecordingActListLazyInstance();
      for (int i = 0; i < this.RecordingActs.Count; i++) {
        RecordingActs[i].Index = i + 1;
        RecordingActs[i].Save();
      }
      this.recordingActList = GetNewRecordingActListLazyInstance();
    }

    public void Update(RecordingDTO data) {
      this.LoadData(data);
      data.MainDocument.Save();
      this.Save();
    }

    #endregion Public methods

    #region Private methods

    private void Delete(bool publicCall) {
      Assertion.Require(this.RecordingActs.Count == 0,
                        "This recording can't be deleted because it has recording acts.");
      Assertion.Require(!publicCall || this.RecordingBook.IsAvailableForManualEditing ||
                        this.RecordingBook.Status == RecordingBookStatus.Opened,
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



    private Lazy<FixedList<RecordingAct>> GetNewRecordingActListLazyInstance() {
      return new Lazy<FixedList<RecordingAct>>(() => RecordingActsData.GetPhysicalRecordingRecordedActs(this));
    }


    private void LoadData(RecordingDTO dto) {
      if (this.IsNew) {
        this.MainDocument = dto.MainDocument;
      }

      this.MainDocument.PresentationTime = dto.PresentationTime;
      this.MainDocument.SetAuthorizationTime(dto.AuthorizationDate);

      this.StartImageIndex = dto.StartImageIndex;
      this.EndImageIndex = dto.EndImageIndex;

      this.AuthorizedBy = dto.AuthorizedBy;
      this.Notes = dto.Notes;
      this.Status = dto.Status;
    }

    #endregion Private methods

  } // class PhysicalRecording

} // namespace Empiria.Land.Registration
