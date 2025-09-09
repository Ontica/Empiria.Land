/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                        Component : Domain Layer                           *
*  Assembly : Empiria.Land.Core.dll                       Pattern   : Data holder                            *
*  Type     : LRSPayment                                  License   : Please read LICENSE.txt file           *
*                                                                                                            *
*  Summary  : Represents a payment for a recorder office transaction.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;
using Empiria.StateEnums;

using Empiria.Land.Transactions.Payments.Data;

namespace Empiria.Land.Transactions.Payments {

  /// <summary>Represents a payment for a recorder office transaction.</summary>
  public class LRSPayment : BaseObject, IProtected {

    #region Constructors and parsers

    private LRSPayment() {
      // Required by Empiria Framework.
    }

    /// <summary>Initialize payment for transaction.</summary>
    internal LRSPayment(LRSTransaction transaction, string receiptNo,
                        decimal receiptTotal) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(!transaction.Equals(LRSTransaction.Empty),
                        "transaction shouldn't be the empty instance.");
      Assertion.Require(receiptNo, nameof(receiptNo));
      Assertion.Require(receiptTotal >= 0, "receiptTotal shouldn't be a negative amount.");

      this.Transaction = transaction;
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

    #region Properties

    [DataField("TransactionId")]
    LazyInstance<LRSTransaction> _transaction = LazyInstance<LRSTransaction>.Empty;


    public LRSTransaction Transaction {
      get {
        return _transaction.Value;
      }
      private set {
        _transaction = LazyInstance<LRSTransaction>.Parse(value);
      }
    }


    [DataField("PaymentOfficeId")]
    LazyInstance<Organization> _paymentOffice = LazyInstance<Organization>.Empty;
    public Organization PaymentOffice {
      get {
        return _paymentOffice.Value;
      }
      private set {
        _paymentOffice = LazyInstance<Organization>.Parse(value);
      }
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

    [DataField("PostingTime")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PaymentStatus", Default = OpenCloseStatus.Closed)]
    public OpenCloseStatus Status {
      get;
      private set;
    } = OpenCloseStatus.Closed;


    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionId", this.Transaction.Id,
          "PaymentOfficeId", this.PaymentOffice.Id,
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

    #endregion Properties

    #region Methods

    internal void Delete() {
      this.Status = OpenCloseStatus.Deleted;

      this.Save();
    }


    public void SetReceipt(string receiptNo, decimal receiptTotal) {
      this.ReceiptNo = receiptNo;
      this.ReceiptTotal = receiptTotal;
    }


    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = ExecutionServer.CurrentContact;
        this.PostingTime = DateTime.Now;
      }
      TransactionPaymentsDataService.WritePayment(this);
    }

    #endregion Methods

  } // class LRSPayment

} // namespace Empiria.Land.Transactions.Payments
