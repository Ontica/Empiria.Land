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
using System.Threading.Tasks;

using Empiria.WebApi;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to work with transactions.</summary>
  public class TransactionsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/clone")]
    public SingleObjectModel CloneTransaction([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.CloneTransaction(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions")]
    public SingleObjectModel CreateTransaction([FromBody] TransactionFields fields) {

      base.RequireBody(fields);

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.CreateTransaction(fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpDelete]
    [Route("v5/land/transactions/{transactionUID:length(19)}")]
    public NoDataModel DeleteTransaction([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        usecases.DeleteTransaction(transactionUID);

        return new NoDataModel(this.Request);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/preprocessing-data")]
    public SingleObjectModel GetPreprocessingData([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionPreprocessingDto preprocessingDto = usecases.GetPreprocessingData(transactionUID);

        return new SingleObjectModel(this.Request, preprocessingDto);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}")]
    public SingleObjectModel GetTransaction([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.GetTransaction(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions")]
    public CollectionModel SearchTransactions([FromUri] SearchTransactionCommand searchCommand) {

      Assertion.Require(searchCommand, "searchCommand");

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        FixedList<TransactionShortModel> list = usecases.SearchTransactions(searchCommand);

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/submit")]
    public async Task<SingleObjectModel> SubmitTransaction([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.SubmitTransaction(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(19)}")]
    public SingleObjectModel UpdateTransaction([FromUri] string transactionUID,
                                               [FromBody] TransactionFields fields) {

      base.RequireBody(fields);

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.UpdateTransaction(transactionUID, fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionsController

}  //namespace Empiria.Land.Transactions.WebApi
