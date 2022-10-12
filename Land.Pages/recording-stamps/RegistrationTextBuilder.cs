/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : RegistrationTextBuilder                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Text builder for instrument registration data.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.Instruments;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Pages {

  /// <summary>Text builder for recording acts.</summary>
  internal class RegistrationTextBuilder {

    private readonly RecordingDocument _document;
    private readonly LRSTransaction _transaction;

    internal RegistrationTextBuilder(RecordingDocument document) {
      Assertion.Require(document, nameof(document));

      _document = document;
      _transaction = document.GetTransaction();
    }



    internal string PrelationText() {
      if (_document.IsHistoricDocument) {
        return PrelationTextForHistoricDocuments();
      } else {
        return PrelationTextForDocumentsWithTransaction();
      }
    }

    internal string RecordingPlaceAndDate() {
      if (_document.IsHistoricDocument) {
        return PlaceAndDateTextForHistoricDocuments();
      } else {
        return PlaceAndDateTextForDocumentsWithTransaction();
      }
    }


    private string PrelationTextForDocumentsWithTransaction() {
      const string template =
           "<b style='text-transform:uppercase'>{INSTRUMENT_AS_TEXT}</b>, instrumento presentado " +
           "para su examen y registro {REENTRY_TEXT} el <b>{DATE} a las {TIME} horas</b>, " +
           "bajo el número de trámite <b>{NUMBER}</b>, y para el cual {COUNT}";

      DateTime presentationTime = _transaction.IsReentry ? _transaction.LastReentryTime : _transaction.PresentationTime;

      string x = template.Replace("{DATE}", CommonMethods.GetDateAsText(presentationTime));

      if (_transaction.HasInstrument) {
        var instrument = Instrument.Parse(_document.InstrumentId);
        x = x.Replace("{INSTRUMENT_AS_TEXT}", instrument.AsText);

      } else {
        x = x.Replace("{INSTRUMENT_AS_TEXT}", "**INSTRUMENTO NO DETERMINADO**");
      }

      x = x.Replace("{TIME}", presentationTime.ToString("HH:mm:ss"));
      x = x.Replace("{NUMBER}", _transaction.UID);
      x = x.Replace("{REENTRY_TEXT}", _transaction.IsReentry ? "(como reingreso)" : String.Empty);

      int recordingActsCount = GetRecordingActsCount();

      if (recordingActsCount > 1) {
        x = x.Replace("{COUNT}", "se registraron los siguientes " + recordingActsCount.ToString() +
                      " (" + EmpiriaString.SpeechInteger(recordingActsCount).ToLower() + ") " +
                      "actos jurídicos:");

      } else if (recordingActsCount == 1) {
        x = x.Replace("{COUNT}", "se registró el siguiente acto jurídico:");

      } else {
        x = x.Replace("{COUNT}", CommonMethods.AsWarning("<u>NO SE HAN REGISTRADO</u> actos jurídicos."));
      }
      return x;
    }

    private int GetRecordingActsCount() {
      return _document.RecordingActs.CountAll(x => !x.IsChild);
    }

    private string PrelationTextForHistoricDocuments() {
      return "<h3>" + _document.RecordingActs[0].PhysicalRecording.AsText + "</h3>";
    }


    private string PlaceAndDateTextForDocumentsWithTransaction() {
      const string t = "Registrado en {CITY}, a las {TIME} horas del {DATE}. Doy Fe.";

      string x = t.Replace("{DATE}", CommonMethods.GetDateAsText(_document.AuthorizationTime));

      x = x.Replace("{TIME}", _document.AuthorizationTime.ToString(@"HH:mm"));

      x = x.Replace("{CITY}", _document.RecorderOffice.GetPlace());

      return x;
    }

    internal string PaymentText() {
      if (_document.IsHistoricDocument) {
        return String.Empty;
      }

      string template;

      var payment = LRSPayment.Empty;

      if (_transaction.Payments.Count > 0) {
        payment = _transaction.Payments[0];
      }

      if (!this._transaction.FormerPaymentOrderData.IsEmptyInstance) {
        template = "Derechos por <b>{AMOUNT}</b> según la línea de captura <b>{RECEIPT}</b> expedida por " +
                   "la Secretaría de Finanzas del Estado, y cuyo comprobante se archiva.";
        template = template.Replace("{RECEIPT}", _transaction.FormerPaymentOrderData.RouteNumber);

      } else {
        template = "Derechos por <b>{AMOUNT}</b> según recibo <b>{RECEIPT}</b> expedido por " +
                   "la Secretaría de Finanzas del Estado, que se archiva.";
        template = template.Replace("{RECEIPT}", _transaction.Payments.ReceiptNumbers);
      }

      template = template.Replace("{AMOUNT}", payment.ReceiptTotal.ToString("C2"));
      template = template.Replace("{RECEIPT}", payment.ReceiptNo);

      return template;
    }


    private string PlaceAndDateTextForHistoricDocuments() {
      const string template =
            "De acuerdo a lo que consta en libros físicos y en documentos históricos:<br/>" +
            "Fecha de presentación: <b>{PRESENTATION.DATE}</b>. " +
            "Fecha de registro: <b>{AUTHORIZATION.DATE}</b>.<br/><br/>" +
            "Fecha de la captura histórica: <b>{RECORDING.DATE}<b>.<br/>";

      string x = template.Replace("{PRESENTATION.DATE}",
                                  CommonMethods.GetDateAsText(_document.PresentationTime));

      x = x.Replace("{AUTHORIZATION.DATE}",
                    CommonMethods.GetDateAsText(_document.AuthorizationTime));

      x = x.Replace("{RECORDING.DATE}",
                    CommonMethods.GetDateAsText(_document.PostingTime));

      return x;
    }

  }  // class RegistrationTextBuilder

}  // namespace Empiria.Land.Pages
