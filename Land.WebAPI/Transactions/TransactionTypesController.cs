﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionTypesController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction types information.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

  /// <summary>Web API used to retrive transaction types information.</summary>
  public class TransactionTypesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/agencies")]
    public CollectionModel GetAgenciesList() {

      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> agencies = usecases.Agencies();

        return new CollectionModel(this.Request, agencies);
      }
    }


    [HttpGet]
    [Route("v5/land/filing-offices")]
    public CollectionModel GetFilingOfficesList() {

      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> offices = usecases.FilingOffices();

        return new CollectionModel(this.Request, offices);
      }
    }


    [HttpGet]
    [Route("v5/land/provided-services")]
    public CollectionModel GetProvidedServices() {

      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        FixedList<ProvidedServiceGroupDto> list = usecases.ProvidedServices();

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpGet]
    [Route("v5/land/transaction-types")]
    public CollectionModel GetTransactionTypesList() {

      using (var usecases = TransactionTypeUseCases.UseCaseInteractor()) {
        FixedList<TransactionTypeDto> list = usecases.TransactionTypes();

        return new CollectionModel(this.Request, list);
      }
    }

    #endregion Web Apis

  }  // class TransactionTypesController

}  //namespace Empiria.Land.Transactions.WebApi
