/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionsController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to work with transactions.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Instruments.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to work with transactions.</summary>
  public class TransactionsController : WebApiController {

    #region Public methods

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(16)}")]
    public SingleObjectModel GetTransaction([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.GetTransaction(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(16)}/instrument")]
    public SingleObjectModel GetTransactionInstrument([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        InstrumentDto instrumentDto = usecases.GetTransactionInstrument(transactionUID);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions")]
    public CollectionModel SearchTransactions([FromUri] SearchTransactionCommand searchCommand) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        FixedList<TransactionListItemDto> list = usecases.SearchTransactions(searchCommand);

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(16)}/instrument")]
    public SingleObjectModel UpdateInstrument([FromUri] string transactionUID,
                                              [FromBody] InstrumentFields fields) {


      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.UpdateTransactionInstrument(transactionUID, fields);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    #endregion Public methods

  }  // class TransactionsController

}  //namespace Empiria.Land.Transactions.WebApi
