/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : OnLineSearchServicesController               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains general web methods for the Empiria Land Online Search Services system.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.WebApi.Models;

namespace Empiria.Land.WebApi {

  /// <summary>Contains general web methods forthe Empiria Land Online Search Services system.</summary>
  public class OnLineSearchServicesController : WebApiController {

    private static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                            ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly string PRINT_SERVICES_SERVER_BASE_ADDRESS =
                                            ConfigurationData.Get<string>("PrintServicesServerBaseAddress");

    private readonly DateTime hashCodeValidationStartDate = DateTime.Parse("2020-01-01");
    private readonly DateTime certificateHashCodeValidationStartDate = DateTime.Parse("2020-01-01");

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/certificates/{certificateUID}")]
    public CollectionModel GetCertificate([FromUri] string certificateUID,
                                          [FromUri] string hash = "") {
      try {
        certificateUID = FormatParameter(certificateUID);
        hash = FormatParameter(hash);

        var certificate = FormerCertificate.TryParse(certificateUID, true);

        if (certificate == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.Certificate.NotFound",
                                              "No tenemos registrado ningún certificado con número '{0}'.\n" +
                                              "Favor de revisar la información proporcionada.",
                                              certificateUID);

        } else if (certificate == null && hash.Length != 0) {
          throw new ResourceNotFoundException("Land.Certificate.InvalidQRCode",
                                              "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                              "referencia a un certificado con número '{0}' que NO está registrado en nuestros archivos.\n\n" +
                                              "MUY IMPORTANTE: Es muy probable que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              certificateUID);

        } else if (certificate != null && certificate.Status == FormerCertificateStatus.Deleted) {
          throw new ResourceNotFoundException("Land.Certificate.Deleted",
                                              $"El certificado {certificate.UID} que está consultando fue ELIMINADO posteriormente a " +
                                              "su impresión, por lo que no tiene ninguna validez oficial.\n\n" +
                                              "Es posible que se lo hayan entregado por equivocación o que haya sido víctima de un fraude.\n\n" +
                                              "MUY IMPORTANTE: Para obtener más información comuníquese de inmediato a la " +
                                              "oficina del Registro Público.",
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

        return new CollectionModel(this.Request, BuildCertificateResponse(certificate, hash),
                                  "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/documents/{documentUID}")]
    public CollectionModel GetDocument([FromUri] string documentUID,
                                       [FromUri] string hash = "") {
      try {
        documentUID = FormatParameter(documentUID);
        hash = FormatParameter(hash);

        var document = RecordingDocument.TryParse(documentUID, true);

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
                   document.AuthorizationTime >= hashCodeValidationStartDate && hash != document.Security.QRCodeSecurityHash()) {
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

        return new CollectionModel(this.Request, BuildRecordingDocumentResponse(document, hash),
                                   "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/resources/{resourceUID}")]
    public CollectionModel GetResource([FromUri] string resourceUID,
                                       [FromUri] string hash = "") {
      try {
        resourceUID = FormatParameter(resourceUID);
        hash = FormatParameter(hash);

        var resource = Resource.TryParseWithUID(resourceUID, true);

        if (resource == null && hash.Length == 0) {
          throw new ResourceNotFoundException("Land.Resource.NotFound",
                                              "No tenemos registrado ningún predio o asociación con folio electrónico '{0}'.\n" +
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
                                              "referencia al predio o asociación con folio electrónico '{0}' que sí tenemos registrado " +
                                              "en nuestros archivos pero el código de validación del QR no es correcto.\n\n" +
                                              "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                              "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                              resourceUID);
        }

        return new CollectionModel(this.Request, BuildResourceStatusResponse(resource),
                                   "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/transactions/{transactionUID}")]
    public CollectionModel GetTransaction([FromUri] string transactionUID,
                                          [FromUri] string hash = "",
                                          [FromUri] string messageUID = "") {
      try {
        LRSTransaction transaction = EnsureValidTransactionRequest(transactionUID, hash, messageUID);

        return new CollectionModel(this.Request, BuildTransactionResponse(transaction, messageUID),
                                  "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v1/online-services/transactions/{transactionUID}/electronic-delivery")]
    public async Task<CollectionModel> ElectronicDelivery([FromUri] string transactionUID,
                                                          [FromUri] string hash = "",
                                                          [FromUri] string messageUID = "") {
      try {
        if (IsPassThroughServer) {
          var apiClient = new OnLineSearchServicesClient();

          var data = await apiClient.ElectronicDelivery(this.Request);

          return new CollectionModel(this.Request, data, "Empiria.PropertyBag");
        }

        LRSTransaction transaction = EnsureValidTransactionRequest(transactionUID, hash, messageUID);

        if (!transaction.Workflow.IsReadyForElectronicDelivery(messageUID)) {
          throw new ResourceNotFoundException("Land.Transaction.NotReadyForElectronicalDelivery",
                                              "El trámite {0} NO está disponible para entrega electrónica.\n\n" +
                                              "Posiblemente su estado cambió después de que usted recibió el mensaje.\n" +
                                              "Si este es el caso, en breve recibirá un nuevo mensaje sobre la situación del mismo.",
                                              transaction.UID);

        }

        transaction.Workflow.DeliverElectronically(messageUID);

        return new CollectionModel(this.Request, BuildTransactionResponse(transaction, messageUID),
                                   "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private List<PropertyBagItem> BuildCertificateResponse(FormerCertificate certificate, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del certificado", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Número de certificado", certificate.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de certificado", certificate.CertificateType.DisplayName, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de expedición", GetDateTime(certificate.IssueTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", certificate.IssuedBy.Alias));

      propertyBag.Add(new PropertyBagItem("Resumen", GetCertificateText(certificate), "small-text"));

      if (!certificate.Property.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Certificado expedido sobre el predio", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Folio real", certificate.Property.UID + "<br/>" +
                                            GetResourceLink(certificate.Property.UID), "enhanced-text"));

        propertyBag.Add(new PropertyBagItem("Clave catastral",
                                             certificate.Property.CadastralKey.Length != 0 ?
                                             certificate.Property.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
        propertyBag.Add(new PropertyBagItem("Descripción", certificate.Property.AsText));
      }

      var unsigned = certificate.UseESign && certificate.Unsigned();

      propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty,
                                    unsigned ? "section-error" : "section"));

      if (hash.Length != 0 && certificate.IssueTime < certificateHashCodeValidationStartDate) {
        propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Código de verificación", certificate.QRCodeSecurityHash(), "bold-text"));
      }
      propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(certificate.GetDigitalSeal()), "mono-space-text"));

      if (unsigned) {
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada",
                        "MUY IMPORTANTE: El certificado NO ES VÁLIDO. NO HA SIDO FIRMADO ELECTRÓNICAMENTE.", "warning-status-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada", certificate.GetDigitalSignature()));
        propertyBag.Add(new PropertyBagItem("Firmado por", certificate.SignedBy.FullName, "bold-text"));
        propertyBag.Add(new PropertyBagItem("Puesto", certificate.SignedBy.JobTitle));
      }

      propertyBag.AddRange(TransactionSectionItems(certificate.Transaction));

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
        throw Assertion.EnsureNoReachThisCode("Unrecognized resource type");
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

      propertyBag.Add(new PropertyBagItem("Información del predio", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio real", property.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Clave catastral",
                                           property.CadastralKey.Length != 0 ?
                                           property.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
      propertyBag.Add(new PropertyBagItem("Descripción", property.Description));

      propertyBag.AddRange(PropertyTractSection(property));


      var physicalRecording = property.Tract.GetLastPhysicalRecording();

      if (!physicalRecording.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Partida origen del predio en libros físicos", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Partida", physicalRecording.AsText));
        propertyBag.Add(new PropertyBagItem("Nota importante","Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todos los predios se deben identificar mediante su folio real, " +
                                            "no con el número de inscripción que tenían en libros físicos."));
      }

      return propertyBag;
    }


