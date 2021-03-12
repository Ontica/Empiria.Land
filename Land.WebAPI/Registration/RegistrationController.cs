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

using Empiria.Land.Instruments.Adapters;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API with methods that perform registrar tasks over legal instruments.</summary>
  public class RegistrationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/registration/{instrumentUID:guid}/recording-acts")]
    public SingleObjectModel RecordingActRegistration([FromUri] string instrumentUID,
                                                      [FromBody] RegistrationCommand command) {

      base.RequireBody(command);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.RecordingActRegistration(instrumentUID, command);

        return new SingleObjectModel(this.Request, instrument);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel DeleteRecordingActRegistration([FromUri] string instrumentUID,
                                                            [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.RemoveRecordingActRegistation(instrumentUID, recordingActUID);

        return new SingleObjectModel(this.Request, instrument);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/registration/{instrumentUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel UpdateRecordingAct([FromUri] string instrumentUID,
                                                [FromUri] string recordingActUID,
                                                [FromBody] RegistrationCommand command) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.UpdateRecordingActRegistration(instrumentUID,
                                                                           recordingActUID,
                                                                           command);

        return new SingleObjectModel(this.Request, instrument);
      }
    }

    #endregion Web Apis

  }  // class RegistrationController

}  //namespace Empiria.Land.Registration.WebApi
