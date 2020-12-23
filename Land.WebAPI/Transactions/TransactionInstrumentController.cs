/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionInstrumentController              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to get, update and link transaction instruments.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to get, update and link transaction instruments.</summary>
  public class TransactionInstrumentController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(16)}/instrument")]
    public SingleObjectModel GetTransactionInstrument([FromUri] string transactionUID) {

      using (var usecases = TransactionInstrumentUseCases.UseCaseInteractor()) {
        InstrumentDto instrumentDto = usecases.GetTransactionInstrument(transactionUID);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(16)}/instrument")]
    public SingleObjectModel UpdateTransactionInstrument([FromUri] string transactionUID,
                                                         [FromBody] InstrumentFields fields) {

      base.RequireBody(fields);

      using (var usecases = TransactionInstrumentUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.UpdateTransactionInstrument(transactionUID, fields);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionInstrumentController

}  //namespace Empiria.Land.Transactions.WebApi
