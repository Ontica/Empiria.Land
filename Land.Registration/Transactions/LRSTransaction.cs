/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOfficeTransaction                      Pattern  : Association Class                   *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a transaction or process in a land registration office.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Data;
using Empiria.Land.Providers;

using Empiria.Land.Certification;
using Empiria.Land.Registration.Forms;
using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions;

using Empiria.Land.Integration.PaymentServices;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a transaction or process on a land registration office.</summary>
  public class LRSTransaction : BaseObject, IPayable, IProtected {

    #region Fields

    private static readonly decimal BaseSalaryValue = decimal.Parse(ConfigurationData.GetString("BaseSalaryValue"));

    private Lazy<LRSTransactionItemList> recordingActs = null;
    private Lazy<LRSPaymentList> payments = null;
    private Lazy<LRSWorkflow> workflow = null;

    #endregion Fields

    #region Constructors and parsers

    private LRSTransaction() {
      // Required by Empiria Framework.
    }

    public LRSTransaction(LRSTransactionType transactionType) {
      this.TransactionType = transactionType;
    }

    public LRSTransaction(TransactionFields fields) {
      this.EnsureFieldsAreValid(fields);

      this.LoadFields(fields);
    }

    static public LRSTransaction Parse(int id) {
      return BaseObject.ParseId<LRSTransaction>(id);
    }

    static public LRSTransaction Parse(string uid) {
      return BaseObject.ParseKey<LRSTransaction>(uid);
    }

    static public LRSTransaction TryParse(string transactionUID, bool reload = false) {
      return BaseObject.TryParse<LRSTransaction>("TransactionUID = '" + transactionUID + "'", reload);
    }


    public static FixedList<LRSTransaction> GetList(string filter, string orderBy, int pageSize) {
      return TransactionData.GetTransactionsList(filter, orderBy, pageSize);
    }

    static public LRSTransaction Empty {
      get { return BaseObject.ParseEmpty<LRSTransaction>(); }
    }

    static public FixedList<Contact> GetAgenciesList() {
      GeneralList listType = GeneralList.Parse("LRSTransaction.ManagementAgencies.List");

      return listType.GetItems<Contact>((x, y) => x.Alias.CompareTo(y.Alias));
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TransactionTypeId")]
    public LRSTransactionType TransactionType {
      get;
      private set;
    }

    [DataField("TransactionUID", IsOptional = false)]
    private string _transactionUID = "Nuevo trámite";


    public override string UID {
      get {
        return _transactionUID;
      }
    }

    [DataField("DocumentTypeId")]
    public LRSDocumentType DocumentType {
      get;
      set;
    }

    [DataField("DocumentDescriptor")]
    public string DocumentDescriptor {
      get;
      set;
    }

    [DataField("DocumentId")]
    LazyInstance<RecordingDocument> _document = LazyInstance<RecordingDocument>.Empty;
    public RecordingDocument Document {
      get { return _document.Value; }
      private set {
        _document = LazyInstance<RecordingDocument>.Parse(value);
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


    public bool ComesFromAgencyExternalFilingSystem {
      get {
        return !String.IsNullOrWhiteSpace(this.ExternalTransactionNo) && !this.Agency.IsEmptyInstance;
      }
    }


    public bool ComesFromCitysFilingSystem {
      get {
        return !String.IsNullOrWhiteSpace(this.ExternalTransactionNo) && !this.ExternalTransaction.IsEmptyInstance
               && this.DocumentDescriptor.StartsWith("CITyS");
      }
    }


    [DataField("ExternalTransactionNo")]
    public string ExternalTransactionNo {
      get;
      internal set;
    }


    public string GetInstrumentUID() {
      return TransactionData.GetTransactionInstrumentUID(this);
    }


    public void SetInstrument(IIdentifiable instrument) {
      this.InstrumentId = instrument.Id;

      TransactionData.SetTransactionInstrument(this, instrument);
    }


    public LRSTransactionExtData ExtensionData {
      get;
      private set;
    }


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


    [DataField("PresentationTime", Default = "ExecutionServer.DateMaxValue")]
    public DateTime PresentationTime {
      get;
      internal set;
    }


    [DataField("ExpectedDelivery")]
    public DateTime ExpectedDelivery {
      get;
      private set;
    }


    [DataField("LastReentryTime")]
    public DateTime LastReentryTime {
      get;
      internal set;
    }


    [DataField("ClosingTime")]
    public DateTime ClosingTime {
      get;
      internal set;
    }


    [DataField("LastDeliveryTime")]
    public DateTime LastDeliveryTime {
      get;
      internal set;
    }


    [DataField("NonWorkingTime")]
    public int NonWorkingTime {
      get;
      private set;
    }


    [DataField("ComplexityIndex")]
    public decimal ComplexityIndex {
      get;
      private set;
    }


    public DateTime EstimatedDueTime {
      get {
        int addedDays = (this.TransactionType.Id == 699 || this.TransactionType.Id == 702) ? 2 : 7;
        DateTime date = this.PresentationTime.Date.AddDays(addedDays);
        if (this.PresentationTime.Hour > 12) {
          addedDays++;
        }
        if (date.DayOfWeek == DayOfWeek.Saturday) {
          date = date.AddDays(2);
        } else if (date.DayOfWeek == DayOfWeek.Sunday) {
          date = date.AddDays(1);
        }
        return date;
      }
    }

    public TransactionControlData ControlData {
      get {
        return new TransactionControlData(this);
      }
    }

    [DataField("IsArchived")]
    public bool IsArchived {
      get;
      private set;
    }

    public LRSTransactionItemList Items {
      get {
        return recordingActs.Value;
      }
    }

    public bool HasInstrument {
      get {
        return this.InstrumentId != 1;
      }
    }

    public bool HasCertificates {
      get {
        return this.GetIssuedCertificates().Count != 0;
      }
    }

    public bool HasPayment {
      get {
        return (this.Payments.Count != 0 &&
                this.Payments[0].ReceiptNo.Length != 0);
      }
    }

    public bool HasPaymentOrder {
      get {
        return (!this.PaymentOrder.IsEmpty ||
                this.FormerPaymentOrderData.RouteNumber.Length != 0);
      }
    }

    public bool HasServices {
      get {
        return this.Items.Count != 0;
      }
    }

    public LRSPaymentList Payments {
      get {
        return payments.Value;
      }
    }

    public LRSWorkflow Workflow {
      get {
        return workflow.Value;
      }
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

    public Contact ReceivedBy {
      get {
        return this.Workflow.GetReceivedBy();
      } // get
    }

    public bool IsReentry {
      get {
        return this.LastReentryTime <= DateTime.Now;
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
          "DocumentDescriptor", this.DocumentDescriptor, "DocumentId", this.Document.Id,
          "RecorderOfficeId", this.RecorderOffice.Id, "RequestedBy", this.RequestedBy,
          "AgencyId", this.Agency.Id, "ExtensionData", this.ExtensionData.ToString(),
          "PresentationTime", this.PresentationTime, "ExpectedDelivery", this.ExpectedDelivery,
          "LastReentryTime", this.LastReentryTime, "ClosingTime", this.ClosingTime,
          "LastDeliveryTime", this.LastDeliveryTime, "NonWorkingTime", this.NonWorkingTime,
          "ComplexityIndex", this.ComplexityIndex, "Status", (char) this.Workflow.CurrentStatus
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

    #endregion Public properties

    #region Public methods

    public LRSTransactionItem AddItem(RecordingActType transactionItemType,
                                      LRSLawArticle treasuryCode, decimal recordingRights) {
      this.EnsureCanEditServices();

      var item = new LRSTransactionItem(this, transactionItemType, treasuryCode,
                                        Money.Zero, Quantity.One,
                                        new LRSFee() { RecordingRights = recordingRights });

      return this.PerformAddItem(item);
    }


    public LRSTransactionItem AddItem(RequestedServiceFields requestedService) {
      this.EnsureCanEditServices();

      var service = RecordingActType.Parse(requestedService.ServiceUID);
      var treasuryCode = LRSLawArticle.Parse(requestedService.FeeConceptUID);
      var operationValue = Money.Parse(requestedService.TaxableBase);
      var quantity = Quantity.Parse(Unit.Parse(requestedService.UnitUID),
                                    requestedService.Quantity);

      var fee = new LRSFee {
        RecordingRights = requestedService.Subtotal
      };

      var item = new LRSTransactionItem(this, service, treasuryCode,
                                        operationValue, quantity, fee);

      if (requestedService.Notes.Length != 0) {
        item.Notes = requestedService.Notes;

        item.Save();
      }

      return this.PerformAddItem(item);
    }


    public LRSTransactionItem AddItem(RecordingActType transactionItemType,
                                      LRSLawArticle treasuryCode, Money operationValue,
                                      Quantity quantity, LRSFee fee) {
      this.EnsureCanEditServices();
      var item = new LRSTransactionItem(this, transactionItemType, treasuryCode,
                                        operationValue, quantity, fee);

      return this.PerformAddItem(item);
    }


    public LRSTransactionItem AddItem(RecordingActType transactionItemType,
                                      LRSLawArticle treasuryCode, Quantity quantity,
                                      Money operationValue) {
      this.EnsureCanEditServices();

      var item = new LRSTransactionItem(this, transactionItemType, treasuryCode,
                                        operationValue, quantity);

      return this.PerformAddItem(item);
    }


    private LRSTransactionItem PerformAddItem(LRSTransactionItem item) {
      this.Items.Add(item);

      item.Save();

      return item;
    }

    public void Delete() {
      this.Workflow.Delete();
    }

    public bool IsFeeWaiverApplicable {
      get {
        return LRSPaymentRules.IsFeeWaiverApplicable(this);
      }
    }


    public void SetPayment(PaymentFields paymentFields) {
      this.SetPayment(paymentFields.ReceiptNo, paymentFields.Total);

      this.PaymentOrder.Status = paymentFields.Status;

      this.Save();
    }


    public void SetPayment(string receiptNo, decimal receiptTotal) {
      LRSPayment payment = null;

      if (this.Payments.Count == 0) {
        payment = new LRSPayment(this, receiptNo, receiptTotal);

        this.Payments.Add(payment);

      } else {
        payment = this.Payments[0];

        payment.SetReceipt(receiptNo, receiptTotal);
      }

      payment.Save();
    }


    public void CancelPayment() {
      Assertion.Assert(this.HasPayment,
                       $"There are not any registered payments for transaction '{this.UID}'.");

      Assertion.Assert(this.ControlData.CanCancelPayment,
                       $"Can not cancel the payment for transaction '{this.UID}'.");

      var payment = this.Payments[0];

      this.Payments.Remove(payment);

      payment.Delete();

      this.PaymentOrder.Status = String.Empty;

      this.Save();
    }


    public void ApplyFeeWaiver() {
      this.Payments.Add(LRSPayment.FeeWaiver);
    }


    public IForm GetForm() {
      return FormsProvider.GetForm(this);
    }


    public void RemoveItem(LRSTransactionItem item) {
      EnsureCanEditServices();

      this.Items.Remove(item);
      item.Delete();
    }

    public LRSTransaction MakeCopy() {
      LRSTransaction copy = new LRSTransaction(this.TransactionType);
      copy.RecorderOffice = this.RecorderOffice;
      copy.DocumentDescriptor = this.DocumentDescriptor;
      copy.DocumentType = this.DocumentType;
      copy.RequestedBy = this.RequestedBy;
      copy.Agency = this.Agency;

      if (this.IsFeeWaiverApplicable) {
        copy.ApplyFeeWaiver();
      }

      copy.Save();

      foreach (LRSTransactionItem item in this.Items) {
        LRSTransactionItem itemCopy = item.MakeCopy();
        if (this.IsFeeWaiverApplicable) {
          // ToDo: Apply Fee Waiver on payment for each itemCopy ???
        }
        itemCopy.Save();
      }
      return copy;
    }

    public void AttachDocument(RecordingDocument documentToAttach) {
      Assertion.Assert(!this.IsEmptyInstance && !this.IsNew,
                       "Document can't be attached to a new or empty transaction.");
      Assertion.Assert(!documentToAttach.IsEmptyInstance, "Attached documents can't be the empty instance.");
      Assertion.Assert(this.Document.IsEmptyInstance ||
                       documentToAttach.Equals(this.Document),
                       "Transaction's document should be empty in order to be changed.");

      /// ToDo: Should be a DB transactional op
      documentToAttach.PresentationTime = this.PresentationTime;
      documentToAttach.Save();
      this.Document = documentToAttach;
      this.Save();
    }

    public void RemoveDocument() {
      Assertion.Assert(!this.IsEmptyInstance && !this.IsNew,
                       "Document can't be detached from a new or empty transaction.");
      Assertion.Assert(!this.Document.IsEmptyInstance,
                       "Document can't be removed because it's the empty instance.");
      Assertion.Assert(this.Document.RecordingActs.Count == 0,
                       "Document has recording acts. It's not possible to delete it.");

      var tempDocument = this.Document;

      this.Document = RecordingDocument.Empty;
      this.Save();

      tempDocument.Delete();
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
      foreach (LRSTransactionItem act in this.Items) {
        temp += "|" + act.Id.ToString() + "^" + act.TransactionItemType.Id.ToString() + "^";
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

    public FixedList<Certificate> GetIssuedCertificates() {
      return Empiria.Land.Data.CertificatesData.GetTransactionIssuedCertificates(this);
    }

    protected override void OnInitialize() {
      recordingActs = new Lazy<LRSTransactionItemList>(() => new LRSTransactionItemList());
      payments = new Lazy<LRSPaymentList>(() => new LRSPaymentList());
      workflow = new Lazy<LRSWorkflow>(() => new LRSWorkflow(this));
      this.ExtensionData = new LRSTransactionExtData();
    }

    protected override void OnLoadObjectData(System.Data.DataRow row) {
      recordingActs = new Lazy<LRSTransactionItemList>(() => LRSTransactionItemList.Parse(this));
      payments = new Lazy<LRSPaymentList>(() => LRSPaymentList.Parse(this));
      workflow = new Lazy<LRSWorkflow>(
                            () => LRSWorkflow.Parse(this, (LRSTransactionStatus) Convert.ToChar(row["TransactionStatus"])));

      this.ExtensionData = LRSTransactionExtData.Parse((string) row["TransactionExtData"]);
    }

    internal void OnRecordingActsUpdated() {
      recordingActs = new Lazy<LRSTransactionItemList>(() => LRSTransactionItemList.Parse(this));
      this.UpdateComplexityIndex();
    }

    protected override void OnBeforeSave() {
      if (base.IsNew) {
        this._transactionUID = ExternalProviders.UniqueIDGeneratorProvider.GenerateTransactionUID();
      }
    }

    protected override void OnSave() {
      this.Keywords = EmpiriaString.BuildKeywords(this.UID, this.Document.UID,
                                                  this.DocumentDescriptor, this.RequestedBy,
                                                  this.Agency.FullName,
                                                  this.TransactionType.Name,
                                                  this.RecorderOffice.Alias);
      TransactionData.WriteTransaction(this);
      if (base.IsNew) {
        var newWorkflow = LRSWorkflow.Create(this);
        workflow = new Lazy<LRSWorkflow>(() => newWorkflow);

        this.AddAutomaticItems();
      }
    }


    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return FormerCryptographer.CreateHashCode(this.Id.ToString("00000000"), this.UID).Substring(0, 8);
      } else {
        return String.Empty;
      }
    }


    internal void SetExternalTransaction(LRSExternalTransaction externalTransaction) {
      Assertion.AssertObject(externalTransaction, "externalTransaction");

      this.ExtensionData.ExternalTransaction = externalTransaction;
      this.ExternalTransactionNo = this.ExtensionData.ExternalTransaction.ExternalTransactionNo;
    }

    #endregion Public methods

    #region TransactionFields related methods

    public void Update(TransactionFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.UpdateFields(fields);
    }

    private void EnsureFieldsAreValid(TransactionFields fields) {
      Assertion.AssertObject(fields, "fields");

      Assertion.AssertObject(fields.TypeUID, "fields.TypeUID");
      Assertion.AssertObject(fields.SubtypeUID, "fields.SubtypeUID");
      Assertion.AssertObject(fields.RecorderOfficeUID, "fields.RecorderOfficeUID");
      Assertion.AssertObject(fields.AgencyUID, "fields.AgencyUID");
      Assertion.AssertObject(fields.RequestedBy, "fields.RequestedBy");
    }

    private void LoadFields(TransactionFields fields) {
      this.TransactionType = LRSTransactionType.Parse(fields.TypeUID);
      this.DocumentType = LRSDocumentType.Parse(fields.SubtypeUID);

      Assertion.Assert(this.TransactionType.GetDocumentTypes().Contains(this.DocumentType),
            $"The transaction subtype '{this.TransactionType.Name}' is not related with the " +
            $"given transaction type '{this.DocumentType.Name}'.");

      this.Agency = Contact.Parse(fields.AgencyUID);
      this.RecorderOffice = RecorderOffice.Parse(fields.RecorderOfficeUID);

      this.RequestedBy = EmpiriaString.TrimAll(fields.RequestedBy);
      Assertion.AssertObject(RequestedBy, "fields.RequestedBy");

      this.DocumentDescriptor = EmpiriaString.TrimAll(fields.InstrumentDescriptor);

      this.ExtensionData.Load(fields);
    }

    private void UpdateFields(TransactionFields fields) {
      this.TransactionType = PatchField(fields.TypeUID, this.TransactionType);
      this.DocumentType = PatchField(fields.SubtypeUID, this.DocumentType);

      this.RecorderOffice = PatchField(fields.RecorderOfficeUID, this.RecorderOffice);
      this.Agency = PatchField(fields.AgencyUID, this.Agency);

      this.RequestedBy = PatchField(fields.RequestedBy, this.RequestedBy);
      this.RequestedBy = EmpiriaString.TrimAll(this.RequestedBy);

      Assertion.AssertObject(RequestedBy, "RequestedBy");

      this.ExtensionData.Update(fields);
    }

    private string PatchField(string newValue, string defaultValue) {
      if (!String.IsNullOrWhiteSpace(newValue)) {
        return newValue;
      }
      return defaultValue;
    }

    private U PatchField<U>(string newValue, U defaultValue) where U : BaseObject {
      if (!String.IsNullOrWhiteSpace(newValue)) {
        return BaseObject.ParseKey<U>(newValue);
      }
      return defaultValue;
    }

    #endregion TransactionFields related methods

    #region IPayable implementation

    public void CancelPaymentOrder() {
      this.SetPaymentOrder(PaymentOrder.Empty);
      ((IPayable) this).SetFormerPaymentOrderData(FormerPaymentOrderDTO.Empty);
    }


    public PaymentOrder PaymentOrder {
      get {
        return this.ExtensionData.PaymentOrder;
      }
    }

    public FormerPaymentOrderDTO FormerPaymentOrderData {
      get {
        return this.ExtensionData.FormerPaymentOrderData;
      }
    }

    public void SetPaymentOrder(IPaymentOrder paymentOrder) {
      Assertion.AssertObject(paymentOrder, "paymentOrder");

      this.ExtensionData.PaymentOrder = new PaymentOrder(paymentOrder);

      if (!this.IsNew) {
        this.Save();
      }
    }


    /// <summary>Former Version 4. Tlaxcala.</summary>
    void IPayable.SetFormerPaymentOrderData(FormerPaymentOrderDTO paymentOrderData) {
      this.ExtensionData.FormerPaymentOrderData = paymentOrderData;

      if (!this.IsNew) {
        this.Save();
      }
    }


    FormerPaymentOrderDTO IPayable.TryGetFormerPaymentOrderData() {
      if (!this.ExtensionData.FormerPaymentOrderData.IsEmptyInstance) {
        return this.ExtensionData.FormerPaymentOrderData;
      } else {
        return null;
      }
    }

    #endregion IPayable implementation

    #region Private methods

    private void AddAutomaticItems() {
      foreach (var item in this.DocumentType.DefaultRecordingActs) {
        this.AddItem(item, item.GetFinancialLawArticles()[0],
                     BaseSalaryValue * item.GetFeeUnits());
      }
    }

    private string BuildControlNumber() {
      return String.Empty;

      // Uncomment this code in order to generate a transaction's consecutive control number

      //int current = TransactionData.GetLastControlNumber(this.RecorderOffice);

      //current++;

      //return current.ToString();
    }


    private void EnsureCanEditServices() {
      Assertion.Assert(this.ControlData.CanEditServices,
          "The transaction is in a status that doesn't permit aggregate new services or products," +
          "or the user doesn't have enough privileges.");

    }


    private void UpdateComplexityIndex() {
      this.ComplexityIndex = 0;
      foreach (LRSTransactionItem act in this.Items) {
        this.ComplexityIndex += act.ComplexityIndex;
      }
    }

    #endregion Private methods

  } // class LRSTransaction

} // namespace Empiria.Land.Registration.Transactions
