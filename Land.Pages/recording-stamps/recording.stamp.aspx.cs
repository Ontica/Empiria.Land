/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : RecordingStamp                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Recording stamp for instruments with recording acts.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.Land.Instruments;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Pages {

  /// <summary>Recording stamp for instruments with recording acts.</summary>
  public partial class RecordingStamp : System.Web.UI.Page {

    #region Fields

    protected static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                          ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly bool DISPLAY_VEDA_ELECTORAL_UI =
                                          ConfigurationData.Get<bool>("DisplayVedaElectoralUI", false);

    RecordingStampBuilder builder;

    protected RecordingDocument document = null;
    protected LRSTransaction transaction = null;

    private FixedList<RecordingAct> recordingActs = null;
    private RecordingAct _selectedRecordingAct;
    private bool _isMainDocument;

    private Instrument instrument;

    #endregion Fields

    #region Constructors and parsers

    protected void Page_Load(object sender, EventArgs e) {
      string documentUID = Request.QueryString["uid"];

      int selectedRecordingActId = int.Parse(Request.QueryString["selectedRecordingActId"] ?? "-1");
      _isMainDocument = bool.Parse(Request.QueryString["main"] ?? "false");

      document = RecordingDocument.TryParse(documentUID, true);

      transaction = document.GetTransaction();

      document.RefreshRecordingActs();

      _selectedRecordingAct = RecordingAct.Parse(selectedRecordingActId);

      recordingActs = document.RecordingActs;

      this.instrument = Instrument.Parse(document.InstrumentId, true);

      builder = new RecordingStampBuilder(document);
    }

    #endregion Constructors and parsers

    #region Protected methods


    protected string GetDigitalSeal() {
      if (document.IsHistoricDocument) {
        return AsWarning("Los documentos históricos no tienen sello digital.");
      } else if (document.Status != RecordableObjectStatus.Closed) {
        return AsWarning("El documento está ABIERTO por lo que no tiene sello digital.");
      } else {
        return document.Security.GetDigitalSeal().Substring(0, 64);
      }
    }


    protected string GetDigitalSignature() {
      if (document.IsHistoricDocument) {
        return AsWarning("Los documentos históricos no tienen firma digital.");
      }
      if (document.Status != RecordableObjectStatus.Closed) {
        return AsWarning("El documento está incompleto. No tiene validez.");
      }
      if (!document.Security.UseESign) {
        return "Documento firmado de forma autógrafa. Requiere también sello oficial.";

      } else if (document.Security.UseESign && document.Security.Unsigned()) {
        return AsWarning("Este documento NO HA SIDO FIRMADO digitalmente. No tiene valor oficial.");

      } else if (document.Security.UseESign && document.Security.Signed()) {
        return document.Security.GetDigitalSignature();

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    protected bool CanBePrinted() {
      if (document.Status != RecordableObjectStatus.Closed) {
        return false;
      }
      if (document.Security.UseESign && document.Security.Unsigned()) {
        return false;
      }
      //if (transaction.Workflow.IsFinished) {
      //  return true;
      //}
      return true;
      //if (ExecutionServer.IsAuthenticated) {
      //  return true;
      //} else if (!String.IsNullOrWhiteSpace(Request.QueryString["msg"])) {
      //  return true;
      //}
    }


    protected string GetDocumentDescriptionText() {
      if (document.Notes.Length > 30) {
        return "DESCRIPCIÓN:<br />" + document.Notes + "<br /><br />";

      } else if (document.IsHistoricDocument) {
        return "* PARTIDA HISTÓRICA SIN DESCRIPCIÓN *";

      } else {
        return string.Empty;

      }
    }


    protected string GetDocumentLogo() {
      if (DISPLAY_VEDA_ELECTORAL_UI) {
        return "../themes/default/customer/government.seal.veda.png";
      }
      return "../themes/default/customer/government.seal.png";
    }


    protected string GetPaymentText() {
      if (document.IsHistoricDocument) {
        return String.Empty;
      }

      string template;

      var payment = LRSPayment.Empty;

      if (transaction.Payments.Count > 0) {
        payment = transaction.Payments[0];
      }

      if (!this.transaction.FormerPaymentOrderData.IsEmptyInstance) {
        template = "Derechos por <b>{AMOUNT}</b> según la línea de captura <b>{RECEIPT}</b> expedida por " +
                   "la Secretaría de Finanzas del Estado, y cuyo comprobante se archiva.";
        template = template.Replace("{RECEIPT}", transaction.FormerPaymentOrderData.RouteNumber);

      } else {
        template = "Derechos por <b>{AMOUNT}</b> según recibo <b>{RECEIPT}</b> expedido por " +
                   "la Secretaría de Finanzas del Estado, que se archiva.";
        template = template.Replace("{RECEIPT}", transaction.Payments.ReceiptNumbers);
      }

      template = template.Replace("{AMOUNT}", payment.ReceiptTotal.ToString("C2"));
      template = template.Replace("{RECEIPT}", payment.ReceiptNo);

      return template;
    }


    protected string GetInstrumentText() {
      return instrument.AsText;
    }


    protected string GetPrelationText() {
      if (document.IsHistoricDocument) {
        return PrelationTextForHistoricDocuments();
      } else {
        return PrelationTextForDocumentsWithTransaction();
      }
    }


    private string PrelationTextForDocumentsWithTransaction() {
      const string template =
           "Documento presentado para su examen y registro {REENTRY_TEXT} el <b>{DATE} a las {TIME} horas</b>, " +
           "bajo el número de trámite <b>{NUMBER}</b>, y para el cual {COUNT}";

      DateTime presentationTime = transaction.IsReentry ? transaction.LastReentryTime : transaction.PresentationTime;

      string x = template.Replace("{DATE}", GetDateAsText(presentationTime));

      x = x.Replace("{TIME}", presentationTime.ToString("HH:mm:ss"));
      x = x.Replace("{NUMBER}", transaction.UID);
      x = x.Replace("{REENTRY_TEXT}", transaction.IsReentry ? "(como reingreso)" : String.Empty);

      if (this.recordingActs.Count > 1) {
        x = x.Replace("{COUNT}", "se registraron los siguientes " + this.recordingActs.Count.ToString() +
                      " (" + EmpiriaString.SpeechInteger(this.recordingActs.Count).ToLower() + ") " +
                      "actos jurídicos:");

      } else if (this.recordingActs.Count == 1) {
        x = x.Replace("{COUNT}", "se registró el siguiente acto jurídico:");

      } else {
        x = x.Replace("{COUNT}", AsWarning("<u>NO SE HAN REGISTRADO</u> actos jurídicos."));
      }
      return x;
    }


    private string PrelationTextForHistoricDocuments() {
      return "<h3>" + this.recordingActs[0].PhysicalRecording.AsText + "</h3>";
    }


    protected string GetRecordingActsText() {
      return builder.RecordingActsText(_selectedRecordingAct, _isMainDocument);
    }


    protected string GetRecordingOfficialsInitials() {
      string temp = String.Empty;

      List<Contact> recordingOfficials = document.GetRecordingOfficials();

      foreach (Contact official in recordingOfficials) {
        if (temp.Length != 0) {
          temp += " ";
        }
        temp += official.Nickname;
      }
      return temp;
    }


    protected string GetCurrentUserInitials() {
      if (ExecutionServer.IsAuthenticated) {
        var user = Security.EmpiriaUser.Current.AsContact();

        return user.Nickname;
      }
      return String.Empty;
    }


    // For future use
    protected string GetRecordingOfficialsNames() {
      string temp = String.Empty;

      List<Contact> recordingOfficials = document.GetRecordingOfficials();

      foreach (Contact official in recordingOfficials) {
        if (temp.Length != 0) {
          temp += ", ";
        }
        temp += official.FullName;
      }
      return temp;
    }


    protected string GetRecordingPlaceAndDate() {
      if (document.IsHistoricDocument) {
        return PlaceAndDateTextForHistoricDocuments();
      } else {
        return PlaceAndDateTextForDocumentsWithTransaction();
      }
    }


    private string PlaceAndDateTextForDocumentsWithTransaction() {
      const string t = "Registrado en {CITY}, a las {TIME} horas del {DATE}. Doy Fe.";

      string x = t.Replace("{DATE}", GetDateAsText(document.AuthorizationTime));
      x = x.Replace("{TIME}", document.AuthorizationTime.ToString(@"HH:mm"));
      x = x.Replace("{CITY}", document.RecorderOffice.GetPlace());

      return x;
    }


    private string PlaceAndDateTextForHistoricDocuments() {
      const string template =
            "De acuerdo a lo que consta en libros físicos y en documentos históricos:<br/>" +
            "Fecha de presentación: <b>{PRESENTATION.DATE}</b>. " +
            "Fecha de registro: <b>{AUTHORIZATION.DATE}</b>.<br/><br/>" +
            "Fecha de la captura histórica: <b>{RECORDING.DATE}<b>.<br/>";

      string x = template.Replace("{PRESENTATION.DATE}", GetDateAsText(document.PresentationTime));
      x = x.Replace("{AUTHORIZATION.DATE}", GetDateAsText(document.AuthorizationTime));
      x = x.Replace("{RECORDING.DATE}", GetDateAsText(document.PostingTime));

      return x;
    }


    private string GetDateAsText(DateTime date) {
      return CommonMethods.GetDateAsText(date);
    }


    protected string GetRecordingSignerName() {
      if (document.IsHistoricDocument) {
        return String.Empty;
      }
      if (!CanBePrinted()) {
        return AsWarning("ESTE DOCUMENTO NO ES VÁLIDO EN EL ESTADO ACTUAL.");
      } else {
        return document.Security.GetSignedBy().FullName;
      }
    }


    protected string GetRecordingSignerJobTitle() {
      if (document.IsHistoricDocument) {
        return String.Empty;
      }
      return document.Security.GetSignedBy().JobTitle;
    }


    private Resource _uniqueInvolvedResource = null;
    protected Resource UniqueInvolvedResource {
      get {
        if (_uniqueInvolvedResource == null) {
          _uniqueInvolvedResource = document.GetUniqueInvolvedResource();
        }
        return _uniqueInvolvedResource;
      }
    }

    #endregion Protected methods


    #region Private methods


    private string AsWarning(string text) {
      return "<span style='color:red;'><strong>*****" + text + "*****</strong></span>";
    }

    #endregion Private methods

  } // class RegistrationStamp

} // namespace Empiria.Land.Pages