/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Messaging Services                           Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : MessagingEngineController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to interact with the Empiria Land messaging engine.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Messaging.UseCases;

namespace Empiria.Land.Messaging.WebApi {

  /// <summary>Web API used to interact with the Empiria Land messaging engine.</summary>
  public class MessagingEngineController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/messaging/engine")]
    public SingleObjectModel MessagingEngineStatus() {

      using (var usecases = MessagingEngineUseCases.UseCaseInteractor()) {
        string status = usecases.EngineStatus();

        return new SingleObjectModel(this.Request, status);
      }
    }


    [HttpPost]
    [Route("v5/land/messaging/engine/start")]
    public NoDataModel StartMessagingEngine() {

      using (var usecases = MessagingEngineUseCases.UseCaseInteractor()) {
        usecases.StartEngine();

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/messaging/engine/stop")]
    public NoDataModel StopMessagingEngine() {

      using (var usecases = MessagingEngineUseCases.UseCaseInteractor()) {
        usecases.StopEngine();

        return new NoDataModel(this.Request);
      }
    }

    #endregion Web Apis

  }  // class MessagingEngineController

}  //namespace Empiria.Land.Messaging.WebApi
