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

using Empiria.Land.Certificates.Adapters;

using Empiria.Land.Registration;
using Empiria.Land.Transactions;
using Empiria.Land.Transactions.Workflow;

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

      //CertificateDto certificate;

      //using (var usecase = CertificatesUseCases.UseCaseInteractor()) {
      //  certificate = usecase.GetCertificateWithID(certificateUID);
      //}

      //propertyBag.Add(new PropertyBagItem("Información del certificado", String.Empty, "section"));
      //propertyBag.Add(new PropertyBagItem("Número de certificado", certificate.UID, "enhanced-text"));
      //propertyBag.Add(new PropertyBagItem("Tipo de certificado", certificate.Type, "enhanced-text"));
      //propertyBag.Add(new PropertyBagItem("Fecha de expedición", GetDateTime(certificate.IssueTime), "date-time"));
      //propertyBag.Add(new PropertyBagItem("Elaborado por", certificate.IssuedBy));

      //propertyBag.Add(new PropertyBagItem("Resumen", certificate.AsText, "small-text"));

      //if (certificate.OverRecordableSubject) {
      //  RealEstate resource = RealEstate.TryParseWithUID(certificate.RecordableSubject.UID);

      //  propertyBag.Add(new PropertyBagItem("Certificado expedido sobre el predio", String.Empty, "section"));
      //  propertyBag.Add(new PropertyBagItem("Folio real", GetResourceLink(resource) + "enhanced-text"));

      //  propertyBag.Add(new PropertyBagItem("Clave catastral",
      //                                       resource.CadastralKey.Length != 0 ?
      //                                       resource.CadastralKey : "Clave catastral no proporcionada.", "bold-text"));
      //  propertyBag.Add(new PropertyBagItem("Descripción", resource.AsText));
      //}

      //var unsigned = certificate.UseESign && certificate.Unsigned();

      //propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty,
      //                              unsigned ? "section-error" : "section"));

      //if (hash.Length != 0 && certificate.IssueTime < certificateHashCodeValidationStartDate) {
      //  propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      //} else {
      //  propertyBag.Add(new PropertyBagItem("Código de verificación", certificate.QRCodeSecurityHash(), "bold-text"));
      //}
      //propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(certificate.GetDigitalSeal()), "mono-space-text"));

      //if (unsigned) {
      //  propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada",
      //                  "MUY IMPORTANTE: El certificado NO ES VÁLIDO. NO HA SIDO FIRMADO ELECTRÓNICAMENTE.", "warning-status-text"));
      //} else {
      //  propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada", certificate.GetDigitalSignature()));
      //  propertyBag.Add(new PropertyBagItem("Firmado por", certificate.SignedBy.FullName, "bold-text"));
      //  propertyBag.Add(new PropertyBagItem("Puesto", certificate.SignedBy.JobTitle));
      //}

      //propertyBag.AddRange(TransactionSectionItems(certificate.Transaction));

      return propertyBag;
    }


    internal List<PropertyBagItem> BuildResourceStatus(string resourceUID) {

      var resource = Resource.TryParseWithUID(resourceUID);

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


    internal List<PropertyBagItem> BuildLandRecord(string landRecordUID, string hash) {
      var propertyBag = new List<PropertyBagItem>(32);

      var landRecord = LandRecord.TryParse(landRecordUID);

      propertyBag.Add(new PropertyBagItem("Información del sello registral", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Sello registral número", landRecord.UID, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Instrumento", landRecord.Instrument.AsText));
      propertyBag.Add(new PropertyBagItem("Fecha de presentación", GetDateTime(landRecord.PresentationTime), "date-time"));
      propertyBag.Add(new PropertyBagItem("Fecha de registro", GetDateTime(landRecord.AuthorizationTime), "date"));
      propertyBag.Add(new PropertyBagItem("Registrado por", landRecord.AuthorizedBy.FullName));

      if (landRecord.Instrument.Summary.Length != 0) {
        propertyBag.Add(new PropertyBagItem("Resumen", landRecord.Instrument.Summary, "small-text"));
      }

      BuildLandRecordRecordingActs(landRecord, propertyBag);

      BuildLandRecordSecurityData(landRecord, hash, propertyBag);

      propertyBag.AddRange(TransactionSectionItems(landRecord.Transaction));

      return propertyBag;
    }


    internal List<PropertyBagItem> BuildTransaction(string transactionUID, string messageUID) {
      var propertyBag = new List<PropertyBagItem>(32);

      var transaction = LRSTransaction.TryParse(transactionUID);

      BuildTransactionMainInfoSection(transaction, propertyBag);

      BuildTransactionPaymentSection(transaction, propertyBag);

      BuildTransactionStatusSection(transaction, propertyBag, messageUID);

      BuildTransactionServicesSection(transaction, propertyBag);

      if ((transaction.Workflow.CurrentStatus != TransactionStatus.Delivered &&
           transaction.Workflow.CurrentStatus != TransactionStatus.Archived) ||
          (transaction.LandRecord.IsEmptyInstance && !transaction.HasCertificates)) {
        return propertyBag;
      }

      BuildTransactionDocumentsSection(transaction, propertyBag, messageUID);

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

      propertyBag.AddRange(GetResourceTractSection(association));

      BookEntrySection(association, propertyBag);

      return propertyBag;
    }


    private void BuildLandRecordRecordingActs(LandRecord landRecord, List<PropertyBagItem> propertyBag) {
      if (landRecord.RecordingActs.Count == 0) {
        propertyBag.Add(new PropertyBagItem("Actos jurídicos registrados", String.Empty, "section-error"));
        propertyBag.Add(new PropertyBagItem("Nota importante",
                                            "Este documento NO TIENE actos jurídicos registrados.<br/>" +
                                            "Debe acudir a la oficina del Registro Público de la Propiedad, " +
                                            "ya que es MUY posible que la inscripción de su propiedad" +
                                            "haya sido alterada indebidamente.", "warning-status-text"));
        return;
      }

      propertyBag.Add(new PropertyBagItem("Actos jurídicos registrados", String.Empty, "section"));

      var lastProcessedResource = Resource.Empty;

      foreach (var recordingAct in landRecord.RecordingActs) {
        string text = string.Empty;

        var showResourceInfo = lastProcessedResource.Distinct(recordingAct.Resource);

        if (recordingAct.Resource is RealEstate) {
          text = showResourceInfo ? $"{GetResourceLink(recordingAct.Resource)}<br/>" :
                                    $"<label class='small-text'>(Mismo predio del acto anterior)</label><br/>";
        } else if (recordingAct.Resource is Association) {
          text = showResourceInfo ? $"{GetResourceLink(recordingAct.Resource)}<br/>" :
                                    $"<label class='small-text'>(Misma sociedad del acto anterior)</label><br/>";
        } else if (recordingAct.Resource is NoPropertyResource) {
          text = showResourceInfo ? $"{GetResourceLink(recordingAct.Resource)}<br/>" :
                                    $"<label class='small-text'>(Mismo documento del acto anterior)</label><br/>";
        } else {
          throw Assertion.EnsureNoReachThisCode();
        }

        if (showResourceInfo && recordingAct.Resource is RealEstate realEstate) {
          if (realEstate.CadastralKey.Length != 0) {
            text += $"<label class='small-text'>Clave catastral:</label> " +
                    $"<span class='bold-text'>{realEstate.CadastralKey}</span><br/>";
          } else {
            text += $"<span class='small-text warning-status-text'>Clave catastral no registrada</span><br/>";
          }
        }

        if (recordingAct.OperationAmount != 0) {
          text += $"<label class='small-text'>Monto de la operación:</label> " +
                  $"{recordingAct.OperationCurrency.Format(recordingAct.OperationAmount)}<br/>";
        }

        var title = "<span class='bold-text'>" +
                        (recordingAct.Kind.Length != 0 ? recordingAct.Kind: recordingAct.DisplayName) +
                    "</span>";

        propertyBag.Add(new PropertyBagItem(title, text));

        lastProcessedResource = recordingAct.Resource;
      }
    }


    private void BuildLandRecordSecurityData(LandRecord landRecord, string hash, List<PropertyBagItem> propertyBag) {
      var securityData = landRecord.SecurityData;

      propertyBag.Add(new PropertyBagItem("Verificación de elementos de seguridad", String.Empty,
                                          securityData.IsSigned ? "section": "section-error"));

      if (securityData.IsUnsigned && securityData.UsesESign) {
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada",
               "MUY IMPORTANTE: El documento NO ES VÁLIDO. NO HA SIDO FIRMADO ELECTRÓNICAMENTE.", "warning-status-text"));
        return;
      } else if (securityData.IsUnsigned && !securityData.UsesESign) {
        propertyBag.Add(new PropertyBagItem("Firma",
               "MUY IMPORTANTE: El documento NO ES VÁLIDO YA QUE NO HA SIDO FIRMADO.", "warning-status-text"));
        return;
      }

      if (hash.Length != 0 && landRecord.AuthorizationTime < hashCodeValidationStartDate) {
        propertyBag.Add(new PropertyBagItem("Código de verificación", hash, "bold-text"));
      } else {
        propertyBag.Add(new PropertyBagItem("Código de verificación", securityData.SecurityHash, "bold-text"));
      }

      propertyBag.Add(new PropertyBagItem("Sello digital", GetDigitalText(securityData.DigitalSeal), "bold-text mono-space-text"));


      if (securityData.UsesESign) {

        propertyBag.Add(new PropertyBagItem("Identificador de la firma electrónica", securityData.SignGuid, "bold-text mono-space-text"));
        propertyBag.Add(new PropertyBagItem("Cadena de digestión (datos estampillados)",
                                            EmpiriaString.DivideLongString(securityData.Digest, 34, "&#8203;"), "bold-text mono-space-text"));
        propertyBag.Add(new PropertyBagItem("Firma electrónica avanzada", securityData.DigitalSignature, "bold-text mono-space-text"));

      } else {

        propertyBag.Add(new PropertyBagItem("Firma", securityData.DigitalSignature, "bold-text"));
      }

      propertyBag.Add(new PropertyBagItem("Firmado por", securityData.SignedBy.FullName, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Puesto", securityData.SignedByJobTitle));
      propertyBag.Add(new PropertyBagItem("Fecha y hora de firmado", securityData.SignedTime.ToString("dd/MMMM/yyyy HH:mm:ss")));
    }


    private List<PropertyBagItem> BuildNoPropertyStatusResponse(NoPropertyResource noproperty) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del documento", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio electrónico", noproperty.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Nombre", noproperty.Name, "bold-text"));
      propertyBag.Add(new PropertyBagItem("Tipo", noproperty.Kind));
      propertyBag.Add(new PropertyBagItem("Descripción", noproperty.Description));

      propertyBag.AddRange(GetResourceTractSection(noproperty));

      BookEntrySection(noproperty, propertyBag);

      return propertyBag;
    }


    private List<PropertyBagItem> BuildRealEstateStatusResponse(RealEstate realEstate) {
      var propertyBag = new List<PropertyBagItem>(16);

      propertyBag.Add(new PropertyBagItem("Información del predio", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Folio real", realEstate.UID, "enhanced-text"));
      propertyBag.Add(new PropertyBagItem("Clave catastral",
                                           realEstate.CadastralKey.Length != 0 ?
                                           realEstate.CadastralKey : "<span class='warning-status-text '>Clave catastral no proporcionada.", "bold-text"));
      propertyBag.Add(new PropertyBagItem("Descripción", realEstate.Description));

      propertyBag.AddRange(GetResourceTractSection(realEstate));

      BookEntrySection(realEstate, propertyBag);

      return propertyBag;
    }

    private void BookEntrySection(Resource recordableSubject, List<PropertyBagItem> propertyBag) {
      BookEntry lastBookEntry = recordableSubject.Tract.GetLastDomainBookEntry();

      if (lastBookEntry.IsEmptyInstance) {
        return;
      }
      propertyBag.Add(new PropertyBagItem("Partida origen del predio en libros físicos", String.Empty, "section"));
      propertyBag.Add(new PropertyBagItem("Partida", lastBookEntry.AsText));

      if (recordableSubject is RealEstate) {

        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todos los predios se deben identificar mediante su folio real, " +
                                            "no con el número de inscripción que tenían en libros físicos."));

      } else if (recordableSubject is Association) {

        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todas las sociedades se deben identificar mediante su folio único, " +
                                            "no con el número de inscripción que tenían en libros físicos."));

      } else if (recordableSubject is NoPropertyResource) {

        propertyBag.Add(new PropertyBagItem("Nota importante", "Los datos de la partida sólo se muestran con fines informativos.<br/>" +
                                            "A partir de junio del año 2022, todos los documentos se deben identificar mediante su folio electrónico, " +
                                            "no con el número de inscripción que tenían en libros físicos."));
      }
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


    private string GetLandRecordUIDAsLink(string landRecordUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=document&uid={landRecordUID}'>" +
             $"{landRecordUID}</a>";
    }


    private string GetPrintableLandRecordLink(string landRecordUID, TransactionStatus status, string messageUID) {
      if (String.IsNullOrWhiteSpace(messageUID)) {
        return String.Empty;
      }
      if (status == TransactionStatus.Archived || status == TransactionStatus.Delivered) {
        return $" &nbsp; <a target='_blank' href='{PRINT_SERVICES_SERVER_BASE_ADDRESS}/recording.seal.aspx?uid={landRecordUID}&msg={messageUID}'>" +
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


    private string GetResourceLink(Resource resource) {
      if (resource is RealEstate) {
        return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=realestate&uid={resource.UID}'>" +
                $"{resource.UID}</a> <label class='small-text'>[folio real predio]</label>";
      } else if (resource is Association) {
        return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=association&uid={resource.UID}'>" +
                $"{resource.UID}</a> <label class='small-text'>[folo único sociedad]</label>";
      } else if (resource is NoPropertyResource) {
        return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=noproperty&uid={resource.UID}'>" +
                $"{resource.UID}</a> <label class='small-text'>[documento con folio electrónico]</label>";
      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    private string GetTransactionLink(string transactionUID) {
      return $"<a href='{SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?type=transaction&uid={transactionUID}'>" +
             $"Consultar este trámite</a>";
    }


    private IEnumerable<PropertyBagItem> TransactionSectionItems(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new List<PropertyBagItem>();
      }

      var items = new List<PropertyBagItem>(8);

      bool transactionHasErrors = !LRSWorkflowRules.IsFinished(transaction) ||
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

      if (LRSWorkflowRules.IsFinished(transaction)) {
        items.Add(new PropertyBagItem("Estado del trámite", transaction.Workflow.CurrentStatus.GetStatusName(), "ok-status-text"));

        if (transaction.Workflow.CurrentStatus == TransactionStatus.Archived) {
          items.Add(new PropertyBagItem("Fecha de entrega",
                                        "Por su naturaleza, este trámite se procesa pero no se entrega al interesado en ventanilla."));
        }

      } else {
        var msg = $"Este trámite aún NO ha sido marcado como entregado al interesado.<br/><br/>" +
                  $"Su estado actual es '{transaction.Workflow.CurrentStatus.GetStatusName()}'.<br/><br/>" +
                  $"Debido a lo anterior, <u>usted corre el riesgo</u> de que los documentos registrados " +
                  $"o certificados expedidos en el trámite <u>puedan ser alterados sin previo aviso</u>.<br/><br/>" +
                  $"Favor de comunicarse al Registro Público para que lo cierren correctamente.<br/>";

        items.Add(new PropertyBagItem("Estado del trámite", msg, "warning-status-text"));
      }

      return items;
    }


    private IEnumerable<PropertyBagItem> GetResourceTractSection(Resource property) {
      FixedList<IResourceTractItem> tract = property.Tract.GetFullRecordingActsWithCertificates();

      if (tract.Count == 0) {
        return new List<PropertyBagItem>();
      }

      var items = new List<PropertyBagItem>(tract.Count);

      items.Add(new PropertyBagItem("Últimos actos jurídicos y certificados expedidos", String.Empty, "section"));

      var lastProcessedDocument = LandRecord.Empty;

      foreach (var tractItem in tract) {
        if (tractItem is RecordingAct) {

          RecordingAct recordingAct = (RecordingAct) tractItem;

          if (lastProcessedDocument.Equals(recordingAct.LandRecord)) {
            continue;
          }

          if (!recordingAct.LandRecord.IsClosed ||
               recordingAct.LandRecord.AuthorizationTime == ExecutionServer.DateMinValue) {
            continue;
          }

          var documentActs = recordingAct.LandRecord.RecordingActs.FindAll(x => x.Resource.Equals(recordingAct.Resource) ||
                                                                                x.RelatedResource.Equals(recordingAct.Resource));
          var documentActsText = string.Empty;

          for (int i = 0; i < documentActs.Count; i++) {
            documentActsText += "<span>" +
                                 $"{i + 1}) " +
                                 (documentActs[i].Kind.Length != 0 ? documentActs[i].Kind : documentActs[i].DisplayName) +
                                "</span><br/>";
            if (documentActs[i].OperationAmount != 0) {
              documentActsText += $"<label class='small-text left-indented'>Monto de la operación:</label> " +
                                  $"{documentActs[i].OperationCurrency.Format(documentActs[i].OperationAmount)}<br/>";
            }
          }

          items.Add(new PropertyBagItem(GetLandRecordUIDAsLink(recordingAct.LandRecord.UID),
                                        "<span class='bold-text'>Sello registral</span><br/>" +
                                        $"<label class='small-text'>Fecha de registro:</label><br/>" +
                                        $"{recordingAct.LandRecord.AuthorizationTime.ToString("dd/MMM/yyyy HH:mm")}<br/>" +
                                        $"<label class='small-text'>Instrumento:</label><br/>" +
                                        $"<i>{recordingAct.LandRecord.Instrument.AsText}</i><br/>" +
                                        $"<label class='small-text'>Actos jurídicos registrados:</label><br/>" +
                                        $"{documentActsText}<br/>"));

          lastProcessedDocument = recordingAct.LandRecord;

        } else if (tractItem is CertificateDto certificate) {
          if (!certificate.IsClosed) {
            continue;
          }
          items.Add(new PropertyBagItem(certificate.IssueTime.ToString("dd/MMM/yyyy"),
                                        $"Certificado de {certificate.Type} <br/>" +
                                        $"{GetCertificateUIDAsLink(certificate.UID)}"));

        } else {
          // no-op
        }
      }  // foreach

      return items;
    }


    private void BuildTransactionDocumentsSection(LRSTransaction transaction, List<PropertyBagItem> propertyBag, string messageUID) {
      propertyBag.Add(new PropertyBagItem("Documentos inscritos y certificados expedidos bajo este trámite",
                                           String.Empty, "section"));

      if (!transaction.LandRecord.IsEmptyInstance) {
        propertyBag.Add(new PropertyBagItem("Documento inscrito", GetLandRecordUIDAsLink(transaction.LandRecord.UID) + "<br/>" +
                                                                  "<i>" + transaction.LandRecord.Instrument.AsText + "</i><br/>" +
                                                                  GetPrintableLandRecordLink(transaction.LandRecord.UID, transaction.Workflow.CurrentStatus, messageUID)));
      }

      foreach (var certificate in transaction.GetIssuedCertificates()) {
        propertyBag.Add(new PropertyBagItem("Certificado", certificate.Type + "<br/>" +
                                                           GetCertificateUIDAsLink(certificate.UID) +
                                                           GetPrintableCertificateLink(certificate.UID, transaction.Workflow.CurrentStatus, messageUID)));
      }
    }


    private void BuildTransactionServicesSection(LRSTransaction transaction, List<PropertyBagItem> propertyBag) {
      if (transaction.Services.Count == 0) {
        return;
      }
      propertyBag.Add(new PropertyBagItem("Conceptos", String.Empty, "section"));
      foreach (var service in transaction.Services) {
        string value = service.Fee.Total == 0m ? service.TreasuryCode.Name : service.Fee.Total.ToString("C2");

        propertyBag.Add(new PropertyBagItem(service.ServiceType.DisplayName,
                                            value));
      }
    }


    private void BuildTransactionStatusSection(LRSTransaction transaction, List<PropertyBagItem> propertyBag, string messageUID) {
      if (transaction.PresentationTime == ExecutionServer.DateMaxValue) {
        // no-op
      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Delivered) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatus.GetStatusName(), "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", GetDateTime(transaction.LastDeliveryTime)));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Returned) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                             transaction.Workflow.CurrentStatus.GetStatusName(), "warning-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de devolución",
                                            GetDateTime(transaction.LastDeliveryTime), "warning-status-text"));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.Archived) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatus.GetStatusName(), "ok-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Por su naturaleza, este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (LRSWorkflowRules.IsArchivable(transaction)) {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatus.GetStatusName(), "in-process-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega", "Este trámite se procesa pero no se entrega al interesado en ventanilla."));

      } else if (transaction.Workflow.CurrentStatus == TransactionStatus.ToDeliver) {

        if (LRSWorkflowRules.IsReadyForElectronicDelivery(transaction, messageUID)) {
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
                                            transaction.Workflow.CurrentStatus.GetStatusName(), "warning-status-text"));
        propertyBag.Add(new PropertyBagItem("Fecha de entrega",
                                            "Desafortunadamente el trámite no procedió.<br />" +
                                            "Se requiere pasar a la oficina donde acudió, a recoger su oficio de devolución.", "warning-status-text"));


      } else {
        propertyBag.Add(new PropertyBagItem("Estado del trámite",
                                            transaction.Workflow.CurrentStatus.GetStatusName(), "in-process-status-text"));

        propertyBag.Add(new PropertyBagItem("Fecha de entrega estimada", "Desafortunadamente no podemos determinarla."));

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
    }

    private void BuildTransactionPaymentSection(LRSTransaction transaction, List<PropertyBagItem> propertyBag) {
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
    }

    private void BuildTransactionMainInfoSection(LRSTransaction transaction, List<PropertyBagItem> propertyBag) {
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
    }


    #endregion Private methods

  }  // class OnLineSearchPropertiesBagBuilder

}  // namespace Empiria.Land.WebApi
