/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Partitioned Type / Information Holder   *
*  Type     : Instrument                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a legal instrument like a deed, contract, mortgage, court order or prevention note. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Ontology;
using Empiria.StateEnums;

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Instruments.Data;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Instruments {

  /// <summary>Represents a legal instrument like deeds, contracts, mortgages, court orders, prevention notes,
  /// and other kind of legally issued or attested instruments.</summary>
  [PartitionedType(typeof(InstrumentType))]
  internal partial class Instrument : BaseObject {

    #region Constructors and parsers

    protected Instrument(InstrumentType instrumentType) : base(instrumentType) {
      // Required by Empiria Framework for all partitioned types.
    }


    internal Instrument(InstrumentType instrumentType,
                        InstrumentFields data) : base(instrumentType) {
      this.LoadData(data);
    }


    static public Instrument Parse(int id) => BaseObject.ParseId<Instrument>(id);

    static public Instrument Parse(string uid) => BaseObject.ParseKey<Instrument>(uid);

    static public Instrument Empty => BaseObject.ParseEmpty<Instrument>();


    #endregion Constructors and parsers

    #region Properties

    public InstrumentType InstrumentType {
      get {
        return (InstrumentType) base.GetEmpiriaType();
      }
    }


    [DataField("InstrumentKind")]
    public string Kind {
      get; private set;
    }


    [DataField("InstrumentControlID")]
    public string ControlID {
      get; private set;
    }


    [DataField("IssuerId")]
    public Issuer Issuer {
      get; private set;
    }


    [DataField("IssueDate")]
    public DateTime IssueDate {
      get; private set;
    } = ExecutionServer.DateMinValue;


    [DataField("InstrumentSummary")]
    public string Summary {
      get; private set;
    }


    [DataField("InstrumentAsText")]
    public string AsText {
      get; private set;
    }


    [DataField("InstrumentExtData")]
    public JsonObject ExtData {
      get; private set;
    }


    public string InstrumentNo {
      get {
        return this.ExtData.Get("InstrumentNo", String.Empty);
      }
      private set {
        this.ExtData.SetIfValue("InstrumentNo", value);
      }
    }


    public string BinderNo {
      get {
        return this.ExtData.Get("BinderNo", String.Empty);
      }
      private set {
        this.ExtData.SetIfValue("BinderNo", value);
      }
    }


    public string Folio {
      get {
        return this.ExtData.Get("Folio", String.Empty);
      }
      private set {
        this.ExtData.SetIfValue("Folio", value);
      }
    }


    public string EndFolio {
      get {
        return this.ExtData.Get("EndFolio", String.Empty);
      }
      private set {
        this.ExtData.SetIfValue("EndFolio", value);
      }
    }


    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.InstrumentType.DisplayName, this.Kind,
                                           this.InstrumentNo, this.BinderNo, this.Folio,
                                           this.Summary) + " " + this.Issuer.Keywords;
      }
    }


    [DataField("SheetsCount")]
    public int SheetsCount {
      get; private set;
    }


    [DataField("InstrumentStatus", Default = OpenCloseStatus.Opened)]
    internal OpenCloseStatus Status {
      get; private set;
    }


    [DataField("PostedById")]
    internal int PostedById {
      get; private set;
    }


    [DataField("PostingTime")]
    internal DateTime PostingTime {
      get; private set;
    }


    internal bool HasDocument {
      get {
        var document = TryGetRecordingDocument();
        if (document != null && !document.IsEmptyInstance) {
          return true;
        }
        return false;
      }
    }


    public bool HasPhysicalRecordings {
      get {
        return (this.PhysicalRecordings.Count != 0);
      }
    }


    public FixedList<PhysicalRecording> PhysicalRecordings {
      get {
        if (!this.HasDocument) {
          return new FixedList<PhysicalRecording>();
        }

        var document = this.TryGetRecordingDocument();

        return PhysicalRecording.GetDocumentRecordings(document.Id);
      }
    }


    #endregion Properties

    #region Methods

    private void ChangeInstrumentTypeIfRequired(InstrumentFields data) {
      if (!data.Type.HasValue) {
        return;
      }

      var instrumentType = InstrumentType.Parse(data.Type.Value);

      if (!instrumentType.Equals(this.InstrumentType)) {
        base.ReclassifyAs(instrumentType);
      }
    }


    private void CreateRecordingDocument() {
      Assertion.Assert(_recordingDocument == null, "RecordingDocument already exists.");

      _recordingDocument = RecordingDocument.CreateFromInstrument(this.Id, InstrumentType.Id, this.Kind);

      SaveRecordingDocument();
    }


    public void EnsureHasRecordingDocument() {
      if (!this.HasDocument) {
        CreateRecordingDocument();
      }
    }


    private void FillRecordingDocument() {
      _recordingDocument.Notes = this.Summary;
      _recordingDocument.ExpedientNo = this.BinderNo;
      _recordingDocument.IssueDate = this.IssueDate;
      _recordingDocument.IssuedBy = this.Issuer.RelatedContact;
      _recordingDocument.IssueOffice = this.Issuer.RelatedEntity;
      _recordingDocument.IssuePlace = this.Issuer.RelatedPlace;
      _recordingDocument.SheetsCount = this.SheetsCount;
    }


    private LRSTransaction _transaction;
    internal LRSTransaction GetTransaction() {
      if (_transaction == null) {
        _transaction = LRSTransaction.TryParseForInstrument(this.Id);
      }
      return _transaction;
    }


    private void LoadData(InstrumentFields data) {
      Kind = data.Kind ?? Kind;
      Summary = data.Summary ?? Summary;
      IssueDate = data.IssueDate ?? IssueDate;
      Issuer = data.Issuer ?? Issuer;
      InstrumentNo = data.InstrumentNo ?? InstrumentNo;
      BinderNo = data.BinderNo ?? BinderNo;
      Folio = data.Folio ?? Folio;
      EndFolio = data.EndFolio ?? EndFolio;
      SheetsCount = data.SheetsCount ?? SheetsCount;
    }


    protected override void OnSave() {
      bool needsDocumentCreation = false;

      if (!this.HasDocument) {
        needsDocumentCreation = true;
      }

      InstrumentsData.WriteInstrument(this);

      if (needsDocumentCreation) {
        this.EnsureHasRecordingDocument();
      } else {
        SaveRecordingDocument();
      }
    }


    private void SaveRecordingDocument() {
      FillRecordingDocument();

      _recordingDocument.Save();
    }


    private RecordingDocument _recordingDocument;
    internal RecordingDocument TryGetRecordingDocument() {
      if (_recordingDocument == null) {
        _recordingDocument = RecordingDocument.TryParseForInstrument(this.Id);
      }
      return _recordingDocument;
    }


    public void Update(InstrumentFields data) {
      Assertion.AssertObject(data, "data");

      this.ChangeInstrumentTypeIfRequired(data);

      this.LoadData(data);
    }

    #endregion Methods

  }  // class Instrument

}  // namespace Empiria.Land.Instruments
