/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSValidator                                   Pattern  : Validation Services Static Class    *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Static class that provides Land Registration System validation methods.                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;


using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Static class that provides Land Registration System validation methods.</summary>
  static public class LRSValidator {

    #region Public methods

    static public int GetOverlappingRecordingsCount(RecordingBook recordingBook, Recording recording,
                                                    int imageStartIndex, int imageEndIndex) {
      FixedList<Recording> list = RecordingBooksData.GetRecordingsOnImageRangeList(recordingBook,
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

    static public LandRegistrationException ValidateDeleteRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.Targets.Count > 1) {
        return new LandRegistrationException(LandRegistrationException.Msg.RecordingActHasTwoOrMoreProperties);
      }
      Resource resource = recordingAct.Targets[0].Resource;
      FixedList<RecordingAct> domainActs = resource.GetRecordingActsTract();
      if ((domainActs.Count > 1) && (resource.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyIsReferencedInOtherDomainActs,
                                             resource.UID);
      }
      if (domainActs.Count == 1 && resource.Annotations.Count > 0) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyHasAnnotations, resource.UID);
      }
      return null;
    }

    static public LandRegistrationException ValidateDeleteRecordingActProperty(RecordingAct recordingAct,
                                                                               Resource resource) {
      FixedList<RecordingAct> domainActs = resource.GetRecordingActsTract();
      if ((domainActs.Count > 1) && (resource.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyIsReferencedInOtherDomainActs,
                                             resource.UID);
      }
      if ((domainActs.Count == 1) && (!resource.FirstRecordingAct.Equals(recordingAct))) {
        return new LandRegistrationException(LandRegistrationException.Msg.OrphanRecordingActIfPropertyDeleted,
                                             resource.UID);
      }
      if ((domainActs.Count == 1) && (resource.Annotations.Count > 0)) {
        return new LandRegistrationException(LandRegistrationException.Msg.PropertyHasAnnotations, resource.UID);
      }
      return null;
    }

    static public LandRegistrationException ValidateRecordingActAsComplete(RecordingAct recordingAct) {
      if (!recordingAct.RecordingActType.BlockAllFields &&
           recordingAct.ExtensionData.AppraisalAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyAppraisalAmount);
      }
      if (recordingAct.RecordingActType.UseOperationAmount &&
          recordingAct.ExtensionData.OperationAmount.Equals(Money.Empty)) {
        return new LandRegistrationException(LandRegistrationException.Msg.EmptyOperationAmount);
      }
      // Parties Validation
      if (recordingAct.RecordingActType.AllowsEmptyParties) {
        return null;
      }
      if (!recordingAct.IsAnnotation) {
        FixedList<RecordingActParty> parties = RecordingActParty.GetDomainPartyList(recordingAct);
        if (parties.Count == 0) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithoutOwnerParties);
        }
        if (parties.CountAll((x) => x.OwnershipMode == OwnershipMode.Coowner) == 1) {
          return new LandRegistrationException(LandRegistrationException.Msg.RecordingActWithOnlyOneOwnerParty);
        }
      } else {
        FixedList<RecordingActParty> parties = RecordingActParty.GetList(recordingAct);
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

    static public LandRegistrationException ValidateRecordingNumber(RecordingBook recordingBook, Recording recording,
                                                                    int recordingNumber, string recordingSubNumber,
                                                                    string bisSuffixRecordingNumber,
                                                                    int imageStartIndex, int imageEndIndex) {
      string recordingNo = recordingBook.BuildRecordingNumber(recordingNumber, recordingSubNumber, bisSuffixRecordingNumber);
      string filter = "RecordingId <> " + recording.Id + " AND RecordingNumber = '" + recordingNo + "'";
      Recording findResult = RecordingBooksData.FindRecording(recordingBook, filter);

      if (!findResult.IsEmptyInstance) {
        return new LandRegistrationException(LandRegistrationException.Msg.RecordingNumberAlreadyExists, recordingNo);
      }

      int imageCount = 0; // OOJJOO recordingBook.ImagingFilesFolder.FilesCount;

      if ((imageStartIndex == 0) || (imageEndIndex == 0) ||
          (imageStartIndex > imageEndIndex) || (imageEndIndex > imageCount)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingImageRange,
                                             recordingBook.AsText, imageStartIndex, imageEndIndex, imageCount);
      }
      return null;
    }

    static public LandRegistrationException ValidateRecordingDates(RecordingBook recordingBook, Recording recording,
                                                                   DateTime presentationTime, DateTime authorizationDate) {
      if (!recordingBook.RecordingsControlTimePeriod.IsInRange(presentationTime)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingPresentationTime,
                                             recordingBook.RecordingsControlTimePeriod.StartTime.ToString("dd/MMM/yyyy"),
                                             recordingBook.RecordingsControlTimePeriod.EndTime.ToString("dd/MMM/yyyy"), recordingBook.AsText);
      }
      if (!recordingBook.RecordingsControlTimePeriod.IsInRange(authorizationDate)) {
        return new LandRegistrationException(LandRegistrationException.Msg.InvalidRecordingAuthorizationDate,
                                             recordingBook.RecordingsControlTimePeriod.StartTime.ToString("dd/MMM/yyyy"),
                                             recordingBook.RecordingsControlTimePeriod.EndTime.ToString("dd/MMM/yyyy"), recordingBook.AsText);
      }
      return null;
    }

    #endregion Public methods

  } // class LRSValidator

} // namespace Empiria.Land.Registration
