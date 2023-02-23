/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign                                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : ESignController                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public Web API used to generate and retrieve ESign.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;
using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;
using Empiria.WebApi;


namespace Empiria.Land.ESign.WebAPI {

  /// <summary>Public Web API used to generate and retrieve ESign.</summary>
  public class ESignController : WebApiController {


    #region Web Apis


    [HttpPost]
    [Route("v5/land/esign/sign-document")]
    public SingleObjectModel GetEsign() {
      
      ESignQuery query = new ESignQuery();

      base.RequireBody(query);

      using (var usecases = ESignEngineUseCases.UseCaseInteractor()) {

        ESignDTO esign = usecases.Build(query);

        return new SingleObjectModel(this.Request, esign);
      }
    }


    #endregion Web Apis


  } // class ESignController

} // namespace Empiria.Land.ESign.WebAPI
