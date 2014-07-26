﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : TransactionData                              Pattern  : Data Services Static Class            *
*  Version   : 2.0        Date: 23/Oct/2014                 License  : GNU AGPLv3  (See license.txt)         *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording office transactions.                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording office transactions.</summary>
  static public class TransactionData {

    #region Public methods

    static public LRSTransactionTask GetTransactionLastTask(LRSTransaction transaction) {
      DataRow row = DataReader.GetDataRow(DataOperation.Parse("getLRSLastTransactionTrack", transaction.Id));

      return LRSTransactionTask.Parse(row);
    }

    static public DataView GetLRSResponsibleTransactionInbox(Contacts.Contact contact, TrackStatus status,
                                                         string filter, string sort) {
      DataOperation op = DataOperation.Parse("qryLRSResponsibleTransactionInbox", contact.Id, (char) status);
      return DataReader.GetDataView(op, filter, sort);
    }

    static public DataView GetLRSTransactionsForUI(string filter, string sort) {
      string sql = "SELECT TOP 500 * FROM vwLRSTransactionsAndCurrentTrack";
      if (filter.Length != 0 && sort.Length != 0) {
        sql += " WHERE " + filter + " ORDER BY " + sort;
      } else if (filter.Length != 0 && sort.Length == 0) {
        sql += " WHERE " + filter;
      } else if (filter.Length == 0 && sort.Length != 0) {
        sql += " ORDER BY " + sort;
      } else if (filter.Length == 0 && sort.Length == 0) {
        // no-op
      }
      return DataReader.GetDataView(DataOperation.Parse(sql));
    }

    static public DataRow GetLRSTransactionWithKeyRow(string transactionKey) {
      string sql = "SELECT * FROM LRSTransactions WHERE TransactionKey = '" + transactionKey + "'";

      return DataReader.GetDataRow(DataOperation.Parse(sql));
    }

    static public DataSet GetLRSTransactionWithKey(string transactionKey) {
      DataSet dataset = new DataSet("LRSTransaction");

      string sql = "SELECT * FROM vwLRSTransactionForWS WHERE TransactionKey = '" + transactionKey + "'";
      DataTable table = DataReader.GetDataTable(DataOperation.Parse(sql), "Header");

      dataset.Tables.Add(table);
      int transactionId = 0;

      if (table.Rows.Count != 0) {
        transactionId = (int) table.Rows[0]["TransactionId"];
      }
      sql = "SELECT * FROM vwLRSTransactionItemsForWS WHERE TransactionId = " + transactionId;

      table = DataReader.GetDataTable(DataOperation.Parse(sql), "Items");

      dataset.Tables.Add(table);

      return dataset;
    }

    static public List<LRSTransactionItem> GetLRSTransactionItems(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionItems", transaction.Id);

      return DataReader.GetList<LRSTransactionItem>(operation, (x) => LRSTransactionItem.Parse(x));
    }

    static internal List<LRSPayment> GetLRSTransactionPayments(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionPayments", transaction.Id);

      return DataReader.GetList<LRSPayment>(operation, (x) => LRSPayment.Parse(x));
    }

    static public List<LRSTransactionTask> GetLRSTransactionTaskList(LRSTransaction transaction) {
      var operation = DataOperation.Parse("qryLRSTransactionTrack", transaction.Id);

      return DataReader.GetList<LRSTransactionTask>(operation, (x) => LRSTransactionTask.Parse(x));
    }

    static public DataView GetContactsWithActiveTransactions() {
      return DataReader.GetDataView(DataOperation.Parse("SELECT * FROM vwLRSTransactionsTotals"));
    }

    static public FixedList<Contact> GetContactsWithOutboxDocuments() {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSContactsWithOutboxDocuments"));

      return new FixedList<Contact>((x) => Contact.Parse(x), view);
    }

    #endregion Public methods

    #region Internal methods

    static internal string GenerateDocumentKey() {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 7; i++) {
        if (useLetters) {
          temp += EmpiriaMath.GetRandomCharacter(temp);
          temp += EmpiriaMath.GetRandomCharacter(temp);
        } else {
          temp += EmpiriaMath.GetRandomDigit(temp);
          temp += EmpiriaMath.GetRandomDigit(temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) + 
                      Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
        useLetters = !useLetters;
      }
      string prefix = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";
      temp = "RP" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      temp += "ABCDEFGHJKMLNPQRSTUVWXYZ".Substring(hashCode % 24, 1);
      temp += "9A8B7C6D5E4F3G2H1JKR".Substring(hashCode % 20, 1);

      return temp;
    }  

    static public string GeneratePropertyKey() {
      string temp = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigitOrCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter();
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter(temp);
      temp += EmpiriaMath.GetRandomDigit(temp);
      temp += EmpiriaMath.GetRandomCharacter();

      int hashCode = 0;
      for (int i = 0; i < temp.Length; i++) {
        hashCode += (Convert.ToInt32(temp[i]) + Convert.ToInt32(i == 0 ? 0 : temp[i - 1])) * (i + 1);
      }
      temp += GetChecksumCharacterCode(hashCode);

      return temp.Substring(0, 4) + "-" + temp.Substring(4, 4) + "-" + temp.Substring(8, 4) + "-" + temp.Substring(12, 4);
    }

    static internal string GetChecksumCharacterCode(int hashCode) {
      string hashCodeConvertionRule = ExecutionServer.LicenseName == "Zacatecas" ?
                                                                     "AL8GD7E95ZJSXYTKBHFWR43MCU6N21QPV0" :
                                                                     "NAXMT1C5WZ7J3HE489RLGV6F2PUQKYD0BS";
      return hashCodeConvertionRule.Substring(hashCode % hashCodeConvertionRule.Length, 1);
    }

    static internal string GenerateTransactionKey() {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 5; i++) {
        if (useLetters) {
          temp += EmpiriaMath.GetRandomCharacter(temp);
          temp += EmpiriaMath.GetRandomCharacter(temp);
        } else {
          temp += EmpiriaMath.GetRandomDigit(temp);
          temp += EmpiriaMath.GetRandomDigit(temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) + 
                      Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
        useLetters = !useLetters;
      }
      string prefix = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";
      temp = prefix + temp;
      hashCode = (hashCode * Convert.ToInt32(prefix[0])) % 49;
      hashCode = (hashCode * Convert.ToInt32(prefix[1])) % 53;

      return temp + "-" + (hashCode % 10).ToString();
    }

    static internal int GetLastControlNumber(RecorderOffice recorderOffice) {
      string sql = "SELECT MAX(ControlNumber) FROM vwLRSTransactions WHERE RecorderOfficeId = " + recorderOffice.Id.ToString();

      string max = DataReader.GetScalar<String>(DataOperation.Parse(sql), String.Empty);

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 1;
      }
    }

    static internal int WritePaymentOrder(LRSPayment o) {
      Assertion.Require(o.Id != 0, "LRSPayment.Id can't be zero");
      var dataOperation = DataOperation.Parse("writeLRSPaymentOrder", o.Id, o.Transaction.Id, 
                                  o.Recording.Id, o.PaymentExternalID, o.PaymentOffice.Id, o.ReceiptNo,
                                  o.ReceiptTotal, o.ReceiptIssuedTime, o.VerificationTime, o.Notes,
                                  o.PostingTime, o.PostedBy.Id, o.Integrity.GetUpdatedHashCode());

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTransaction(LRSTransaction o) {
      return DataWriter.Execute(WriteTransactionOp(o));
    }

    static internal DataOperation WriteTransactionOp(LRSTransaction o) {
      Assertion.Require(o.Id != 0, "LRSTransaction.Id can't be zero");
      return DataOperation.Parse("writeLRSTransaction", o.Id, o.TransactionType.Id, o.UniqueCode,
                                 o.DocumentType.Id, o.DocumentDescriptor, o.Document.Id, o.RecorderOffice.Id,
                                 o.RequestedBy, o.ManagementAgency.Id, o.ExtensionData.ToJson(), o.Keywords,
                                 o.PresentationTime, o.ExpectedDelivery, o.LastReentryTime, o.ClosingTime,
                                 o.LastDeliveryTime, o.NonWorkingTime, o.ComplexityIndex, o.IsArchived, 
                                 (char) o.Status, o.Integrity.GetUpdatedHashCode());
    }

    static internal int WriteTransactionItem(LRSTransactionItem o) {
      Assertion.Require(o.Id != 0, "LRSTransactionAct.Id can't be zero");
      var operation = DataOperation.Parse("writeLRSTransactionItem", o.Id, o.Transaction.Id,
                                          o.TransactionItemType.Id, o.TreasuryCode.Id, o.CalculationRule.Id,
                                          o.Payment.Id, o.Quantity.Amount, o.Quantity.Unit.Id,
                                          o.OperationValue.Amount, o.OperationValue.Currency.Id,
                                          o.Fee.RecordingRights, o.Fee.SheetsRevision, o.Fee.Aclaration,
                                          o.Fee.Usufruct, o.Fee.Easement, o.Fee.SignCertification,
                                          o.Fee.ForeignRecord, o.Fee.OthersCharges, 
                                          o.Fee.Discount.DiscountType.Id, o.Fee.Discount.Amount, 
                                          o.Fee.Discount.Authorization.Id, o.Notes, o.PostingTime,
                                          o.PostedBy.Id, o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteTransactionTask(LRSTransactionTask o) {
      Assertion.Require(o.Id != 0, "LRSTransactionTask.Id can't be zero");
      var operation = DataOperation.Parse("writeLRSTransactionTrack", o.Id, o.Transaction.Id,
                                          o.EventId, (char) o.Mode, o.AssignedBy.Id, o.Responsible.Id, 
                                          o.NextContact.Id, (char) o.CurrentStatus, (char) o.NextStatus, 
                                          o.CheckInTime, o.EndProcessTime, o.CheckOutTime, o.Notes, 
                                          o.PreviousTask.Id, o.NextTask.Id, (char) o.Status, String.Empty);

      return DataWriter.Execute(operation);
    }

    #endregion Internal methods


  } // class TransactionData

} // namespace Empiria.Land.Registration.Registration.Data