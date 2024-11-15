/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Builder                                 *
*  Type     : CertificateTextBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds the text for a land certificate.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration;

namespace Empiria.Land.Certificates {

  /// <summary>Builds the text for a land certificate.</summary>
  internal class CertificateTextBuilder {

    private readonly Certificate _certificate;

    internal CertificateTextBuilder(Certificate certificate) {
      _certificate = certificate;
    }

    public CertificateType CertificateType {
      get {
        return _certificate.CertificateType;
      }
    }

    internal string Build() {
      if (this.CertificateType.Equals(CertificateType.Propiedad)) {
        return GeneratePropertyCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoPropiedad)) {
        return GenerateNoPropertyCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LibertadGravamen)) {
        return GenerateLibertadGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.Gravamen)) {
        return GenerateGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.Inscripcion)) {
        return GenerateInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoInscripcion)) {
        return GenerateNoInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LimitacionAnotacion)) {
        return GenerateLimitacionAnotacionCertificateText();

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unhandled certificate type {this.CertificateType.Name}.");
      }
    }

    #region Helpers

    private string GenerateGravamenCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "SOBRE EL BIEN INMUEBLE CON <strong>FOLIO REAL ELECTRÓNICO</strong>: " +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div>" +
                       "{{ON.RESOURCE.TEXT}}{{ON.RESOURCE.METES.AND.BOUNDS}}</br></br>" +
                       "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>REPORTA LOS SIGUIENTES GRAVÁMENES</b>:<br>" +
                       "{{LIMITATION.ACTS}}.<br/>";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", _certificate.OnRecordableSubject.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", _certificate.OnRecordableSubject.AsText);

      if (_certificate.OnRecordableSubject is RealEstate realEstate) {
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}",
                      $"<br/><br/><b>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</b>:<br/>" +
                      $"{realEstate.MetesAndBounds}");
      } else {
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}", string.Empty);
      }

      x = x.Replace("{{LIMITATION.ACTS}}", "Embargo");

      return x.ToUpperInvariant();
    }


    private string GenerateInscripcionCertificateText() {
      return string.Empty;
    }


    private string GenerateLibertadGravamenCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "SOBRE EL BIEN INMUEBLE CON <strong>FOLIO REAL ELECTRÓNICO</strong>: " +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div>" +
                       "{{ON.RESOURCE.TEXT}}{{ON.RESOURCE.METES.AND.BOUNDS}}</br></br>" +
                       "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>NO REPORTA GRAVÁMENES</b>, " +
                       "ES DECIR, SE ENCUENTRA <b>LIBRE DE GRAVAMEN</b>.<br/>";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", _certificate.OnRecordableSubject.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", _certificate.OnRecordableSubject.AsText);

      if (_certificate.OnRecordableSubject is RealEstate realEstate) {
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}",
                      $"<br/><br/><b>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</b>:<br/>" +
                      $"{realEstate.MetesAndBounds}");
      } else {
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}", string.Empty);
      }

      return x.ToUpperInvariant();
    }


    private string GenerateLimitacionAnotacionCertificateText() {
      return string.Empty;
    }


    private string GenerateNoInscripcionCertificateText() {
      return string.Empty;
    }


    private string GenerateNoPropertyCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ REGISTRADO</b> NINGÚN BIEN INMUEBLE A NOMBRE DE:" +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.PERSON.NAME}}</strong></div>";

      string x = t.Replace("{{ON.PERSON.NAME}}", _certificate.OnPersonName);

      return x.ToUpperInvariant();
    }


    private string GeneratePropertyCertificateText() {
      return "";
    }

    #endregion Helpers

  }  // class CertificateTextBuilder

}  // namespace Empiria.Land.Certificates
