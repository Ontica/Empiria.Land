/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Global ASP .NET Class                   *
*  Type     : Global                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Derived ASP WebApplication implementation.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Presentation.Web;

namespace Empiria.Land.Pages {

  /// <summary>Derived ASP WebApplication implementation.</summary>
  public class Global : WebApplication {

    public Global() {

    }

    protected virtual void Application_PreSendRequestHeaders(Object sender, EventArgs e) {
      base.OnPreSendRequestHeaders(sender, e);
    }

    protected virtual void Application_Start(Object sender, EventArgs e) {
      base.OnStart(sender, e);

      Empiria.WebApi.WebApiApplication.Register();
    }

    protected virtual void Application_AuthenticateRequest(Object sender, EventArgs e) {
      base.OnAuthenticateRequest(sender, e);
    }

    protected virtual void Application_End(Object sender, EventArgs e) {
      base.OnEnd(sender, e);
    }

    protected virtual void Application_Error(Object sender, EventArgs e) {
      base.OnError(sender, e);
    }

    protected virtual void Session_End(Object sender, EventArgs e) {
      base.OnSessionEnd(sender, e);
    }

    protected virtual void Session_Start(Object sender, EventArgs e) {
      base.OnSessionStart(sender, e);
    }

  } // class Global

} // namespace Empiria.Land.Pages
