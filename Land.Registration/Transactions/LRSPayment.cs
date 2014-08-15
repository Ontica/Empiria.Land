/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPayment                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a payment for a recorder office transaction.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;
using Empiria.Security;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a payment for a recorder office transaction.</summary>
  public class LRSPayment : BaseObject, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSPayment";

    #endregion Fields

    #region Constuctors and parsers

    /// <summary>Initialize payment for transaction.</summary>
    internal LRSPayment(LRSTransaction transaction, string receiptNo, 
                        decimal receiptTotal) : base(thisTypeName) {
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
                        decimal receiptTotal) : base(thisTypeName) {
      Assertion.AssertObject(recording, "recording");
      Assertion.Assert(recording != Recording.Empty, "recording shouldn't be the empty instance.");
      Assertion.AssertObject(receiptNo, "receiptNo");
      Assertion.Assert(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      this.Recording = recording;
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }

    protected LRSPayment(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSPayment Parse(int id) {
      return BaseObject.Parse<LRSPayment>(thisTypeName, id);
    }

    static internal LRSPayment Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSPayment>(thisTypeName, dataRow);
    }

    static public LRSPayment Empty {
      get {
        return BaseObject.ParseEmpty<LRSPayment>(thisTypeName);
      }
    }

    static public LRSPayment FeeWaiver {
      get {
        return BaseObject.Parse<LRSPayment>(thisTypeName, -3);
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TransactionId")]
    LazyObject<LRSTransaction> _transaction = LazyObject<LRSTransaction>.Empty;
    public LRSTransaction Transaction {
      get { return _transaction.Instance; }
      private set { _transaction.Instance = value; }
    }

    [DataField("RecordingId")]
    LazyObject<Recording> _recording = LazyObject<Recording>.Empty;
    public Recording Recording {
      get { return _recording.Instance; }
      private set { _recording.Instance = value; }
    }

    [DataField("PaymentExternalID")]
    public string PaymentExternalID {
      get;
      private set;
    }

    [DataField("PaymentOfficeId")]
    LazyObject<Organization> _paymentOffice = LazyObject<Organization>.Empty;
    public Organization PaymentOffice {
      get { return _paymentOffice.Instance; }
      private set { _paymentOffice.Instance = value; }
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

    [DataField("VerificationTime")]
    public DateTime VerificationTime {
      get;
      private set;
    }

    [DataField("Notes")]
    public string Notes {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("PostedById", Default = "Contacts.Person.Empty")]
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
