/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                         Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : CertificatesController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web api for Land certificates issuing and retrieving.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certificates.UseCases;

namespace Empiria.Land.Certificates.WebApi {

  /// <summary>Web api for Land certificates issuing and retrieving.</summary>
  public class CertificatesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/certificates/{certificateGuid:guid}")]
    public CertificateDto GetCertificateByGuid([FromUri] Guid certificateGuid) {

      using (var usecases = CertificateIssuingUseCases.UseCaseInteractor()) {
        return usecases.GetCertificate(certificateGuid);
      }
    }

    #endregion Web Apis

  }  // class CertificatesController

}  // namespace Empiria.Land.Certificates.WebApi
