/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                System   : Land Registration System              *
*  Namespace : Empiria.Government.LandRegistration.Data     Assembly : Empiria.Government.LandRegistration   *
*  Type      : TransactionData                              Pattern  : Data Services Static Class            *
*  Date      : 25/Jun/2013                                  Version  : 5.1     License: CC BY-NC-SA 3.0      *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording office process or transactions.        *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Data;

using Empiria.Government.LandRegistration.Transactions;

namespace Empiria.Government.LandRegistration.Data {

  /// <summary>Provides database read and write methods for recording office process or transactions.</summary>
  static public class TransactionData {

    static Random random = new Random();

    #region Public methods

    static public LRSTransactionTrack GetLastTransactionTrack(LRSTransaction transaction) {
      DataRow row = DataReader.GetDataRow(DataOperation.Parse("getLRSLastTransactionTrack", transaction.Id));

      return LRSTransactionTrack.Parse(row);
    }

    static public DataView GetLRSTransactions(string filter, string sort) {
      string sql = "SELECT TOP 500 * FROM qryLRSTransactions()";
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
      sql = "SELECT * FROM vwLRSTransactionItemsForWS WHERE TransactionId = '" + transactionId + "'";

      table = DataReader.GetDataTable(DataOperation.Parse(sql), "Items");

      dataset.Tables.Add(table);

      return dataset;
    }

    static public ObjectList<LRSTransactionTrack> GetLRSTransactionTrack(LRSTransaction transaction) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSTransactionTrack", transaction.Id));

      return new ObjectList<LRSTransactionTrack>((x) => LRSTransactionTrack.Parse(x), view);
    }

    static public ObjectList<LRSTransactionAct> GetLRSTransactionActs(LRSTransaction transaction) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSTransactionActs", transaction.Id));

