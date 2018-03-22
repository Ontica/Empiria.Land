using System;
using System.Web.Http;

using Empiria.WebApi;

namespace Empiria.Land.WebApi.Citys {

  /// <summary>Contains Citys security services (to be deprecated).</summary>
  public class CitysEndpointsController : WebApiController {

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

    #endregion Public APIs

  }  // class CitysEndpointsController

}  // namespace Empiria.Land.WebApi.Citys
