/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPaymentOrder                                Pattern  : Standard Class                      *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a payment order for a recorder office transaction LRSTransaction.                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a payment order for a recorder office transaction LRSTransaction.</summary>
  public class LRSPaymentOrder : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSPaymentOrder";

    private LRSTransaction transaction = LRSTransaction.Empty;

    private string number = "Nueva orden de pago";
    private string notes = String.Empty;
    private Contact issuedBy = Person.Empty;
    private DateTime issuedTime = ExecutionServer.DateMaxValue;
    private Contact approvedBy = Person.Empty;
    private DateTime approvedTime = ExecutionServer.DateMaxValue;
    private Contact canceledBy = Person.Empty;
    private DateTime cancelationTime = ExecutionServer.DateMaxValue;
    private string keywords = String.Empty;
    private string receiptNumber = String.Empty;
    private string receiptCaptureLine = String.Empty;
    private string receiptVerificationCode = String.Empty;
    private decimal receiptTotal = decimal.Zero;
    private DateTime receiptIssueTime = ExecutionServer.DateMaxValue;
    private GeneralDocumentStatus status = GeneralDocumentStatus.Pending;
    private string digitalString = String.Empty;
    private string digitalSign = String.Empty;
    private string integrityHashCode = String.Empty;

    #endregion Fields

    #region Constuctors and parsers

    protected LRSPaymentOrder()
      : base(thisTypeName) {

    }

    public LRSPaymentOrder(LRSTransaction transaction)
      : base(thisTypeName) {
      this.transaction = transaction;
    }

    protected LRSPaymentOrder(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSPaymentOrder Parse(int id) {
      return BaseObject.Parse<LRSPaymentOrder>(thisTypeName, id);
    }

    static internal LRSPaymentOrder Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSPaymentOrder>(thisTypeName, dataRow);
    }

    static public LRSPaymentOrder Empty {
      get { return BaseObject.ParseEmpty<LRSPaymentOrder>(thisTypeName); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get { return transaction; }
    }

    public string Number {
      get { return number; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public Contact IssuedBy {
      get { return issuedBy; }
      set { issuedBy = value; }
    }

    public DateTime IssuedTime {
      get { return issuedTime; }
      set { issuedTime = value; }
    }

    public Contact ApprovedBy {
      get { return approvedBy; }
      set { approvedBy = value; }
    }

    public DateTime ApprovedTime {
      get { return approvedTime; }
      set { approvedTime = value; }
    }

    public Contact CanceledBy {
      get { return canceledBy; }
      set { canceledBy = value; }
    }

    public DateTime CancelationTime {
      get { return cancelationTime; }
      set { cancelationTime = value; }
    }

    public string Keywords {
      get { return keywords; }
    }

    public string ReceiptNumber {
      get { return receiptNumber; }
      set { receiptNumber = value; }
    }

    public string ReceiptCaptureLine {
      get { return receiptCaptureLine; }
      set { receiptCaptureLine = value; }
    }

    public string ReceiptVerificationCode {
      get { return receiptVerificationCode; }
      set { receiptVerificationCode = value; }
    }

    public decimal ReceiptTotal {
      get { return receiptTotal; }
      set { receiptTotal = value; }
    }

    public DateTime ReceiptIssueTime {
      get { return receiptIssueTime; }
      set { receiptIssueTime = value; }
    }

    public GeneralDocumentStatus Status {
      get { return status; }
    }

    public string DigitalString {
      get { return digitalString; }
      set { digitalString = value; }
    }

    public string DigitalSign {
      get { return digitalSign; }
      set { digitalSign = value; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
      set { integrityHashCode = value; }
    }

    public string StatusName(GeneralDocumentStatus status) {
      switch (status) {
        case GeneralDocumentStatus.Pending:
          return "Pendiente de pago";
        case GeneralDocumentStatus.Completed:
          return "Pagada";
        case GeneralDocumentStatus.Deleted:
          return "Eliminada";
        default:
          throw new LandRegistrationException(LandRegistrationException.Msg.InvalidPaymentOrderStatus, System.Enum.GetName(typeof(GeneralDocumentStatus), status), this.Id.ToString());
      }
    }

    public FixedList<LRSTransactionAct> GetConcepts() {
      return TransactionData.GetLRSTransactionActs(this.Transaction);
    }

    #endregion Public properties

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.number = (string) row["PaymentOrderNumber"];
      this.notes = (string) row["PaymentOrderNotes"];
      this.issuedBy = Contact.Parse((int) row["IssuedById"]);
      this.issuedTime = (DateTime) row["IssuedTime"];
      this.approvedBy = Contact.Parse((int) row["ApprovedById"]);
      this.approvedTime = (DateTime) row["ApprovedTime"];
      this.canceledBy = Contact.Parse((int) row["CanceledById"]);
      this.cancelationTime = (DateTime) row["CancelationTime"];
      this.keywords = (string) row["PaymentOrderKeywords"];
      this.receiptNumber = (string) row["ReceiptNumber"];
      this.receiptCaptureLine = (string) row["ReceiptCaptureLine"];
      this.receiptVerificationCode = (string) row["ReceiptVerificationCode"];
      this.receiptTotal = (decimal) row["ReceiptTotal"];
      this.receiptIssueTime = (DateTime) row["ReceiptIssueTime"];
      this.status = (GeneralDocumentStatus) Convert.ToChar(row["PaymentOrderStatus"]);
      this.digitalString = (string) row["PaymentOrderDigitalString"];
      this.digitalSign = (string) row["PaymentOrderDigitalSign"];
      this.integrityHashCode = (string) row["PaymentOrderRIHC"];
    }

    protected override void ImplementsSave() {
      bool isnew = this.issuedBy.IsEmptyInstance;
      if (isnew) {
        this.number = "RP/OP-" + this.Id.ToString("000000000");
        this.issuedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.issuedTime = DateTime.Now;
      }
      this.keywords = EmpiriaString.BuildKeywords(this.Number, this.ReceiptNumber, this.ReceiptCaptureLine, this.Transaction.Key, this.Transaction.DocumentNumber,
                                                  this.Transaction.RequestedBy, this.Transaction.ManagementAgency.FullName);
      this.integrityHashCode =
                          EmpiriaString.BuildDigitalString(this.Id, this.Number, this.Transaction.Id);

      TransactionData.WritePaymentOrder(this);
    }

    #endregion Public methods

  } // class LRSPaymentOrder

} // namespace Empiria.Land.Registration.Transactions
