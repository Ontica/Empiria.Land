/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Physical Registration                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : PhysicalRecordingController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.PhysicalBooks.Adapters;
using Empiria.Land.PhysicalBooks.UseCases;

namespace Empiria.Land.PhysicalBooks.WebApi {

  /// <summary>Web API used to retrive transaction workflow data and invoke commands on it.</summary>
  public class PhysicalRecordingController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/recording-sections")]
    public CollectionModel GetRecordingSectionsList() {

      using (var usecases = PhysicalRecordingUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> sections = usecases.GetRecordingSections();

        return new CollectionModel(this.Request, sections);
      }
    }


    [HttpPost]
    [Route("v5/land/instruments/{instrumentUID:guid}/create-next-physical-recording")]
    public SingleObjectModel CreateNextPhysicalRecording([FromUri] string instrumentUID,
                                                       [FromBody] CreateNextPhysicalRecordingFields fields) {
      base.RequireBody(fields);

      using (var usecases = PhysicalRecordingUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.CreateNextPhysicalRecording(instrumentUID, fields);

        return new SingleObjectModel(this.Request, instrument);
      }
    }


    [HttpDelete]
    [Route("v5/land/instruments/{instrumentUID:guid}/physical-recordings/{physicalRecordingUID:guid}")]
    public SingleObjectModel RemovePhysicalRecording([FromUri] string instrumentUID,
                                                     [FromUri] string physicalRecordingUID) {
      using (var usecases = PhysicalRecordingUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.RemovePhysicalRecording(instrumentUID, physicalRecordingUID);

        return new SingleObjectModel(this.Request, instrument);
      }
    }

    #endregion Web Apis

  }  // class PhysicalRecordingController

}  //namespace Empiria.Land.Workflow.WebApi
