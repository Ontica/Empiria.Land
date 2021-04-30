/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : PartiesController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API for recording act parties edition.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API for recording act parties edition.</summary>
  public class PartiesController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}/parties")]
    public SingleObjectModel AppendParty([FromUri] string instrumentRecordingUID,
                                         [FromUri] string recordingActUID,
                                         [FromBody] RecordingActPartyFields fields) {

      base.RequireBody(fields);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.AppendParty(instrumentRecordingUID,
                                                            recordingActUID, fields);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/" +
           "{recordingActUID:guid}/parties/{partyUID:guid}")]
    public SingleObjectModel RemoveParty([FromUri] string instrumentRecordingUID,
                                         [FromUri] string recordingActUID,
                                         [FromUri] string partyUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.RemoveParty(instrumentRecordingUID,
                                                            recordingActUID, partyUID);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    #endregion Web Apis

  }  // class PartiesController

}  //namespace Empiria.Land.Registration.WebApi
