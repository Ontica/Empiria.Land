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

      this.instrument = Instrument.Parse(document.InstrumentId, true);

      builder = new RecordingStampBuilder(document);
    }

    #endregion Constructors and parsers

    #region Protected methods


    protected string GetDigitalSeal() {
      if (document.IsHistoricDocument) {
        return CommonMethods.AsWarning("Los documentos históricos no tienen sello digital.");

      } else if (document.Status != RecordableObjectStatus.Closed) {
        return CommonMethods.AsWarning("El documento está ABIERTO por lo que no tiene sello digital.");

      } else {
        return document.Security.GetDigitalSeal().Substring(0, 64);

      }
    }


    protected string GetDigitalSignature() {
      if (document.IsHistoricDocument) {
        return CommonMethods.AsWarning("Los documentos históricos no tienen firma digital.");
      }
      if (document.Status != RecordableObjectStatus.Closed) {
        return CommonMethods.AsWarning("El documento está incompleto. No tiene validez.");
      }
      if (!document.Security.UseESign) {
        return "Documento firmado de forma autógrafa. Requiere también sello oficial.";

      } else if (document.Security.UseESign && document.Security.Unsigned()) {
        return CommonMethods.AsWarning("Este documento NO HA SIDO FIRMADO digitalmente. No tiene valor oficial.");

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
      return builder.PaymentText();
    }


    protected bool DocumentHasNotes {
      get {
        return document.Notes.Length != 0;
      }
    }


    protected string GetDocumentNotes() {
      var notes = document.Notes.Replace("<br>", "<br/>");

      return notes;
    }


    protected string GetInstrumentText() {
      return instrument.AsText;
    }


    protected string GetPrelationText() {
      return builder.PrelationText();
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
        var user = ExecutionServer.CurrentContact;

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
      return builder.RecordingPlaceAndDate();
    }


    protected string GetRecordingSignerName() {
      if (document.IsHistoricDocument) {
        return String.Empty;
      }
      if (!CanBePrinted()) {
        return CommonMethods.AsWarning("ESTE DOCUMENTO NO ES VÁLIDO EN EL ESTADO ACTUAL.");
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

  } // class RegistrationStamp

} // namespace Empiria.Land.Pages
