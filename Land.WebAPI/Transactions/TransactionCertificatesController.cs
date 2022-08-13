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
    public CollectionModel GetTransactionCertificateTypes([FromUri] string transactionUID) {

      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {
        FixedList<CertificateTypeDto> types = usecases.GetCertificateTypes(transactionUID);

        return new CollectionModel(this.Request, types);
      }
    }

    [HttpGet]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates")]
    public CollectionModel GetCertificates([FromUri] string transactionUID) {

      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {

        FixedList<CertificateDto> certificates = usecases.GetCertificates(transactionUID);

        return new CollectionModel(this.Request, certificates);
      }
    }



    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates")]
    public SingleObjectModel CreateCertificate([FromUri] string transactionUID,
                                               [FromBody] CreateCertificateCommand command) {
      base.RequireBody(command);

      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {

        CertificateDto certificate = usecases.CreateCertificate(transactionUID, command);

        return new SingleObjectModel(this.Request, certificate);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates/{certificateUID:guid}/close")]
    public SingleObjectModel CloseCertificate([FromUri] string transactionUID,
                                              [FromUri] string certificateUID) {
      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {

        CertificateDto certificate = usecases.CloseCertificate(transactionUID, certificateUID);

        return new SingleObjectModel(this.Request, certificate);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates/{certificateUID:guid}")]
    public SingleObjectModel EditCertificate([FromUri] string transactionUID,
                                         [FromUri] string certificateUID,
                                         [FromBody] object fields) {
      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {

        CertificateDto certificate = usecases.EditCertificate(transactionUID, certificateUID, fields);

        return new SingleObjectModel(this.Request, certificate);
      }
    }


    [HttpPost]
    [Route("v5/land/transactions/{transactionUID:length(19)}/certificates/{certificateUID:guid}/open")]
    public SingleObjectModel OpenCertificate([FromUri] string transactionUID,
                                             [FromUri] string certificateUID) {

      using (var usecases = TransactionCertificatesUseCases.UseCaseInteractor()) {

        CertificateDto certificate = usecases.OpenCertificate(transactionUID, certificateUID);

        return new SingleObjectModel(this.Request, certificate);
      }
    }

    #endregion Web Apis

  }  // class TransactionCertificatesController

}  // namespace Empiria.Land.Certificates.WebApi
