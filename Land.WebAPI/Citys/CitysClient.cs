/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : OnLineSearchServicesController               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains general web methods for the Empiria Land Online Search Services system.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Empiria.Json;
using Empiria.WebApi.Client;

namespace Empiria.Land.WebApi.Citys {

  internal class CitysClient {

    private readonly JsonObject targetWebApiServer = null;
    private readonly HttpApiClient apiClient = null;

    public CitysClient() {
      targetWebApiServer = targetWebApiServer = ConfigurationData.Get<JsonObject>("PassThrough.TargetServer");
      apiClient = new HttpApiClient(targetWebApiServer.Get<string>("baseAddress"));
    }


    internal async Task<object[]> ElectronicDelivery(HttpRequestMessage request) {
      var path = GetPath(request);

      var response = await apiClient.PostAsync<ResponseModel<object[]>>(path);

      return response.Data;
    }


    internal async Task<object> RequestCertificate(HttpRequestMessage request, CertificateRequest certificateRequest) {
      var path = GetPath(request);

      var response = await apiClient.PostAsync<ResponseModel<object>>(certificateRequest, path);

      return response.Data;
    }


    internal async Task<object> RequestPendingNoteRecording(HttpRequestMessage request, PendingNoteRequest pendingNoteRequest) {
      var path = GetPath(request);

      var response = await apiClient.PostAsync<ResponseModel<object>>(pendingNoteRequest, path);

      return response.Data;
    }


    private string GetPath(HttpRequestMessage request) {
      if (!targetWebApiServer.Contains("pathRule")) {
        return request.RequestUri.PathAndQuery;
      }

      string replace = targetWebApiServer.Get<string>("pathRule/replace");
      string with = targetWebApiServer.Get<string>("pathRule/with");

      return request.RequestUri.PathAndQuery.Replace(replace, with);
    }


  }

}
