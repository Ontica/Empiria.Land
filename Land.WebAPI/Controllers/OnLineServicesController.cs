/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : OnLineServicesController                         Pattern  : Web API                           *
*  Version   : 2.1                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains general web methods for the Empiria Land Online Services system.                     *
*                                                                                                            *
********************************* Copyright (c) 2014-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Web.Http;

using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.WebApi.Models;

namespace Empiria.Land.WebApi {

  /// <summary>Contains general web methods forthe Empiria Land Online Services system.</summary>
  public class OnLineServicesController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/certificates/{certificateUID}")]
    public SingleObjectModel GetCertificate([FromUri] string certificateUID,
                                            [FromUri] string hash = "") {
      try {
        var certificate = Certificate.TryParse(certificateUID);

        if (certificate == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.Certificate.NotFound",
                                              "No tenemos registrado ningún certificado con número '{0}'.\n" +
                                              "Favor de revisar la información proporcionada.",
                                              certificateUID);

        } else if (certificate == null && hash.Length != 0) {
          throw new ResourceNotFoundException("Land.Certificate.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia a un certificado con número '{0}' que NO está registrado en nuestros archivos.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              certificateUID);

        } else if (certificate != null && hash.Length != 0 && hash != certificate.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.Certificate.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al certificado con número '{0}' que está registrado en nuestros archivos " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              certificateUID);

        }

        return new SingleObjectModel(this.Request, BuildCertificateResponse(certificate),
                                     "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/documents/{documentUID}")]
    public SingleObjectModel GetDocument([FromUri] string documentUID,
                                         [FromUri] string hash = "") {
      try {
        var document = RecordingDocument.TryParse(documentUID);

        if (document == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.RecordingDocument.NotFound",
                                              "No tenemos registrado ningún documento o sello registral con número '{0}'.\n" +
                                              "Favor de revisar la información proporcionada.",
                                              documentUID);

        } else if (document == null && hash.Length != 0) {
          throw new ResourceNotFoundException("Land.RecordingDocument.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al sello registral con número '{0}' que NO está registrado en nuestros archivos.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              documentUID);

        } else if (document != null && hash.Length != 0 && hash != document.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.RecordingDocument.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al sello registral con número '{0}' que está registrado en nuestros archivos " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              documentUID);

        }

        return new SingleObjectModel(this.Request, BuildRecordingDocumentResponse(document),
                                     "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/resources/{resourceUID}")]
    public SingleObjectModel GetResource([FromUri] string resourceUID,
                                         [FromUri] string hash = "") {
      try {
        var resource = Resource.TryParseWithUID(resourceUID);

        if (resource == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.Resource.NotFound",
                                              "No tenemos registrado ningún predio o asociación con folio real '{0}'.\n" +
                                              "Favor de revisar la información proporcionada.",
                                              resourceUID);

        } else if (resource == null && hash.Length != 0) {
          throw new ResourceNotFoundException("Land.Resource.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia a un predio o asociación con folio real '{0}' que NO está " +
                                              "registrado en nuestros archivos.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              resourceUID);

        } else if (resource != null && hash.Length != 0 && hash != resource.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.Resource.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al predio o asociación con folio real '{0}' que está registrado en nuestros archivos " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              resourceUID);
        }

        return new SingleObjectModel(this.Request, BuildResourceStatusResponse(resource),
                                     "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/transactions/{transactionUID}")]
    public SingleObjectModel GetTransaction([FromUri] string transactionUID,
                                            [FromUri] string hash = "") {
      try {
        var transaction = LRSTransaction.TryParse(transactionUID);

        if (transaction == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.Transaction.NotFound",
                                              "No tenemos registrado ningún trámite con número '{0}'.\n" +
                                              "Favor de revisar la información proporcionada.",
                                              transactionUID);

        } else if (transaction == null && hash.Length != 0) {
          throw new ResourceNotFoundException("Land.Transaction.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia a un trámite con número '{0}' que NO está registrado en nuestros archivos.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              transactionUID);

        } else if (transaction != null && hash.Length != 0 && hash != transaction.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.Transaction.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al trámite con número '{0}' que SÍ está registrado en nuestros archivos " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              transactionUID);

        }

        return new SingleObjectModel(this.Request, BuildTransactionResponse(transaction),
                                    "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private List<PropertyBagItem> BuildCertificateResponse(Certificate certificate) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del certificado", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de certificado", certificate.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de certificado", certificate.CertificateType.DisplayName, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de expedición", certificate.IssueTime, "date-time"));
      propertyBag.Add(new PropertyBagItem("Sello digital", certificate.GetDigitalSeal(), "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Firma digital",
                                          EmpiriaString.DivideLongString(certificate.GetDigitalSignature(), 32, " "),
                                          "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", certificate.IssuedBy.Nickname));
      propertyBag.Add(new PropertyBagItem("Estado del certificado", certificate.StatusName, "ok-status-text"));

      if (!certificate.Property.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Certificado expedido sobre el predio", String.Empty, "new-section"));
        propertyBag.Add(new PropertyBagItem("Folio real", certificate.Property.UID, "enhanced-text"));
        propertyBag.Add(new PropertyBagItem("Clave catastral", certificate.Property.CadastralKey, "bold-text"));
        propertyBag.Add(new PropertyBagItem("Descripción", certificate.Property.AsText()));
      }

      propertyBag.Add(new PropertyBagItem("Información del trámite", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de trámite", certificate.Transaction.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Solicitado por", certificate.Transaction.RequestedBy));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", certificate.Transaction.PresentationTime, "date-time"));
      propertyBag.Add(new PropertyBagItem("Estado del trámite", certificate.Transaction.Workflow.CurrentStatusName, "warning-status-text"));

      return propertyBag;
    }

    private List<PropertyBagItem> BuildResourceStatusResponse(Resource resource) {
      if (resource is RealEstate) {
        return BuildRealEstateStatusResponse((RealEstate) resource);
      } else if (resource is Association) {
        return BuildAssociationStatusResponse((Association) resource);
      } else if (resource is NoPropertyResource) {
        return BuildNoPropertyStatusResponse((NoPropertyResource) resource);
      } else {
        throw Assertion.AssertNoReachThisCode("Unrecognized resource type");
      }
    }

    private List<PropertyBagItem> BuildAssociationStatusResponse(Association resource) {
      throw new NotImplementedException();
    }

    private List<PropertyBagItem> BuildNoPropertyStatusResponse(NoPropertyResource resource) {
      throw new NotImplementedException();
    }

    private List<PropertyBagItem> BuildRealEstateStatusResponse(RealEstate property) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del predio", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Folio real", property.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Clave catastral", property.CadastralKey, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Descripción", property.LocationReference));

      var tract = property.GetRecordingActsTract();

      if (tract.Count != 0) {
        propertyBag.Add(new PropertyBagItem("Últimos actos jurídicos del predio", String.Empty, "new-section"));
        foreach (var act in tract) {
          propertyBag.Add(new PropertyBagItem(act.Document.AuthorizationTime.ToString("dd/MMM/yyyy"),
                                              act.DisplayName));
        }
      }

      return propertyBag;
    }

    private List<PropertyBagItem> BuildRecordingDocumentResponse(RecordingDocument document) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Sello registral número", document.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de documento", document.DocumentType.DisplayName, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", document.PresentationTime, "date-time"));
      propertyBag.Add(new PropertyBagItem("Fecha de registro", document.AuthorizationTime, "date"));
      propertyBag.Add(new PropertyBagItem("Sello digital", EmpiriaString.DivideLongString(document.GetDigitalSeal(), 32, " "),
                                          "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Firma digital",
                                          EmpiriaString.DivideLongString(document.GetDigitalSignature(), 32, " "),
                                          "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", document.IssuedBy.Nickname));
      propertyBag.Add(new PropertyBagItem("Estado del documento", document.Status, "ok-status-text"));
      propertyBag.Add(new PropertyBagItem("Resumen", document.Notes));

      if (document.RecordingActs.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Actos jurídicos registrados", String.Empty, "new-section"));
        foreach (var recordingAct in document.RecordingActs) {
          propertyBag.Add(new PropertyBagItem(recordingAct.DisplayName, "Folio real: " + recordingAct.Resource.UID));
        }
      }
      var uniqueResource = document.GetUniqueInvolvedResource();
      if (!uniqueResource.Equals(Resource.Empty)) {
        propertyBag.Add(new PropertyBagItem("Documento registral expedido sobre el folio real", String.Empty, "new-section"));
        propertyBag.Add(new PropertyBagItem("Folio real", uniqueResource.UID, "enhanced-text"));
        //propertyBag.Add(new PropertyBagItem("Clave catastral", uniqueResource.CadastralKey, "bold-text"));
        //propertyBag.Add(new PropertyBagItem("Descripción", uniqueResource.LocationReference));
      }
      return propertyBag;
    }


    private List<PropertyBagItem> BuildTransactionResponse(LRSTransaction transaction) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del trámite", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de trámite", transaction.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de trámite", transaction.TransactionType.Name));
      propertyBag.Add(new PropertyBagItem("Solicitado por", transaction.RequestedBy));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", transaction.PresentationTime, "date-time"));
      propertyBag.Add(new PropertyBagItem("Boleta de pago", transaction.Payments.ReceiptNumbers));
      propertyBag.Add(new PropertyBagItem("Pago de derechos por", transaction.Payments.Total, "money"));
      propertyBag.Add(new PropertyBagItem("Estado del trámite", transaction.Workflow.CurrentStatusName, "warning-status-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de entrega", transaction.ClosingTime, "date"));

      return propertyBag;
    }

    #endregion Private methods

  }  // class OnLineServicesController

}  // namespace Empiria.Land.WebApi
