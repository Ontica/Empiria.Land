/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateBuilder                             Pattern  : Builder Class                       *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Builds a certificate Html output using a text-based template.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.IO;
using System.Text;

using Empiria.Land.Registration;

namespace Empiria.Land.Certification {

  /// <summary>Builds a certificate Html output using a text-based template.</summary>
  internal class CertificateBuilder {

    #region Fields

    private static readonly string SEARCH_SERVICES_SERVER_BASE_ADDRESS =
                                          ConfigurationData.Get<string>("SearchServicesServerBaseAddress");

    private static readonly bool DISPLAY_VEDA_ELECTORAL_UI =
                                        ConfigurationData.Get<bool>("DisplayVedaElectoralUI", false);

    #endregion Fields

    #region Constructors and parsers

    private CertificateBuilder(Certificate certificate) {
      this.Certificate = certificate;
    }

    internal static string Build(Certificate certificate) {
      Assertion.AssertObject(certificate, "certificate");

      var builder = new CertificateBuilder(certificate);

      return builder.Build();
    }

    #endregion Constructors and parsers

    #region Properties

    private Certificate Certificate {
      get;
      set;
    }

    #endregion Properties

    #region Private methods

    private string Build() {
      var template = new StringBuilder(this.GetTemplate());

      var o = this.Certificate;

      template.Replace("{{CERTIFICATE_LOGO_SOURCE}}", this.GetLogoSource());

      template.Replace("{{QR.CODE.SOURCE}}",
                       "{{QR.CODE.SERVICE.URL}}?size=120&amp;data={{SEARCH.SERVICES.SERVER.ADDRESS}}/?" +
                        "type=certificate%26uid={{NUMERO.CERTIFICADO}}%26hash={{QR.CODE.HASH}}");

      if (!o.Property.IsEmptyInstance) {
        template.Replace("{{RESOURCE.QR.CODE.SOURCE}}",
                         "{{QR.CODE.SERVICE.URL}}?size=120&amp;data={{SEARCH.SERVICES.SERVER.ADDRESS}}/?" +
                         "type=resource%26uid={{FOLIO REAL}}%26hash={{RESOURCE.QR.CODE.HASH}}");
        template.Replace("{{DISPLAY.RESOURCE.QR.CODE}}", "inline");
        template.Replace("{{RESOURCE.QR.CODE.HASH}}", o.Property.QRCodeSecurityHash());
      } else {
        template.Replace("{{RESOURCE.QR.CODE.SOURCE}}", String.Empty);
        template.Replace("{{DISPLAY.RESOURCE.QR.CODE}}", "none");
      }

      template.Replace("{{QR.CODE.SERVICE.URL}}", ConfigurationData.GetString("QRCodeServiceURL"));
      template.Replace("{{SEARCH.SERVICES.SERVER.ADDRESS}}", SEARCH_SERVICES_SERVER_BASE_ADDRESS);

      template.Replace("{{DOCUMENT.OR.PHYSICAL.RECORDING}}", this.GetDocumentOrPhysicalRecording());
      template.Replace("{{NUMERO.CERTIFICADO}}", o.UID);
      template.Replace("{{TIPO.CERTIFICADO}}",
                   this.Certificate.CertificateType.DisplayName.ToUpperInvariant());
      template.Replace("{{FIRMA}}", o.SignedBy.FullName);
      template.Replace("{{FECHA.EXPEDICION}}", this.GetIssueDate());
      template.Replace("{{CADENA.ORIGINAL}}", this.GetDigitalSeal());
      template.Replace("{{SELLO.DIGITAL}}", this.GetDigitalSignature());
      template.Replace("{{QR.CODE.HASH}}", this.Certificate.QRCodeSecurityHash());


      template.Replace("{{SOLICITANTE}}", o.ExtensionData.SeekForName.ToUpperInvariant());
      template.Replace("{{DISTRITO}}", o.RecorderOffice.Alias);
      template.Replace("{{FOLIO REAL}}", o.Property.UID);
      template.Replace("{{DENOMINACION.PREDIO}}", o.ExtensionData.PropertyCommonName);
      template.Replace("{{UBICACION.PREDIO}}", o.ExtensionData.PropertyLocation);
      template.Replace("{{MEDIDAS.Y.COLINDANCIAS}}", o.ExtensionData.PropertyMetesAndBounds);

      template.Replace("{{AÑO.BÚSQUEDA}}", o.ExtensionData.StartingYear >= 1900 ?
                          EmpiriaString.SpeechInteger(o.ExtensionData.StartingYear).ToLowerInvariant() :
                          AsWarning("AÑO DE BÚSQUEDA NO PROPORCIONADO"));

      template.Replace("{{ELABORADO.POR}}", o.IssuedBy.Nickname);
      template.Replace("{{NUMERO.TRAMITE}}", o.Transaction.UID);
      template.Replace("{{FECHA.PRESENTACION}}",
                        o.Transaction.PresentationTime.ToString("dd/MMM/yyyy HH:mm"));
      template.Replace("{{NUMERO.RECIBO}}", o.Transaction.Payments.ReceiptNumbers);
      template.Replace("{{IMPORTE.DERECHOS}}", o.Transaction.Payments.Total.ToString("C2"));

      template.Replace("{{OPERACION}}", o.ExtensionData.Operation.ToLowerInvariant());
      template.Replace("{{OTORGADA.POR}}", o.ExtensionData.FromOwnerName);
      template.Replace("{{A.FAVOR.DE}}", o.OwnerName.ToUpperInvariant());

      template.Replace("{{NOTAS.MARGINALES}}", this.GetMarginalNotes());

      if (this.Certificate.ExtensionData.UseMarginalNotesAsFullBody) {
        template.Replace("{{CERTIFICATE.BODY}}", "<p>" + this.GetMarginalNotes() + "</p>");
      }

      return template.ToString();
    }


