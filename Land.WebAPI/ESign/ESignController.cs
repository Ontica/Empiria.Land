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
    [Route("v5/land/e-sign/request-pending-esign")]
    public SingleObjectModel GetPendingESigns([FromBody] int recorderOfficeId) {
      
      using (var usecases = ESignEngineUseCases.UseCaseInteractor()) {

        FixedList<SignDocumentDto> esign = usecases.GetSignedDocuments(recorderOfficeId);

        return new SingleObjectModel(this.Request, esign);
      }
    }


    [HttpPost]
    [Route("v5/land/e-sign/generate-esign")]
#pragma warning disable CS3001 // El tipo de argumento no es conforme a CLS
    public SingleObjectModel GenerateESign([FromBody] ESignQuery query) {
#pragma warning restore CS3001 // El tipo de argumento no es conforme a CLS

      using (var usecases = ESignEngineUseCases.UseCaseInteractor()) {

        ESignDTO esign = usecases.BuildRequest(query);

        return new SingleObjectModel(this.Request, esign);
      }
    }


    #endregion Web Apis


  } // class ESignController

} // namespace Empiria.Land.ESign.WebAPI
