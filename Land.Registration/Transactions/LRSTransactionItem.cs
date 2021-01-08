/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionItem                             Pattern  : Association Class                   *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a transaction concept in the context of a land registration transaction.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a transaction concept in the context of a land registration transaction.</summary>
  public class LRSTransactionItem : BaseObject, IProtected {

    #region Constructors and parsers

    private LRSTransactionItem() {
      // Required by Empiria Framework.
    }

    internal LRSTransactionItem(LRSTransaction transaction, RecordingActType transactionItemType,
                                LRSLawArticle treasuryCode, Money operationValue,
                                Quantity quantity, LRSFee fee) {
      this.Transaction = transaction;
      this.TransactionItemType = transactionItemType;
      this.TreasuryCode = treasuryCode;
      this.Quantity = quantity;
      this.OperationValue = operationValue;
      this.CalculationRule = CalculationRule.Empty;
      this.Fee = fee;
    }

    internal LRSTransactionItem(LRSTransaction transaction, RecordingActType transactionItemType,
                                LRSLawArticle treasuryCode, Money operationValue,
                                Quantity quantity) {
      this.Transaction = transaction;
      this.TransactionItemType = transactionItemType;
      this.TreasuryCode = treasuryCode;
      this.Quantity = quantity;
      this.OperationValue = operationValue;

      // Derives the calculation rule from parameters and automatically calculates the right fee
      this.CalculationRule = CalculationRule.Empty;
      this.Fee = new LRSFee();
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

    [DataField("TransactionItemTypeId")]
    public RecordingActType TransactionItemType {
      get;
      private set;
    }

    [DataField("TreasuryCodeId")]
    public LRSLawArticle TreasuryCode {
      get;
      private set;
    }

    //[DataField("CalculationRuleId")]
    public CalculationRule CalculationRule {
      get;
      private set;
    }

    [DataField("PaymentId")]
    LazyInstance<LRSPayment> _payment = LazyInstance<LRSPayment>.Empty;
    public LRSPayment Payment {
      get { return _payment.Value; }
      private set {
        _payment = LazyInstance<LRSPayment>.Parse(value);
      }
    }

    public Quantity Quantity {
      get;
      private set;
    }

    public Money OperationValue {
      get;
      private set;
    }

    public LRSFee Fee {
      get;
      private set;
    }

    //[DataField("Notes")]
    public string Notes {
      get;
      set;
    }

    [DataField("TransactionItemExtData")]
    internal JsonObject ExtensionData {
      get;
      private set;
    }

    //[DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    //[DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("TransactionItemStatus", Default = 'A')]
    public char Status {
      get;
      private set;
    }

    public decimal ComplexityIndex {
      get {
        if (this.Quantity.Amount == decimal.Zero) {
          return decimal.One;
        } else {
          return this.Quantity.Amount;
        }
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
          1, "Id", this.Id, "TransactionId", this.Transaction.Id,
          "TransactionItemTypeId", this.TransactionItemType.Id,
          "TreasuryCodeId", this.TreasuryCode.Id, "PaymentId", this.Payment.Id, "Qty", this.Quantity.Amount, "QtyUnitId", this.Quantity.Unit.Id,
          "OpValue", this.OperationValue.Amount, "OpValueCurrencyId", this.OperationValue.Currency.Id,
          "RecordingRightsFee", this.Fee.RecordingRights, "SheetsRevisionFee", this.Fee.SheetsRevision,
          "ForeignFee", this.Fee.ForeignRecordingFee, "Discount", this.Fee.Discount.Amount,
          "Status", (char) this.Status
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

    internal void Delete() {
      this.Status = 'X';
      this.Save();
    }

    internal LRSTransactionItem MakeCopy() {
      LRSTransactionItem newItem = new LRSTransactionItem();

      newItem.TransactionItemType = this.TransactionItemType;
      newItem.TreasuryCode = this.TreasuryCode;
      newItem.OperationValue = this.OperationValue;
      newItem.Quantity = this.Quantity;
      newItem.Notes = this.Notes;
      newItem.Fee = this.Fee;

      return newItem;
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.Quantity = Quantity.Parse(Unit.Parse((int) row["UnitId"]), (decimal) row["Quantity"]);
      this.OperationValue = Money.Parse(Currency.Parse((int) row["OperationValueCurrencyId"]),
                                        (decimal) row["OperationValue"]);
      this.Fee = LRSFee.Parse(row);
    }

    protected override void OnSave() {
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.PostingTime = DateTime.Now;
      TransactionData.WriteTransactionItem(this);

      this.Transaction.OnRecordingActsUpdated();
    }

    #endregion Public methods

  } // class LRSTransactionItem

} // namespace Empiria.Land.Registration.Transactions
