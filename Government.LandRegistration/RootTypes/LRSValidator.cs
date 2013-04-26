/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                System   : Land Registration System              *
*  Namespace : Empiria.Government.LandRegistration          Assembly : Empiria.Government.LandRegistration   *
*  Type      : LRSValidator                                 Pattern  : Validation Services Static Class      *
*  Date      : 25/Jun/2013                                  Version  : 5.1     License: CC BY-NC-SA 3.0      *
*                                                                                                            *
*  Summary   : Static class that provides Land Registration System validation methods.                       *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;


using Empiria.Government.LandRegistration.Data;

namespace Empiria.Government.LandRegistration {

  /// <summary>Static class that provides Land Registration System validation methods.</summary>
  static public class LRSValidator {

    #region Public methods

    static public int FindAnnotationId(RecordingBook recordingBook, RecordingActType annotationType,
                                       string annotationNumber,
                                       int imageStartIndex, int imageEndIndex,
                                       DateTime presentationTime, DateTime authorizationDate,
                                       Contact authorizedBy, Property toAppendProperty) {
      Recording annotation = recordingBook.FindRecording(annotationNumber);
      if (annotation == null) {
        return 0;
      }
      if (!annotation.RecordingActs[0].RecordingActType.Equals(annotationType)) {
        return -1;
      }
      if (annotation.StartImageIndex != imageStartIndex || annotation.EndImageIndex != imageEndIndex) {
        return -1;
      }
      if (annotation.PresentationTime != presentationTime || annotation.AuthorizedTime != authorizationDate) {
        return -1;
      }
      if (!annotation.AuthorizedBy.Equals(authorizedBy)) {
        return -1;
      }
      if (annotation.RecordingActs[0].PropertiesEvents.Contains((x) => x.Property.Equals(toAppendProperty))) {
        return -1;
      }
      return annotation.RecordingActs[0].Id;
    }

    static public int GetOverlappingRecordingsCount(RecordingBook recordingBook, Recording recording,
                                                    int imageStartIndex, int imageEndIndex) {
      ObjectList<Recording> list = RecordingBooksData.GetRecordingsOnImageRangeList(recordingBook,
                                                                                    imageStartIndex, imageEndIndex);
      if (list.Count == 0) {
        return 0;
      }
      int counter = 0;
      for (int i = 0; i < list.Count; i++) {
        if (list[i].Equals(recording)) {
          continue;
        } else if ((list[i].StartImageIndex == imageStartIndex) &&
                   (list[i].EndImageIndex == imageEndIndex) &&
                   (imageStartIndex == imageEndIndex)) {
          continue;
        } else if (list[i].EndImageIndex == imageStartIndex) {
          continue;
        } else {
          counter++;
        }
      }
      return counter;
    }

    static public LandRegistrationException ValidateAnnotationNumber(RecordingBook recordingBook, Recording annotation,
                                                                    RecordingActType annotationType,
                                                                    string annotationNumber, int imageStartIndex, int imageEndIndex,
                                                                    DateTime presentationTime, DateTime authorizationDate,
                                                                    Contact authorizedBy, Property toAppendProperty) {

      int imageCount = recordingBook.ImagingFilesFolder.FilesCount;

      if ((imageStartIndex <= 0) || (imageEndIndex) <= 0 ||
          (imageStartIndex > imageEndIndex) || (imageEndIndex > imageCount)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingImageRange,
                                             recordingBook.FullName, imageStartIndex, imageEndIndex, imageCount);
      }

      int annotationId = FindAnnotationId(recordingBook, annotationType, annotationNumber, imageStartIndex, imageEndIndex,
                                          presentationTime, authorizationDate, authorizedBy, toAppendProperty);
      if (annotationId == -1) {
        return new LandRegistrationException(LandRegistrationException.Msg.OtherAnnotationWithEqualNumberExistsInBook,
                                             recordingBook.FullName, annotationNumber, toAppendProperty.TractKey);
      }

      return null;
    }

