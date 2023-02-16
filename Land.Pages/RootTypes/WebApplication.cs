/* Empiria Extensions Framework ******************************************************************************
*                                                                                                            *
*  Solution  : Empiria Extensions Framework                     System   : Web Presentation Services         *
*  Namespace : Empiria.Presentation.Web                         Assembly : Empiria.Presentation.Web.dll      *
*  Type      : WebApplication                                   Pattern  : Standard Class                    *
*  Version   : 6.8                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Provides the methods from the current web application.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading;
using System.Web;
using System.Web.Security;

using Empiria.Security;

namespace Empiria.Presentation.Web {

  /// <summary>Provides the methods from the current web application.</summary>
  public abstract class WebApplication : HttpApplication {

    #region Constructors and parsers

    protected WebApplication() {
      // no-op
    }

    #endregion Constructors and parsers

    #region Public methods

    public void OnAuthenticateRequest(object sender, EventArgs e) {
      HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

      if (authCookie != null) {
        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

        authTicket = FormsAuthentication.RenewTicketIfOld(authTicket);

        EmpiriaPrincipal principal = AuthenticationService.Authenticate(authTicket.UserData);

        Thread.CurrentPrincipal = principal;
        this.Context.User = principal;
      }
    }

    public void OnEnd(object sender, EventArgs e) {
      //no-op
    }

    public void OnError(object sender, EventArgs e) {
      //no-op
    }

    public void OnPreSendRequestHeaders(object sender, EventArgs e) {
      // no-op
    }

    public void OnSessionEnd(object sender, EventArgs e) {
      try {
        if (ExecutionServer.IsAuthenticated) {
          ExecutionServer.CurrentPrincipal.CloseSession();
        }
        if (Request.IsAuthenticated) {
          FormsAuthentication.SignOut();
        }

        if (Response.IsClientConnected) {
          Response.Redirect("http://www.ontica.org");
        }
      } catch {
        // no-op
      }
    }

    public void OnSessionStart(object sender, EventArgs e) {
      Session.Timeout = 20;
    }

    public void OnStart(object sender, EventArgs e) {
      // no-op
    }

    #endregion Public methods

  } // class WebApplication

} // namespace Empiria.Presentation.Web
