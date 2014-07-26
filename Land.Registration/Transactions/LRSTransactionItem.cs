﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionItem                             Pattern  : Association Class                   *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a transaction concept in the context of a land registration transaction.           *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Land.Registration.Data;
using Empiria.Security;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Represents a transaction concept in the context of a land registration transaction.</summary>
  public class LRSTransactionItem : BaseObject, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransactionItem";

    LazyObject<LRSPayment> _payment = LazyObject<LRSPayment>.Empty;

    #endregion Fields

    #region Constructors and parsers

    private LRSTransactionItem() : base(thisTypeName) {

    }

    internal LRSTransactionItem(LRSTransaction transaction, RecordingActType transactionItemType, 
                                LRSLawArticle treasuryCode, Money operationValue, 
                                Quantity quantity, LRSFee fee) : base(thisTypeName) {
      this.Initialize();
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
                                Quantity quantity) : base(thisTypeName) {
      this.Initialize();
      this.Transaction = transaction;
      this.TransactionItemType = transactionItemType;
      this.TreasuryCode = treasuryCode;
      this.Quantity = quantity;
      this.OperationValue = operationValue;

      // Derives the calculation rule from parameters and automatically calculates the right fee
      this.CalculationRule = CalculationRule.Empty;
      this.Fee = new LRSFee();
    }

    protected LRSTransactionItem(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise.
      Initialize();
    }

    private void Initialize() {
      this.CalculationRule = CalculationRule.Empty;
      this.Notes = String.Empty;
      this.PostingTime = DateTime.Now;
      this.PostedBy = Person.Empty;
      this.Status = 'A';
    }

    static internal LRSTransactionItem Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSTransactionItem>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get;
      private set;
    }

    public RecordingActType TransactionItemType {
      get;
      private set;
    }

    public LRSLawArticle TreasuryCode {
      get;
      private set;
    }

    public CalculationRule CalculationRule {
      get;
      private set;
    }

    public LRSPayment Payment {
      get {
        return _payment.Instance;
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
          "TreasuryCodeId", this.TreasuryCode.Id, "CalculationRuleId", this.CalculationRule.Id,
          "PaymentId", this.Payment.Id, "Qty", this.Quantity.Amount, "QtyUnitId", this.Quantity.Unit.Id,
          "OpValue", this.OperationValue.Amount, "OpValueCurrencyId", this.OperationValue.Currency.Id,
          "RecordingRightsFee", this.Fee.RecordingRights, "SheetsRevisionFee", this.Fee.SheetsRevision,
          "AclarationFee", this.Fee.Aclaration, "UsufructFee", this.Fee.Usufruct,
          "EasementFee", this.Fee.Easement, "SignCertFee", this.Fee.SignCertification,
          "ForeignFee", this.Fee.ForeignRecord, "OthersFee", this.Fee.OthersCharges,
          "DiscountType", this.Fee.Discount.DiscountType.Id, "Discount", this.Fee.Discount.Amount,
          "DiscountAuthId", this.Fee.Discount.Authorization.Id, "PostingTime", this.PostingTime,
          "PostedById", this.PostedBy.Id, "Status", (char) this.Status
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

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.Transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.TransactionItemType = RecordingActType.Parse((int) row["TransactionItemTypeId"]);
      this.TreasuryCode = LRSLawArticle.Parse((int) row["TreasuryCodeId"]);
      this.CalculationRule = CalculationRule.Parse((int) row["CalculationRuleId"]);
      _payment = LazyObject<LRSPayment>.Parse((int) row["PaymentId"]);
      this.Quantity = Quantity.Parse(Unit.Parse((int) row["UnitId"]), (decimal) row["Quantity"]);
      this.OperationValue = Money.Parse(Currency.Parse((int) row["OperationValueCurrencyId"]), 
                                        (decimal) row["OperationValue"]);
      this.Fee = LRSFee.Parse(row);
      this.Notes = (string) row["Notes"];
      this.PostingTime = (DateTime) row["PostingTime"];
      this.PostedBy = Contact.Parse((int) row["PostedById"]);
      this.Status = Convert.ToChar(row["TransactionItemStatus"]);
    }

    protected override void ImplementsSave() {
      this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.PostingTime = DateTime.Now;
      TransactionData.WriteTransactionItem(this);

      this.Transaction.OnRecordingActsUpdated();
    }

    internal LRSTransactionItem MakeCopy() {
      LRSTransactionItem newItem = new LRSTransactionItem();

      newItem.TransactionItemType = this.TransactionItemType;
      newItem.TreasuryCode = this.TreasuryCode;
      newItem.OperationValue = this.OperationValue;
      newItem.Quantity = this.Quantity;
      newItem.Notes = this.Notes;

      return newItem;
    }

    #endregion Public methods


  } // class LRSTransactionItem

} // namespace Empiria.Land.Registration.Transactions