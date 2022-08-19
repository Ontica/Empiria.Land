﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests Management              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : CertificateRequestsController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to request land certificates within a transaction context.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.CertificateRequests.UseCases;

namespace Empiria.Land.Transactions.CertificateRequests.WebApi {

  /// <summary>Web API used to request land certificates within a transaction context.</summary>
  public class CertificateRequestsController : WebApiController {

    #region Query Apis

    [HttpGet]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates/certificate-types")]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificate-request-types")]
    public CollectionModel GetCertificateRequestTypes([FromUri] string transactionID) {

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        FixedList<CertificateRequestTypeDto> requestTypes = usecases.GetCertificateRequestTypes(transactionID);

        return new CollectionModel(this.Request, requestTypes);
      }
    }


    [HttpGet]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates")]
    [Route("v5/land/transactions/{transactionID:length(19)}/requested-certificates")]
    public CollectionModel GetRequestedCertificates([FromUri] string transactionID) {

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        FixedList<CertificateRequestDto> requests = usecases.GetRequestedCertificates(transactionID);

        return new CollectionModel(this.Request, requests);
      }
    }

    #endregion Query Apis

    #region Command Apis

    [HttpPost]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates/{certificateGuid:guid}/close")]
    [Route("v5/land/transactions/{transactionID:length(19)}/requested-certificates/{certificateGuid:guid}/close")]
    public SingleObjectModel CloseRequestedCertificate([FromUri] string transactionID,
                                                       [FromUri] Guid certificateGuid) {

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        CertificateRequestDto certificateRequest = usecases.CloseRequestedCertificate(transactionID, certificateGuid);

        return new SingleObjectModel(this.Request, certificateRequest);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates/{certificateGuid:guid}")]
    [Route("v5/land/transactions/{transactionID:length(19)}/requested-certificates/{certificateGuid:guid}")]
    public SingleObjectModel EditRequestedCertificate([FromUri] string transactionID,
                                                      [FromUri] Guid certificateGuid,
                                                      [FromBody] object fields) {

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        CertificateRequestDto certificateRequest = usecases.EditRequestedCertificate(transactionID, certificateGuid, fields);

        return new SingleObjectModel(this.Request, certificateRequest);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates/{certificateGuid:guid}/open")]
    [Route("v5/land/transactions/{transactionID:length(19)}/requested-certificates/{certificateGuid:guid}/open")]
    public SingleObjectModel OpenRequestedCertificate([FromUri] string transactionID,
                                                      [FromUri] Guid certificateGuid) {

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        CertificateRequestDto certificateRequest = usecases.OpenRequestedCertificate(transactionID, certificateGuid);

        return new SingleObjectModel(this.Request, certificateRequest);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionID:length(19)}/certificates")]
    [Route("v5/land/transactions/{transactionID:length(19)}/requested-certificates")]
    public SingleObjectModel RequestCertificate([FromUri] string transactionID,
                                                [FromBody] CertificateRequestCommand command) {
      base.RequireBody(command);

      using (var usecases = RequestCertificatesUseCases.UseCaseInteractor()) {

        CertificateRequestDto certificateRequest = usecases.RequestCertificate(transactionID, command);

        return new SingleObjectModel(this.Request, certificateRequest);
      }
    }

    #endregion Command Apis

  }  // class CertificateRequestsController

}  // namespace Empiria.Land.Certificates.WebApi
