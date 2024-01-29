/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Services provider                     *
*  Type     : OnLineSearchPropertiesBagBuilder             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Builds PropertyBagItem lists for objects invoked using Online Search Services.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Certification;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi {

  /// <summary>Builds PropertyBagItem lists for objects invoked using Online Search Services.</summary>
  internal class OnLineSearchPropertiesBagBuilder {

    private static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                        ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly string PRINT_SERVICES_SERVER_BASE_ADDRESS =
                                            ConfigurationData.Get<string>("PrintServicesServerBaseAddress");

    private readonly DateTime certificateHashCodeValidationStartDate = DateTime.Parse("2020-01-01");
    private readonly DateTime hashCodeValidationStartDate = DateTime.Parse("2020-01-01");

    #region Internal methods

    internal List<PropertyBagItem> BuildCertificate(string certificateUID, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      var certificate = FormerCertificate.TryParse(certificateUID, true);

      propertyBag.Add(new PropertyBagItem("Información del certificado", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Número de certificado", certificate.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de certificado", certificate.CertificateType.DisplayName, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Fecha de expedición", GetDateTime(certificate.IssueTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Elaborado por", certificate.IssuedBy.ShortName));

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


    internal List<PropertyBagItem> BuildResourceStatus(string resourceUID) {

      var resource = Resource.TryParseWithUID(resourceUID, true);

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


    internal List<PropertyBagItem> BuildRecordingDocument(string documentUID, string hash) {
      var propertyBag = new List<PropertyBagItem>(16);

      var document = RecordingDocument.TryParse(documentUID, true);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Sello registral número", document.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo de documento", document.DocumentType.DisplayName, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Emitido por", document.IssuedBy.ShortName));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(document.PresentationTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Fecha de registro", GetDateTime(document.AuthorizationTime), "date"));
      propertyBag.Add(new PropertyBagItem("Registrado por", document.GetRecordingOfficials()[0].ShortName));
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
                                          unsigned ? "section-error" : "section"));

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


    internal List<PropertyBagItem> BuildTransaction(string transactionUID, string messageUID) {
      var propertyBag = new List<PropertyBagItem>(16);

      var transaction = LRSTransaction.TryParse(transactionUID, true);

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

      if (!transaction.PaymentData.FormerPaymentOrderData.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Línea de captura",
                                            transaction.PaymentData.FormerPaymentOrderData.RouteNumber, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Boleta de pago", transaction.PaymentData.Payments.ReceiptNumbers));
      }

      if (transaction.PaymentData.Payments.Count > 0 && transaction.PaymentData.Payments.Total != decimal.Zero) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos",
                                            transaction.PaymentData.Payments.Total.ToString("C2"), "bold-text"));

      } else if (transaction.Services.TotalFee.Total > 0) {
        propertyBag.Add(new PropertyBagItem("Derechos a pagar",
                                            transaction.Services.TotalFee.Total.ToString("C2"), "bold-text"));

      } else if (transaction.PaymentData.IsFeeWaiverApplicable) {
        propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no requiere pago alguno."));
      } else {
        // propertyBag.Add(new PropertyBagItem("Pago de derechos", "Este trámite no pagó derechos", "warning-status-text"));

      }

      if (transaction.PresentationTime == ExecutionServer.DateMaxValue) {
        // no-op
      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Delivered) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", GetDateTime(transaction.LastDeliveryTime)));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Returned) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                             transaction.Workflow.CurrentStatusName, "warning-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de devolución",
                                            GetDateTime(transaction.LastDeliveryTime), "warning-status-text"));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Archived) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Por su naturaleza, este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (transaction.Workflow.IsArchivable) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatusName, "in-process-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", "Este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.ToDeliver) {
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

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.ToReturn) {
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

      if (transaction.Services.Count > 0) {
        propertyBag.Add(new PropertyBagItem("Conceptos", String.Empty, "section"));
        foreach (var service in transaction.Services) {
          string value = service.Fee.Total == 0m ? service.TreasuryCode.Name : service.Fee.Total.ToString("C2");

          propertyBag.Add(new PropertyBagItem(service.ServiceType.DisplayName,
                                              value));
        }
      }

      if ((transaction.Workflow.CurrentStatus != TransactionStatus.Delivered &&
           transaction.Workflow.CurrentStatus != TransactionStatus.Archived) ||
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


    #endregion Internal methods

    #region Private methods

    private List<PropertyBagItem> BuildAssociationStatusResponse(Association association) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información de la sociedad", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio único", association.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Nombre", association.Name, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo", association.Kind));

      propertyBag.AddRange(ResourceTractSection(association));

      BookEntry lastBookEntry = association.Tract.GetLastBookEntry();

      if (!lastBookEntry.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Partida origen de la sociedad en libros físicos", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Partida", lastBookEntry.AsText));
        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todas las sociedades se deben identificar mediante su folio único, " +
                                            "no con el número de inscripción que tenían en libros físicos."));
      }

      return propertyBag;
    }


    private List<PropertyBagItem> BuildNoPropertyStatusResponse(NoPropertyResource noproperty) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio electrónico", noproperty.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Nombre", noproperty.Name, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo", noproperty.Kind));
      propertyBag.Add(new PropertyBagItem("Descripción", noproperty.Description));

      propertyBag.AddRange(ResourceTractSection(noproperty));

      BookEntry lastBookEntry = noproperty.Tract.GetLastBookEntry();

      if (!lastBookEntry.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Partida origen del documento en libros físicos", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Partida", lastBookEntry.AsText));
        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todos los documentos se deben identificar mediante su folio electrónico, " +
                                            "no con el número de inscripción que tenían en libros físicos."));
      }

      return propertyBag;
    }


    private List<PropertyBagItem> BuildRealEstateStatusResponse(RealEstate property) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del predio", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio real", property.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Clave catastral",
                                           property.CadastralKey.Length != 0 ?
                                           property.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
      propertyBag.Add(new PropertyBagItem("Descripción", property.Description));

      propertyBag.AddRange(ResourceTractSection(property));


      BookEntry lastBookEntry = property.Tract.GetLastBookEntry();

      if (!lastBookEntry.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Partida origen del predio en libros físicos", String.Empty, "section"));
        propertyBag.Add(new PropertyBagItem("Partida", lastBookEntry.AsText));
        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todos los predios se deben identificar mediante su folio real, " +
                                            "no con el número de inscripción que tenían en libros físicos."));
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


    private string GetCertificateUIDAsLink(string certificateUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=certificate&uid={certificateUID}'>" +
             $"{certificateUID}</a>";
    }


    private string GetDocumentUIDAsLink(string documentUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=document&uid={documentUID}'>" +
             $"{documentUID}</a>";
    }


    private string GetPrintableDocumentLink(string documentUID, TransactionStatus status, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return String.Empty;
      }
      if (status == TransactionStatus.Archived || status == TransactionStatus.Delivered) {
        return $" &nbsp; <a target='_blank' href='{PRINT_SERVICES_SERVER_BASE_ADDRESS}/recording.seal.aspx?uid={documentUID}&msg={messageUID}'>" +
               $"Imprimir</a>";
      }
      return String.Empty;
    }


    private string GetPrintableCertificateLink(string certificateUID, TransactionStatus status, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return String.Empty;
      }
      if (status == TransactionStatus.Archived || status == TransactionStatus.Delivered) {
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
                                  (transaction.PaymentData.Payments.Count == 0 &&
                                  transaction.Services.TotalFee.Total > 0 && !transaction.PaymentData.IsFeeWaiverApplicable);

      items.Add(new PropertyBagItem("Información del trámite", String.Empty,
                                    transactionHasErrors ? "section-error" : "section"));

      items.Add(new PropertyBagItem("Número de trámite", transaction.UID + "<br/>" +
                                    GetTransactionLink(transaction.UID), "bold-text"));

      items.Add(new PropertyBagItem("Solicitado por", transaction.RequestedBy));
      items.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(transaction.PresentationTime), "date-time"));

      if (!transaction.PaymentData.FormerPaymentOrderData.IsEmptyInstance) {
        items.Add(new PropertyBagItem("Línea de captura",
                                       transaction.PaymentData.FormerPaymentOrderData.RouteNumber, "bold-text"));
      } else {
        items.Add(new PropertyBagItem("Boleta de pago", transaction.PaymentData.Payments.ReceiptNumbers));
      }

      if (transaction.PaymentData.Payments.Count > 0 && transaction.PaymentData.Payments.Total != decimal.Zero) {
        items.Add(new PropertyBagItem("Pago de derechos",
                                       transaction.PaymentData.Payments.Total.ToString("C2"), "bold-text"));

      } else if (transaction.Services.TotalFee.Total > 0) {
        items.Add(new PropertyBagItem("Derechos a pagar",
                                       transaction.Services.TotalFee.Total.ToString("C2"), "bold-text"));

      } else if (transaction.PaymentData.IsFeeWaiverApplicable) {
        items.Add(new PropertyBagItem("Pago de derechos", "Este trámite no requiere pago alguno."));
      } else {
        items.Add(new PropertyBagItem("Pago de derechos", "Este trámite no pagó derechos.", "warning-status-text"));
      }

      if (transaction.Workflow.IsFinished) {
        items.Add(new PropertyBagItem("Estado del trámite", transaction.Workflow.CurrentStatusName, "ok-status-text"));

        if (transaction.Workflow.CurrentStatus == TransactionStatus.Archived) {
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


    private IEnumerable<PropertyBagItem> ResourceTractSection(Resource property) {
      var tract = property.Tract.GetFullRecordingActsWithCertificates();

      if (tract.Count == 0) {
        return new List<PropertyBagItem>();
      }

      var items = new List<PropertyBagItem>(tract.Count);

      items.Add(new PropertyBagItem("Últimos actos jurídicos y certificados expedidos", String.Empty, "section"));

      foreach (var tractItem in tract) {
        if (tractItem is RecordingAct) {
          RecordingAct recordingAct = (RecordingAct) tractItem;

          if (!recordingAct.Document.IsClosed ||
               recordingAct.Document.AuthorizationTime == ExecutionServer.DateMinValue) {
            continue;
          }

          items.Add(new PropertyBagItem(recordingAct.Document.AuthorizationTime.ToString("dd/MMM/yyyy"),
                                        (recordingAct.Kind.Length != 0 ? recordingAct.Kind : recordingAct.DisplayName) + "<br/>" +
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

  }  // class OnLineSearchPropertiesBagBuilder

}  // namespace Empiria.Land.WebApi
