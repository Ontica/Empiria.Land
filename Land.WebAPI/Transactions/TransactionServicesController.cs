/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionServicesController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to add and remove requested transaction services.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to add and remove requested transaction services.</summary>
  public class TransactionServicesController : WebApiController {

    #region Web Apis

    [HttpDelete]
    [Route("v5/land/transactions/{transactionUID:length(16)}/requested-services/{requestedServiceUID:guid}")]
    public SingleObjectModel DeleteService([FromUri] string transactionUID,
                                           [FromUri] string requestedServiceUID) {

      using (var usecases = TransactionServicesUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.DeleteService(transactionUID, requestedServiceUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/requested-services")]
    public SingleObjectModel RequestService([FromUri] string transactionUID,
                                            [FromBody] RequestedServiceFields fields) {

      using (var usecases = TransactionServicesUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.RequestService(transactionUID, fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    #endregion Web Apis

  }  // class TransactionServicesController

}  //namespace Empiria.Land.Transactions.WebApi
