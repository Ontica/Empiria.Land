/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Service provider                        *
*  Type     : RecordableSubjectRegistrationHelper        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs antecedent recordable subjects registration if needed by a CertificateRequestCommand. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Transactions.CertificateRequests {

  /// <summary>Performs antecedent recordable subjects registration
  /// if it is needed by a CertificateRequestCommand.</summary>
  internal class RecordableSubjectRegistrationHelper {

    private readonly CertificateRequestCommand _command;

    internal RecordableSubjectRegistrationHelper(CertificateRequestCommand command) {
      _command = command;
    }


    internal Resource GetRecordableSubject() {
      EnsureHasRecordableSubject();

      return Resource.ParseGuid(_command.Payload.RecordableSubjectUID);
    }


    private void EnsureHasRecordableSubject() {
      if (!String.IsNullOrWhiteSpace(_command.Payload.RecordableSubjectUID)) {
        return;
      }

      EnsureHasBookEntry(_command);

      var bookEntry = BookEntry.Parse(_command.Payload.BookEntryUID);

      Resource recordableSubject = CreateRecordableSubject(_command.Type.Rules().SubjectType);

      recordableSubject.Save();

      var precedentAct = new InformationAct(RecordingActType.Empty, bookEntry.LandRecord,
                                            recordableSubject, bookEntry);
      precedentAct.Save();

      bookEntry.Refresh();

      _command.Payload.RecordableSubjectUID = recordableSubject.GUID;
    }


    private void EnsureHasBookEntry(CertificateRequestCommand command) {
      if (!String.IsNullOrWhiteSpace(command.Payload.BookEntryUID)) {
        return;
      }

      string bookEntryNo = command.Payload.BookEntryNo;

      var book = RecordingBook.Parse(command.Payload.RecordingBookUID);

      BookEntry entry = book.TryGetBookEntry(bookEntryNo);

      if (entry == null) {
        entry = RegistrationEngine.CreatePrecedentBookEntry(book, bookEntryNo,
                                                            command.Payload.PresentationTime,
                                                            command.Payload.AuthorizationDate);
      }

      command.Payload.BookEntryUID = entry.UID;
    }


    private Resource CreateRecordableSubject(RecordableSubjectType subjectType) {
      switch (subjectType) {
        case RecordableSubjectType.Association:
          return new Association();

        case RecordableSubjectType.RealEstate:
          return new RealEstate(new RealEstateExtData());

        case RecordableSubjectType.NoProperty:
          return new NoPropertyResource();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled subjectType '{subjectType}'.");
      }
    }

  }  // class RecordableSubjectRegistrationHelper

}  // namespace Empiria.Land.Transactions.CertificateRequests
