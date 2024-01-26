/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : RecordingBookValidator                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides validation services for recording books.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes.Time;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Provides validation services for recording books.</summary>
  static public class RecordingBookValidator {

    #region Public methods

    static public LandRegistrationException ValidateRecordingAuthorizer(RecordingBook recordingBook, Person authorizedBy,
                                                                        DateTime autorizationDate) {
      if (authorizedBy.IsEmptyInstance) {
        return null;
      }
      RecorderOffice office = recordingBook.RecorderOffice;
      FixedList<Person> officers = office.GetRecorderOfficials(new TimeFrame(autorizationDate, autorizationDate));

      if (!officers.Contains(authorizedBy)) {
        return new LandRegistrationException(LandRegistrationException.Msg.RecorderOfficialOutOfPeriod,
                                             authorizedBy.FullName, office.FullName, autorizationDate.ToString("dd/MMM/yyyy"));
      }
      return null;
    }

    static public LandRegistrationException ValidateBookEntryNumber(RecordingBook recordingBook, BookEntry bookEntry,
                                                                    string bookEntryNumberToValidate) {
      string formatted = RecordingBook.FormatBookEntryNumber(bookEntryNumberToValidate);

      string filter = "PhysicalRecordingId <> " + bookEntry.Id + " AND RecordingNo = '" + formatted + "'";

      BookEntry findResult = RecordingBooksData.FindRecordingBookEntry(recordingBook, filter);

      if (!findResult.IsEmptyInstance) {
        return new LandRegistrationException(LandRegistrationException.Msg.BookEntryNumberAlreadyExists, formatted);
      }

      return null;
    }

    static public LandRegistrationException ValidateRecordingDates(RecordingBook recordingBook,
                                                                   DateTime presentationTime, DateTime authorizationDate) {
      if (!recordingBook.BookEntriesControlTimePeriod.Includes(presentationTime)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidBookEntryPresentationTime,
                                             recordingBook.BookEntriesControlTimePeriod.StartTime.ToString("dd/MMM/yyyy"),
                                             recordingBook.BookEntriesControlTimePeriod.EndTime.ToString("dd/MMM/yyyy"), recordingBook.AsText);
      }

      if (!recordingBook.BookEntriesControlTimePeriod.Includes(authorizationDate)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidBookEntryAuthorizationDate,
                                             recordingBook.BookEntriesControlTimePeriod.StartTime.ToString("dd/MMM/yyyy"),
                                             recordingBook.BookEntriesControlTimePeriod.EndTime.ToString("dd/MMM/yyyy"), recordingBook.AsText);
      }

      return null;
    }

    #endregion Public methods

  } // class RecordingBookValidator

} // namespace Empiria.Land.Registration
