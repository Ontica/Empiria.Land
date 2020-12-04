/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : GetTransactionsController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API to search and retrieve transactions                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API to search and retrieve transactions.</summary>
  public class GetTransactionsController : WebApiController {

    #region Public methods

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(16)}")]
    public SingleObjectModel GetTransaction([FromUri] string transactionUID) {

      using (var usecases = GetTransactionsUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.GetTransaction(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions")]
    public CollectionModel SearchTransactions([FromUri] SearchTransactionCommand searchCommand) {

      using (var usecases = GetTransactionsUseCases.UseCaseInteractor()) {
        FixedList<TransactionListItemDto> list = usecases.SearchTransactions(searchCommand);

        return new CollectionModel(this.Request, list);
      }
    }

    #endregion Public methods

  }  // class GetTransactionsController

}  //namespace Empiria.Land.Transactions.WebApi
