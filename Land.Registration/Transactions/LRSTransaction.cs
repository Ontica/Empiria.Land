/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction services                         Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LRSTransaction                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents a transaction or procedure in the context of a land registration office.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Data;
using Empiria.Land.Providers;

using Empiria.Land.Certification;

using Empiria.Land.Transactions;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a transaction or procedure in the context of a land registration office.</summary>
  public class LRSTransaction : BaseObject, IProtected {

    #region Fields

    private Lazy<LRSTransactionServicesList> _services = null;
    private Lazy<LRSTransactionPaymentData> _paymentData = null;
    private Lazy<LRSWorkflow> _workflow = null;

    #endregion Fields

    #region Constructors and parsers

    private LRSTransaction() {
      // Required by Empiria Framework.
      this.Initialize();
    }

    public LRSTransaction(LRSTransactionType transactionType) {
      this.Initialize();

      this.TransactionType = transactionType;
    }


    public LRSTransaction(TransactionFields fields) {
      this.Initialize();

      this.EnsureFieldsAreValid(fields);

      this.LoadFields(fields);
    }


    private void Initialize() {
      _services = new Lazy<LRSTransactionServicesList>(() => new LRSTransactionServicesList(this));
      _paymentData = new Lazy<LRSTransactionPaymentData>(() => new LRSTransactionPaymentData(this));
      _workflow = new Lazy<LRSWorkflow>(() => new LRSWorkflow(this));
    }


    static public LRSTransaction Parse(int id) {
      return BaseObject.ParseId<LRSTransaction>(id);
    }


    static public LRSTransaction Parse(string uid) {
      return BaseObject.ParseKey<LRSTransaction>(uid);
    }


    // TODO: Remove reload flag when it is possible.
    static public LRSTransaction TryParse(string transactionUID, bool reload = false) {
      return BaseObject.TryParse<LRSTransaction>($"TransactionUID = '{transactionUID}'", reload);
    }


    public static LRSTransaction TryParseWithAnyKey(string transactionKey) {
      if (EmpiriaString.IsInteger(transactionKey)) {
        return BaseObject.TryParse<LRSTransaction>($"InternalControlNo = '{transactionKey}'");
      }

      if (MatchesWithTransactionOldKey(transactionKey)) {
        transactionKey = LRSTransaction.GetTransactionUIDFromOldKey(transactionKey);
      }
      return BaseObject.TryParse<LRSTransaction>($"TransactionUID = '{transactionKey}'");
    }


    static public LRSTransaction TryParseForInstrument(int instrumentId) {
      return BaseObject.TryParse<LRSTransaction>($"InstrumentId = {instrumentId}");
    }


    static public FixedList<LRSTransaction> GetList(string filter, string orderBy, int pageSize) {
      return TransactionData.GetTransactionsList(filter, orderBy, pageSize);
    }


    static public LRSTransaction Empty {
      get {
        return BaseObject.ParseEmpty<LRSTransaction>();
      }
    }


    static public FixedList<Contact> GetAgenciesList() {
      GeneralList listType = GeneralList.Parse("LRSTransaction.ManagementAgencies.List");

      return listType.GetItems<Contact>((x, y) => x.ShortName.CompareTo(y.ShortName));
    }


    static public string GetTransactionUIDFromOldKey(string oldTransactionKey) {
      var temp = "TR-ZS-" + oldTransactionKey.Substring(2);

      return temp.Substring(0, 11) + "-" + temp.Substring(11);
    }


    static public bool MatchesWithTransactionOldKey(string oldTransactionKey) {
      return (oldTransactionKey.Length == 14 &&
             oldTransactionKey.StartsWith("ZS") &&
             oldTransactionKey.Contains("-"));
    }

    public static bool MatchesWithTransactionUID(string transactionUID) {
      return (transactionUID.StartsWith("TR-ZS-") && transactionUID.Length == 19);
    }


    #endregion Constructors and parsers

    #region Properties

    [DataField("TransactionUID", IsOptional = false)]
    private string _transactionUID = "Nuevo trámite";

    public override string UID {
      get {
        return _transactionUID;
      }
    }


    [DataField("TransactionGUID")]
    public string GUID {
      get;
      private set;
    }


    [DataField("InternalControlNo")]
    public string InternalControlNumber {
      get;
      private set;
    }


    public string InternalControlNumberFormatted {
      get {
        if (this.InternalControlNumber.Length == 0) {
          return string.Empty;
        }
        return string.Format("{0:n0}", int.Parse(this.InternalControlNumber));
      }
    }


    [DataField("ExternalTransactionNo")]
    public string ExternalTransactionNo {
      get;
      internal set;
    }


    [DataField("TransactionTypeId")]
    public LRSTransactionType TransactionType {
      get;
      private set;
    }


    [DataField("DocumentTypeId")]
    public LRSDocumentType DocumentType {
      get;
      set;
    }

    [DataField("RecorderOfficeId")]
    public RecorderOffice RecorderOffice {
      get;
      set;
    }

    [DataField("RequestedBy")]
    public string RequestedBy {
      get;
      set;
    }

    [DataField("AgencyId")]
    public Contact Agency {
      get;
      set;
    }


    [DataField("DocumentDescriptor")]
    public string DocumentDescriptor {
      get;
      set;
    }


    [DataField("DocumentId")]
    LazyInstance<LandRecord> _landRecord = LazyInstance<LandRecord>.Empty;

    public LandRecord LandRecord {
      get {
        return _landRecord.Value;
      }
      private set {
        _landRecord = LazyInstance<LandRecord>.Parse(value);
      }
    }


    [DataField("InstrumentId", Default = -1)]
    public int InstrumentId {
      get;
      private set;
    } = -1;


    public Resource BaseResource {
      get {
        return this.ExtensionData.BaseResource;
      }
    }


    public LRSTransactionExtData ExtensionData {
      get;
      private set;
    } = new LRSTransactionExtData();


    public LRSExternalTransaction ExternalTransaction {
      get {
        return this.ExtensionData.ExternalTransaction;
      }
    }


    [DataField("TransactionKeywords")]
    public string Keywords {
      get;
      private set;
    }


    public bool HasCertificates {
      get {
        return this.GetIssuedCertificates().Count != 0;
      }
    }


    public bool HasInstrument {
      get {
        return this.InstrumentId != -1;
      }
    }


    public bool HasServices {
      get {
        return this.Services.Count != 0;
      }
    }


    public LRSTransactionPaymentData PaymentData {
      get {
        return _paymentData.Value;
      }
    }


    public LRSTransactionServicesList Services {
      get {
        return _services.Value;
      }
    }


    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionTypeId", this.TransactionType.Id,
          "UID", this.UID, "DocumentTypeId", this.DocumentType.Id,
          "DocumentDescriptor", this.DocumentDescriptor, "DocumentId", this.LandRecord.Id,
          "RecorderOfficeId", this.RecorderOffice.Id, "RequestedBy", this.RequestedBy,
          "AgencyId", this.Agency.Id, "ExtensionData", this.ExtensionData.ToString(),
          "PresentationTime", this.PresentationTime, "ExpectedDelivery", this.ExpectedDelivery,
          "LastReentryTime", this.LastReentryTime, "ClosingTime", this.ClosingTime,
          "LastDeliveryTime", this.LastDeliveryTime, "NonWorkingTime", this.NonWorkingTime,
          "ComplexityIndex", this.Services.ComplexityIndex, "Status", (char) this.Workflow.CurrentStatus
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

    #endregion Properties

    #region Workflow related properties

    [DataField("ClosingTime")]
    public DateTime ClosingTime {
      get;
      internal set;
    }


    public TransactionControlData ControlData {
      get {
        return new TransactionControlData(this);
      }
    }


    [DataField("ExpectedDelivery")]
    public DateTime ExpectedDelivery {
      get;
      private set;
    }


    [DataField("IsArchived")]
    public bool IsArchived {
      get;
      private set;
    }

    public bool IsReentry {
      get {
        return this.LastReentryTime <= DateTime.Now;
      }
    }


    [DataField("LastDeliveryTime")]
    public DateTime LastDeliveryTime {
      get;
      internal set;
    }


    [DataField("LastReentryTime")]
    public DateTime LastReentryTime {
      get;
      internal set;
    }


    [DataField("NonWorkingTime")]
    public int NonWorkingTime {
      get;
      private set;
    }


    public Contact PostedBy {
      get {
        return this.Workflow.GetPostedBy();
      }
    }

    public DateTime PostingTime {
      get {
        return this.Workflow.GetPostingTime();
      }
    }


    [DataField("PresentationTime", Default = "ExecutionServer.DateMaxValue")]
    public DateTime PresentationTime {
      get;
      internal set;
    }


    public Contact ReceivedBy {
      get {
        return this.Workflow.GetReceivedBy();
      } // get
    }


    public LRSWorkflow Workflow {
      get {
        return _workflow.Value;
      }
    }

    #endregion Workflow related properties

    #region Methods

    public void AttachLandRecord(LandRecord landRecord) {
      Assertion.Require(!this.IsEmptyInstance && !this.IsNew,
                       "Land record can't be attached to a new or empty transaction.");
      Assertion.Require(!landRecord.IsEmptyInstance, "Attached land records can't be the empty instance.");
      Assertion.Require(this.LandRecord.IsEmptyInstance,
                        "Transaction's land record should be empty in order to be changed.");

      this.LandRecord = landRecord;

      this.Save();
    }


    public void Delete() {
      this.Workflow.Delete();
    }


    public string GetDigitalSign() {
      return Cryptographer.SignTextWithSystemCredentials(GetDigitalString())
                          .Substring(0, 64);
    }


    public string GetDigitalString() {
      string temp = String.Empty;

      temp = "||" + this.Id.ToString() + "|" + this.UID + "|";
      if (this.PresentationTime != ExecutionServer.DateMaxValue) {
        temp += this.PresentationTime.ToString("yyyyMMddTHH:mm:ss") + "|";
      }
      foreach (LRSTransactionService act in this.Services) {
        temp += "|" + act.Id.ToString() + "^" + act.ServiceType.Id.ToString() + "^";
        if (act.OperationValue.Amount != decimal.Zero) {
          temp += "B" + act.OperationValue.Amount.ToString("F4") + "^";
        }
        if (!act.Quantity.Unit.IsEmptyInstance) {
          temp += "Q" + act.Quantity.Amount.ToString("N2") + "^";
          temp += "U" + act.Quantity.Unit.Id.ToString() + "^";
        }
        temp += act.TreasuryCode.Id.ToString();

        temp += "^" + "S" + act.Fee.SubTotal.ToString("N2") + "^";
        temp += "D" + act.Fee.Discount.Amount.ToString("N2") + "^";
        temp += "T" + act.Fee.Total.ToString("N2");

      }
      temp += "||";

      return temp;
    }


    public FixedList<FormerCertificate> GetIssuedCertificates() {
      return FormerCertificatesData.GetTransactionIssuedCertificates(this);
    }


    public LRSTransaction MakeCopy() {
      var copy = new LRSTransaction(this.TransactionType);

      copy.RecorderOffice = this.RecorderOffice;
      copy.DocumentDescriptor = this.DocumentDescriptor;
      copy.DocumentType = this.DocumentType;
      copy.RequestedBy = this.RequestedBy;
      copy.Agency = this.Agency;

      if (this.PaymentData.IsFeeWaiverApplicable) {
        copy.PaymentData.ApplyFeeWaiver();
      }

      copy.Save();

      foreach (LRSTransactionService service in this.Services) {

        LRSTransactionService serviceCopy = service.MakeCopy();

        if (this.PaymentData.IsFeeWaiverApplicable) {
          // ToDo: Apply Fee Waiver on payment for each itemCopy ???
        }

        serviceCopy.Save();
      }

      return copy;
    }


    protected override void OnLoadObjectData(System.Data.DataRow row) {
      _services = new Lazy<LRSTransactionServicesList>(() => LRSTransactionServicesList.Parse(this));

      _paymentData = new Lazy<LRSTransactionPaymentData>(() => LRSTransactionPaymentData.Parse(this));

      _workflow = new Lazy<LRSWorkflow>(
                            () => LRSWorkflow.Parse(this, (TransactionStatus) Convert.ToChar(row["TransactionStatus"])));

      this.ExtensionData = LRSTransactionExtData.Parse((string) row["TransactionExtData"]);
    }


    protected override void OnSave() {
      if (base.IsNew) {
        IUniqueIDGeneratorProvider provider = ExternalProviders.GetUniqueIDGeneratorProvider();

        _transactionUID = provider.GenerateTransactionID();

        this.GUID = Guid.NewGuid().ToString();
      }

      this.Keywords = EmpiriaString.BuildKeywords(this.InternalControlNumber, this.UID,
                                                  this.LandRecord.UID,
                                                  this.PaymentData.Payments.Count == 1 ? this.PaymentData.Payments[0].ReceiptNo : string.Empty,
                                                  this.DocumentDescriptor, this.RequestedBy,
                                                  this.Agency.FullName,
                                                  this.DocumentType.Name, this.TransactionType.Name,
                                                  this.RecorderOffice.ShortName);

      TransactionData.WriteTransaction(this);

      if (base.IsNew) {

        var newWorkflow = LRSWorkflow.Create(this);

        _workflow = new Lazy<LRSWorkflow>(() => newWorkflow);

        // this.AddAutomaticItems();
      }
    }


    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return Cryptographer.CreateHashCode(this.Id.ToString("00000000"), this.UID)
                            .Substring(0, 8);
      } else {
        return String.Empty;
      }
    }


    internal void SetExternalTransaction(LRSExternalTransaction externalTransaction) {
      Assertion.Require(externalTransaction, nameof(externalTransaction));

      this.ExtensionData.ExternalTransaction = externalTransaction;
      this.ExternalTransactionNo = this.ExtensionData.ExternalTransaction.ExternalTransactionNo;
    }


    public void SetInstrument(IIdentifiable instrument) {
      this.InstrumentId = instrument.Id;

      TransactionData.SetTransactionInstrument(this, instrument);
    }


    internal void SetInternalControlNumber() {
      this.InternalControlNumber = this.BuildControlNumber();
    }


    public void Update(TransactionFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.UpdateFields(fields);
    }

    #endregion Methods

    #region Helpers

    private string BuildControlNumber() {
      int current = TransactionData.GetLastControlNumber(this.RecorderOffice);

      current++;

      return $"{current:000000}";
    }


    private void EnsureFieldsAreValid(TransactionFields fields) {
      Assertion.Require(fields, nameof(fields));

      Assertion.Require(fields.TypeUID, "fields.TypeUID");
      Assertion.Require(fields.SubtypeUID, "fields.SubtypeUID");
      Assertion.Require(fields.FilingOfficeUID, "fields.FilingOfficeUID");
      Assertion.Require(fields.AgencyUID, "fields.AgencyUID");
      Assertion.Require(fields.RequestedBy, "fields.RequestedBy");
    }


    private void LoadFields(TransactionFields fields) {
      this.TransactionType = LRSTransactionType.Parse(fields.TypeUID);
      this.DocumentType = LRSDocumentType.Parse(fields.SubtypeUID);

      Assertion.Require(this.TransactionType.GetDocumentTypes().Contains(this.DocumentType),
            $"The transaction subtype '{this.TransactionType.Name}' is not related with the " +
            $"given transaction type '{this.DocumentType.Name}'.");

      this.Agency = Contact.Parse(fields.AgencyUID);
      this.RecorderOffice = RecorderOffice.Parse(fields.FilingOfficeUID);

      this.RequestedBy = EmpiriaString.TrimAll(fields.RequestedBy);

      Assertion.Require(RequestedBy, "fields.RequestedBy");

      this.DocumentDescriptor = EmpiriaString.TrimAll(fields.InstrumentDescriptor);

      this.ExtensionData.Load(fields);
    }


    private void UpdateFields(TransactionFields fields) {
      this.TransactionType = PatchField(fields.TypeUID, this.TransactionType);
      this.DocumentType = PatchField(fields.SubtypeUID, this.DocumentType);

      this.Agency = PatchField(fields.AgencyUID, this.Agency);
      this.RecorderOffice = PatchField(fields.FilingOfficeUID, this.RecorderOffice);

      this.RequestedBy = PatchField(fields.RequestedBy, this.RequestedBy);
      this.RequestedBy = EmpiriaString.TrimAll(this.RequestedBy);

      Assertion.Require(RequestedBy, nameof(RequestedBy));

      this.DocumentDescriptor = EmpiriaString.TrimAll(fields.InstrumentDescriptor);

      this.ExtensionData.Update(fields);
    }

    #endregion Helpers

  } // class LRSTransaction

} // namespace Empiria.Land.Registration.Transactions
