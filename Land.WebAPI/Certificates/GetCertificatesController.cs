/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                         Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query Controller                      *
*  Type     : GetCertificatesController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api used for Land certificates searching and retrieving.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certificates.Services;

namespace Empiria.Land.Certificates.WebApi {

  /// <summary>Web api used for Land certificates generation and edition.</summary>
  public class GetCertificatesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/certificates/{certificateGuid:guid}")]
    public CertificateDto GetCertificateByGuid([FromUri] Guid certificateGuid) {

      using (var searcher = SearchCertificatesServices.ServiceInteractor()) {
        return searcher.GetCertificate(certificateGuid);
      }
    }

    #endregion Web Apis

  }  // class GetCertificatesController

}  // namespace Empiria.Land.Certificates.WebApi
