/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentRecordingController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to get, update and link transaction instruments.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.Adapters;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API used to get, update and link transaction instruments.</summary>
  public class InstrumentRecordingController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel GetTransactionInstrumentRecording([FromUri] string transactionUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecordingDto = usecases.GetTransactionInstrumentRecording(transactionUID);

        return new SingleObjectModel(this.Request, instrumentRecordingDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel CreateTransactionInstrumentRecording([FromUri] string transactionUID,
                                                                  [FromBody] InstrumentFields fields) {

      base.RequireBody(fields);

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.CreateTransactionInstrumentRecording(transactionUID, fields);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(19)}/instrument-recording")]
    public SingleObjectModel UpdateTransactionInstrumentRecording([FromUri] string transactionUID,
                                                                  [FromBody] InstrumentFields fields) {

      base.RequireBody(fields);

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.UpdateTransactionInstrumentRecording(transactionUID, fields);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionInstrumentController

}  //namespace Empiria.Land.Transactions.WebApi