    private List<PropertyBagItem> BuildRecordingDocumentResponse(RecordingDocument document, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Sello registral número", document.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de documento", document.DocumentType.DisplayName, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Emitido por", document.IssuedBy.Alias));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(document.PresentationTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Fecha de registro", GetDateTime(document.AuthorizationTime), "date"));
      propertyBag.Add(new PropertyBagItem("Registrado por", document.GetRecordingOfficials()[0].Alias));
      propertyBag.Add(new PropertyBagItem("Resumen", document.Notes, "small-text"));

      if (document.RecordingActs.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Actos jurídicos registrados", String.Empty, "section"));
        foreach (var recordingAct in document.RecordingActs) {
          propertyBag.Add(new PropertyBagItem(recordingAct.DisplayName, "Folio real: " + recordingAct.Resource.UID));
        }
      }

      var uniqueResource = document.GetUniqueInvolvedResource();
      if (!uniqueResource.Equals(Resource.Empty) && uniqueResource is RealEstate) {
        propertyBag.Add(new PropertyBagItem("Documento registral expedido sobre el folio electrónico", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Folio real", uniqueResource.UID + "<br/>" +
                                            GetResourceLink(uniqueResource.UID), "enhanced-text"));

        propertyBag.Add(new PropertyBagItem("Clave catastral",
                                             ((RealEstate) uniqueResource).CadastralKey.Length != 0 ?
                                             ((RealEstate) uniqueResource).CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
        propertyBag.Add(new PropertyBagItem("Descripción", uniqueResource.AsText));
      }

      var unsigned = document.Security.UseESign && document.Security.Unsigned();

      propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty,
                                          unsigned ? "section-error": "section"));

