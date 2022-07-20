/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : PartiesRegistrationController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API for recording act parties edition.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API for recording act parties edition.</summary>
  public class PartiesRegistrationController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/" +
           "{recordingActUID:guid}/parties")]
    public CollectionModel SearchParties([FromUri] string instrumentRecordingUID,
                                         [FromUri] string recordingActUID,
                                         [FromUri] SearchPartiesCommand command) {

      using (var usecases = PartiesRegistrationUseCases.UseCaseInteractor()) {
        FixedList<PartyDto> parties = usecases.SearchParties(instrumentRecordingUID,
                                                             recordingActUID, command);

        return new CollectionModel(this.Request, parties);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}/parties")]
    public SingleObjectModel AppendParty([FromUri] string instrumentRecordingUID,
                                         [FromUri] string recordingActUID,
                                         [FromBody] RecordingActPartyFields fields) {

      base.RequireBody(fields);

      using (var usecases = PartiesRegistrationUseCases.UseCaseInteractor()) {
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

      using (var usecases = PartiesRegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.RemoveParty(instrumentRecordingUID,
                                                            recordingActUID, partyUID);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    #endregion Web Apis

  }  // class PartiesRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
