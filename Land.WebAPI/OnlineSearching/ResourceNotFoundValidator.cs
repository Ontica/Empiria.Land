/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Services provider                     *
*  Type     : ResourceNotFoundValidator                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains methods for online search services validation.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi {

  /// <summary>Contains methods for online search services validation.</summary>
  internal class ResourceNotFoundValidator {

    private readonly DateTime certificateHashCodeValidationStartDate = DateTime.Parse("2020-01-01");
    private readonly DateTime hashCodeValidationStartDate = DateTime.Parse("2020-01-01");

    internal void ValidateTransaction(string transactionUID, string hash) {

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
    }


    internal void ValidateCertificate(string certificateUID, string hash) {

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
    }


    internal void ValidateLandRecord(string landRecordUID, string hash) {

      var landRecord = RecordingDocument.TryParse(landRecordUID, true);

      if (landRecord == null && hash.Length == 0) {
        throw new ResourceNotFoundException("Land.LandRecord.NotFound",
                                            "No tenemos registrado ningún documento o sello registral con número '{0}'.\n" +
                                            "Favor de revisar la información proporcionada.",
                                            landRecordUID);

      } else if (landRecord == null && hash.Length != 0) {
        throw new ResourceNotFoundException("Land.LandRecord.InvalidQRCode",
                                            "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                            "referencia al sello registral con número '{0}' que NO está registrado en nuestros archivos.\n\n" +
                                            "MUY IMPORTANTE: Es posible que su documento sea falso.\n\n" +
                                            "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                            landRecordUID);

      } else if (landRecord != null && hash.Length != 0 &&
                 landRecord.AuthorizationTime >= hashCodeValidationStartDate && hash != landRecord.Security.QRCodeSecurityHash()) {
        throw new ResourceNotFoundException("Land.LandRecord.InvalidQRCode",
                                            "El código QR que está impreso en su documento y que acaba de escanear hace " +
                                            "referencia al sello registral con número '{0}' que sí tenemos registrado " +
                                            "pero el código de validación del QR no es correcto.\n\n" +
                                            "MUY IMPORTANTE: Es posible que su documento sea falso o que haya sido modificado " +
                                            "posteriormente a la impresión que tiene en la mano.\n\nEsto último significa que su " +
                                            "documento impreso no es válido y que debe solicitar una reposición del mismo.\n\n" +
                                            "Para obtener más información comuníquese inmediatamente a la oficina del Registro Público.",
                                            landRecordUID);

      }
    }


    internal void ValidateResource(string resourceUID, string hash) {
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
    }

    internal void ValidateTransactionForElectronicDelivery(string transactionUID, string messageUID) {

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      if (!(LRSWorkflowRules.IsReadyForElectronicDelivery(transaction, messageUID))) {
        throw new ResourceNotFoundException("Land.Transaction.NotReadyForElectronicalDelivery",
                                            "El trámite {0} NO está disponible para entrega electrónica.\n\n" +
                                            "Posiblemente su estado cambió después de que usted recibió el mensaje.\n" +
                                            "Si este es el caso, en breve recibirá un nuevo mensaje sobre la situación del mismo.",
                                            transaction.UID);

      }
    }
  }  // class ResourceNotFoundValidator

}  // namespace Empiria.Land.WebApi
