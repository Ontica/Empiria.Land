/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RegistrationController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API with methods that perform registrar tasks over legal instruments.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API with methods that perform registrar tasks over legal instruments.</summary>
  public class RegistrationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts")]
    public SingleObjectModel CreateRecordingAct([FromUri] string instrumentRecordingUID,
                                                [FromBody] RegistrationCommand command) {

      base.RequireBody(command);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                    usecases.CreateRecordingAct(instrumentRecordingUID, command);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel DeleteRecordingAct([FromUri] string instrumentRecordingUID,
                                                [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                    usecases.RemoveRecordingAct(instrumentRecordingUID, recordingActUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    #endregion Web Apis

  }  // class RegistrationController

}  //namespace Empiria.Land.Registration.WebApi
