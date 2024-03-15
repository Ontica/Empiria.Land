/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query Controller                      *
*  Type     : TransactionESignRequestsController           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api used to retrieve transactions electronic sign requests.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;

namespace Empiria.Land.ESign.WebAPI {

  /// <summary>Query web api used to retrieve transactions electronic sign requests.</summary>
  public class TransactionESignRequestsController : WebApiController {

    #region Web apis


    [HttpGet]
    [Route("v5/land/electronic-sign/transactions/requests/mine/refused")]
    public CollectionModel GetMyTransactionsRefusedRequests([FromUri] ESignRequestsQuery query) {

      using (var usecases = TransactionESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> eSignRequests = usecases.GetMyTransactionsRefusedRequests(query);

        return new CollectionModel(this.Request, eSignRequests);
      }
    }


    [HttpGet]
    [Route("v5/land/electronic-sign/transactions/requests/mine/revoked")]
    public CollectionModel GetMyTransactionsRevokedRequests([FromUri] ESignRequestsQuery query) {

      using (var usecases = TransactionESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> eSignRequests = usecases.GetMyTransactionsRevokedRequests(query);

        return new CollectionModel(this.Request, eSignRequests);
      }
    }


    [HttpGet]
    [Route("v5/land/electronic-sign/transactions/requests/mine/signed")]
    public CollectionModel GetMyTransactionsSignedRequests([FromUri] ESignRequestsQuery query) {

      using (var usecases = TransactionESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> eSignRequests = usecases.GetMyTransactionsSignedRequests(query);

        return new CollectionModel(this.Request, eSignRequests);
      }
    }



    [HttpGet]
    [Route("v5/land/electronic-sign/transactions/requests/mine/to-revoke")]
    public CollectionModel GetMyTransactionsToRevokeRequests([FromUri] ESignRequestsQuery query) {

      using (var usecases = TransactionESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> eSignRequests = usecases.GetMyTransactionsToRevokeRequests(query);

        return new CollectionModel(this.Request, eSignRequests);
      }
    }


    [HttpGet]
    [Route("v5/land/electronic-sign/transactions/requests/mine/to-sign")]
    public CollectionModel GetMyTransactionsToSignRequests([FromUri] ESignRequestsQuery query) {

      using (var usecases = TransactionESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> eSignRequests = usecases.GetMyTransactionsToSignRequests(query);

        return new CollectionModel(this.Request, eSignRequests);
      }
    }

    #endregion Web apis

  } // class TransactionESignRequestsController

} // namespace Empiria.Land.ESign.WebAPI
