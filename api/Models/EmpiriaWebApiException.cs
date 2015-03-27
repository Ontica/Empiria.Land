/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Http/REST WebAPI               *
*  Namespace : Empiria.WebAPI                                 Assembly : Empiria.Land.Registration           *
*  Type      : LandRegistrationException                      Pattern  : Empiria Exception Class             *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : The exception that is thrown when a problem occurs in Empiria Web API Framework.              *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Reflection;

namespace Empiria.WebAPI {

  /// <summary>The exception that is thrown when a problem occurs in Empiria Web API Framework.</summary>
  [Serializable]
  public sealed class EmpiriaWebApiException : EmpiriaException {

    public enum Msg {
      LoginFails
    }

    static private string resourceBaseName = "Empiria.Land.WebAPI.Models.EmpiriaWebApiExceptionMsg";

    #region Constructors and parsers

    /// <summary>Initializes a new instance of EmpiriaWebApiException class with a specified error
    /// message.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public EmpiriaWebApiException(Msg message, params object[] args)
                                        : base(message.ToString(), GetMessage(message, args)) {

    }

    /// <summary>Initializes a new instance of EmpiriaWebApiException class with a specified error
    ///  message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">Used to indicate the description of the exception.</param>
    /// <param name="exception">This is the inner exception.</param>
    /// <param name="args">An optional array of objects to format into the exception message.</param>
    public EmpiriaWebApiException(Msg message, Exception exception, params object[] args)
                                        : base(message.ToString(), GetMessage(message, args), exception) {
    }

    #endregion Constructors and parsers

    #region Private methods

    static private string GetMessage(Msg message, params object[] args) {
      return GetResourceMessage(message.ToString(), resourceBaseName, Assembly.GetExecutingAssembly(), args);
    }

    #endregion Private methods

  } // class EmpiriaWebApiException

} // namespace Empiria.WebAPI
