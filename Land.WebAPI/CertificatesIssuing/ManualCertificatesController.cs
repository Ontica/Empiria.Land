/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : ManualCertificatesController                     Pattern  : Web API                           *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains services for manual land certificates issuing                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certification;

namespace Empiria.Land.WebApi {

  /// <summary>Contains services for manual land certificates issuing.</summary>
  public class ManualCertificatesController : WebApiController {

    #region Public APIs

    [HttpPost]
    [Route("v1/certificates")]
    public SingleObjectModel CreateCertificate([FromBody] CertificateDTO certificateData) {
      try {
        base.RequireBody(certificateData);

        var certificate = Certificate.Create(certificateData);

        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpPost]
    [Route("v1/certificates/{certificateUID}/close")]
    public SingleObjectModel CloseCertificate([FromUri] string certificateUID) {
      try {
        Certificate certificate = this.ReadCertificate(certificateUID);
        certificate.Close();

        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpPost]
    [Route("v1/certificates/{certificateUID}/open")]
    public SingleObjectModel OpenCertificate([FromUri] string certificateUID) {
      try {
        Certificate certificate = this.ReadCertificate(certificateUID);
        certificate.Open();

        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpDelete]
    [Route("v1/certificates/{certificateUID}")]
    public SingleObjectModel DeleteOrCancelCertificate([FromUri] string certificateUID) {
      try {
        Certificate certificate = this.ReadCertificate(certificateUID);

        if (certificate.CanDelete()) {
          certificate.Delete();
        } else if (certificate.CanCancel()) {
          certificate.Cancel();
        }
        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet]
    [Route("v1/certificates/{certificateUID}")]
    public SingleObjectModel GetCertificate([FromUri] string certificateUID) {
      try {
        Certificate certificate = this.ReadCertificate(certificateUID);

        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet]
    [Route("v1/certificates/{certificateUID}/as-text")]
    public SingleObjectModel GetCertificateAsText([FromUri] string certificateUID) {
      try {
        Certificate certificate = this.ReadCertificate(certificateUID);

        return this.BuildCertificateAsTextResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpPut, HttpPatch]
    [Route("v1/certificates/{certificateUID}")]
    public SingleObjectModel UpdateCertificate([FromUri] string certificateUID,
                                               [FromBody] CertificateDTO certificateData) {
      try {
        base.RequireBody(certificateData);

        Certificate certificate = this.ReadCertificate(certificateUID);
        certificate.Update(certificateData);

        return this.BuildCertificateResponse(certificate);
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private SingleObjectModel BuildCertificateResponse(Certificate certificate) {
      return new SingleObjectModel(this.Request, this.GetCertificateModel(certificate),
                                   "Empiria.Land.Certificate");
    }

    private SingleObjectModel BuildCertificateAsTextResponse(Certificate certificate) {
      return new SingleObjectModel(this.Request, this.GetCertificateAsTextModel(certificate),
                                   "Empiria.Land.CertificateAsText");
    }

    private object GetCertificateModel(Certificate o) {
      return new {
        uid = o.UID,
        type = new {
          uid = o.CertificateType.Name,
          displayName = o.CertificateType.DisplayName,
        },
        transaction = new {
          uid = o.Transaction.UID,
          requestedBy = o.Transaction.RequestedBy,
          presentationTime = o.Transaction.PresentationTime,
          paymentReceipt = !o.Transaction.FormerPaymentOrderData.IsEmptyInstance ?
                                o.Transaction.FormerPaymentOrderData.RouteNumber :
                                o.Transaction.Payments.ReceiptNumbers,
        },
        status = new {
          uid = o.Status.ToString(),
          code = (char) o.Status,
        },
        recorderOffice = new {
          id = o.RecorderOffice.Id,
          name = o.RecorderOffice.Alias,
        },
        property = new {
          uid = o.Property.UID,
          commonName = o.ExtensionData.PropertyCommonName,
          location = o.ExtensionData.PropertyLocation,
          metesAndBounds = o.ExtensionData.PropertyMetesAndBounds,
        },
        fromOwnerName = o.ExtensionData.FromOwnerName,
        toOwnerName = o.OwnerName,
        operation = o.ExtensionData.Operation,
        operationDate = o.ExtensionData.OperationDate,
        seekForName = o.ExtensionData.SeekForName,
        startingYear = o.ExtensionData.StartingYear,
        marginalNotes = o.ExtensionData.MarginalNotes,
        useMarginalNotesAsFullBody = o.ExtensionData.UseMarginalNotesAsFullBody,
        //userNotes = o.UserNotes,
      };
    }

    private object GetCertificateAsTextModel(Certificate o) {
      return new {
        uid = o.UID,
        type = new {
          uid = o.CertificateType.Name,
          displayName = o.CertificateType.DisplayName,
        },
        status = new {
          uid = o.Status.ToString(),
          code = (char) o.Status,
        },
        text = o.AsText,
      };
    }

    private Certificate ReadCertificate(string certificateUID) {
      base.RequireResource(certificateUID, "certificateUID");

      var certificate = Certificate.TryParse(certificateUID);

      if (certificate == null) {
        throw new ResourceNotFoundException("Empiria.Land.Certificate.NotFound",
                             "There is not a certificate with unique ID '{0}'.",
                             certificateUID);
      }
      return certificate;
    }

    #endregion Private methods

  }  // class ManualCertificatesController

}  // namespace Empiria.Land.WebApi
