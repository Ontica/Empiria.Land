/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : OnLineServicesController                         Pattern  : Web API                           *
*  Version   : 2.1                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains general web methods for Empiria Land online services.                                *
*                                                                                                            *
********************************* Copyright (c) 2014-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Web.Http;

using Empiria.Land.Certification;
using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi {

  internal enum DocumentItemType {
    LandTransaction,
    LandCertificate,
    RecordingDocument,
    Empty
  }

  /// <summary>Contains general web methods for Empiria Land online services.</summary>
  public class OnLineServicesController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/document-item/{documentItemType}/{itemUID}")]
    public SingleObjectModel GetDocumentItem([FromUri] string documentItemType,
                                             [FromUri] string itemUID) {
      try {
        base.RequireResource(documentItemType, "documentItemType");
        base.RequireResource(itemUID, "itemUID");

        var docItemType = (DocumentItemType) Enum.Parse(typeof(DocumentItemType),
                                                        documentItemType, true);

        switch (docItemType) {
          case DocumentItemType.LandTransaction:
            throw new NotImplementedException();

          case DocumentItemType.LandCertificate:
            var certificate = Certification.Certificate.TryParse(itemUID);

            if (certificate != null) {
              return new SingleObjectModel(this.Request, BuildDocumentItemResponse(certificate),
                                           "Land.DocumentItemType");
            } else {
              throw new ResourceNotFoundException("Certificate.UID",
                        "Certificate with identifier '{0}' was not found.", itemUID);
            }

          case DocumentItemType.RecordingDocument:
            throw new NotImplementedException();

          case DocumentItemType.Empty:
            throw new ValidationException("InvalidDocumentItemType",
                      "documentItemType can't have the empty value (DocumentItemType.Empty).");

          default:
            throw Assertion.AssertNoReachThisCode("Unrecognized documentItemType value {0}.",
                                                  docItemType.ToString());
        }

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    private object BuildDocumentItemResponse(Certificate certificate) {
      return new {
        uid = certificate.UID,
        itemType = certificate.GetType().ToString(),
        documentType = certificate.CertificateType.ToString(),
        digitalSeal = certificate.GetDigitalSeal(),
        digitalSignature = certificate.GetDigitalSignature(),
        status = certificate.Status.ToString()
      };
    }

    #endregion Public APIs

  }  // class OnLineServicesController

}  // namespace Empiria.Land.WebApi
