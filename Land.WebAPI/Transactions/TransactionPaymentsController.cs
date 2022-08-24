/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                         Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionPaymentsController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used for transaction payments.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;
using System.Threading.Tasks;

using Empiria.WebApi;

using Empiria.Land.Transactions.Payments.UseCases;

namespace Empiria.Land.Transactions.Payments.WebApi {

  /// <summary>Web API used to add and remove requested transaction services.</summary>
  public class TransactionPaymentsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/cancel-payment")]
    public async Task<SingleObjectModel> CancelPayment([FromUri] string transactionUID) {

      using (var usecases = TransactionPaymentUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.CancelPayment(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/cancel-payment-order")]
    public async Task<SingleObjectModel> CancelPaymentOrder([FromUri] string transactionUID) {

      using (var usecases = TransactionPaymentUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.CancelPaymentOrder(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }

    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/generate-payment-order")]
    public async Task<SingleObjectModel> GeneratePaymentOrder([FromUri] string transactionUID) {

      using (var usecases = TransactionPaymentUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.GeneratePaymentOrder(transactionUID);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }



    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/set-payment")]
    public async Task<SingleObjectModel> SetPayment([FromUri] string transactionUID,
                                                    [FromBody] PaymentFields fields) {

      using (var usecases = TransactionPaymentUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = await usecases.SetPayment(transactionUID, fields);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionPaymentsController

}  //namespace Empiria.Land.Transactions.Payments.WebApi
