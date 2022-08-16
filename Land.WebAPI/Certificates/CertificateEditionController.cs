/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                         Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : CertificateEditionController                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web api used for Land certificates generation and edition.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certificates.UseCases;

namespace Empiria.Land.Certificates.WebApi {

  /// <summary>Web api used for Land certificates generation and edition.</summary>
  public class CertificateEditionController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/certificates/{certificateUID:guid}")]
    public CertificateDto GetCertificate([FromUri] string certificateUID) {

      using (var usecases = CertificatesUseCases.UseCaseInteractor()) {
        return usecases.GetCertificate(certificateUID);
      }

    }

    #endregion Web Apis

  }  // class CertificateEditionController

}  // namespace Empiria.Land.Certificates.WebApi