    static public LandRegistrationException ValidateDeleteRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.PropertiesEvents.Count > 1) {
        return new LandRegistrationException(LandRegistrationException.Msg.RecordingActHasTwoOrMoreProperties);
      }
      Property property = recordingAct.PropertiesEvents[0].Property;
      ObjectList<RecordingAct> domainActs = property.GetRecordingActsTract();
      if ((domainActs.Count > 1) && (property.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyIsReferencedInOtherDomainActs,
                                             property.TractKey);
      }
      if (domainActs.Count == 1 && property.Annotations.Count > 0) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyHasAnnotations, property.TractKey);
      }
      return null;
    }

    static public LandRegistrationException ValidateDeleteRecordingActProperty(RecordingAct recordingAct, Property property) {
      ObjectList<RecordingAct> domainActs = property.GetRecordingActsTract();
      if ((domainActs.Count > 1) && (property.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyIsReferencedInOtherDomainActs,
                                             property.TractKey);
      }
      if ((domainActs.Count == 1) && (!property.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.OrphanRecordingActIfPropertyDeleted,
                                             property.TractKey);
      }
      if ((domainActs.Count == 1) && (property.Annotations.Count > 0)) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyHasAnnotations, property.TractKey);
      }
      return null;
    }

    //static public LandRegistrationException ValidateNextTransactionStatus(LRSTransaction transaction, TransactionStatus nextStatus) {
    //  string s = transaction.ValidateStatusChange(nextStatus);
    //  return (s == String.Empty) ? null : s;
    //}

    static public LandRegistrationException ValidateRecordingActAsComplete(RecordingAct recordingAct) {
      if (recordingAct.RecordingActType.UseFirstPropertyOwner && recordingAct.IsFirstRecordingAct() && !recordingAct.HasFirstKnownOwner) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyFirstKnownOwner);
      }
      if (!recordingAct.RecordingActType.BlockAllFields && recordingAct.AppraisalAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyAppraisalAmount);
      }
      if (recordingAct.RecordingActType.UseOperationAmount && recordingAct.OperationAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyOperationAmount);
      }
      // Parties Validation
      if (recordingAct.RecordingActType.AllowEmptyParties) {
        return null;
      }
      if (!recordingAct.IsAnnotation) {
        ObjectList<RecordingActParty> parties = RecordingActParty.GetDomainPartyList(recordingAct);
        if (parties.Count == 0) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithoutOwnerParties);
        }
        if (parties.CountAll((x) => x.OwnershipMode == OwnershipMode.Coowner) == 1) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithOnlyOneOwnerParty);
        }
      } else {
        ObjectList<RecordingActParty> parties = RecordingActParty.GetList(recordingAct);
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
      ObjectList<Person> officers = office.GetRecorderOfficials(new TimePeriod(autorizationDate, autorizationDate));

      if (!officers.Contains(authorizedBy)) {
        return new LandRegistrationException(LandRegistrationException.Msg.RecorderOfficialOutOfPeriod,
                                             authorizedBy.FullName, office.FullName, autorizationDate.ToString("dd/MMM/yyyy"));
      }
      return null;
    }

    static public LandRegistrationException ValidateRecordingNumber(RecordingBook recordingBook, Recording recording,
                                                                    int recordingNumber, bool useBisRecordingNumber,
                                                                    int imageStartIndex, int imageEndIndex) {
      if (!useBisRecordingNumber) {
        DataRow row = RecordingBooksData.GetRecordingWithRecordingNumber(recordingBook, Recording.FormatNumber(recordingNumber, false));
        if (row != null && (recording.Id != (int) row["RecordingId"])) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingNumberAlreadyExists,
                                               Recording.FormatNumber(recordingNumber, false));
        }
      } else {
        DataRow row = RecordingBooksData.GetRecordingWithRecordingNumber(recordingBook, Recording.FormatNumber(recordingNumber, false));
        if (row == null) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingNotBisNumberNotFound,
                                               Recording.FormatNumber(recordingNumber, false));
        }
        row = RecordingBooksData.GetRecordingWithRecordingNumber(recordingBook, Recording.FormatNumber(recordingNumber, true));
        if (row != null && (recording.Id != (int) row["RecordingId"])) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingNumberAlreadyExists,
                                               Recording.FormatNumber(recordingNumber, true));
        }
      }
      int imageCount = recordingBook.ImagingFilesFolder.FilesCount;

      if ((imageStartIndex <= 0) || (imageEndIndex) <= 0 ||
          (imageStartIndex > imageEndIndex) || (imageEndIndex > imageCount)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingImageRange,
                                             recordingBook.FullName, imageStartIndex, imageEndIndex, imageCount);
      }
      return null;
    }

    static public LandRegistrationException ValidateRecordingDates(RecordingBook recordingBook, Recording recording,
                                                                   DateTime presentationTime, DateTime authorizationDate) {
      if (!recordingBook.RecordingsControlTimePeriod.IsInRange(presentationTime)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingPresentationTime,
                                             recordingBook.RecordingsControlTimePeriod.FromDate.ToString("dd/MMM/yyyy"),
                                             recordingBook.RecordingsControlTimePeriod.ToDate.ToString("dd/MMM/yyyy"), recordingBook.FullName);
      }
      if (!recordingBook.RecordingsControlTimePeriod.IsInRange(authorizationDate)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingAuthorizationDate,
                                             recordingBook.RecordingsControlTimePeriod.FromDate.ToString("dd/MMM/yyyy"),
                                             recordingBook.RecordingsControlTimePeriod.ToDate.ToString("dd/MMM/yyyy"), recordingBook.FullName);
      }
      return null;
    }

    #endregion Public methods

  } // class LRSValidator

} // namespace Empiria.Government.LandRegistration