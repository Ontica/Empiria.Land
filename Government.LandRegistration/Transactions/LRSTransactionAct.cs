/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LRSTransactionAct                              Pattern  : Association Class                   *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents a transaction concept in the context of a land registration transaction.           *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;

using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration.Transactions {

  /// <summary>Represents a transaction concept in the context of a land registration transaction.</summary>
  public class LRSTransactionAct : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransactionAct";

    private LRSTransaction transaction = null;
    private RecordingActType recordingActType = RecordingActType.Empty;
    private LRSLawArticle lawArticle = LRSLawArticle.Empty;
    private string receiptNumber = String.Empty;
    private decimal quantity = decimal.Zero;
    private Unit unit = Unit.Empty;
    private Money operationValue = Money.Empty;
    private LRSFee fee = new LRSFee();
    private string notes = String.Empty;
    private DateTime postingTime = DateTime.Now;
    private Contact postedBy = Person.Empty;
    private char status = 'A';
    private string integrityHashCode = String.Empty;


    #endregion Fields

    #region Constructors and parsers

    public LRSTransactionAct(LRSTransaction transaction)
      : base(thisTypeName) {
      this.transaction = transaction;
    }

    protected LRSTransactionAct()
      : base(thisTypeName) {
      // Instance creation of this type may be invoked with ....
    }

    protected LRSTransactionAct(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSTransactionAct Parse(int id) {
      return BaseObject.Parse<LRSTransactionAct>(thisTypeName, id);
    }

    static internal LRSTransactionAct Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSTransactionAct>(thisTypeName, dataRow);
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransaction Transaction {
      get { return transaction; }
    }

    public RecordingActType RecordingActType {
      get { return recordingActType; }
      set { recordingActType = value; }
    }

    public LRSLawArticle LawArticle {
      get { return lawArticle; }
      set { lawArticle = value; }
    }

    public string ReceiptNumber {
      get { return receiptNumber; }
      set { receiptNumber = value; }
    }

    public decimal Quantity {
      get { return quantity; }
      set { quantity = value; }
    }
    public Unit Unit {
      get { return unit; }
      set { unit = value; }
    }

    public Money OperationValue {
      get { return operationValue; }
      set { operationValue = value; }
    }

    public LRSFee Fee {
      get { return fee; }
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

    public char Status {
      get { return status; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public decimal ComplexityIndex {
      get {
        if (this.quantity == 0m) {
          return 1m;
        } else {
          return quantity;
        }
      }
    }

    #endregion Public properties

    #region Public methods

    public void Delete() {
      this.status = 'X';
      this.Save();
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.transaction = LRSTransaction.Parse((int) row["TransactionId"]);
      this.recordingActType = RecordingActType.Parse((int) row["RecordingActTypeId"]);
      this.lawArticle = LRSLawArticle.Parse((int) row["LawArticleId"]);
      this.receiptNumber = (string) row["ReceiptNumber"];
      this.quantity = (decimal) row["Quantity"];
      this.unit = Unit.Parse((int) row["UnitId"]);
      this.operationValue = Money.Parse(Currency.Parse((int) row["OperationValueCurrencyId"]), (decimal) row["OperationValue"]);
      this.fee = LRSFee.Parse(row);
      this.notes = (string) row["Notes"];
      this.postingTime = (DateTime) row["PostingTime"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.status = Convert.ToChar(row["TransactionActStatus"]);
      this.integrityHashCode = (string) row["TransactionRIHC"];
    }

    protected override void ImplementsSave() {
      this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.postingTime = DateTime.Now;
      TransactionData.WriteTransactionAct(this);

      this.Transaction.OnRecordingActsUpdated();
    }

    #endregion Public methods

  } // class LRSTransactionAct

} // namespace Empiria.Government.LandRegistration.Transactions