    private string GetMarginalNotes() {
      if (this.Certificate.ExtensionData.MarginalNotes.Length != 0) {
        return this.Certificate.ExtensionData.MarginalNotes;
      } else if (this.Certificate.ExtensionData.UseMarginalNotesAsFullBody) {
        return AsWarning("FALTA REDACTAR EL TEXTO DE ESTE CERTIFICADO MANUAL");
      } else {
        return "1) No presenta notas al margen.";
      }
    }

    private string GetIssueDate() {
      if (this.Certificate.Status != CertificateStatus.Pending) {
        return EmpiriaString.SpeechDate(this.Certificate.IssueTime).ToUpperInvariant();
      } else {
        return AsWarning("SIN FECHA DE EXPEDICIÓN");
      }
    }

    protected string GetDigitalSeal() {
      if (this.Certificate.Status != CertificateStatus.Pending) {
        return this.Certificate.GetDigitalSeal();
      } else {
        return AsWarning("SIN VALOR LEGAL * * * * * SIN VALOR LEGAL");
      }
    }

    protected string GetDigitalSignature() {
      if (this.Certificate.Status != CertificateStatus.Pending) {
        return this.Certificate.GetDigitalSignature();
      } else {
        return AsWarning("SIN VALOR LEGAL * * * * * SIN VALOR LEGAL");
      }
    }


    private string GetLogoSource() {
      if (DISPLAY_VEDA_ELECTORAL_UI) {
        return "assets/government.seal.veda.png";
      }
      return "assets/government.seal.png";
    }


    private string GetTemplate() {
      string mainTemplateFile = ConfigurationData.GetString("Certificates.MainTemplate");
      string templatesPath = ConfigurationData.GetString("Certificates.Templates.Path");

      string content = File.ReadAllText(Path.Combine(templatesPath, mainTemplateFile));

      if (!this.Certificate.ExtensionData.UseMarginalNotesAsFullBody) {
        string bodyTemplateFile = this.Certificate.CertificateType.GetHtmlTemplateFileName();
        string bodyTemplate = File.ReadAllText(Path.Combine(templatesPath, bodyTemplateFile));

        return content.Replace("{{CERTIFICATE.BODY}}", bodyTemplate);
      } else {
        return content;
      }
    }

    private string GetDocumentOrPhysicalRecording() {
      var antecedent = this.Certificate.Property.Tract.GetRecordingAntecedent();

      if (antecedent.Equals(RecordingAct.Empty)) {
        return AsWarning("NO SE ENCONTRÓ INFORMACIÓN DEL ANTECEDENTE REGISTRAL. " +
                         "FAVOR DE REVISAR EL FOLIO DEL PREDIO. " +
                         "ES POSIBLE QUE EL CERTIFICADO NO DEBA SER EMITIDO");
      }
      if (antecedent.PhysicalRecording.IsEmptyInstance) {
        return this.GetPhysicalDocument(antecedent.Document);
      } else {
        return this.GetPhysicalRecording(antecedent.PhysicalRecording);
      }
    }

    private string GetPhysicalRecording(Recording physicalRecording) {
      const string template =
              "Que bajo la partida {{NUMBER}} de la Sección {{SECTION}} Volumen {{BOOK}} " +
              "del Distrito Judicial de {{DISTRICT}} de fecha {{DATE}}";
      var text = new StringBuilder(template);

      text.Replace("{{NUMBER}}", physicalRecording.Number);
      text.Replace("{{SECTION}}", physicalRecording.RecordingBook.RecordingSection.Name);
      text.Replace("{{BOOK}}", physicalRecording.RecordingBook.BookNumber);
      text.Replace("{{DISTRICT}}", physicalRecording.RecordingBook.RecorderOffice.Alias);
      text.Replace("{{DATE}}",
                    this.Certificate.ExtensionData.OperationDate.ToString("dd \\de MMMM \\de yyyy"));

      return text.ToString();
    }

    private string GetPhysicalDocument(RecordingDocument document) {
      const string template =
              "Que bajo el documento con número electrónico {{DOCUMENT}} " +
              "registrado en esta oficina el {{DATE}}";

      var text = new StringBuilder(template);

      text.Replace("{{DOCUMENT}}", document.UID);
      if (document.AuthorizationTime != ExecutionServer.DateMinValue) {
        text.Replace("{{DATE}}", document.AuthorizationTime.ToString("dd \\de MMMM \\de yyyy"));

      } else {
        text.Replace("{{DATE}}", AsWarning("FECHA DE INSCRIPCIÓN NO DETERMINADA"));
      }
      return text.ToString();
    }

    #endregion Private methods

    #region Helpers

    private string AsWarning(string text) {
      return "<span style='color:red;'><strong>*****" + text + "*****</strong></span>";
    }

    #endregion Helpers

  } // class CertificateBuilder

} // namespace Empiria.Land.Certification
