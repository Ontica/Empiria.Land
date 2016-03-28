/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateBuilder                             Pattern  : Builder Class                       *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Builds a certificate output using a text-based template.                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.IO;
using System.Text;

using Empiria.Land.Registration;

namespace Empiria.Land.Certification {

  /// <summary>Builds a certificate output using a text-based template.</summary>
  internal class CertificateBuilder {

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

      template.Replace("{{DOCUMENT.OR.PHYSICAL.RECORDING}}", GetDocumentOrPhysicalRecording());
      template.Replace("{{NUMERO.CERTIFICADO}}", o.UID);
      template.Replace("{{TIPO.CERTIFICADO}}",
                   this.Certificate.CertificateType.DisplayName.ToUpperInvariant());
      template.Replace("{{FIRMA}}", o.SignedBy.FullName);
      template.Replace("{{FECHA.EXPEDICION}}", this.GetIssueDate());
      template.Replace("{{CADENA.ORIGINAL}}", this.GetDigitalSeal());
      template.Replace("{{SELLO.DIGITAL}}", this.GetDigitalSignature());

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
      if (this.Certificate.Status == CertificateStatus.Pending) {
        return AsWarning("SIN VALOR LEGAL * * * * * SIN VALOR LEGAL");
      }
      var seal = new StringBuilder("||1|" + this.Certificate.UID +
                                      "|" + this.Certificate.Transaction.UID);
      seal.Append("|" + this.Certificate.Transaction.Payments.ReceiptNumbers);
      if (!this.Certificate.Property.IsEmptyInstance) {
        seal.Append("|" + this.Certificate.Property.UID);
      } else if (this.Certificate.OwnerName.Length != 0) {
        seal.Append("|" + this.Certificate.OwnerName.ToUpperInvariant());
      }
      if (this.Certificate.ExtensionData.UseMarginalNotesAsFullBody) {
        seal.Append("|" + "Manual");
      }
      seal.Append("|" + this.Certificate.IssueTime.ToString("yyyyMMddTHH:mm"));
      seal.Append("|" + this.Certificate.SignedBy.Id + "|" + this.Certificate.IssuedBy.Id);
      seal.Append("|" +
            this.Certificate.Integrity.GetUpdatedHashCode().Substring(0, 12).ToUpperInvariant() + "||");

      return EmpiriaString.DivideLongString(seal.ToString(), 72, "&#8203;");
    }

    protected string GetDigitalSignature() {
      if (this.Certificate.Status == CertificateStatus.Pending) {
        return AsWarning("SIN VALOR LEGAL * * * * * SIN VALOR LEGAL");
      }
      string s = Empiria.Security.Cryptographer.CreateDigitalSign(this.GetDigitalSeal());
      return s.Substring(s.Length - 72);
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
      var antecedent = this.Certificate.Property.GetDomainAntecedent();

      if (antecedent.Equals(RecordingAct.Empty)) {
        // If there are not domain acts, then try to get a provisional domain act
        antecedent = this.Certificate.Property.GetProvisionalDomainAct();
        if (antecedent.Equals(RecordingAct.Empty)) {
          return AsWarning("NO SE ENCONTRÓ INFORMACIÓN DEL ANTECEDENTE REGISTRAL. " +
                           "FAVOR DE REVISAR EL FOLIO DEL PREDIO. " +
                           "ES POSIBLE QUE EL CERTIFICADO NO DEBA SER EMITIDO");
        }
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
      if (document.AuthorizationTime != ExecutionServer.DateMaxValue) {
        text.Replace("{{DATE}}", document.AuthorizationTime.ToString("dd \\de MMMM \\de yyyy"));

        return text.ToString();
      }

      var antecedent = this.Certificate.Property.GetProvisionalDomainAct();

      if (!antecedent.Equals(Recording.Empty)) {
        text.Replace("{{DATE}}", antecedent.RegistrationTime.ToString("dd \\de MMMM \\de yyyy"));
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