      if (hash.Length != 0 && document.AuthorizationTime < hashCodeValidationStartDate) {
        propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Código de verificación", document.Security.QRCodeSecurityHash(), "bold-text"));
      }
      propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(document.Security.GetDigitalSeal()), "mono-space-text"));
      if (unsigned) {
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada",
                        "MUY IMPORTANTE: El documento NO ES VÁLIDO. NO HA SIDO FIRMADO ELECTRÓNICAMENTE.", "warning-status-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada", document.Security.GetDigitalSignature()));
        propertyBag.Add(new PropertyBagItem("Firmado por", document.Security.GetSignedBy().FullName, "bold-text"));
        propertyBag.Add(new PropertyBagItem("Puesto", document.Security.GetSignedBy().JobTitle));
      }

      propertyBag.AddRange(TransactionSectionItems(document.GetTransaction()));

      return propertyBag;
    }


    private List<PropertyBagItem> BuildTransactionResponse(LRSTransaction transaction, string messageUID) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del trámite", String.Empty, "section"));
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

      if (!transaction.FormerPaymentOrderData.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Línea de captura",
                                            transaction.FormerPaymentOrderData.RouteNumber, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Boleta de pago", transaction.Payments.ReceiptNumbers));
      }

      if (transaction.Payments.Count > 0 && transaction.Payments.Total != decimal.Zero) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos",
                                            transaction.Payments.Total.ToString("C2"), "bold-text"));

      } else if (transaction.Items.TotalFee.Total > 0) {
        propertyBag.Add(new PropertyBagItem("Derechos a pagar",
                                            transaction.Items.TotalFee.Total.ToString("C2"), "bold-text"));

      } else if (transaction.IsFeeWaiverApplicable) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no requiere pago alguno."));
      } else {
        // propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no pagó derechos", "warning-status-text"));

      }

      if (transaction.PresentationTime == ExecutionServer.DateMaxValue) {
        // no-op
      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", GetDateTime(transaction.LastDeliveryTime)));

      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Returned) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                             transaction.Workflow.CurrentStatusName, "warning-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de devolución",
                                            GetDateTime(transaction.LastDeliveryTime), "warning-status-text"));

      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Archived) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Por su naturaleza, este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (transaction.Workflow.IsArchivable) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "in-process-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", "Este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.ToDeliver) {
        if (transaction.Workflow.IsReadyForElectronicDelivery(messageUID)) {
          propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                              "<b>¡Su trámite está listo!</b><br />" +
                                              "Puede pasar a recoger sus documentos, o mejor aún,<br/>" +
                                              "recíbalos electrónicamente oprimiendo el botón de abajo."));
          propertyBag.Add(new PropertyBagItem("Entrega electrónica",
                                              "ELECTRONIC_DELIVERY_COMMAND", "command"));
        } else {
          propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                              "<b>¡Su trámite está listo!</b><br />" +
                                              "Ya puede pasar a recoger sus documentos.", "ok-status-text"));
          propertyBag.Add(new PropertyBagItem("Entrega electrónica",
                                              "Desafortunadamente este trámite no puede entregársele de forma electrónica."));

        }

      } else if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.ToReturn) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "warning-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Desafortunadamente el trámite no procedió.<br />" +
                                            "Requiere pasar a recoger su oficio de devolución.", "warning-status-text"));


      } else {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "in-process-status-text"));

        propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada", "Por COVID-19, desafortunadamente no podemos determinarla."));

        //if (transaction.EstimatedDueTime < DateTime.Today) {
        //  propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada",
        //                                      transaction.EstimatedDueTime.ToString("dd/MMM/yyyy") + " (atrasado)<br/>" +
        //                                      "Lo sentimos, este trámite nos ha llevado hacerlo un poco más de lo normal.",
        //                                      "warning-status-text"));
        //} else {
        //  propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada",
        //                                      transaction.EstimatedDueTime.ToString("dd/MMM/yyyy")));
        //}
      }

      if (transaction.Items.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Conceptos", String.Empty, "section"));
        foreach (var item in transaction.Items) {
          string value = item.Fee.Total == 0m ? item.TreasuryCode.Name : item.Fee.Total.ToString("C2");

          propertyBag.Add(new PropertyBagItem(item.TransactionItemType.DisplayName,
                                              value));
        }
      }

      if ((transaction.Workflow.CurrentStatus != LRSTransactionStatus.Delivered &&
           transaction.Workflow.CurrentStatus != LRSTransactionStatus.Archived) ||
          (transaction.Document.IsEmptyInstance && transaction.GetIssuedCertificates().Count == 0)) {
        return propertyBag;
      }

      propertyBag.Add(new PropertyBagItem("Documentos inscritos y certificados expedidos bajo este trámite",
                                           String.Empty, "section"));

      if (!transaction.Document.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Documento inscrito", transaction.Document.DocumentType.DisplayName + "<br/>" +
                                                                  GetDocumentUIDAsLink(transaction.Document.UID) +
                                                                  GetPrintableDocumentLink(transaction.Document.UID, transaction.Workflow.CurrentStatus, messageUID)));
      }

      foreach (var certificate in transaction.GetIssuedCertificates()) {
        propertyBag.Add(new PropertyBagItem("Certificado", certificate.CertificateType.DisplayName + "<br/>" +
                                                           GetCertificateUIDAsLink(certificate.UID) +
                                                           GetPrintableCertificateLink(certificate.UID, transaction.Workflow.CurrentStatus, messageUID)));
      }

      return propertyBag;
    }


    private LRSTransaction EnsureValidTransactionRequest(string transactionUID, string hash, string messageUID) {
      transactionUID = FormatParameter(transactionUID);
      hash = FormatParameter(hash);
      messageUID = FormatParameter(messageUID);

      var transaction = LRSTransaction.TryParse(transactionUID, true);

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
                                            $"pero el código de validación del QR no es correcto [{hash}].\n\n" +
                                            "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                            "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                            transaction.UID);

      }

      return transaction;
    }


    private string GetDateTime(DateTime dateTime) {
      var temp = dateTime.ToString("dd/MMM/yyyy a la\\s HH:mm");

      return temp.Replace(".", String.Empty) + " hrs.";
    }


    private string GetDigitalText(string sign) {
      return EmpiriaString.DivideLongString(sign.Substring(0, 64), 34, "&#8203;");
    }


    private string GetCertificateUIDAsLink(string certificateUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=certificate&uid={certificateUID}'>" +
             $"{certificateUID}</a>";
    }


    private string GetDocumentUIDAsLink(string documentUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=document&uid={documentUID}'>" +
             $"{documentUID}</a>";
    }


    private string GetPrintableDocumentLink(string documentUID, LRSTransactionStatus status, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return String.Empty;
      }
      if (status == LRSTransactionStatus.Archived || status == LRSTransactionStatus.Delivered) {
        return $" &nbsp; <a target='_blank' href='{PRINT_SERVICES_SERVER_BASE_ADDRESS}/recording.seal.aspx?uid={documentUID}&msg={messageUID}'>" +
               $"Imprimir</a>";
      }
      return String.Empty;
    }


    private string GetPrintableCertificateLink(string certificateUID, LRSTransactionStatus status, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return String.Empty;
      }
      if (status == LRSTransactionStatus.Archived || status == LRSTransactionStatus.Delivered) {
        return $" &nbsp; <a target='_blank' href='{PRINT_SERVICES_SERVER_BASE_ADDRESS}/certificate.aspx?uid={certificateUID}&msg={messageUID}'>" +
               $"Imprimir</a>";
      }
      return String.Empty;
    }


    private string GetResourceLink(string resourceUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=resource&uid={resourceUID}'>" +
             $"Consultar este folio electrónico</a>";
    }


    private string GetTransactionLink(string transactionUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=transaction&uid={transactionUID}'>" +
             $"Consultar este trámite</a>";
    }


    private string ExtractCertificateText(string source) {
      if (source.Length == 0) {
        return source;
      }

      source = source.Replace(Environment.NewLine, " ");
      source = EmpiriaString.TrimAll(source);

      int start = source.IndexOf("<p>");
      int end = source.LastIndexOf("<p> SE EXPIDE EL PRESENTE");

      if (start < end && start != -1) {
        return source.Substring(start, end - start);
      }

      return source;
    }


    private string FixHtmlErrors(string source) {
      source = source.Replace("INSCRITO<strong>", "INSCRITO</strong>");
      source = source.Replace("<BR>", "<br/>");
      source = source.Replace("&", "&amp;");
      source = source.Replace("&amp;nbsp;", "&nbsp;");
      source = source.Replace("&amp;amp;", "&amp;");

      return source;
    }


    private string FormatParameter(string parameter) {
      return EmpiriaString.TrimSpacesAndControl(parameter);
    }


    private string GetCertificateText(FormerCertificate certificate) {
      if (certificate.UseESign) {
        return certificate.AsText;
      } else {
        var certificateText = ExtractCertificateText(certificate.AsText);

        return FixHtmlErrors(certificateText);
      }
    }


    private IEnumerable<PropertyBagItem> TransactionSectionItems(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new List<PropertyBagItem>();
      }

      var items = new List<PropertyBagItem>(8);

      bool transactionHasErrors = !transaction.Workflow.IsFinished ||
                                  (transaction.Payments.Count == 0 &&
                                  transaction.Items.TotalFee.Total > 0 && !transaction.IsFeeWaiverApplicable);

      items.Add(new PropertyBagItem("Información del trámite", String.Empty,
                                    transactionHasErrors ? "section-error" : "section"));

      items.Add(new PropertyBagItem("Número de trámite", transaction.UID + "<br/>" +
                                    GetTransactionLink(transaction.UID), "bold-text"));

      items.Add(new PropertyBagItem("Solicitado por", transaction.RequestedBy));
      items.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(transaction.PresentationTime), "date-time"));

      if (!transaction.FormerPaymentOrderData.IsEmptyInstance) {
        items.Add(new PropertyBagItem("Línea de captura",
                                       transaction.FormerPaymentOrderData.RouteNumber, "bold-text"));
      } else {
        items.Add(new PropertyBagItem("Boleta de pago", transaction.Payments.ReceiptNumbers));
      }

      if (transaction.Payments.Count > 0 && transaction.Payments.Total != decimal.Zero) {
        items.Add(new PropertyBagItem("Pago de derechos",
                                       transaction.Payments.Total.ToString("C2"), "bold-text"));

      } else if (transaction.Items.TotalFee.Total > 0) {
        items.Add(new PropertyBagItem("Derechos a pagar",
                                       transaction.Items.TotalFee.Total.ToString("C2"), "bold-text"));

      } else if (transaction.IsFeeWaiverApplicable) {
        items.Add(new PropertyBagItem("Pago de derechos", "Este trámite no requiere pago alguno."));
      } else {
        items.Add(new PropertyBagItem("Pago de derechos", "Este trámite no pagó derechos.", "warning-status-text"));
      }

      if (transaction.Workflow.IsFinished) {
        items.Add(new PropertyBagItem("Estado del trámite", transaction.Workflow.CurrentStatusName, "ok-status-text"));

        if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Archived) {
          items.Add(new PropertyBagItem("Fecha de entrega",
                                        "Por su naturaleza, este trámite se procesa pero no se entrega al interesado en ventanilla."));
        }

      } else {
        var msg = $"Este trámite aún NO ha sido marcado como entregado al interesado.<br/><br/>" +
                  $"Su estado actual es '{transaction.Workflow.CurrentStatusName}'.<br/><br/>" +
                  $"Debido a lo anterior, <u>usted corre el riesgo</u> de que los documentos registrados " +
                  $"o certificados expedidos en el trámite <u>puedan ser alterados sin previo aviso</u>.<br/><br/>" +
                  $"Favor de comunicarse al Registro Público para que lo cierren correctamente.<br/>";

        items.Add(new PropertyBagItem("Estado del trámite", msg, "warning-status-text"));
      }

      return items;
    }


    private IEnumerable<PropertyBagItem> PropertyTractSection(RealEstate property) {
      var tract = property.Tract.GetFullRecordingActsWithCertificates();

      if (tract.Count == 0) {
        return new List<PropertyBagItem>();
      }

      var items = new List<PropertyBagItem>(tract.Count);

      items.Add(new PropertyBagItem("Últimos actos jurídicos y certificados expedidos sobre el predio", String.Empty, "section"));

      foreach (var tractItem in tract) {
        if (tractItem is RecordingAct) {
          RecordingAct recordingAct = (RecordingAct) tractItem;

          if (!recordingAct.Document.IsClosed ||
               recordingAct.Document.AuthorizationTime == ExecutionServer.DateMinValue) {
            continue;
          }

          items.Add(new PropertyBagItem(recordingAct.Document.AuthorizationTime.ToString("dd/MMM/yyyy"),
                                        recordingAct.DisplayName + "<br/>" +
                                        $"{GetDocumentUIDAsLink(recordingAct.Document.UID)}"));

        } else if (tractItem is FormerCertificate) {
          FormerCertificate certificate = (FormerCertificate) tractItem;

          if (!certificate.IsClosed) {
            continue;
          }
          items.Add(new PropertyBagItem(certificate.IssueTime.ToString("dd/MMM/yyyy"),
                                        $"Certificado de {certificate.CertificateType.DisplayName} <br/>" +
                                        $"{GetCertificateUIDAsLink(certificate.UID)}"));

        } else {
          // no-op
        }
      }  // foreach

      return items;
    }

    #endregion Private methods

  }  // class OnLineSearchServicesController

}  // namespace Empiria.Land.WebApi
