﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSValidator                                   Pattern  : Validation Services Static Class    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Static class that provides Land Registration System validation methods.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.DataTypes.Time;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Static class that provides Land Registration System validation methods.</summary>
  static public class LRSValidator {

    #region Public methods

    static public LandRegistrationException ValidateRecordingActAsComplete(RecordingAct recordingAct) {
      if (recordingAct.RecordingActType.RecordingRule.EditAppraisalAmount &&
          recordingAct.ExtensionData.AppraisalAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyAppraisalAmount);
      }
      if (recordingAct.RecordingActType.RecordingRule.EditOperationAmount &&
          recordingAct.ExtensionData.OperationAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyOperationAmount);
      }
      // Parties Validation
      if (recordingAct.RecordingActType.RecordingRule.AllowNoParties) {
        return null;
      }
      if (!recordingAct.IsAnnotation) {
        FixedList<RecordingActParty> parties = PartyData.GetInvolvedDomainParties(recordingAct);
        if (parties.Count == 0) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithoutOwnerParties);
        }
      } else {
        FixedList<RecordingActParty> parties = PartyData.GetRecordingPartyList(recordingAct);
        if (parties.Count == 0) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithoutOwnerParties);
        }
      }
      return null;
    }

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

  } // class LRSValidator

} // namespace Empiria.Land.Registration
