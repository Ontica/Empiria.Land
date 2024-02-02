/* Empiria Land **********************************************************************************************
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

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording books.</summary>
  static public class RecordingBooksData {

    #region Public methods


    static internal DataTable GetBookEntriesNumbers(RecordingBook book) {
      string sql = "SELECT RecordingNo FROM LRSPhysicalRecordings" +
                   $" WHERE PhysicalBookId = {book.Id}" +
                   " AND RecordingStatus <> 'X' ORDER BY RecordingNo";

      return DataReader.GetDataTable(DataOperation.Parse(sql));
    }


    static internal int GetLastBookEntryNumber(RecordingBook book) {
      string sql = "SELECT MAX(RecordingNo) FROM LRSPhysicalRecordings" +
                   " WHERE PhysicalBookId = " + book.Id.ToString() +
                   " AND RecordingStatus <> 'X'";
      return int.Parse(DataReader.GetScalar<string>(DataOperation.Parse(sql), "0"));
    }


    static internal int GetNextBookEntryNumberWithReuse(RecordingBook book) {
      DataTable table = GetBookEntriesNumbers(book);

      if (table.Rows.Count == 0 && book.UsePerpetualNumbering) {
        return book.StartRecordingIndex;
      } else if (table.Rows.Count == 0 && !book.UsePerpetualNumbering) {
        return 1;
      }

      int indexValue = book.UsePerpetualNumbering ? book.StartRecordingIndex : 1;

      for (int i = 0; i < table.Rows.Count; i++, indexValue++) {

        int currentBookEntryNumber = int.Parse((string) table.Rows[i]["RecordingNo"]);

        if (indexValue == currentBookEntryNumber) {
          continue;
        } else if ((indexValue) < currentBookEntryNumber) {
          return indexValue;
        } else if ((indexValue) > currentBookEntryNumber) {
          throw new LandRegistrationException(LandRegistrationException.Msg.BookEntryNumberAlreadyExists,
                                              currentBookEntryNumber);
        }
      }

      return indexValue;
    }


    static internal int GetNextBookEntryNumberWithNoReuse(RecordingBook book) {
      int currentRecordNumber = GetLastBookEntryNumber(book);

      if (currentRecordNumber > 0) {
        return currentRecordNumber + 1;
      } else if (currentRecordNumber == 0 && book.UsePerpetualNumbering) {
        return book.StartRecordingIndex;
      } else if (currentRecordNumber == 0 && !book.UsePerpetualNumbering) {
        return 1;
      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    static internal int GetBookTotalSheets(RecordingBook book) {
      string sql = "SELECT SheetsCount FROM vwLRSPhysicalBooksStats " +
                   $"WHERE PhysicalBookId = {book.Id}";

      return DataReader.GetScalar<int>(DataOperation.Parse(sql));
    }


    static internal RecordingBook GetOpenedBook(RecorderOffice office, RecordingSection recordingSection) {
      string sql = "SELECT * FROM LRSPhysicalBooks " +
                   $"WHERE (RecorderOfficeId = {office.Id} AND " +
                   $"RecordingSectionId = {recordingSection.Id} AND BookStatus = 'O')";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<RecordingBook>(op);
    }


    static public FixedList<RecorderOffice> GetRecorderOffices(RecordingSection sectionType) {
      string sql = "SELECT DISTINCT Contacts.* FROM LRSPhysicalBooks INNER JOIN Contacts" +
                   " ON LRSPhysicalBooks.RecorderOfficeId = Contacts.ContactId" +
                   " WHERE LRSPhysicalBooks.RecordingSectionId = " + sectionType.Id +
                   " ORDER BY Contacts.NickName";

      return DataReader.GetFixedList<RecorderOffice>(DataOperation.Parse(sql));
    }


    static internal FixedList<BookEntry> GetBookEntriesForLandRecord(RecordingDocument landRecord) {
      string sql = $"SELECT * FROM LRSPhysicalRecordings " +
                   $"WHERE MainDocumentId = {landRecord.Id} AND RecordingStatus <> 'X' " +
                   $"ORDER BY PhysicalRecordingId";

      return DataReader.GetFixedList<BookEntry>(DataOperation.Parse(sql));
    }


    static public FixedList<RecordingBook> GetRecordingBooksInSection(RecorderOffice recorderOffice,
                                                                      RecordingSection sectionType,
                                                                      string keywords = "") {
      string filter = "RecorderOfficeId = " + recorderOffice.Id.ToString() + " AND " +
                      "RecordingSectionId = " + sectionType.Id.ToString();

      if (!String.IsNullOrWhiteSpace(keywords)) {
        filter += " AND " + SearchExpression.ParseAndLike("BookKeywords", keywords);
      }

      string sql = "SELECT * FROM LRSPhysicalBooks " +
                   $"WHERE {filter} " +
                   $"ORDER BY BookNo, BookAsText";

      return DataReader.GetFixedList<RecordingBook>(DataOperation.Parse(sql));
    }


    static public FixedList<BookEntry> GetRecordingBookEntries(RecordingBook recordingBook) {
      var operation = DataOperation.Parse("qryLRSPhysicalBookRecordings", recordingBook.Id);

      return DataReader.GetFixedList<BookEntry>(operation);
    }


    static public DataRow GetRecordingWithBookEntryNumber(RecordingBook recordingBook,
                                                          string bookEntryNumber) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingWithRecordingNumber",
                                                       recordingBook.Id, bookEntryNumber));
    }


    static internal BookEntry FindRecordingBookEntry(RecordingBook recordingBook, string filter) {
      string sql = "SELECT * FROM LRSPhysicalRecordings WHERE " +
                   $"(PhysicalBookId = {recordingBook.Id} AND RecordingStatus <> 'X')";

      if (!String.IsNullOrWhiteSpace(filter)) {
        sql += $" AND {filter}";
      }

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<BookEntry>(op, BookEntry.Empty);
    }


    static internal void WriteBookEntry(BookEntry o) {
      Assertion.Require(o.LandRecord.Id > 0,
                       "Wrong data for book entry. LandRecord was missed.");

      var op = DataOperation.Parse("writeLRSPhysicalRecording", o.Id, o.UID, o.RecordingBook.Id,
                                   o.LandRecord.Id, o.Number, o.AsText, o.ExtendedData.GetJson().ToString(),
                                   o.Keywords, o.RecordedBy.Id, o.RecordingTime,
                                   (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingBook(RecordingBook o) {
      var op = DataOperation.Parse("writeLRSPhysicalBook", o.Id, o.UID, o.RecorderOffice.Id, o.RecordingSection.Id,
                                   o.BookNumber, o.AsText, o.ExtensionData.ToString(), o.Keywords,
                                   o.StartRecordingIndex, o.EndRecordingIndex, (char) o.Status,
                                   string.Empty);

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingDocument(RecordingDocument o) {
      var op = DataOperation.Parse("writeLRSDocument", o.Id, o.GUID, o.Instrument.Id, o.DocumentType.Id, -1, o.UID,
                                   o.ImagingControlID, o.Notes, o.AsText, o.ExtensionData.GetJson(o).ToString(),
                                   o.Keywords, o.PresentationTime, o.AuthorizationTime,
                                   o.IssuePlace.Id, o.IssueOffice.Id, o.IssuedBy.Id, o.IssueDate,
                                   o.SheetsCount, (char) o.Status, o.PostedBy.Id, o.PostingTime,
                                   o.Security.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class RecordingBooksData

} // namespace Empiria.Land.Data
