/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certification Services                       Component : Certificate builder                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Builder Class                         *
*  Type     : CertificateBuilder                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Builds a certificate Html output using a text-based template.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Text;

using Empiria.Land.Registration;

namespace Empiria.Land.Certification {

  /// <summary>Builds a certificate Html output using a text-based template.</summary>
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

      template.Replace("{{DOCUMENT.OR.PHYSICAL.RECORDING}}", this.GetDocumentOrPhysicalRecording());

      template.Replace("{{SOLICITANTE}}", o.ExtensionData.SeekForName.ToUpperInvariant());
      template.Replace("{{DISTRITO}}", o.RecorderOffice.Alias);
      template.Replace("{{FOLIO REAL}}", o.Property.UID);
      template.Replace("{{DENOMINACION.PREDIO}}", o.ExtensionData.PropertyCommonName);
      template.Replace("{{UBICACION.PREDIO}}", o.ExtensionData.PropertyLocation);
      template.Replace("{{MEDIDAS.Y.COLINDANCIAS}}", o.ExtensionData.PropertyMetesAndBounds);

      template.Replace("{{AÑO.BÚSQUEDA}}", o.ExtensionData.StartingYear >= 1900 ?
                          EmpiriaString.SpeechInteger(o.ExtensionData.StartingYear).ToLowerInvariant() :
                          AsWarning("AÑO DE BÚSQUEDA NO PROPORCIONADO"));

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


    private string GetTemplate() {
      string templatesPath = ConfigurationData.GetString("Templates.Path");

      if (!this.Certificate.ExtensionData.UseMarginalNotesAsFullBody) {

        string bodyTemplateFile = this.Certificate.CertificateType.GetHtmlTemplateFileName();
        string bodyTemplate = File.ReadAllText(Path.Combine(templatesPath, bodyTemplateFile));

        return bodyTemplate;
      } else {
        return "{{CERTIFICATE.BODY}}";

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

    private string GetPhysicalRecording(PhysicalRecording physicalRecording) {
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