      return new ObjectList<LRSTransactionAct>((x) => LRSTransactionAct.Parse(x), view);
    }

    static public DataView GetLRSResponsibleTransactionInbox(Contacts.Contact contact, TrackStatus status,
                                                             string filter, string sort) {
      return DataReader.GetDataView(DataOperation.Parse("qryLRSResponsibleTransactionInbox", contact.Id, (char) status), filter, sort);
    }

    static public DataView GetContactsWithActiveTransactions() {
      return DataReader.GetDataView(DataOperation.Parse("SELECT * FROM vwLRSTransactionsTotals"));
    }

    static public ObjectList<Contact> GetContactsWithOutboxDocuments() {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSContactsWithOutboxDocuments"));

      return new ObjectList<Contact>((x) => Contact.Parse(x), view);
    }

    //static public ObjectList<RecorderOfficeTransactionFile> GetTransactionFiles(RecorderOfficeTransaction transaction) {
    //  string sql = "SELECT * FROM LRSTransactionFiles WHERE TransactionId = " + transaction.Id.ToString() + 
    //               " AND TransactionFileStatus = 'A'";
    //  DataView view = DataReader.GetDataView(DataOperation.Parse(sql));

    //  return new ObjectList<RecorderOfficeTransactionFile>((x) => RecorderOfficeTransactionFile.Parse(x), view);
    //}

    //static public ObjectList<RecorderOfficeTransaction> GetTransactions(RecorderOffice office, DateTime fromDate, DateTime toDate, string filter, string sort) {
    //  DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSTransactions", office.Id, fromDate, toDate), filter, sort);

    //  return new ObjectList<RecorderOfficeTransaction>((x) => RecorderOfficeTransaction.Parse(x), view);
    //}

    //static public DataView GetTransactions(RecorderOffice office, DateTime fromDate, DateTime toDate) {
    //  DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSTransactions", office.Id, fromDate, toDate));

    //  return view;
    //}

    //static public DataView GetTransactionsView(RecorderOffice office, DateTime fromDate, DateTime toDate, string filter) {
    //  return DataReader.GetDataView(DataOperation.Parse("qryLRSTransactions", office.Id, fromDate, toDate), filter);
    //}

    #endregion Public methods

    #region Internal methods

    static internal string GenerateDocumentKey() {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 7; i++) {
        if (useLetters) {
          temp += GetRandomCharacter(random, temp);
          temp += GetRandomCharacter(random, temp);
        } else {
          temp += GetRandomDigit(random, temp);
          temp += GetRandomDigit(random, temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) + Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
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

    static internal string GenerateTransactionKey() {

      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 5; i++) {
        if (useLetters) {
          temp += GetRandomCharacter(random, temp);
          temp += GetRandomCharacter(random, temp);
        } else {
          temp += GetRandomDigit(random, temp);
          temp += GetRandomDigit(random, temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) + Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
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

      string max = DataReader.GetScalar(DataOperation.Parse(sql)) as string;

      if (max != null && max.Length != 0) {
        return int.Parse(max);
      } else {
        return 1;
      }
    }

    static private char GetRandomCharacter(Random random, string current) {
      const string characters = "ABCDEFGHJKMLNPQRSTUVWXYZ";

      while (true) {
        char character = characters[random.Next(characters.Length)];
        if (!current.Contains(character.ToString())) {
          return character;
        }
      }
    }

    static private char GetRandomDigit(Random random, string current) {
      const string digits = "0123456789";

      while (true) {
        char digit = digits[random.Next(digits.Length)];
        if (!current.Contains(digit.ToString())) {
          return digit;
        }
      }
    }

    static internal int WritePaymentOrder(LRSPaymentOrder o) {
      DataOperation dataOperation = DataOperation.Parse("writeLRSPaymentOrder", o.Id, o.Transaction.Id, o.Number, o.Notes,
                                                         o.IssuedBy.Id, o.IssuedTime, o.ApprovedBy.Id, o.ApprovedTime,
                                                         o.CanceledBy.Id, o.CancelationTime, o.Keywords, o.ReceiptNumber,
                                                         o.ReceiptCaptureLine, o.ReceiptVerificationCode, o.ReceiptTotal,
                                                         o.ReceiptIssueTime, (char) o.Status, o.DigitalString, o.DigitalSign, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTransaction(LRSTransaction o) {
      DataOperation dataOperation = DataOperation.Parse("writeLRSTransaction", o.Id, o.TransactionType.Id, o.Key,
                                                         o.DocumentType.Id, o.DocumentNumber, o.Keywords,
                                                         o.RecorderOffice.Id, o.RequestedBy, o.ManagementAgency.Id, o.ContactEMail, o.ContactPhone,
                                                         o.RequestNotes, o.ReceiptNumber, o.ReceiptTotal, o.ReceiptIssueTime, o.PresentationTime,
                                                         o.ReceivedBy.Id, o.OfficeNotes, o.Document.Id, o.ComplexityIndex, o.LastReentryTime,
                                                         o.ElaborationTime, o.ElaboratedBy.Id, o.SignTime, o.SignedBy.Id, o.ClosedBy.Id, o.ClosingTime,
                                                         o.ClosingNotes, o.LastDeliveryTime, o.DeliveryNotes, o.NonWorkingTime,
                                                         o.PostingTime, o.PostedBy.Id, (char) o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTransactionAct(LRSTransactionAct o) {
      DataOperation dataOperation = DataOperation.Parse("writeLRSTransactionAct", o.Id, o.Transaction.Id,
                                                         o.RecordingActType.Id, o.LawArticle.Id, o.ReceiptNumber, o.Quantity, o.Unit.Id,
                                                         o.OperationValue.Amount, o.OperationValue.Currency.Id, o.Fee.RecordingRights,
                                                         o.Fee.SheetsRevision, o.Fee.Aclaration, o.Fee.Usufruct, o.Fee.Easement,
                                                         o.Fee.SignCertification, o.Fee.ForeignRecord, o.Fee.OthersCharges, o.Fee.Discount, o.Notes,
                                                         o.PostingTime, o.PostedBy.Id, o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTransactionItem(LRSTransactionItem o) {
      DataOperation dataOperation = DataOperation.Parse("writeLRSTransactionItem", o.Id, o.ObjectTypeInfo.Id, o.Transaction.Id,
                                                         o.PaymentOrder.Id, o.AppliedLawArticle.Id, o.AppliedConcept.Id,
                                                         o.CalculationRule.Id, o.OperationValue, o.SheetsCount, o.OperationRightsFee,
                                                         o.SheetsRevisionFee, o.AclarationFee, o.UsufructFee, o.EasementFee,
                                                         o.SignCertificationFee, o.ForeignRecordFee, o.OthersFee, o.Discount, o.AuthorizationId,
                                                         o.Notes, o.PostingTime, o.PostedBy, (char) o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTransactionTrack(LRSTransactionTrack o) {
      DataOperation dataOperation = DataOperation.Parse("writeLRSTransactionTrack", o.Id, o.Transaction.Id,
                                                         o.EventId, (char) o.Mode, o.AssignedBy.Id, o.Responsible.Id, o.NextContact.Id,
                                                         (char) o.CurrentStatus, (char) o.NextStatus, o.CheckInTime, o.EndProcessTime, o.CheckOutTime,
                                                         o.Notes, o.PreviousNode.Id, o.NextNode.Id, (char) o.Status, String.Empty);

      return DataWriter.Execute(dataOperation);
    }


    //static internal int WriteTransaction(DocumentRecordingTransaction o) {
    //  DataOperation dataOperation = DataOperation.Parse("writeLRSTransaction", o.Id, o.RecorderOffice.Id, 
    //                    o.TransactionType.Id, o.Key, o.Recording.Id, o.PresentationTime, o.RequestedBy, o.RequestedByEMail, 
    //                    o.RequestedByPhone, o.Notes, o.Keywords, o.ReceivedBy.Id, o.CapturedBy.Id, o.CapturedTime, 
    //                    o.ClosedBy.Id, o.ClosingTime, o.ClosingReasonId, o.ClosingNotes, o.DigitalString, o.DigitalSign, 
    //                    (char) o.Status, o.IntegrityHashCode);

    //  return DataWriter.Execute(dataOperation);
    //}

    //static internal int WriteTransaction(CertificateEmissionTransaction o) {
    //  DataOperation dataOperation = DataOperation.Parse("writeLRSTransaction", o.Id, o.RecorderOffice.Id,
    //                    o.TransactionType.Id, o.Key, o.Recording.Id, o.PresentationTime, o.RequestedBy, o.RequestedByEMail,
    //                    o.RequestedByPhone, o.Notes, o.Keywords, o.ReceivedBy.Id, o.CapturedBy.Id, o.CapturedTime,
    //                    o.ClosedBy.Id, o.ClosingTime, o.ClosingReasonId, o.ClosingNotes, o.DigitalString, o.DigitalSign,
    //                    (char) o.Status, o.IntegrityHashCode);

    //  return DataWriter.Execute(dataOperation);
    //}

    //static internal int WriteTransaction(DocumentCertificationTransaction o) {
    //  DataOperation dataOperation = DataOperation.Parse("writeLRSTransaction", o.Id, o.RecorderOffice.Id,
    //                    o.TransactionType.Id, o.Key, o.Recording.Id, o.PresentationTime, o.RequestedBy, o.RequestedByEMail,
    //                    o.RequestedByPhone, o.Notes, o.Keywords, o.ReceivedBy.Id, o.CapturedBy.Id, o.CapturedTime,
    //                    o.ClosedBy.Id, o.ClosingTime, o.ClosingReasonId, o.ClosingNotes, o.DigitalString, o.DigitalSign,
    //                    (char) o.Status, o.IntegrityHashCode);

    //  return DataWriter.Execute(dataOperation);
    //}

    //static internal int WriteTransactionFile(RecorderOfficeTransactionFile o) {
    //  DataOperation dataOperation = DataOperation.Parse("writeLRSTransactionFile", o.Id, o.Type.Id,
    //                    o.Transaction.Id, o.FileFormat.Id, o.Name, o.Alias, o.Notes, o.DigitalizedBy.Id, 
    //                    o.PostedBy.Id, o.PostingTime, (char) o.Status, o.FileIntegrityHashCode, 
    //                    o.IntegrityHashCode);

    //  return DataWriter.Execute(dataOperation);
    //}

    #endregion Internal methods

  } // class TransactionData

} // namespace Empiria.Government.LandRegistration.Data