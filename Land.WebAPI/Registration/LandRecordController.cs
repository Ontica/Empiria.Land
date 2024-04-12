/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : LandRecordController                         License   : Please read LICENSE.txt file          *
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
  public class LandRecordController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/registration/{landRecordUID:guid}")]
    public SingleObjectModel GetLandRecord([FromUri] string landRecordUID) {

      using (var usecases = LandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.GetLandRecord(landRecordUID);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{landRecordUID:guid}/open-registration")]
    public SingleObjectModel OpenLandRecord([FromUri] string landRecordUID) {

      using (var usecases = LandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.OpenLandRecord(landRecordUID);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{landRecordUID:guid}/close-registration")]
    public SingleObjectModel CloseLandRecord([FromUri] string landRecordUID) {

      using (var usecases = LandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.CloseLandRecord(landRecordUID);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v5/land/registration/manual_close/{recordID}")]
    public SingleObjectModel ManualCloseLandRecord([FromUri] string recordID,
                                                   [FromBody] ManualCloseRecordFields fields) {

      using (var usecases = LandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.ManualCloseLandRecord(recordID, fields);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }

    #endregion Web Apis

  }  // class LandRecordController

}  //namespace Empiria.Land.Registration.WebApi
