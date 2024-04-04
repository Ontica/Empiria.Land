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

namespace Empiria.Land.Registration {

  /// <summary>Provides validation services for recording books.</summary>
  static public class RecordingBookValidator {

    #region Public methods


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
