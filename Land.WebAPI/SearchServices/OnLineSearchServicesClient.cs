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

using Empiria.WebApi;
using Empiria.WebApi.Client;

namespace Empiria.Land.WebApi {

  internal class OnLineSearchServicesClient {

    private readonly string TARGET_WEB_API_SERVER = ConfigurationData.GetString("Target.WebApi.Server");

    HttpApiClient apiClient = null;

    public OnLineSearchServicesClient() {
      apiClient = new HttpApiClient(TARGET_WEB_API_SERVER);
    }


    internal async Task<SingleObjectModel> ElectronicDelivery(HttpRequestMessage request) {
      var path = GetPath(request);

      return await apiClient.GetAsync<SingleObjectModel>(path);
    }


    static private string GetPath(HttpRequestMessage request) {
      return request.RequestUri.PathAndQuery.Replace("/web.api/", "");
    }

  }

}
