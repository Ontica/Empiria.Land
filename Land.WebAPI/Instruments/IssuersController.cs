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

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Instruments.UseCases;

namespace Empiria.Land.Instruments.WebApi {

  /// <summary>Public API to retrieve legal instrument issuers, like notaries,
  /// judges and other authorities.</summary>
  public class IssuersController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/instrument-issuers")]
    public CollectionModel SearchIssuers([FromUri] IssuersQuery query) {
      if (query == null) {
        query = new IssuersQuery();
      }

      using (var usecases = IssuerUseCases.UseCaseInteractor()) {
        FixedList<IssuerDto> list = usecases.SearchIssuers(query);

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpPost]
    [Route("v5/land/instrument-issuers/update-all")]
    public NoDataModel UpdateAllIssuers() {

      using (var usecases = IssuerUseCases.UseCaseInteractor()) {
        usecases.UpdateAll();

        return new NoDataModel(this.Request);
      }
    }

    #endregion Web Apis

  }  // class IssuersController

} //  namespace Empiria.Land.Instruments.WebApi
