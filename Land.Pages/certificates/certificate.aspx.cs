﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : CertificatePage                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Land property certificate page.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Certificates;
using Empiria.Land.Instruments;
using Empiria.Land.Registration;
using Empiria.Land.Transactions;

namespace Empiria.Land.Pages {

  /// <summary>Land property certificate page.</summary>
  public partial class CertificatePage : System.Web.UI.Page {

    #region Fields

    protected static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                          ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly bool DISPLAY_VEDA_ELECTORAL_UI =
                                          ConfigurationData.Get<bool>("DisplayVedaElectoralUI", false);

    RecordingStampBuilder builder;

    protected Certificate certificate;
    protected LandRecord landRecord = null;
    protected LRSTransaction transaction = null;

    private RecordingAct _selectedRecordingAct;

    private bool _isMainLandRecord;

    private Instrument instrument;

    #endregion Fields

    #region Constructors and parsers

    protected void Page_Load(object sender, EventArgs e) {
      string certificateUID = Request.QueryString["uid"];

      certificate = Certificate.Parse(certificateUID);


      string landRecordUID = Request.QueryString["landRecordUID"];

      int selectedRecordingActId = int.Parse(Request.QueryString["selectedRecordingActId"] ?? "-1");

      _isMainLandRecord = bool.Parse(Request.QueryString["main"] ?? "false");

      landRecord = LandRecord.TryParse(landRecordUID);

      // landRecord.EnsureIntegrity();

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
      if (landRecord.SecurityData.IsUnsigned) {
        return false;
      }
      return true;
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


    protected string GetCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALIA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>15 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ REGISTRADO</b> NINGÚN BIEN INMUEBLE A NOMBRE DE:" +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.PERSON.NAME}}</strong></div>";

      string x = t.Replace("{{ON.PERSON.NAME}}", certificate.OnPersonName);

      return x.ToUpperInvariant();
    }

    protected string GetCertificateType() {
      return $"CERTIFICADO DE {certificate.CertificateType.DisplayName}".ToUpperInvariant();
    }

    protected string GetRecordingActsText() {
      return builder.RecordingActsText(_selectedRecordingAct, _isMainLandRecord);
    }


    protected string GetElaboratedByInitials() {
      return certificate.PostedBy.Initials;
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


    protected string GetCertificatePlaceAndDate() {
      const string t = "EN LA CIUDAD DE <b>{CITY}</b>, A LAS <b>{TIME}</b> HORAS DEL DÍA <b>{DATE}</b>; " +
                       "{LA.EL} <b>{SIGNER.NAME}</b>, {JOB.TITLE}:";

      string x = t.Replace("{DATE}", CommonMethods.GetDateAsText(certificate.IssueTime));

      x = x.Replace("{TIME}", certificate.IssueTime.ToString(@"HH:mm"));

      x = x.Replace("{CITY}", certificate.RecorderOffice.Place);

      x = x.Replace("{LA.EL}", landRecord.SecurityData.SignedBy.IsFemale ? "LA" : "EL");
      x = x.Replace("{SIGNER.NAME}", landRecord.SecurityData.SignedBy.FullName);
      x = x.Replace("{JOB.TITLE}", landRecord.SecurityData.SignedByJobTitle);

      return x.ToUpperInvariant();
    }

    protected string GetCertificateSignerName() {
      if (landRecord.IsHistoricRecord) {
        return String.Empty;
      }
      if (!CanBePrinted()) {
        return CommonMethods.AsWarning("ESTE DOCUMENTO NO ES VÁLIDO EN EL ESTADO ACTUAL.");
      } else {
        return landRecord.SecurityData.SignedBy.FullName;
      }
    }


    protected string GetCertificateSignerJobTitle() {
      if (landRecord.IsHistoricRecord) {
        return String.Empty;
      }
      return landRecord.SecurityData.SignedByJobTitle;
    }


    protected Resource UniqueInvolvedResource {
      get {
        return certificate.OnRecordableSubject;
      }
    }

    #endregion Protected methods

  } // class CertificatePage

} // namespace Empiria.Land.Pages
