/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : LandDocumentationException                     Pattern  : Exception Class                     *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : The exception that is thrown when a problem occurs in Empiria Land documentation services.    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Reflection;

namespace Empiria.Land.Documentation {

  /// <summary>The exception that is thrown when a problem occurs in Empiria Land
  /// documentation services.</summary>
  [Serializable]
  public sealed class LandDocumentationException : EmpiriaException {

    public enum Msg {
      AttachmentFolderNotFound,
      DocumentAlreadyDigitalized,
      DocumentForFileNameNotFound,
      FileNameBadFormed,
      FileNotExists,
      FolderNameBadFormed,
      ImagingFolderNotExists,
      InvalidImagePosition,
      InvalidRecordingImageRange,
      RecordingBookForFolderNameNotFound,
    }

    static private string resourceBaseName =
                    "Empiria.Land.Documentation.RootTypes.LandDocumentationExceptionMsg";

    #region Constructors and parsers

    /// <summary>Initializes a new instance of LandDocumentationException class with a specified error
    /// message.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public LandDocumentationException(Msg message, params object[] args) :
                                      base(message.ToString(), GetMessage(message, args)) {

    }

    /// <summary>Initializes a new instance of LandDocumentationException class with a specified error
    ///  message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="innerException">This is the inner exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public LandDocumentationException(Msg message, Exception innerException, params object[] args) :
                                      base(message.ToString(), GetMessage(message, args), innerException) {

    }

    #endregion Constructors and parsers

    #region Private methods

    static private string GetMessage(Msg message, params object[] args) {
      return GetResourceMessage(message.ToString(), resourceBaseName, Assembly.GetExecutingAssembly(), args);
    }

    #endregion Private methods

  } // class LandDocumentationException

} // namespace Empiria.Land.Documentation
