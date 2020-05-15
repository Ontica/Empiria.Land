﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : RecordingBooksData                           Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording books.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class RecordingBooksData {

    #region Public methods


    static internal DataTable GetBookRecordingNumbers(RecordingBook book) {
      string sql = "SELECT RecordingNumber FROM LRSRecordings" +
                   " WHERE RecordingBookId = " + book.Id.ToString() +
                   " AND RecordingStatus <> 'X' ORDER BY RecordingNumber";

      return DataReader.GetDataTable(DataOperation.Parse(sql));
    }


    static internal int GetLastBookRecordingNumber(RecordingBook book) {
      string sql = "SELECT MAX(RecordingNumber) FROM LRSRecordings" +
                   " WHERE RecordingBookId = " + book.Id.ToString() +
                   " AND RecordingStatus <> 'X'";
      return int.Parse(DataReader.GetScalar<string>(DataOperation.Parse(sql), "0"));
    }


    static internal int GetNextRecordingNumberWithReuse(RecordingBook book) {
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


    static internal int GetNextRecordingNumberWithNoReuse(RecordingBook book) {
      int currentRecordNumber = GetLastBookRecordingNumber(book);
      if (currentRecordNumber > 0) {
        return currentRecordNumber + 1;
      } else if (currentRecordNumber == 0 && book.UsePerpetualNumbering) {
        return book.StartRecordingIndex;
      } else if (currentRecordNumber == 0 && !book.UsePerpetualNumbering) {
        return 1;
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    static public FixedList<RecorderOffice> GetRecorderOffices(RecordingSection sectionType) {
      string sql = "SELECT DISTINCT Contacts.* FROM LRSPhysicalBooks INNER JOIN Contacts" +
                   " ON LRSPhysicalBooks.RecorderOfficeId = Contacts.ContactId" +
                   " WHERE LRSPhysicalBooks.RecordingSectionId = " + sectionType.Id +
                   " ORDER BY Contacts.NickName";

      return DataReader.GetList<RecorderOffice>(DataOperation.Parse(sql),
                                               (x) => BaseObject.ParseList<RecorderOffice>(x)).ToFixedList();
    }


    static internal FixedList<Recording> GetRecordings(RecordingDocument document,
                                                       LRSTransaction transaction) {
      string sql = "SELECT * FROM LRSRecordings WHERE TransactionId = {T} " +
                   "AND DocumentId = {D} AND RecordingStatus <> 'X' ORDER BY RecordingId";
      sql = sql.Replace("{T}", transaction.Id.ToString());
      sql = sql.Replace("{D}", document.Id.ToString());

      return DataReader.GetList<Recording>(DataOperation.Parse(sql),
                                           (x) => BaseObject.ParseList<Recording>(x)).ToFixedList();
    }


    static public FixedList<RecordingBook> GetRecordingBooks(string filter, string sort = "BookAsText") {
      string sql = "SELECT * FROM LRSPhysicalBooks" +  GeneralDataOperations.GetFilterSortSqlString(filter, sort);

      return DataReader.GetList<RecordingBook>(DataOperation.Parse(sql),
                                              (x) => BaseObject.ParseList<RecordingBook>(x)).ToFixedList();
    }


    static public FixedList<RecordingBook> GetRecordingBooks(RecorderOffice recorderOffice) {
      string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString();

      string sql = "SELECT * FROM LRSPhysicalBooks" +
                   GeneralDataOperations.GetFilterSortSqlString(filter, "BookAsText");

      return DataReader.GetList<RecordingBook>(DataOperation.Parse(sql),
                                              (x) => BaseObject.ParseList<RecordingBook>(x)).ToFixedList();
    }


    static public FixedList<RecordingBook> GetRecordingBooksInSection(RecorderOffice recorderOffice,
                                                                      RecordingSection sectionType) {
      string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString() + " AND " +
                      "RecordingSectionId = " + sectionType.Id.ToString();

      string sql = "SELECT * FROM LRSPhysicalBooks" +
                    GeneralDataOperations.GetFilterSortSqlString(filter, "BookAsText");

      return DataReader.GetList<RecordingBook>(DataOperation.Parse(sql),
                                              (x) => BaseObject.ParseList<RecordingBook>(x)).ToFixedList();
    }


    static public FixedList<Recording> GetRecordings(RecordingBook recordingBook) {
      var operation = DataOperation.Parse("qryLRSPhysicalBookRecordings", recordingBook.Id);

      return DataReader.GetList<Recording>(operation,
                                          (x) => BaseObject.ParseList<Recording>(x)).ToFixedList();
    }


    static public DataRow GetRecordingWithRecordingNumber(RecordingBook recordingBook, string recordingNumber) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingWithRecordingNumber",
                                                       recordingBook.Id, recordingNumber));
    }


    static internal Recording FindRecording(RecordingBook recordingBook, string filter) {
      string sql = "SELECT * FROM LRSPhysicalRecordings WHERE " +
                   "(PhysicalBookId = " + recordingBook.Id.ToString() + " AND RecordingStatus <> 'X')";
      if (!String.IsNullOrWhiteSpace(filter)) {
        sql += " AND " + filter;
      }
      DataRow row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return BaseObject.ParseDataRow<Recording>(row);
      } else {
        return Recording.Empty;
      }
    }


    static public DataView GetVolumeRecordingBooks(RecorderOffice recorderOffice, RecordingBookStatus status, string filter, string sort) {
      return DataReader.GetDataView(DataOperation.Parse("rptLRSVolumeBooks", recorderOffice.Id, (char) status), filter, sort);
    }


    static internal void UpdateRecordingsImageIndex(RecordingBook recordingBook, int startImageIndex, int offset) {
      DataOperation dataOperation = DataOperation.Parse("doLRSUpdateRecordingsImageIndexes",
                                                        recordingBook.Id, startImageIndex, offset);

      DataWriter.Execute(dataOperation);
    }


    static internal void WriteRecording(Recording o) {
      Assertion.Assert(o.MainDocument.Id > 0,
                       "Wrong data for physical recording. MainDocument was missed.");

      var op = DataOperation.Parse("writeLRSPhysicalRecording", o.Id, o.RecordingBook.Id,
                                   o.MainDocument.Id, o.Number, o.AsText, o.ExtendedData.GetJson().ToString(),
                                   o.Keywords, o.RecordedBy.Id, o.RecordingTime,
                                   (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingBook(RecordingBook o) {
      var op = DataOperation.Parse("writeLRSPhysicalBook", o.Id, o.RecorderOffice.Id, o.RecordingSection.Id,
                                   o.BookNumber, o.AsText, o.ExtensionData.ToString(), o.Keywords,
                                   o.StartRecordingIndex, o.EndRecordingIndex, (char) o.Status,
                                   o.RecordIntegrityHashCode);

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingDocument(RecordingDocument o) {
      var op = DataOperation.Parse("writeLRSDocument", o.Id, o.DocumentType.Id, o.Subtype.Id, o.UID,
                                   o.Imaging.ImagingControlID, o.Notes, o.AsText, o.ExtensionData.GetJson(o).ToString(),
                                   o.Keywords, o.PresentationTime, o.AuthorizationTime,
                                   o.IssuePlace.Id, o.IssueOffice.Id, o.IssuedBy.Id, o.IssueDate,
                                   o.SheetsCount, (char) o.Status, o.PostedBy.Id, o.PostingTime,
                                   o.Security.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    #endregion Public methods

  } // class RecordingBooksData

} // namespace Empiria.Land.Data
