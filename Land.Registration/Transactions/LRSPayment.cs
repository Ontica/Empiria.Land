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

    protected LRSPayment() : base(thisTypeName) {
      Initialize();
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
      this.ReceiptNo = "Nueva orden de pago";
      this.ReceiptTotal = decimal.Zero;
      this.ReceiptIssuedTime = ExecutionServer.DateMaxValue;
      this.VerificationTime = ExecutionServer.DateMaxValue;
      this.Notes = String.Empty;
      this.PostingTime = DateTime.Now;
      this.PostedBy = Person.Empty;
    }

    public LRSPayment(LRSTransaction transaction) : base(thisTypeName) {
      Initialize();
      this.Transaction = transaction;
    }

    public LRSPayment(Recording recording) : base(thisTypeName) {
      Initialize();
      this.Recording = recording;
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
      set;
    }

    public Contact PaymentOffice {
      get;
      set;
    }

    public string ReceiptNo {
      get;
      set;
    }

    public decimal ReceiptTotal {
      get;
      set;
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
        this.ReceiptNo = "RP/OP-" + this.Id.ToString("000000000");
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
      TransactionData.WritePaymentOrder(this);
    }

    #endregion Public methods

  } // class LRSPayment

} // namespace Empiria.Land.Registration.Transactions
