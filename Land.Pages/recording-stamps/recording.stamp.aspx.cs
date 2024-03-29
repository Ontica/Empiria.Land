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

    protected LandRecord landRecord = null;
    protected LRSTransaction transaction = null;

    private RecordingAct _selectedRecordingAct;

    private bool _isMainLandRecord;

    private Instrument instrument;

    #endregion Fields

    #region Constructors and parsers

    protected void Page_Load(object sender, EventArgs e) {
      string landRecordUID = Request.QueryString["uid"];

      int selectedRecordingActId = int.Parse(Request.QueryString["selectedRecordingActId"] ?? "-1");

      _isMainLandRecord = bool.Parse(Request.QueryString["main"] ?? "false");

      landRecord = LandRecord.TryParse(landRecordUID);

      landRecord.EnsureIntegrity();

      transaction = landRecord.Transaction;

      landRecord.RefreshRecordingActs();

      _selectedRecordingAct = RecordingAct.Parse(selectedRecordingActId);

      this.instrument = landRecord.Instrument;

      builder = new RecordingStampBuilder(landRecord);
    }

    #endregion Constructors and parsers

    #region Protected methods


    protected string GetDigitalSeal() {
      if (landRecord.IsHistoricRecord) {
        return CommonMethods.AsWarning("Los documentos históricos no tienen sello digital.");

      } else if (!landRecord.IsClosed) {
        return CommonMethods.AsWarning("El documento está ABIERTO por lo que no tiene sello digital.");

      } else {
        return landRecord.SecurityData.DigitalSeal.Substring(0, 64);

      }
    }


    protected string GetDigest() {
      if (!landRecord.SecurityData.UsesESign ||
           landRecord.SecurityData.IsUnsigned ||
           landRecord.SecurityData.Digest.Length == 0) {
        return string.Empty;
      }

      return $"<b>Cadena de digestión (datos estampillados):</b><br />" +
             $"{landRecord.SecurityData.Digest}" +
             $"<br />";
    }


    protected string GetSignatureGuid() {
      if (!landRecord.SecurityData.UsesESign ||
           landRecord.SecurityData.IsUnsigned ||
           landRecord.SecurityData.SignGuid.Length == 0) {
        return string.Empty;
      }

      return $"<b>Identificador de la firma electrónica:</b><br />" +
             $"{landRecord.SecurityData.SignGuid}" +
             $"<br />";
    }


    protected string GetDigitalSignature() {
      if (landRecord.IsHistoricRecord) {
        return CommonMethods.AsWarning("Los documentos históricos no tienen firma digital.");

      } else if (!landRecord.IsClosed) {
        return CommonMethods.AsWarning("El documento no ha sido cerrado. No tiene validez.");

      } else if (landRecord.SecurityData.IsUnsigned && landRecord.SecurityData.UsesESign) {
        return CommonMethods.AsWarning("Este documento NO HA SIDO FIRMADO digitalmente. No tiene validez oficial.");

      } else if (landRecord.SecurityData.IsUnsigned && !landRecord.SecurityData.UsesESign) {
        return CommonMethods.AsWarning("Este documento no ha sido firmado manualmente. No tiene validez oficial.");

      } else if (landRecord.SecurityData.IsSigned && landRecord.SecurityData.UsesESign) {
        return EmpiriaString.DivideLongString(landRecord.SecurityData.DigitalSignature, 96, "<br />");

      } else if (landRecord.SecurityData.IsSigned && !landRecord.SecurityData.UsesESign) {
        return "Documento firmado de forma autógrafa. Requiere también sello oficial.";

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    protected bool CanBePrinted() {
      if (!landRecord.IsClosed) {
        return false;
      }
      if (landRecord.SecurityData.UsesESign && landRecord.SecurityData.IsUnsigned) {
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
      if (instrument.Summary.Length > 30) {
        return "DESCRIPCIÓN:<br />" + instrument.Summary + "<br /><br />";

      } else if (landRecord.IsHistoricRecord) {
        return "* PARTIDA HISTÓRICA SIN DESCRIPCIÓN *";

      } else {
        return string.Empty;
      }
    }


    protected string GetDocumentLogo() {
      if (DISPLAY_VEDA_ELECTORAL_UI) {
        return "../themes/default/customer/government.seal.veda.png";
      }
      if (landRecord.AuthorizationTime.Year == 2024) {
        return "../themes/default/customer/government.seal.2024.png";
      } else {
        return "../themes/default/customer/government.seal.png";
      }
    }


    protected string GetPaymentText() {
      return builder.PaymentText();
    }


    protected bool DocumentHasNotes {
      get {
        return instrument.Summary.Length != 0;
      }
    }


    protected string GetDocumentNotes() {
      var notes = instrument.Summary.Replace("<br>", "<br/>");

      return notes;
    }


    protected string GetInstrumentText() {
      return instrument.AsText;
    }


    protected string GetPrelationText() {
      return builder.PrelationText();
    }


    protected string GetRecordingActsText() {
      return builder.RecordingActsText(_selectedRecordingAct, _isMainLandRecord);
    }


    protected string GetRecordingOfficialsInitials() {
      string temp = String.Empty;

      List<Contact> recordingOfficials = landRecord.GetRecordingOfficials();

      foreach (Contact official in recordingOfficials) {
        if (temp.Length != 0) {
          temp += " ";
        }
        temp += official.Initials;
      }
      return temp;
    }

    protected string GetUniqueInvolvedResourceQRCodeUrl() {
      string type = String.Empty;

      if (UniqueInvolvedResource is RealEstate) {
        type = "realestate";
      } else if (UniqueInvolvedResource is Association) {
        type = "association";
      } else if (UniqueInvolvedResource is NoPropertyResource) {
        type = "noproperty";
      }

      return $"../user.controls/qrcode.aspx?size=120&#38;data={SEARCH_SERVICES_SERVER_BASE_ADDRESS}/?" +
             $"type={type}%26uid={UniqueInvolvedResource.UID}%26hash={UniqueInvolvedResource.QRCodeSecurityHash()}";
    }

    protected string GetUniqueInvolvedResourceQRCodeText() {
      if (UniqueInvolvedResource is RealEstate) {
        return "Consultar este predio";
      } else if (UniqueInvolvedResource is Association) {
        return "Consultar esta persona moral";
      } else if (UniqueInvolvedResource is NoPropertyResource) {
        return "Consultar folio electrónico";
      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    protected string GetCurrentUserInitials() {
      if (ExecutionServer.IsAuthenticated) {
        var user = ExecutionServer.CurrentContact;

        return user.Initials;
      }
      return String.Empty;
    }


    // For future use
    protected string GetRecordingOfficialsNames() {
      string temp = String.Empty;

      List<Contact> recordingOfficials = landRecord.GetRecordingOfficials();

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
      if (landRecord.IsHistoricRecord) {
        return String.Empty;
      }
      if (!CanBePrinted()) {
        return CommonMethods.AsWarning("ESTE DOCUMENTO NO ES VÁLIDO EN EL ESTADO ACTUAL.");
      } else {
        return landRecord.SecurityData.SignedBy.FullName;
      }
    }


    protected string GetRecordingSignerJobTitle() {
      if (landRecord.IsHistoricRecord) {
        return String.Empty;
      }
      return landRecord.SecurityData.SignedByJobTitle;
    }


    private Resource _uniqueInvolvedResource = null;
    protected Resource UniqueInvolvedResource {
      get {
        if (_uniqueInvolvedResource == null) {
          _uniqueInvolvedResource = landRecord.GetUniqueInvolvedResource();
        }
        return _uniqueInvolvedResource;
      }
    }

    #endregion Protected methods

  } // class RegistrationStamp

} // namespace Empiria.Land.Pages
