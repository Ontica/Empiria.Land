﻿/* Empiria Land **********************************************************************************************
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
using Empiria.Contacts;

namespace Empiria.Land.Instruments {

  /// <summary>Represents a legal instrument like deeds, contracts, mortgages, court orders, prevention notes,
  /// and other kind of legally issued or attested instruments.</summary>
  [PartitionedType(typeof(InstrumentType))]
  public partial class Instrument : BaseObject {

    #region Constructors and parsers

    protected Instrument(InstrumentType instrumentType) : base(instrumentType) {
      // Required by Empiria Framework for all partitioned types.
    }


    public Instrument(InstrumentType instrumentType,
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


    public string AsText {
      get {
        return GetInstrumentAsText();
      }
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
    internal Contact PostedBy {
      get; private set;
    }


    [DataField("PostingTime")]
    internal DateTime PostingTime {
      get; private set;
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


    private string GetInstrumentAsText() {
      string temp = Kind.Length != 0 ? Kind : InstrumentType.DisplayName;

      if (InstrumentNo.Length != 0) {
        temp += " número " + InstrumentNo + " ";
      } else {
        temp += " sin número ";
      }

      if (IssueDate != ExecutionServer.DateMinValue) {
        temp += "de fecha " + IssueDate.ToString("dd/MMM/yyyy") + ", ";
      } else {
        temp += "sin fecha, ";
      }

      if (!Issuer.IsEmptyInstance) {
        temp += Issuer.EntityName;
      } else {
        temp += "sin emisor";
      }

      return temp;
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
      PostedBy = ExecutionServer.CurrentContact;
      PostingTime = DateTime.Now;

      InstrumentsData.WriteInstrument(this);
    }


    public void Update(InstrumentFields data) {
      Assertion.Require(data, "data");

      this.ChangeInstrumentTypeIfRequired(data);

      this.LoadData(data);
    }

    #endregion Methods

  }  // class Instrument

}  // namespace Empiria.Land.Instruments
