/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instrument Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Empiria Object Type                     *
*  Type     : Instrument                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a legal instrument: deed, will, contract, notarial act, court writ, etc.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Registration;

namespace Empiria.Land.Instruments {

  /// <summary>Represents a legal instrument: deed, will, contract, notarial act, court writ, etc.</summary>
  public class LegalInstrument : BaseObject, IProtected {

    #region Constructors and parsers


    private LegalInstrument() {
      // Required by Empiria Framework
    }


    public LegalInstrument(JsonObject data) {
      this.ExtensionData = new JsonObject();

      this.LoadData(data);
    }


    static public LegalInstrument TryParse(string uid) {
      return BaseObject.ParseKey<LegalInstrument>(uid);
    }


    static public FixedList<LegalInstrument> GetList(InstrumentStatus status, string keywords) {
      string filter = status != InstrumentStatus.All ? $"InstrumentStatus = '{(char) status}'" : String.Empty;

      if (!String.IsNullOrWhiteSpace(keywords)) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += SearchExpression.ParseAndLikeKeywords("InstrumentKeywords", keywords);
      }

      string sort = "PostingTime DESC";

      return BaseObject.GetList<LegalInstrument>(filter, sort).ToFixedList();
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("InstrumentNo")]
    public string Number {
      get;
      private set;
    }


    [DataField("RequestedBy")]
    public string RequestedBy {
      get;
      private set;
    }


    public RealEstate Property {
      get {
        var propertyUID = ExtensionData.Get("propertyUID", String.Empty);

        if (propertyUID.Length != 0) {
          return RealEstate.TryParseWithUID(propertyUID, true);
        } else {
          return RealEstate.Empty;
        }
      }
    }


    [DataField("IssueOfficeId")]
    public Organization IssueOffice {
      get;
      private set;
    }


    [DataField("IssuedById")]
    public Contact IssuedBy {
      get;
      private set;
    }


    [DataField("IssueDate", Default = "ExecutionServer.DateMinValue")]
    public DateTime IssueDate {
      get;
      private set;
    }


    [DataField("Summary")]
    public string Summary {
      get;
      private set;
    }


    [DataField("InstrumentExtData")]
    protected internal JsonObject ExtensionData {
      get;
      private set;
    } = new JsonObject();


    public virtual string Keywords {
      get {
        var propertyUID = ExtensionData.Get("propertyUID", String.Empty);

        return EmpiriaString.BuildKeywords(this.Number, propertyUID, this.RequestedBy, this.Summary);
      }
    }


    [DataField("InstrumentStatus", Default = InstrumentStatus.Pending)]
    public InstrumentStatus Status {
      get;
      private set;
    }


    public string StatusName {
      get {
        switch (this.Status) {
          case InstrumentStatus.Pending:
            return "Pendiente";
          case InstrumentStatus.Signed:
            return "Firmado";
          case InstrumentStatus.Requested:
            return "En trámite";
          case InstrumentStatus.Delivered:
            return "Registrado";
          case InstrumentStatus.Returned:
            return "Devuelto";
          case InstrumentStatus.Deleted:
            return "Eliminado";
          default:
            throw Assertion.AssertNoReachThisCode("Unrecognized instrument status.");
        }
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

    public string ElectronicSign {
      get {
        return ExtensionData.Get("esign", String.Empty);
      }
    }

    public string TransactionUID {
      get {
        return ExtensionData.Get("transactionUID", String.Empty);
      }
    }

    #endregion Fields


    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "UID", this.UID, "RequestedBy", this.RequestedBy, "Number", this.Number,
          "Summary", this.Summary, "IssueOffice", this.IssueOffice.Id, "IssuedBy", this.IssuedBy.Id, "IssueDate", this.IssueDate,
          "PostingTime", this.PostingTime, "PostedBy", this.PostedBy.Id,
          "ExtensionData", this.ExtensionData.ToString(), "Status", (char) this.Status
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


    public void RequestFiling(JsonObject data) {
      if (this.TransactionUID.Length != 0) {
        return;
      }

      var creator = new TransactionCreator();

      var transaction = creator.CreateTransaction(this, data);

      ExtensionData.Set("transactionUID", transaction.UID);
    }


    public void FileTransaction(JsonObject data) {
      if (this.TransactionUID.Length == 0) {
        return;
      }

      var creator = new TransactionCreator();

      creator.FileTransaction(this, data);
    }

    #region Public methods

    public void AssertCanBeClosed() {

    }

    public void Delete() {

    }

    public void RevokeSign(JsonObject bodyAsJson) {
      this.ExtensionData.Remove("esign");

      this.Status = InstrumentStatus.Pending;
      this.IssueDate = ExecutionServer.DateMinValue;
    }


    public void Sign(JsonObject bodyAsJson) {
      var signToken = bodyAsJson.Get<string>("signToken");

      // var securedToken = Cryptographer.ConvertToSecureString(signToken);

      var esign = Cryptographer.SignTextWithSystemCredentials(this.GetElectronicSeal());    // securedToken 2nd arg

      this.ExtensionData.Set("esign", esign);
      this.Status = InstrumentStatus.Signed;
      this.IssueDate = DateTime.Now;
    }


    public virtual string GetElectronicSeal() {
      var seed = EmpiriaString.BuildDigitalString(this.Id, this.UID, this.Property.UID, this.RequestedBy,
                                                  this.Summary, this.IssueOffice.Id, this.IssuedBy.Id);

      return Cryptographer.CreateHashCode(seed, this.UID);
    }


    protected override void OnSave() {
      if (this.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
        this.IssueOffice = Organization.Parse(511);
        this.IssuedBy = Contact.Parse(1090);
      }
      InstrumentData.WriteInstrument(this);
    }


    public void Update(JsonObject data) {
      Assertion.AssertObject(data, "data");

      this.LoadData(data);
    }

    #endregion Public methods

    #region Private methods

    private void LoadData(JsonObject data) {
      this.RequestedBy = data.Get<string>("requestedBy");
      this.ExtensionData.Set("propertyUID", data.Get<string>("propertyUID"));
      this.Summary = data.Get<string>("projectedOperation");
    }

    #endregion Private methods

  } // class Instrument

} // namespace Empiria.Land.Instruments
