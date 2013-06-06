/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LandRegistrationException                      Pattern  : Empiria Exception Class             *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : The exception that is thrown when a problem occurs in Empiria® Government Land Registration   *
*              System.                                                                                       *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Reflection;

namespace Empiria.Government.LandRegistration {

  /// <summary>The exception that is thrown when a problem occurs in Empiria® Government Land 
  /// Registration System.</summary>
  [Serializable]
  public sealed class LandRegistrationException : EmpiriaException {

    public enum Msg {
      AnnotationNotBelongsToRecording,
      AttachmentFolderNotFound,
      AskIfAppendPropertyToExistingAnnotation,
      CantAlterAnnotationOnClosedRecording,
      CantAlterClosedAnnotation,
      CantAlterClosedRecordingAct,
      CantAlterRecordingActOnClosedRecording,
      CantReEntryTransaction,
      EmptyAppraisalAmount,
      EmptyFirstKnownOwner,
      EmptyOperationAmount,
      RecordingActWithoutOwnerParties,
      RecordingActWithOnlyOneOwnerParty,
      InvalidRecordingAuthorizationDate,
      InvalidRecordingImageRange,
      InvalidRecordingPresentationTime,
      InvalidTransactionTrack,
      InvalidPaymentOrderStatus,
      NextStatusCantBeEndPoint,
      NotSavedRecording,
      NotSavedRecordingAct,
      OrphanRecordingActIfPropertyDeleted,
      OtherAnnotationWithEqualNumberExistsInBook,
      PropertyAlreadyExistsOnRecordingAct,
      PropertyHasAnnotations,
      PropertyIsReferencedInOtherDomainActs,
      PropertyNotBelongsToRecordingAct,
      RecorderOfficialOutOfPeriod,
      RecordingActHasTwoOrMoreProperties,
      RecordingActNotBelongsToRecording,
      RecordingNotBelongsToRecordingBook,
      RecordingNotBisNumberNotFound,
      RecordingNumberAlreadyExists,
      RecorderOfficeRootRecordingBookAlreadyExists,
      UnrecognizedRecordingBookType,
      VolumeRecordingBooksCantHaveChilds,
      InvalidRecordingNumber,
      InvalidImagePosition,
    }

    static private string resourceBaseName =
                    "Empiria.Government.LandRegistration.RootTypes.LandRegistrationExceptionMsg";

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

} // namespace Empiria.Government.LandRegistration
