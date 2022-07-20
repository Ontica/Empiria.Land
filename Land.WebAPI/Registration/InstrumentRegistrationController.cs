/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentRegistrationController             License   : Please read LICENSE.txt file          *
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
  public class InstrumentRegistrationController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}")]
    public SingleObjectModel GetInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.GetInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/open-registration")]
    public SingleObjectModel OpenInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.OpenInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/close-registration")]
    public SingleObjectModel CloseInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.CloseInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    #endregion Web Apis

  }  // class InstrumentRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
