using System;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;

using Empiria.Security;
using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi {

  public class SecurityController : WebApiController {

    #region Public APIs

    [HttpPost, AllowAnonymous]
    [Route("api/v1/security/change-password")]
    public void ChangePassword(LoginModel login) {
      try {
        base.RequireBody(login);

        EmpiriaUser.ChangePassword(login.api_key, login.user_name, login.password);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch, HttpHead, HttpOptions]
    [AllowAnonymous]
    public void Http404ErrorHandler() {
      var e = new WebApiException(WebApiException.Msg.EndpointNotFound,
                                  base.Request.RequestUri.AbsoluteUri);

      throw base.CreateHttpException(HttpErrorCode.NotFound, e);
    }

    #region Login Controllers

    [HttpPost, AllowAnonymous]
    [Route("api/v1/security/login")]
    public SingleObjectModel Login(LoginModel login) {
      try {
        base.RequireHeader("User-Agent");
        base.RequireBody(login);

        EmpiriaPrincipal principal = AuthenticationHttpModule.Authenticate(login.api_key,
                                                                           login.user_name,
                                                                           login.password);
        Assertion.AssertObject(principal, "principal");

        return new SingleObjectModel(base.Request, LoginModel.ToOAuth(principal),
                                     "Empiria.Security.OAuthObject");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Login Controllers

    [HttpPost]
    [Route("api/v1/security/logout")]
    public void Logout() {
      try {
        throw new NotImplementedException();
        //AuthenticationHttpModule.Logout();
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

  }  // class SecurityController

}  // namespace Empiria.Land.WebApi
