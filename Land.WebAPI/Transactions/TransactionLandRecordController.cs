/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionLandRecordController              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to get, update and link transaction land records.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API used to get, update and link transaction instruments.</summary>
  public class TransactionLandRecordController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel GetTransactionLandRecord([FromUri] string transactionUID) {

      using (var usecases = TransactionLandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.GetTransactionLandRecord(transactionUID);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel CreateTransactionLandRecord([FromUri] string transactionUID,
                                                         [FromBody] InstrumentFields fields) {

      base.RequireBody(fields);

      using (var usecases = TransactionLandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.CreateTransactionLandRecord(transactionUID, fields);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel UpdateTransactionLandRecord([FromUri] string transactionUID,
                                                         [FromBody] InstrumentFields fields) {

      base.RequireBody(fields);

      using (var usecases = TransactionLandRecordUseCases.UseCaseInteractor()) {
        LandRecordDto landRecordDto = usecases.UpdateTransactionLandRecord(transactionUID, fields);

        return new SingleObjectModel(this.Request, landRecordDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionLandRecordController

}  //namespace Empiria.Land.Transactions.WebApi
