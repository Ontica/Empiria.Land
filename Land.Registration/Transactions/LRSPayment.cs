/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPayment                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a payment for a recorder office transaction.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a payment for a recorder office transaction.</summary>
  public class LRSPayment : BaseObject, IProtected {

    #region Constructors and parsers

    private LRSPayment() {
      // Required by Empiria Framework.
    }

    /// <summary>Initialize payment for transaction.</summary>
    internal LRSPayment(LRSTransaction transaction, string receiptNo, 
                        decimal receiptTotal) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.Assert(!transaction.Equals(LRSTransaction.Empty),
                        "transaction shouldn't be the empty instance.");
      Assertion.AssertObject(receiptNo, "receiptNo");
      Assertion.Assert(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      this.Transaction = transaction;
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }

    /// <summary>Initialize payment for recording. Used for historic recordings
    /// without a transaction.</summary>
    internal LRSPayment(Recording recording, string receiptNo, 
                        decimal receiptTotal) {
      Assertion.AssertObject(recording, "recording");
      Assertion.Assert(recording != Recording.Empty, "recording shouldn't be the empty instance.");
      Assertion.AssertObject(receiptNo, "receiptNo");
      Assertion.Assert(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      this.Recording = recording;
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }

    static public LRSPayment Parse(int id) {
      return BaseObject.ParseId<LRSPayment>(id);
    }

    static public LRSPayment Empty {
      get {
        return BaseObject.ParseEmpty<LRSPayment>();
      }
    }

    static public LRSPayment FeeWaiver {
      get {
        return BaseObject.ParseId<LRSPayment>(-3);
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TransactionId")]
    LazyInstance<LRSTransaction> _transaction = LazyInstance<LRSTransaction>.Empty;
    public LRSTransaction Transaction {
      get { return _transaction.Value; }
      private set {
        _transaction = LazyInstance<LRSTransaction>.Parse(value);
      }
    }

    [DataField("PhysicalRecordingId")]
    LazyInstance<Recording> _recording = LazyInstance<Recording>.Empty;
    public Recording Recording {
      get { return _recording.Value; }
      private set {
        _recording = LazyInstance<Recording>.Parse(value);
      }
    }

    //[DataField("PaymentExternalID")]
    public string PaymentExternalID {
      get;
      private set;
    }

    [DataField("PaymentOfficeId")]
    LazyInstance<Organization> _paymentOffice = LazyInstance<Organization>.Empty;
    public Organization PaymentOffice {
      get { return _paymentOffice.Value; }
      private set { _paymentOffice = LazyInstance<Organization>.Parse(value); }
    }

    [DataField("ReceiptNo", Default = "No asignado")]
    public string ReceiptNo {
      get;
      private set;
    }

    [DataField("ReceiptTotal")]
    public decimal ReceiptTotal {
      get;
      private set;
    }

    [DataField("ReceiptIssuedTime")]
    public DateTime ReceiptIssuedTime {
      get;
      private set;
    }

    //[DataField("VerificationTime")]
    public DateTime VerificationTime {
      get;
      private set;
    }

    //[DataField("Notes")]
    public string Notes {
      get;
      private set;
    }

    [DataField("PaymentExtData")]
    internal JsonObject ExtensionData {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionId", this.Transaction.Id, "RecordingId", this.Recording.Id,
          "PaymentExternalID", this.PaymentExternalID, "PaymentOfficeId", this.PaymentOffice.Id,
          "ReceiptNo", this.ReceiptNo, "ReceiptTotal", this.ReceiptTotal,
          "ReceiptIssuedTime", this.ReceiptIssuedTime, "VerificationTime", this.VerificationTime,
          "PostingTime", this.PostingTime, "PostedById", this.PostedBy.Id
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

    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      TransactionData.WritePayment(this);
    }

    public void Verify() {
      throw new NotImplementedException();
    }

    #endregion Public methods

  } // class LRSPayment

} // namespace Empiria.Land.Registration.Transactions
