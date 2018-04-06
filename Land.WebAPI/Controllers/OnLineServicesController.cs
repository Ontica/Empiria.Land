/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : OnLineServicesController                         Pattern  : Web API                           *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains general web methods for the Empiria Land Online Services system.                     *
*                                                                                                            *
********************************* Copyright (c) 2014-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.WebApi.Models;

namespace Empiria.Land.WebApi {

  /// <summary>Contains general web methods forthe Empiria Land Online Services system.</summary>
  public class OnLineServicesController : WebApiController {

    private static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                            ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private readonly DateTime hashCodeValidationStartDate = DateTime.Parse("2016-11-24");
    private readonly DateTime certificateHashCodeValidationStartDate = DateTime.Parse("2020-01-01");

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/certificates/{certificateUID}")]
    public SingleObjectModel GetCertificate([FromUri] string certificateUID,
                                            [FromUri] string hash = "") {
      try {
        certificateUID = FormatParameter(certificateUID);
        hash = FormatParameter(hash);

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

        } else if (certificate != null && hash.Length != 0 &&
                   certificate.IssueTime >= certificateHashCodeValidationStartDate && hash != certificate.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.Certificate.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al certificado con número '{0}' que está registrado en nuestros archivos " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su certificado sea falso o que haya sido modificado " +
                                              "posteriormente a la impresión que tiene en la mano.\n\nEsto último significa que su " +
                                              "certificado impreso no es válido y que debe solicitar una reposición del mismo.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              certificateUID);

        }

        return new SingleObjectModel(this.Request, BuildCertificateResponse(certificate, hash),
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
        documentUID = FormatParameter(documentUID);
        hash = FormatParameter(hash);

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

        } else if (document != null && hash.Length != 0  &&
                   document.AuthorizationTime >= hashCodeValidationStartDate && hash != document.QRCodeSecurityHash()) {
          throw new ResourceNotFoundException("Land.RecordingDocument.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia al sello registral con número '{0}' que sí tenemos registrado " +
                                              "pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso o que haya sido modificado " +
                                              "posteriormente a la impresión que tiene en la mano.\n\nEsto último significa que su " +
                                              "documento impreso no es válido y que debe solicitar una reposición del mismo.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              documentUID);

        }

        return new SingleObjectModel(this.Request, BuildRecordingDocumentResponse(document, hash),
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
        resourceUID = FormatParameter(resourceUID);
        hash = FormatParameter(hash);

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
                                              "referencia al predio o asociación con folio real '{0}' que sí tenemos registrado " +
                                              "en nuestros archivos pero el código de validación del QR no es correcto.\n\n" +
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
        transactionUID = FormatParameter(transactionUID);
        hash = FormatParameter(hash);

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
                                              "referencia al trámite con número '{0}' que tenemos registrado en nuestros archivos " +
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

    private List<PropertyBagItem> BuildCertificateResponse(Certificate certificate, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del certificado", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de certificado", certificate.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de certificado", certificate.CertificateType.DisplayName, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de expedición", GetDateTime(certificate.IssueTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", certificate.IssuedBy.Alias));
      propertyBag.Add(new PropertyBagItem("Estado del certificado", certificate.StatusName, "ok-status-text"));

      propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty, "new-section"));
      if (hash.Length != 0 && certificate.IssueTime < certificateHashCodeValidationStartDate) {
        propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Código de verificación", certificate.QRCodeSecurityHash(), "bold-text"));
      }
      propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(certificate.GetDigitalSignature()), "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Firma digital", "Documento firmado de forma autógrafa."));

      if (!certificate.Property.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Certificado expedido sobre el predio", String.Empty, "new-section"));
        propertyBag.Add(new PropertyBagItem("Folio real", certificate.Property.UID + "<br/>" +
                                            GetResourceLink(certificate.Property.UID), "enhanced-text"));

        propertyBag.Add(new PropertyBagItem("Clave catastral",
                                             certificate.Property.CadastralKey.Length != 0 ?
                                             certificate.Property.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
        propertyBag.Add(new PropertyBagItem("Descripción", certificate.Property.AsText()));
      }

      propertyBag.Add(new PropertyBagItem("Información del trámite", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de trámite", certificate.Transaction.UID + "<br/>" +
                                          GetTransactionLink(certificate.Transaction.UID), "bold-text"));
      propertyBag.Add(new PropertyBagItem("Solicitado por", certificate.Transaction.RequestedBy));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(certificate.Transaction.PresentationTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Estado del trámite", certificate.Transaction.Workflow.CurrentStatusName, "ok-status-text"));

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
      propertyBag.Add(new PropertyBagItem("Clave catastral",
                                           property.CadastralKey.Length != 0 ?
                                           property.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
      propertyBag.Add(new PropertyBagItem("Descripción", property.LocationReference));

      var tract = property.Tract.GetRecordingActs().FindAll( (x) => !x.RecordingActType.Equals(RecordingActType.Empty) &&
                                                                     x.Document.AuthorizationTime != ExecutionServer.DateMinValue);
      if (tract.Count != 0) {
        propertyBag.Add(new PropertyBagItem("Últimos actos jurídicos del predio", String.Empty, "new-section"));

        tract.Reverse();
        foreach (var act in tract) {
          propertyBag.Add(new PropertyBagItem(act.Document.AuthorizationTime.ToString("dd/MMM/yyyy"),
                                              act.DisplayName));
        }
      }
      var physicalRecording = property.Tract.GetLastPhysicalRecording();

      if (!physicalRecording.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Partida origen del predio en libros físicos", String.Empty, "new-section"));
        propertyBag.Add(new PropertyBagItem("Partida", physicalRecording.AsText));
        propertyBag.Add(new PropertyBagItem("Nota importante","Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir del año 2015 todos los predios se deben identificar mediante su folio real, " +
                                            "no con la partida que tenían en libros físicos."));
      }

      return propertyBag;
    }

    private List<PropertyBagItem> BuildRecordingDocumentResponse(RecordingDocument document, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Sello registral número", document.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de documento", document.DocumentType.DisplayName, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(document.PresentationTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Fecha de registro", GetDateTime(document.AuthorizationTime), "date"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", document.IssuedBy.Alias));
      propertyBag.Add(new PropertyBagItem("Resumen", document.Notes, "small-text"));

      propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty, "new-section"));
      if (hash.Length != 0 && document.AuthorizationTime < hashCodeValidationStartDate) {
        propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Código de verificación", document.QRCodeSecurityHash(), "bold-text"));
      }
      propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(document.GetDigitalSeal()), "mono-space-text"));
      propertyBag.Add(new PropertyBagItem("Firma digital", "Documento firmado de forma autógrafa."));

      if (document.RecordingActs.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Actos jurídicos registrados", String.Empty, "new-section"));
        foreach (var recordingAct in document.RecordingActs) {
          propertyBag.Add(new PropertyBagItem(recordingAct.DisplayName, "Folio real: " + recordingAct.Resource.UID));
        }
      }

      var uniqueResource = document.GetUniqueInvolvedResource();
      if (!uniqueResource.Equals(Resource.Empty) && uniqueResource is RealEstate) {
        propertyBag.Add(new PropertyBagItem("Documento registral expedido sobre el folio real", String.Empty, "new-section"));
        propertyBag.Add(new PropertyBagItem("Folio real", uniqueResource.UID + "<br/>" +
                                            GetResourceLink(uniqueResource.UID), "enhanced-text"));

        propertyBag.Add(new PropertyBagItem("Clave catastral",
                                             ((RealEstate) uniqueResource).CadastralKey.Length != 0 ?
                                             ((RealEstate) uniqueResource).CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
        propertyBag.Add(new PropertyBagItem("Descripción", ((RealEstate) uniqueResource).AsText()));
      }
      return propertyBag;
    }


    private List<PropertyBagItem> BuildTransactionResponse(LRSTransaction transaction) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del trámite", String.Empty, "new-section"));
      propertyBag.Add(new PropertyBagItem("Número de trámite", transaction.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de trámite", transaction.TransactionType.Name, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de documento", transaction.DocumentType.Name));
      propertyBag.Add(new PropertyBagItem("Solicitado por", transaction.RequestedBy));

      if (transaction.PresentationTime != ExecutionServer.DateMaxValue) {
        propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(transaction.PresentationTime), "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Fecha de presentación",
                                            "Este trámite no ha sido ingresado en ventanilla.", "warning-status-text"));
      }

