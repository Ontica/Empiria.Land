/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : GetInstrumentsController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to retrieve legal instruments.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Instruments.WebApi {

  /// <summary>Public API to retrieve recordable documents.</summary>
  public class GetInstrumentsController : WebApiController {

    [HttpGet]
    [Route("v5/land/instruments/{instrumentUID:length(20)}")]
    public SingleObjectModel GetInstrument([FromUri] string instrumentUID) {

      using (var usecases = GetInstrumentsUseCases.UseCaseInteractor()) {
        InstrumentDto instrument = usecases.GetInstrument(instrumentUID);

        return new SingleObjectModel(this.Request, instrument);
      }

    }

  }

}
