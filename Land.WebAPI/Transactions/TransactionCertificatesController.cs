/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                         Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TransactionCertificatesController            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to get, update and link Land certificates over transactions.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certificates.UseCases;

namespace Empiria.Land.Certificates.WebApi {

  /// <summary>Web API used to get, update and link Land certificates over transactions.</summary>
  public class TransactionCertificatesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates/certificate-types")]
    public FixedList<CertificateTypeDto> GetTransactionCertificateTypes([FromUri] string transactionUID) {

      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {
        return usecases.CertificateTypes(transactionUID);
      }

    }

    #endregion Web Apis

  }  // class TransactionCertificatesController

}  // namespace Empiria.Land.Certificates.WebApi
