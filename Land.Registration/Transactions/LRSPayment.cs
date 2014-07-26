/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSPayment                                     Pattern  : Standard Class                      *
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

    internal LRSPayment(LRSTransaction transaction, string receiptNo, 
                        decimal receiptTotal) : base(thisTypeName) {
      Assertion.RequireObject(transaction, "transaction");
      Assertion.Require(transaction != LRSTransaction.Empty, "transaction shouldn't be the empty instance.");
      Assertion.RequireObject(receiptNo, "receiptNo");
      Assertion.Require(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      Initialize();
      this.Transaction = transaction;
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }

    internal LRSPayment(Recording recording, string receiptNo, 
                        decimal receiptTotal) : base(thisTypeName) {
      Assertion.RequireObject(recording, "recording");
      Assertion.Require(recording != Recording.Empty, "recording shouldn't be the empty instance.");
      Assertion.RequireObject(receiptNo, "receiptNo");
      Assertion.Require(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      Initialize();
      this.Recording = recording;
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }

    protected LRSPayment(string typeName) : base(typeName) {
      Initialize();
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    private void Initialize() {
      this.Transaction = LRSTransaction.Empty;
      this.Recording = Recording.Empty;
      this.PaymentExternalID = String.Empty;
      this.PaymentOffice = Person.Empty;
      this.ReceiptNo = "No asignado";
      this.ReceiptTotal = decimal.Zero;
      this.ReceiptIssuedTime = ExecutionServer.DateMaxValue;
      this.VerificationTime = ExecutionServer.DateMaxValue;
      this.Notes = String.Empty;
      this.PostingTime = DateTime.Now;
      this.PostedBy = Person.Empty;
    }

    static public LRSPayment Parse(int id) {
      return BaseObject.Parse<LRSPayment>(thisTypeName, id);
    }

    static internal LRSPayment Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSPayment>(thisTypeName, dataRow);
    }

    static public LRSPayment Empty {
      get { return BaseObject.ParseEmpty<LRSPayment>(thisTypeName); }
    }

    static public LRSPayment FeeWaiver {
      get { return BaseObject.Parse<LRSPayment>(thisTypeName, -3); }
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get;
      private set;
    }

    public Recording Recording {
      get;
      private set;
    }

    public string PaymentExternalID {
      get;
      private set;
    }

    public Contact PaymentOffice {
      get;
      private set;
    }

    public string ReceiptNo {
      get;
      private set;
    }

    public decimal ReceiptTotal {
      get;
      private set;
    }

    public DateTime ReceiptIssuedTime {
      get;
      private set;
    }

    public DateTime VerificationTime {
      get;
      private set;
    }

    public string Notes {
      get;
      set;
    }

    public DateTime PostingTime {
      get;
      private set;
    }

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

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.Transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.Recording = Recording.Parse((int) row["RecordingId"]);
      this.PaymentExternalID = (string) row["PaymentExternalID"];
      this.PaymentOffice = Contact.Parse((int) row["PaymentOfficeId"]);
      this.ReceiptNo = (string) row["ReceiptNo"];
      this.ReceiptTotal = (decimal) row["ReceiptTotal"];
      this.ReceiptIssuedTime = (DateTime) row["ReceiptIssuedTime"];
      this.VerificationTime = (DateTime) row["VerificationTime"];
      this.Notes = (string) row["Notes"];
      this.PostingTime = (DateTime) row["PostingTime"];
      this.PostedBy = Contact.Parse((int) row["PostedById"]);

      Integrity.Assert((string) row["PaymentDIF"]);
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      TransactionData.WritePaymentOrder(this);
    }

    public void Verify() {
      throw new NotImplementedException();
    }

    #endregion Public methods

  } // class LRSPayment

} // namespace Empiria.Land.Registration.Transactions
