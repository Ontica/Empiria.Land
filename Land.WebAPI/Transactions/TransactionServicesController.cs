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
using System.Threading.Tasks;

using Empiria.WebApi;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to add and remove requested transaction services.</summary>
  public class TransactionServicesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/cancel-payment")]
    public async Task<SingleObjectModel> CancelPayment([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.CancelPayment(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/cancel-payment-order")]
    public async Task<SingleObjectModel> CancelPaymentOrder([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.CancelPaymentOrder(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpDelete]
    [Route("v5/land/transactions/{transactionUID:length(16)}/requested-services/{requestedServiceUID:guid}")]
    public SingleObjectModel DeleteService([FromUri] string transactionUID,
                                           [FromUri] string requestedServiceUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.DeleteService(transactionUID, requestedServiceUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/generate-payment-order")]
    public async Task<SingleObjectModel> GeneratePaymentOrder([FromUri] string transactionUID) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.GeneratePaymentOrder(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/requested-services")]
    public async Task<SingleObjectModel> RequestService([FromUri] string transactionUID,
                                                        [FromBody] RequestedServiceFields fields) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.RequestService(transactionUID, fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(16)}/set-payment")]
    public async Task<SingleObjectModel> SetPayment([FromUri] string transactionUID,
                                                    [FromBody] PaymentFields fields) {

      using (var usecases = TransactionUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.SetPayment(transactionUID, fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionServicesController

}  //namespace Empiria.Land.Transactions.WebApi
