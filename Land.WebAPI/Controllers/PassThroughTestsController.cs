/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.WebApi                                   Assembly : Empiria.Land.WebApi.dll           *
*  Type      : PassThroughTestsController                       Pattern  : Web API                           *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Web api used to test pass through server calls using Empiria HttpApiClient.                   *
*                                                                                                            *
********************************* Copyright (c) 2014-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.WebApi;
using Empiria.WebApi.Client;

namespace Empiria.Land.WebApi {

  /// <summary>Web api used to test pass through server calls using Empiria HttpApiClient.</summary>
  public class PassThroughTestsController : WebApiController {

    private readonly string TARGET_WEB_API_SERVER = ConfigurationData.GetString("PassThrough.TargetServer");


    [HttpGet, AllowAnonymous]
    [Route("v1/system/test-pass-through-server-call")]
    public async Task<SingleObjectModel> TestPassThroughServerCall() {
      try {
        if (IsPassThroughServer) {
          var client = new HttpApiClient(TARGET_WEB_API_SERVER);

          var asyncResponse = await client.GetAsync<ResponseModel<string>>($"v1/system/test-pass-through-server-call");

          return new SingleObjectModel(this.Request, asyncResponse.Data);
        }

        return new SingleObjectModel(this.Request, ProductInformation.Description);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/system/test-pass-through-server-error")]
    public async Task<SingleObjectModel> TestPassThroughServerError() {
      try {
        if (IsPassThroughServer) {
          var client = new HttpApiClient(TARGET_WEB_API_SERVER);

          var asyncResponse = await client.GetAsync<ResponseModel<string>>($"v1/system/test-pass-through-server-error");

          return new SingleObjectModel(this.Request, asyncResponse.Data);
        }

        throw new NotImplementedException("TestPassThroughServerError exception");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/system/test-pass-through-server-bad-request-response")]
    public async Task<SingleObjectModel> TestPassThroughBadRequestResponse() {
      try {
        if (IsPassThroughServer) {
          var client = new HttpApiClient(TARGET_WEB_API_SERVER);

          var asyncResponse = await client.GetAsync<ResponseModel<string>>($"v1/system/test-pass-through-server-bad-request-response");

          return new SingleObjectModel(this.Request, asyncResponse.Data);
        }

        base.RequireResource(null, "entityName");

        return new SingleObjectModel(this.Request, ProductInformation.Description);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/system/test-pass-through-server-client-call-error")]
    public async Task<SingleObjectModel> TestPassThroughClientCallError() {
      try {
        if (IsPassThroughServer) {
          var client = new HttpApiClient(TARGET_WEB_API_SERVER);

          await Task.CompletedTask;

          throw new NotFiniteNumberException();
        }

        return new SingleObjectModel(this.Request, ProductInformation.Description);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/system/test-pass-through-server-not-found-response")]
    public async Task<SingleObjectModel> TestPassThroughNotFoundResponse() {
      try {
        if (IsPassThroughServer) {
          var client = new HttpApiClient(TARGET_WEB_API_SERVER);

          var asyncResponse = await client.GetAsync<ResponseModel<string>>($"v1/system/not-defined-endpoint");

          return new SingleObjectModel(this.Request, asyncResponse.Data);
        }

        return new SingleObjectModel(this.Request, ProductInformation.Description);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

  }  // class PassThroughTestsController

}  // namespace Empiria.WebApi
