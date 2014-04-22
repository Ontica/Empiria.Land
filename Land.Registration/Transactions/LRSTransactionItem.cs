/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionTrack                            Pattern  : Association Class                   *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Abstract class that represents a transaction or process on a land registration office.        *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Abstract class that represents a transaction or process on a land registration office.</summary>
  public class LRSTransactionItem : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransactionItem";

    private LRSTransaction transaction = LRSTransaction.Empty;
    private LRSPaymentOrder paymentOrder = LRSPaymentOrder.Empty;
    private LRSLawArticle appliedLawArticle = LRSLawArticle.Empty;
    private RecordingActType appliedConcept = RecordingActType.Empty;
    private CalculationRule calculationRule = CalculationRule.Empty;
    private decimal operationValue = decimal.Zero;
    private int sheetsCount = 0;
    private decimal operationRightsFee = decimal.Zero;

    private decimal sheetsRevisionFee = decimal.Zero;
    private decimal aclarationFee = decimal.Zero;
    private decimal usufructFee = decimal.Zero;
    private decimal easementFee = decimal.Zero;
    private decimal signCertificationFee = decimal.Zero;
    private decimal foreignRecordFee = decimal.Zero;
    private decimal othersFee = decimal.Zero;
    private decimal discount = decimal.Zero;
    private int authorizationId = 0;
    private string notes = String.Empty;
    private DateTime postingTime = DateTime.Now;
    private Contact postedBy = Person.Empty;
    private GeneralObjectStatus status = GeneralObjectStatus.Pending;
    private string integrityHashCode = String.Empty;
    // use null initalization instead LRSTransactionItem.Empty because is a fractal object and Empty instance 
    // parsing throws an infinite loop
    private LRSTransactionItem parent = null;
    private int parentId = -1;

    #endregion Fields

    #region Constructors and parsers

    public LRSTransactionItem(LRSTransaction transaction)
      : base(thisTypeName) {
      this.transaction = transaction;
    }

    protected LRSTransactionItem()
      : base(thisTypeName) {
      // Instance creation of this type may be invoked with ....
    }

    protected LRSTransactionItem(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSTransactionItem Parse(int id) {
      return BaseObject.Parse<LRSTransactionItem>(thisTypeName, id);
    }

    static internal LRSTransactionItem Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSTransactionItem>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get { return transaction; }
    }

    public LRSPaymentOrder PaymentOrder {
      get { return paymentOrder; }
      set { paymentOrder = value; }
    }

    public LRSLawArticle AppliedLawArticle {
      get { return appliedLawArticle; }
      set { appliedLawArticle = value; }
    }

    public RecordingActType AppliedConcept {
      get { return appliedConcept; }
      set { appliedConcept = value; }
    }

    public CalculationRule CalculationRule {
      get { return calculationRule; }
      set { calculationRule = value; }
    }

    public decimal OperationValue {
      get { return operationValue; }
      set { operationValue = value; }
    }

    public int SheetsCount {
      get { return sheetsCount; }
      set { sheetsCount = value; }
    }

    public decimal OperationRightsFee {
      get { return operationRightsFee; }
      set { operationRightsFee = value; }
    }

    public decimal SheetsRevisionFee {
      get { return sheetsRevisionFee; }
      set { sheetsRevisionFee = value; }
    }

    public decimal AclarationFee {
      get { return aclarationFee; }
      set { aclarationFee = value; }
    }

    public decimal UsufructFee {
      get { return usufructFee; }
      set { usufructFee = value; }
    }

    public decimal EasementFee {
      get { return easementFee; }
      set { easementFee = value; }
    }

    public decimal SignCertificationFee {
      get { return signCertificationFee; }
      set { signCertificationFee = value; }
    }

    public decimal ForeignRecordFee {
      get { return foreignRecordFee; }
      set { foreignRecordFee = value; }
    }

    public decimal OthersFee {
      get { return othersFee; }
      set { othersFee = value; }
    }

    public decimal Discount {
      get { return discount; }
      set { discount = value; }
    }

    public int AuthorizationId {
      get { return authorizationId; }
      set { authorizationId = value; }
    }

    public string Notes {
      get { return notes; }
      set { notes = value; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public GeneralObjectStatus Status {
      get { return status; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public LRSTransactionItem Parent {
      get {
        if (parent == null) {
          parent = LRSTransactionItem.Parse(parentId);
        }
        return parent;
      }
    }

    #endregion Public properties

    #region Public methods

    public decimal Subtotal() {
      decimal subtotal = this.OperationRightsFee + this.SheetsRevisionFee + this.AclarationFee +
                         this.UsufructFee + this.EasementFee + this.SignCertificationFee +
                         this.ForeignRecordFee + this.OthersFee;

      return subtotal;
    }

    public decimal Total() {
      return Subtotal() - this.Discount;
    }

    public void Delete() {
      this.status = GeneralObjectStatus.Deleted;
      this.Save();
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.paymentOrder = LRSPaymentOrder.Parse((int) row["PaymentOrderId"]);
      this.appliedLawArticle = LRSLawArticle.Parse((int) row["AppliedLawArticleId"]);
      this.appliedConcept = RecordingActType.Parse((int) row["AppliedConceptId"]);
      this.calculationRule = CalculationRule.Parse((int) row["CalculationRuleId"]);
      this.operationValue = (decimal) row["OperationValue"];
      this.sheetsCount = (int) row["SheetsCount"];
      this.operationRightsFee = (decimal) row["OperationRightsFee"];
      this.sheetsRevisionFee = (decimal) row["SheetsRevisionFee"];
      this.aclarationFee = (decimal) row["AclarationFee"];
      this.usufructFee = (decimal) row["UsufructFee"];
      this.easementFee = (decimal) row["EasementFee"];
      this.signCertificationFee = (decimal) row["SignCertificationFee"];
      this.foreignRecordFee = (decimal) row["ForeignRecordFee"];
      this.othersFee = (decimal) row["OthersFee"];
      this.discount = (decimal) row["Discount"];
      this.authorizationId = (int) row["AuthorizationId"];
      this.notes = (string) row["TransactionItemNotes"];
      this.postingTime = (DateTime) row["PostingTime"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.status = (GeneralObjectStatus) Convert.ToChar(row["TransactionItemStatus"]);
      this.parentId = (int) row["ParentTransactionItemId"];
      this.integrityHashCode = (string) row["TransactionRIHC"];
    }

    protected override void ImplementsSave() {
      this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.postingTime = DateTime.Now;
      TransactionData.WriteTransactionItem(this);
    }

    #endregion Public methods

  } // class LRSTransactionItem

} // namespace Empiria.Land.Registration.Transactions
