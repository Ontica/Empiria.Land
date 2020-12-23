/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentsController                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to retrieve legal instruments like titles, mortgages, deeds or contracts.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Instruments.WebApi {

  /// <summary>Public API to retrieve legal instruments like titles, mortgages, deeds or contracts.</summary>
  public class InstrumentsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/instruments/{instrumentUID:length(20)}")]
    public SingleObjectModel GetInstrument([FromUri] string instrumentUID) {

      using (var usecases = InstrumentUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.GetInstrument(instrumentUID);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }

    #endregion Web Apis

  }  // class InstrumentsController

}  // namespace Empiria.Land.Instruments.WebApi
