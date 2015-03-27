using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;

using Empiria.Security;
using Empiria.WebServices;
using Empiria.WebAPI;

namespace Empiria.Land.WebAPI {

  public class SecurityController : WebApiController {

    #region Public APIs

    [HttpPost, AllowAnonymous]
    [Route("v0/security/login")]
    public object Login(LoginModel login) {
      base.AssertHeader("User-Agent");
      base.AssertValue(login, "login");
      base.AssertValidModel();

      try {
        EmpiriaPrincipal principal = AuthenticationHttpModule.Authenticate(login.api_key,
                                                                           login.user_name,
                                                                           login.password);
        Assertion.AssertObject(principal, "principal");

        return principal.ToOAuth();
      } catch (SecurityException innerEx) {
        throw WebApiException(HttpErrorCode.Unauthorized, innerEx);
      } catch (Exception innerEx) {
        throw WebApiException(HttpErrorCode.InternalServerError,
                          new EmpiriaWebApiException(EmpiriaWebApiException.Msg.LoginFails, innerEx));
      }
    }

    [HttpPost, AllowAnonymous]
    [Route("v0/security/changepassword")]
    public void ChangePassword(LoginModel login) {
      base.AssertValue(login, "login");
      base.AssertValidModel();

      try {
        EmpiriaUser.ChangePassword(login.api_key, login.user_name, login.password);
      } catch (Exception innerEx) {
        throw WebApiException(HttpErrorCode.InternalServerError,
                          new EmpiriaWebApiException(EmpiriaWebApiException.Msg.LoginFails, innerEx));
      }
    }

    [HttpPost]
    [Route("v0/security/logout")]
    public void Logout() {
      //return new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);
    }

    [HttpGet]
    [Route("v0/security/testprotected")]
    public string TestProtected() {
      return "Protected API @ " + DateTime.Now.ToString() + ". Current authenticated user: " +
             HttpContext.Current.User.Identity.Name + " " + ExecutionServer.IsAuthenticated;
    }

    [HttpGet, AllowAnonymous]
    [Route("v0/security/testunprotected")]
    public string TestUnprotected() {
      return "Unprotected API @ " + DateTime.Now.ToString();
    }

    #endregion Public APIs

  }  // class SecurityController

}  // namespace Empiria.Land.WebAPI
