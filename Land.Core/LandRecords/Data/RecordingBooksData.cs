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
  static internal class RecordingBooksData {

    #region Methods


    static internal DataTable GetBookEntriesNumbers(RecordingBook book) {
      string sql = "SELECT RecordingNo FROM LRSPhysicalRecordings" +
                   $"WHERE PhysicalBookId = {book.Id} AND RecordingStatus <> 'X' " +
                   $"ORDER BY RecordingNo";

      var op = DataOperation.Parse(sql);

      return DataReader.GetDataTable(op);
    }


    static internal int GetLastBookEntryNumber(RecordingBook book) {
      string sql = "SELECT MAX(RecordingNo) FROM LRSPhysicalRecordings " +
                  $"WHERE PhysicalBookId = {book.Id} " +
                   "AND RecordingStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return int.Parse(DataReader.GetScalar(op, "0"));
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

      var op = DataOperation.Parse(sql);

      return DataReader.GetScalar<int>(op);
    }


    static internal RecordingBook GetOpenedBook(RecorderOffice office, RecordingSection recordingSection) {
      string sql = "SELECT * FROM LRSPhysicalBooks " +
                   $"WHERE (RecorderOfficeId = {office.Id} AND " +
                   $"RecordingSectionId = {recordingSection.Id} AND BookStatus = 'O')";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<RecordingBook>(op);
    }


    static internal FixedList<RecorderOffice> GetRecorderOffices(RecordingSection sectionType) {
      string sql = "SELECT DISTINCT Contacts.* FROM LRSPhysicalBooks INNER JOIN Contacts" +
                   " ON LRSPhysicalBooks.RecorderOfficeId = Contacts.ContactId" +
                   " WHERE LRSPhysicalBooks.RecordingSectionId = " + sectionType.Id +
                   " ORDER BY Contacts.NickName";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RecorderOffice>(op);
    }


    static internal FixedList<BookEntry> GetBookEntriesForLandRecord(LandRecord landRecord) {
      if (landRecord.IsEmptyInstance) {
        return new FixedList<BookEntry>();
      }

      string sql = $"SELECT * FROM LRSPhysicalRecordings " +
                   $"WHERE MainDocumentId = {landRecord.Id} AND RecordingStatus <> 'X' " +
                   $"ORDER BY PhysicalRecordingId";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<BookEntry>(op);
    }


    static internal FixedList<RecordingBook> GetRecordingBooksInSection(RecorderOffice recorderOffice,
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

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RecordingBook>(op);
    }


    static internal FixedList<BookEntry> GetRecordingBookEntries(RecordingBook recordingBook) {

      var sql = "SELECT * FROM LRSPhysicalRecordings " +
               $"WHERE PhysicalBookId = {recordingBook.Id} AND RecordingStatus <> 'X' " +
               $"ORDER BY RecordingNo, PhysicalRecordingId";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<BookEntry>(op);
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

    #endregion Methods

  } // class RecordingBooksData

} // namespace Empiria.Land.Data
