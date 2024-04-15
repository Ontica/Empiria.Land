/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Preprocessing                   Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query Controller                      *
*  Type     : TransactionPreprocessingController           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api used to return transaction's preprocesing data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Preprocessing.Adapters;
using Empiria.Land.Transactions.Preprocessing.UseCases;

namespace Empiria.Land.Transactions.Preprocessing.WebApi {

  /// <summary>Query web api used to return transaction's preprocesing data.</summary>
  public class TransactionPreprocessingController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/preprocessing-data")]
    public SingleObjectModel GetPreprocessingData([FromUri] string transactionUID) {

      using (var usecases = TransactionPreprocessingUseCases.UseCaseInteractor()) {
        TransactionPreprocessingDto preprocessingDto = usecases.GetPreprocessingData(transactionUID);

        return new SingleObjectModel(this.Request, preprocessingDto);
      }
    }

    #endregion Web Apis

  }  // class TransactionPreprocessingController

}  //namespace Empiria.Land.Transactions.Preprocessing.WebApi
