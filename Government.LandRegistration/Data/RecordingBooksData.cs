﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingBooksData                           Pattern  : Data Services Static Class            *
*  Version   : 5.5        Date: 28/Mar/2014                 License  : GNU AGPLv3  (See license.txt)         *
*                                                                                                            *
*    Summary   : Provides database read and write methods for recording books.                               *
*                                                                                                            *
********************************* Copyright (c) 1999-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class RecordingBooksData {

    #region Public methods


    static internal DataTable GetBookRecordingNumbers(RecordingBook book) {
      string sql = "SELECT RecordingNumber FROM LRSRecordings" +
                   " WHERE RecordingBookId = " + book.Id.ToString() +
                   " AND RecordingStatus <> 'X' AND (BaseRecordingId = -1) ORDER BY RecordingNumber";

      return DataReader.GetDataTable(DataOperation.Parse(sql));
    }

    static internal int GetLastBookRecordingNumber(RecordingBook book) {
      string sql = "SELECT MAX(RecordingNumber) FROM LRSRecordings" +
                   " WHERE RecordingBookId = " + book.Id.ToString() +
                   " AND RecordingStatus <> 'X' AND (BaseRecordingId = -1)";
      return int.Parse(DataReader.GetScalar<string>(DataOperation.Parse(sql), "0"));
    }

    internal static int GetNextRecordingNumberWithReuse(RecordingBook book) {
      DataTable table = GetBookRecordingNumbers(book);

      if (table.Rows.Count == 0 && book.UsePerpetualNumbering) {
        return book.StartRecordingIndex;
      } else if (table.Rows.Count == 0 && !book.UsePerpetualNumbering) {
        return 1;
      }
      int indexValue = book.UsePerpetualNumbering ? book.StartRecordingIndex : 1;
      for (int i = 0; i < table.Rows.Count; i++, indexValue++) {
        int currentRecordNumber = int.Parse((string) table.Rows[i]["RecordingNumber"]);
        if (indexValue == currentRecordNumber) {
          continue;
        } else if ((indexValue) < currentRecordNumber) {
          return indexValue;
        } else if ((indexValue) > currentRecordNumber) {
          throw new LandRegistrationException(LandRegistrationException.Msg.RecordingNumberAlreadyExists,
                                              currentRecordNumber);
        }
      }
      return indexValue;
    }

    internal static int GetNextRecordingNumberWithNoReuse(RecordingBook book) {
      int currentRecordNumber = GetLastBookRecordingNumber(book);
      if (currentRecordNumber > 0) {
        return currentRecordNumber + 1;
      } else if (currentRecordNumber == 0 && book.UsePerpetualNumbering) {
        return book.StartRecordingIndex;
      } else if (currentRecordNumber == 0 && !book.UsePerpetualNumbering) {
        return 1;
      } else {
        throw new NotImplementedException();
      }
    }


    internal static RecordingBook GetOpenedBook(RecorderOffice office, RecordingSection recordingSection) {
      string sql = "SELECT * FROM LRSRecordingBooks WHERE RecordingBookType = 'V'" +
                   " AND (RecordingsClassId = {C}) and RecorderOfficeId = {O} AND (RecordingBookStatus = 'O')";
      sql = sql.Replace("{C}", recordingSection.Id.ToString());
      sql = sql.Replace("{O}", office.Id.ToString());

      DataRow row = DataReader.GetDataRow(DataOperation.Parse(sql));

      return RecordingBook.Parse(row);
    }

    internal static int GetBookTotalSheets(RecordingBook book) {
      object result = DataReader.GetFieldValue(DataOperation.Parse("getLRSRecordingBooksStats", book.Id),
                                               "DocumentSheets");
      if (result == null || result == DBNull.Value) {
        return 0;
      } else {
        return (int) result;
      }
    }

    static internal ObjectList<Recording> GetRecordings(RecordingDocument document,
                                                        Transactions.LRSTransaction transaction) {
      string sql = "SELECT * FROM LRSRecordings WHERE TransactionId = {T} " +
                   "AND DocumentId = {D} AND RecordingStatus <> 'X' ORDER BY RecordingId";
      sql = sql.Replace("{T}", transaction.Id.ToString());
      sql = sql.Replace("{D}", document.Id.ToString());

      DataView view = DataReader.GetDataView(DataOperation.Parse(sql));

      return new ObjectList<Recording>((x) => Recording.Parse(x), view);
    }

    static public ObjectList<RecordingBook> GetChildsRecordingBooks(RecordingBook parentRecordingBook) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSChildRecordingBooks", parentRecordingBook.Id,
                                             (char) parentRecordingBook.ChildsRecordingBookType));

      return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetPropertyAnnotationList(Property property) {
      DataOperation op = DataOperation.Parse("qryLRSPropertyAnnotations", property.Id);
      DataView view = DataReader.GetDataView(op);

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetPropertyRecordingActList(Property property) {
      DataOperation op = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);

      DataView view = DataReader.GetDataView(op);

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetPropertyRecordingActListUntil(Property property, RecordingAct breakAct, 
                                                                            bool includeBreakAct) {
      DataOperation op = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);
      DataView view = DataReader.GetDataView(op);
      List<RecordingAct> list = new List<RecordingAct>();
      foreach (DataRowView row in view) {
        var recordingAct = RecordingAct.Parse(row.Row);
        if (recordingAct.Equals(breakAct)) {
          if (includeBreakAct) {
            list.Add(RecordingAct.Parse(row.Row));
          }
          break;
        } else {
          list.Add(RecordingAct.Parse(row.Row));
        }
      }
      return new ObjectList<RecordingAct>(list);
    }


    static public ObjectList<RecordingBook> GetRecordingBooks(string filter, string sort = "RecordingBookFullName") {
      filter = GeneralDataOperations.BuildSqlAndFilter(filter, "RecordingBookType = 'V'");
      DataTable table = GeneralDataOperations.GetEntities("LRSRecordingBooks", filter, sort);

      return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), table);
    }

    static public ObjectList<RecordingBook> GetRecordingBooks(RecorderOffice recorderOffice) {
      string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString();

      DataTable table = GeneralDataOperations.GetEntities("LRSRecordingBooks", filter, "RecordingBookFullName");

      return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), table);
    }

    static public ObjectList<RecordingBook> GetRecordingBooksInSection(RecorderOffice recorderOffice,
                                                                       RecordingSection sectionType) {
      string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString() + " AND " +
                      "RecordingsClassId = " + sectionType.Id.ToString();

      DataTable table = GeneralDataOperations.GetEntities("LRSRecordingBooks", filter, "RecordingBookFullName");

      return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), table);
    }

    //static public ObjectList<RecordingBook> GetRecordingBooksInCategories(RecorderOffice recorderOffice,
    //                                                                      RecordingActTypeCategory[] categories) {
    //  string cats = String.Empty;

    //  for (int i = 0; i < categories.Length; i++) {
    //    if (i != 0) {
    //      cats += ", ";
    //    }
    //    cats += categories[i].Id.ToString();
    //  }
    //  string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString() + " AND " +
    //                  "RecordingsClassId IN (" + cats + ")";

    //  DataTable table = GeneralDataOperations.GetEntities("LRSRecordingBooks", filter, "RecordingBookFullName");

    //  return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), table);
    //}

    static public DataRow GetRecordingMainDocument(Recording recording) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingMainDocument", recording.Id));
    }

    static public ObjectList<Recording> GetRecordings(RecordingBook recordingBook) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSRecordingBookRecordings", recordingBook.Id));

      return new ObjectList<Recording>((x) => Recording.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetRecordingActs(Recording recording) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSRecordingRecordedActs", recording.Id));

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<Recording> GetRecordingsOnImageRangeList(RecordingBook recordingBook,
                                                              int imageStartIndex, int imageEndIndex) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSRecordingsOnImageRange", recordingBook.Id,
                                                                 imageStartIndex, imageEndIndex));
      return new ObjectList<Recording>((x) => Recording.Parse(x), view);
    }

    static public DataRow GetRecordingWithRecordingNumber(RecordingBook recordingBook, string recordingNumber) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingWithRecordingNumber",
                                                       recordingBook.Id, recordingNumber));
    }

    internal static Recording FindRecording(RecordingBook recordingBook, string filter) {
      string sql = "SELECT * FROM LRSRecordings WHERE " + 
                  "(RecordingBookId = " + recordingBook.Id.ToString() + " AND RecordingStatus <> 'X')";
      if (!String.IsNullOrWhiteSpace(filter)) {
        sql += " AND " + filter;
      }
      DataRow row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return Recording.Parse(row);
      } else {
        return Recording.Empty;
      }
    }

    static public ObjectList<RecordingBook> GetRootRecordingBooks(RecorderOffice recorderOffice) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSRootRecordingBooks", recorderOffice.Id));

      return new ObjectList<RecordingBook>((x) => RecordingBook.Parse(x), view);
    }

    static public DataView GetVolumeRecordingBooks(RecorderOffice recorderOffice, RecordingBookStatus status, string filter, string sort) {
      return DataReader.GetDataView(DataOperation.Parse("qryLRSVolumeBooks", recorderOffice.Id, (char) status), filter, sort);
    }

    static internal int UpdateRecordingsImageIndex(RecordingBook recordingBook, int startImageIndex, int offset) {
      DataOperation dataOperation = DataOperation.Parse("doLRSUpdateRecordingsImageIndexes",
                                                        recordingBook.Id, startImageIndex, offset);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteRecording(Recording o) {
      Assertion.Require(o.Id != 0, "Recording.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSRecording", o.Id, o.RecordingBook.Id, o.Transaction.Id,
                                                        o.Document.Id, o.BaseRecordingId, o.Number, o.StartImageIndex, o.EndImageIndex, 
                                                        o.Notes, o.Keywords, o.PresentationTime, o.ReceiptNumber, 
                                                        o.ReceiptTotal, o.ReceiptIssueDate, o.CapturedBy.Id,
                                                        o.CapturedTime, o.QualifiedBy.Id, o.QualifiedTime,
                                                        o.AuthorizedBy.Id, o.AuthorizedTime, o.CanceledBy.Id,
                                                        o.CanceledTime, o.CancelationReasonId, o.CancelationNotes,
                                                        o.DigitalString, o.DigitalSign,
                                                        (char) o.Status, o.RecordIntegrityHashCode);
      if ((o.Id % 500) == 0) {
        Empiria.Data.DataReader.Optimize();
      }
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteRecordingAct(RecordingAct o) {
      Assertion.Require(o.Id != 0, "RecordingAct.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSRecordingAct", o.Id, o.RecordingActType.Id,
                                                        o.Recording.Id, o.Index, o.Notes,
                                                        o.AppraisalAmount.Currency.Id, o.AppraisalAmount.Amount,
                                                        o.OperationAmount.Currency.Id, o.OperationAmount.Amount,
                                                        o.TermPeriods, o.TermUnit.Id, o.InterestRate, o.InterestRateType.Id,
                                                        o.ContractDate, o.ContractPlace.Id, o.ContractNumber,
                                                        o.PostedBy.Id, o.PostingTime,
                                                        (char) o.Status);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteRecordingBook(RecordingBook o) {
      Assertion.Require(o.Id != 0, "RecordingBook.Id can't be zero");
      DataOperation dataOperation = DataOperation.Parse("writeLRSRecordingBook", o.Id, o.RecorderOffice.Id,
                                                        (char) o.BookType, o.RecordingSectionType.Id, o.BookNumber, o.Name,
                                                        o.FullName, o.Description, o.Keywords, o.StartRecordingIndex, o.EndRecordingIndex,
                                                        o.RecordingsControlTimePeriod.FromDate, o.RecordingsControlTimePeriod.ToDate,
                                                        o.ImagingFilesFolder.Id, o.CreationDate, o.ClosingDate,
                                                        o.CreatedBy.Id, o.AssignedTo.Id, o.ReviewedBy.Id, o.ApprovedBy.Id,
                                                        o.Parent.Id, (char) o.Status, o.RecordIntegrityHashCode);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteRecordingDocument(RecordingDocument o) {
      return DataWriter.Execute(WriteRecordingDocumentOp(o));
    }

    static internal DataOperation WriteRecordingDocumentOp(RecordingDocument o) {
      Assertion.Require(o.Id != 0, "Document.Id can't be zero");
      return DataOperation.Parse("writeLRSDocument", o.Id, o.RecordingDocumentType.Id, o.Subtype.Id,
                                 o.DocumentKey, (char) o.DocumentRecordingRole, o.IssuePlace.Id, o.IssueOffice.Id,
                                 o.IssuedBy.Id, o.IssuedByPosition.Id, o.IssueDate, o.MainWitness.Id,
                                 o.MainWitnessPosition.Id, o.SecondaryWitness.Id, o.SecondaryWitnessPosition.Id,
                                 o.Name, o.FileName, o.BookNumber, o.ExpedientNumber, o.Number,
                                 o.SheetsCount, o.SealUpperPosition, o.StartSheet, o.EndSheet,
                                 o.Notes, o.Keywords, o.ReviewedBy.Id, o.AuthorizationKey,
                                 o.DigitalString, o.DigitalSign, o.PostedBy.Id, o.PostingTime,
                                 (char) o.Status, o.RecordIntegrityHashCode);
    }

    #endregion Public methods

  } // class RecordingBooksData

} // namespace Empiria.Land.Registration.Registration.Data
