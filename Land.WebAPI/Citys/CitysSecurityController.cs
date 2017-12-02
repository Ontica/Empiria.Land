using System;
using System.Web.Http;

using Empiria.Security;
using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi.Citys {

  /// <summary>Contains Citys security services (to be deprecated).</summary>
  public class CitysSecurityController : WebApiController {

    #region Public APIs

    /// <summary>Gets a fomer list of available endpoints for an authenticated user
    /// in the context of the client application.</summary>
    /// <returns>A list of former HttpEndpoint objects.</returns>
    [HttpGet]
    [Route("v1/system/api-endpoints")]
    public CollectionModel GetFormerEndpoints() {
      try {
        var endpoints = CitysHttpEndpoint.GetList();
        return new CollectionModel(base.Request, endpoints);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpPost, AllowAnonymous]
    [Route("v1/security/login")]
    public SingleObjectModel FormerLogin(CitysLoginModel login)
    {
      try
      {
        base.RequireHeader("User-Agent");
        base.RequireBody(login);

        EmpiriaPrincipal principal = this.GetPrincipal(login);

        return new SingleObjectModel(base.Request, CitysLoginModel.ToOAuth(principal),
                                     "Empiria.Security.OAuthObject");
      }
      catch (Exception e)
      {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs


    #region Private methods

    private EmpiriaPrincipal GetPrincipal(CitysLoginModel login) {
      login.AssertValid();

      EmpiriaPrincipal principal = AuthenticationHttpModule.Authenticate(login.api_key,
                                                                         login.user_name,
                                                                         login.password);
      Assertion.AssertObject(principal, "principal");

      return principal;
    }

    #endregion Private methods

  }  // class CitysSecurityController

}  // namespace Empiria.Land.WebApi
