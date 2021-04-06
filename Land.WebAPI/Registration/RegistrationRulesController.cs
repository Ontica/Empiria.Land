/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RegistrationRulesController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to get Empiria Land registration rules.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API used to get Empiria Land registration rules.</summary>
  public class RegistrationRulesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/registration/{instrumentUID:guid}/recording-act-types")]
    public CollectionModel GetInstrumentRecordingActTypes([FromUri] string instrumentUID) {

      using (var usecases = RegistrationRulesUseCases.UseCaseInteractor()) {
        FixedList<RecordingActTypeGroupDto> groups = usecases.RecordingActTypesForInstrument(instrumentUID);

        return new CollectionModel(this.Request, groups);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recording-act-types/{listUID}")]
    public SingleObjectModel GetRecordingActTypesList([FromUri] string listUID) {

      using (var usecases = RegistrationRulesUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> recordingActTypes = usecases.RecordingActTypesList(listUID);

        return new SingleObjectModel(this.Request, recordingActTypes);
      }
    }


    #endregion Web Apis

  }  // class RegistrationRulesController

}  //namespace Empiria.Land.Registration.WebApi
