/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : IssuersController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to retrieve legal instrument issuers, like notaries, judges and other authorities.  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Instruments.WebApi {

  /// <summary>Public API to retrieve legal instrument issuers, like notaries,
  /// judges and other authorities.</summary>
  public class IssuersController : WebApiController {

    [HttpGet]
    [Route("v5/land/instrument-issuers")]
    public CollectionModel SearchIssuers([FromUri] IssuersSearchCommand searchCommand) {
      if (searchCommand == null) {
        searchCommand = new IssuersSearchCommand();
      }

      using (var usecases = IssuerUseCases.UseCaseInteractor()) {
        FixedList<IssuerDto> list = usecases.SearchIssuers(searchCommand);

        return new CollectionModel(this.Request, list);
      }
    }

  }  // class IssuersController

} //  namespace Empiria.Land.Instruments.WebApi
