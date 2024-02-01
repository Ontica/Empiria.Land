/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordingActsRegistrationController          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API for instrument's recording act edition.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API for instrument's recording act edition.</summary>
  public class RecordingActsRegistrationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/registration/{landRecordUID:guid}/recording-acts")]
    public SingleObjectModel CreateRecordingAct([FromUri] string landRecordUID,
                                                [FromBody] RegistrationCommand command) {

      base.RequireBody(command);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.CreateRecordingAct(landRecordUID, command);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/{landRecordUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel GetRecordingAct([FromUri] string landRecordUID,
                                             [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.GetRecordingAct(landRecordUID, recordingActUID);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{landRecordUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel DeleteRecordingAct([FromUri] string landRecordUID,
                                                [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.RemoveRecordingAct(landRecordUID, recordingActUID);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    [HttpPatch, HttpPut]
    [Route("v5/land/registration/{landRecordUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel UpdateRecordingAct([FromUri] string landRecordUID,
                                                [FromUri] string recordingActUID,
                                                [FromBody] RecordingActFields fields) {
      base.RequireBody(fields);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.UpdateRecordingAct(landRecordUID,
                                                                   recordingActUID,
                                                                   fields);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    #endregion Web Apis

  }  // class RecordingActsRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
