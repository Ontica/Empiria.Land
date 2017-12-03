﻿/* Empiria Extensions Framework ******************************************************************************
*                                                                                                            *
*  Solution  : Empiria Extensions Framework                     System   : Empiria Microservices             *
*  Namespace : Empiria.Microservices                            Assembly : Empiria.Microservices.dll         *
*  Type      : LoginModel                                       Pattern  : Information Holder                *
*  Version   : 1.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Holds information to log in a user into the web api system.                                   *
*                                                                                                            *
********************************** Copyright(c) 2016-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using Empiria.Security;

namespace Empiria.Land.WebApi.Citys {

  /// <summary>Holds information to log in a user into the web api system.
  /// This model was moved to microservices, so it must be deprecated from here.</summary>
  public class CitysLoginModel {

    #region Properties

    public string api_key {
      get;
      set;
    }

    public string user_name {
      get;
      set;
    }

    public string password {
      get;
      set;
    }

    #endregion Properties

    #region Methods

    public void AssertValid() {
      Assertion.AssertObject(api_key, "api_key");
      Assertion.AssertObject(user_name, "user_name");
      Assertion.AssertObject(password, "password");
    }

    static public object ToOAuth(EmpiriaPrincipal principal) {

      return new {
        access_token = principal.Session.Token,
        token_type = principal.Session.TokenType,
        expires_in = principal.Session.ExpiresIn,
        refresh_token = principal.Session.RefreshToken,
        user = new {
          uid = principal.Identity.User.Id,
          username = principal.Identity.User.UserName,
          email = principal.Identity.User.EMail
        }
      };
    }

    #endregion Methods

  }  // class LoginModel

} // namespace Empiria.Microservices
