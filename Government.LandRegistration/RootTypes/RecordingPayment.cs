/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingPayment                               Pattern  : Empiria Object Type                 *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Contains information about a document recording fee payment.                                  *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;

using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration {

  public enum RecordingPaymentStatus {
    NoActive = 'N',
    NoLegible = 'L',
    Incomplete = 'I',
    InProcess = 'P',
    Captured = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Represents a property.</summary>
  public class RecordingPayment : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.RecordingPayment";
    static private bool useInBatchRegistering = ConfigurationData.GetBoolean("Recording.BatchCaptureRecordingPayment");

    private Recording recording = Recording.Empty;
    private Organization paymentOffice = Organization.Empty;
    private int referenceId = 0;
    private string receiptNumber = String.Empty;
    private string otherReceipts = String.Empty;
    private string notes = String.Empty;
    private int feeTypeId = -1;
    private Contact calculatedBy = Person.Empty;
    private Contact authorizedBy = Person.Empty;
    private int discountTypeId = -1;
    private string discountAuthorizationKey = String.Empty;
    private Money feeAmount = new Money();
    private decimal feeDiscount = 0m;
    private DateTime paymentTime = ExecutionServer.DateMinValue;
    private Contact canceledBy = Person.Empty;
    private Contact postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
    private DateTime postingTime = DateTime.Now;
    private RecordingPaymentStatus status = RecordingPaymentStatus.Incomplete;
    private string recordIntegrityHashCode = String.Empty;

    public Contact AuthorizedBy {
      get { return authorizedBy; }
      set { authorizedBy = value; }
    }

    public Contact CanceledBy {
      get { return canceledBy; }
    }

    public Contact CalculatedBy {
      get { return calculatedBy; }
      set { calculatedBy = value; }
    }

    public string DiscountAuthorizationKey {
      get { return discountAuthorizationKey; }
    }

    public int DiscountTypeId {
      get { return discountTypeId; }
      set { discountTypeId = value; }
    }

    public Money FeeAmount {
      get { return feeAmount; }
      set { feeAmount = value; }
    }

    public decimal FeeDiscount {
      get { return feeDiscount; }
      set { feeDiscount = value; }
    }

    public int FeeTypeId {
      get { return feeTypeId; }
      set { feeTypeId = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public string OtherReceipts {
      get { return otherReceipts; }
      set { otherReceipts = value; }
    }

    public Organization PaymentOffice {
      get { return paymentOffice; }
      set { paymentOffice = value; }
    }

    public DateTime PaymentTime {
      get { return paymentTime; }
      set { paymentTime = value; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
      set { postingTime = value; }
    }

    public string ReceiptNumber {
      get { return receiptNumber; }
      set { receiptNumber = value; }
    }

    public Recording Recording {
      get { return recording; }
      set { recording = value; }
    }

    public string RecordIntegrityHashCode {
      get { return recordIntegrityHashCode; }
    }

    public int ReferenceId {
      get { return referenceId; }
      set { referenceId = value; }
    }

    public RecordingPaymentStatus Status {
      get { return status; }
    }

    #endregion Fields

    #region Constructors and parsers

    protected RecordingPayment(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    public RecordingPayment(Recording recording)
      : base(thisTypeName) {
      this.recording = recording;
    }

    static public RecordingPayment Parse(int id) {
      return BaseObject.Parse<RecordingPayment>(thisTypeName, id);
    }

    static internal RecordingPayment Parse(DataRow dataRow) {
      return BaseObject.Parse<RecordingPayment>(thisTypeName, dataRow);
    }

    static public RecordingPayment Empty {
      get { return BaseObject.ParseEmpty<RecordingPayment>(thisTypeName); }
    }

    static public bool UseInBatchRegistering {
      get { return useInBatchRegistering; }
    }

    #endregion Constructors and parsers

    #region Public properties

    #endregion Public properties

    #region Public methods

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.recording = Recording.Parse((int) row["RecordingId"]);
      this.paymentOffice = Organization.Parse((int) row["PaymentOfficeId"]);
      this.referenceId = (int) row["PaymentReferenceId"];
      this.receiptNumber = (string) row["ReceiptNumber"];
      this.otherReceipts = (string) row["OtherReceipts"];
      this.notes = (string) row["PaymentNotes"];
      this.feeTypeId = (int) row["FeeTypeId"];
      this.calculatedBy = Contact.Parse((int) row["FeeCalculatedById"]);
      this.authorizedBy = Contact.Parse((int) row["FeeAuthorizedById"]);
      this.discountTypeId = (int) row["FeeDiscountTypeId"];
      this.discountAuthorizationKey = (string) row["FeeDiscountAuthorizationKey"];
      this.feeAmount = Money.Parse(Currency.Parse((int) row["FeeCurrencyId"]), (decimal) row["FeeAmount"]);
      this.feeDiscount = (decimal) row["FeeDiscount"];
      this.paymentTime = (DateTime) row["PaymentTime"];
      this.canceledBy = Contact.Parse((int) row["PaymentCanceledById"]);
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.postingTime = (DateTime) row["PostingTime"];
      this.status = (RecordingPaymentStatus) Convert.ToChar(row["RecordingPaymentStatus"]);
      this.recordIntegrityHashCode = (string) row["RecordingPaymentRIHC"];
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.postingTime = DateTime.Now;
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      RecordingBooksData.WriteRecordingPayment(this);
    }

    #endregion Public methods

  } // class RecordingPayment

} // namespace Empiria.Government.LandRegistration