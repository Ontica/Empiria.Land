/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LandRegistrationException                      Pattern  : Exception Class                     *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : The exception that is thrown when a problem occurs in Empiria Government Land Registration    *
*              System.                                                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Reflection;

namespace Empiria.Land.Registration {

  /// <summary>The exception that is thrown when a problem occurs in Empiria Government Land
  /// Registration System.</summary>
  [Serializable]
  public sealed class LandRegistrationException : EmpiriaException {

    public enum Msg {
      AnnotationNotBelongsToRecording,
      AskIfAppendPropertyToExistingAnnotation,
      AttachmentFolderNotFound,
      BookEntryNotBelongsToRecordingBook,
      BookEntryNumberAlreadyExists,
      CantAlterAnnotationOnClosedRecording,
      CantAlterClosedAnnotation,
      CantAlterClosedRecordingAct,
      CantAlterRecordingActOnClosedRecording,
      CantReEntryTransaction,
      EmptyAppraisalAmount,
      EmptyOperationAmount,
      InvalidImagePosition,
      InvalidPaymentOrderStatus,
      InvalidBookEntryAuthorizationDate,
      InvalidBookEntryImageRange,
      InvalidBookEntryNumber,
      InvalidBookEntryPresentationTime,
      InvalidTransactionTrack,
      MistakeInRecordingRuleConfig,
      NextStatusCantBeEndPoint,
      NotReadyForElectronicalDelivery,
      NotSavedRecording,
      NotSavedRecordingAct,
      OrphanRecordingActIfPropertyDeleted,
      OtherAnnotationWithEqualNumberExistsInBook,
      PrecendentPresentationTimeIsAfterThisTransactionDate,
      PropertyAlreadyExistsOnRecordingAct,
      PropertyDoesNotHaveAnyRecordingActs,
      PropertyHasAnnotations,
      PropertyIsReferencedInOtherDomainActs,
      PropertyNotBelongsToRecordingAct,
      RecorderOfficialOutOfPeriod,
      RecordingActNotBelongsToRecording,
      RecordingActWithOnlyOneOwnerParty,
      RecordingActWithoutOwnerParties,
      UnrecognizedRecordingBookType,
      VolumeRecordingBooksCantHaveChilds,
    }

    static private string resourceBaseName =
                    "Empiria.Land.Core.Commons.LandRegistrationExceptionMsg";

    #region Constructors and parsers

    /// <summary>Initializes a new instance of LandRegistrationException class with a specified error
    /// message.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public LandRegistrationException(Msg message, params object[] args) :
      base(message.ToString(), GetMessage(message, args)) {

    }

    /// <summary>Initializes a new instance of LandRegistrationException class with a specified error
    ///  message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="innerException">This is the inner exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public LandRegistrationException(Msg message, Exception innerException, params object[] args)
      : base(message.ToString(), GetMessage(message, args), innerException) {

    }

    #endregion Constructors and parsers

    #region Private methods

    static private string GetMessage(Msg message, params object[] args) {
      return GetResourceMessage(message.ToString(), resourceBaseName, Assembly.GetExecutingAssembly(), args);
    }

    #endregion Private methods

  } // class LandRegistrationException

} // namespace Empiria.Land.Registration
