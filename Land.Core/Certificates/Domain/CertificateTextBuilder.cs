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

      if (this.CertificateType.Equals(CertificateType.Gravamen)) {
        return GenerateGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.Inscripcion)) {
        return GenerateInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LibertadGravamen)) {
        return GenerateLibertadGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LimitacionAnotacion)) {
        return GenerateLimitacionAnotacionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoInscripcion)) {
        return GenerateNoInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoPropiedad)) {
        return GenerateNoPropertyCertificateText();

      } else {

        throw Assertion.EnsureNoReachThisCode($"Unhandled certificate type {this.CertificateType.Name}.");
      }
    }

    #region Helpers


    private string GenerateGravamenCertificateText() {
      var x = GenerateRealEstateText() +
              "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>REPORTA LOS SIGUIENTES GRAVÁMENES</b>:<br/><br/>" +
              "{{LIMITATION.ACTS}}.<br/>";


      var realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var acts = realEstate.GetHardLimitationActs();

      var actsText = string.Empty;

      for (var i = 0; i < acts.Count; i++) {
        var actName = (acts[i].Kind.Length == 0) ? acts[i].DisplayName : acts[i].Kind;

        actsText += $"<strong>{i + 1}) {actName}</strong> " +
                    $"REGISTRADO EL DÍA {acts[i].RegistrationTime.ToString("dd/MMM/yyyy")}, " +
                    $"BAJO EL SELLO REGISTRAL {acts[i].LandRecord.UID}.<br/>" +
                    "{{PARTIES}}<br/>";

        var partiesText = string.Empty;

        foreach (var party in acts[i].Parties.PrimaryParties) {
          partiesText += $"{party.Party.FullName} ({party.PartyRole.Name})<br/>";
        }

        actsText = actsText.Replace("{{PARTIES}}", partiesText);
      }

      x = x.Replace("{{LIMITATION.ACTS}}", actsText);

      return x.ToUpperInvariant();
    }


    private string GenerateInscripcionCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                    "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                    "SE DETERMINA QUE EL BIEN INMUEBLE REFERIDO <strong>SÍ ESTÁ INSCRITO</strong>, " +
                    "Y QUE TIENE ASIGNADO EL <strong>FOLIO REAL ELECTRÓNICO</strong>: " +
                    "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div>" +
                    "{{ON.RESOURCE.TEXT}}{{ON.RESOURCE.METES.AND.BOUNDS}}</br></br>";

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


    private string GenerateLibertadGravamenCertificateText() {
      return GenerateRealEstateText() +
             "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>NO REPORTA GRAVÁMENES</b>, " +
             "ES DECIR, SE ENCUENTRA <b>LIBRE DE GRAVAMEN</b>.<br/>";
    }


    private string GenerateLimitacionAnotacionCertificateText() {
      return string.Empty;
    }


    private string GenerateNoInscripcionCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ INSCRITO</b> EL BIEN INMUEBLE:" +
                       "<div style='font-size:12pt'><strong>{{ON.REAL.STATE.DESCRIPTION}}</strong></div>";

      string x = t.Replace("{{ON.REAL.STATE.DESCRIPTION}}", _certificate.OnRealEstateDescription);

      return x.ToUpperInvariant();
    }


    private string GenerateNoPropertyCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ REGISTRADO</b> NINGÚN BIEN INMUEBLE A NOMBRE DE:" +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.PERSON.NAME}}</strong></div>";

      string x = t.Replace("{{ON.PERSON.NAME}}", _certificate.OnPersonName);

      return x.ToUpperInvariant();
    }


    private string GenerateRealEstateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                 "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                 "SOBRE EL BIEN INMUEBLE CON <strong>FOLIO REAL ELECTRÓNICO</strong>: " +
                 "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div>" +
                 "{{ON.RESOURCE.TEXT}}{{ON.RESOURCE.METES.AND.BOUNDS}}</br></br>";

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

    #endregion Helpers

  }  // class CertificateTextBuilder

}  // namespace Empiria.Land.Certificates
