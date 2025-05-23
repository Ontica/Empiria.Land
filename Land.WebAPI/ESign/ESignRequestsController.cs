﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query Controller                      *
*  Type     : ESignRequestsController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api used to retrieve electronic sign requests.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Adapters;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;

namespace Empiria.Land.ESign.WebApi {

  /// <summary>Query web api used to retrieve electronic sign requests.</summary>
  public class ESignRequestsController : WebApiController {

    #region Web apis

    [HttpPost]
    [Route("v5/land/electronic-sign/requests/documents/mine")]
    public CollectionModel GetMyESignRequestedDocuments([FromBody] ESignRequestsQuery query) {

      using (var usecases = ESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<SignableDocumentDescriptor> documents = usecases.GetMyESignRequestedDocuments(query);

        return new CollectionModel(this.Request, documents);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/requests/transactions/mine")]
    public CollectionModel GetMyESignRequestedTransactions([FromBody] ESignRequestsQuery query) {

      using (var usecases = ESignRequestsUseCases.UseCaseInteractor()) {

        FixedList<TransactionDescriptor> transactions = usecases.GetMyESignRequestedTransactions(query);

        return new CollectionModel(this.Request, transactions);
      }
    }

    #endregion Web apis

  } // class ESignRequestsController

} // namespace Empiria.Land.ESign.WebApi