      if (transaction.Payments.Count > 0 && transaction.Payments.Total != decimal.Zero) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos",
                                            transaction.Payments.Total.ToString("C2"), "bold-text"));
        propertyBag.Add(new PropertyBagItem("Boleta de pago", transaction.Payments.ReceiptNumbers));

      } else if (transaction.Items.TotalFee.Total > 0) {
        propertyBag.Add(new PropertyBagItem("Derechos a pagar",
                                            transaction.Items.TotalFee.Total.ToString("C2"), "bold-text"));

      } else if (transaction.IsFeeWaiverApplicable) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no requiere pago alguno."));
      } else {
        propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no pagó derechos.", "warning-status-text"));
        //propertyBag.Add(new PropertyBagItem("Atendió", transaction.PostedBy.Alias));
      }

      if (transaction.PresentationTime == ExecutionServer.DateMaxValue) {
        // no-op
      } else if (transaction.Workflow.Delivered) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", GetDateTime(transaction.LastDeliveryTime)));
      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Archived) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Por su naturaleza, este trámite se procesa pero no se entrega al interesado."));

      } else if (transaction.Workflow.IsArchivable) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "in-process-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", "Este trámite se procesa pero no se entrega al interesado."));

      } else if (transaction.Workflow.IsReadyForDelivery) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "<b>¡Su trámite está listo!</b><br />" +
                                            "Ya puede pasar a recoger sus documentos.", "ok-status-text"));

      } else {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "in-process-status-text"));

        if (transaction.EstimatedDueTime < DateTime.Today) {
          propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada",
                                              transaction.EstimatedDueTime.ToString("dd/MMM/yyyy") + " (atrasado)<br/>" +
                                              "Lo sentimos, este trámite nos ha llevado hacerlo un poco más de lo normal.",
                                              "warning-status-text"));
        } else {
          propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada",
                                              transaction.EstimatedDueTime.ToString("dd/MMM/yyyy")));
        }
      }

      if (transaction.Items.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Conceptos", String.Empty, "new-section"));
        foreach (var item in transaction.Items) {
          propertyBag.Add(new PropertyBagItem(item.TransactionItemType.DisplayName,
                                              item.Fee.Total.ToString("C2")));
        }
      }

      return propertyBag;
    }

    private string GetDateTime(DateTime dateTime) {
      var temp = dateTime.ToString("dd/MMM/yyyy a la\\s HH:mm");

      return temp.Replace(".", String.Empty) + " hrs.";
    }

    private string GetDigitalText(string sign) {
      return EmpiriaString.DivideLongString(sign.Substring(0, 64), 34, "&#8203;");
    }

    private string GetResourceLink(string resourceUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=resource&uid={resourceUID}'>" +
             $"Consultar este folio real</a>";
    }

    private string GetTransactionLink(string transactionUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=transaction&uid={transactionUID}'>" +
             $"Consultar este trámite</a>";
    }


    private string FormatParameter(string parameter) {
      return EmpiriaString.TrimSpacesAndControl(parameter).ToUpperInvariant();
    }

    #endregion Private methods

  }  // class OnLineServicesController

}  // namespace Empiria.Land.WebApi
