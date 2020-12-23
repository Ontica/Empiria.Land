/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentTypeController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API used to retrieve configuration data about legal instruments types.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Instruments.WebApi {

  /// <summary>Public API used to retrieve configuration data about legal instruments types.</summary>
  public class InstrumentTypeController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/instrument-types/{instrumentTypeName}/instrument-kinds")]
    public CollectionModel GetInstrumentKinds([FromUri] InstrumentTypeEnum instrumentTypeName) {

      using (var usecases = InstrumentTypeUseCases.UseCaseInteractor()) {
        FixedList<string> instrumentKinds = usecases.GetInstrumentKinds(instrumentTypeName);

        return new CollectionModel(this.Request, instrumentKinds);
      }
    }

    #endregion Web Apis

  }  // class InstrumentTypeController

}  // namespace Empiria.Land.Instruments.WebApi
