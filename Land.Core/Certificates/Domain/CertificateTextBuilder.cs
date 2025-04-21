/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Builder                                 *
*  Type     : CertificateTextBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds the text for a land certificate.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Measurement;

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
              "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE " +
              "<b>REPORTA LOS SIGUIENTES GRAVÁMENES</b>:<br/><br/>" +
              "{{LIMITATION.ACTS}}<br/>" +
              $"{GeneratePreemptiveActsText()}";

      var realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var acts = realEstate.GetHardLimitationActs();

      var actsText = string.Empty;

      for (var i = 0; i < acts.Count; i++) {

        var actName = (acts[i].Kind.Length == 0) ? acts[i].DisplayName : acts[i].Kind;

        actsText += $"<strong>{i + 1}) {actName}</strong> " +
                    $"REGISTRADO EL DÍA {acts[i].RegistrationTime.ToString("dd \\de MMMM \\de yyyy")}," +
                    $"{GetRecordingActAmountsText(acts[i])}" +
                    $"BAJO EL SELLO REGISTRAL <strong>{acts[i].LandRecord.UID}</strong>.<br/>" +
                    $"{GetPartiesText(acts[i])}<br/>";
      }

      x = x.Replace("{{LIMITATION.ACTS}}", actsText);

      return x.ToUpperInvariant();
    }


    static private string GetPartiesText(RecordingAct recordingAct) {
      var primaryParties = recordingAct.Parties.PrimaryParties;

      var html = string.Empty;

      foreach (var primaryParty in primaryParties) {

        html += GetPartyText(primaryParty, 0);

        var secondaryParties = recordingAct.Parties.GetSecondaryPartiesOf(primaryParty.Party);

        foreach (var secondaryParty in secondaryParties) {
          html += GetPartyText(secondaryParty, 1);
        }
      }

      return html;
    }


    static private string GetPartyText(RecordingActParty party, int level) {
      const string t = "{TAB}{PARTY-ROLE}: {PARTY-NAME} {OWNERSHIP}<br/>";

      var html = t.Replace("{PARTY-ROLE}", party.PartyRole.Name);

      html = html.Replace("{TAB}", EmpiriaString.Duplicate(" &#160; &#160; &#160; ", level));

      html = html.Replace("{PARTY-NAME}", party.Party.FullName);

      Unit unit = party.OwnershipPart.Unit;

      if (unit.IsEmptyInstance) {
        html = html.Replace("{OWNERSHIP}", string.Empty);

      } else if (unit.IsIndivisible) {
        html = html.Replace("{OWNERSHIP}", $"({unit.Name})");

      } else {
        html = html.Replace("{OWNERSHIP}", $"({ToPartAmountText(party.OwnershipPart)})");

      }

      return html;
    }


    static private string ToPartAmountText(Quantity ownershipPart) {
      if (ownershipPart.Unit.UID == "Unit.Fraction") {
        var fractionParts = ownershipPart.Amount.ToString().Split('.');

        return $"{fractionParts[0]}/{fractionParts[1].TrimEnd('0')} parte";
      }

      return ownershipPart.ToString();
    }


    static private string GetRecordingActAmountsText(RecordingAct recordingAct) {
      var temp = string.Empty;

      if (recordingAct.Percentage != decimal.One) {
        temp += $" del <b>{(recordingAct.Percentage * 100).ToString("N2")} por ciento,";
      }

      if (recordingAct.OperationAmount != 0m) {
        temp += $" por <b>{recordingAct.OperationCurrency.Format(recordingAct.OperationAmount)}</b>, ";
      }
      return temp;
    }


    private string GenerateInscripcionCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                    "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                    "SE DETERMINA QUE EL BIEN INMUEBLE REFERIDO <strong>SÍ ESTÁ INSCRITO</strong>, " +
                    "Y QUE TIENE ASIGNADO EL <strong>FOLIO REAL ELECTRÓNICO</strong>: " +
                    "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div><br/>" +
                    "{{CURRENT.OWNERSHIP}}" +
                    "{{ON.RESOURCE.TEXT}}" +
                    "{{ON.RESOURCE.METES.AND.BOUNDS}}<br/><br/>";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", _certificate.OnRecordableSubject.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", _certificate.OnRecordableSubject.AsText);

      if (_certificate.OnRecordableSubject is RealEstate realEstate) {

        x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));

        var metesAndBounds = realEstate.MetesAndBounds.Length != 0 ? realEstate.MetesAndBounds : "NO CONSTAN";
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}",
                      $"<br/><br/><b>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</b>:<br/>" +
                      $"{metesAndBounds}");
      } else {
        x = x.Replace("{{CURRENT.OWNERSHIP}}", string.Empty);
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}", string.Empty);
      }

      return x.ToUpperInvariant();
    }


    private string GenerateLibertadGravamenCertificateText() {
      return GenerateRealEstateText() +
             "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE " +
             "<b>NO REPORTA GRAVÁMENES</b>, " +
             "ES DECIR, SE ENCUENTRA <b>LIBRE DE GRAVAMEN</b>.<br/><br/>" +
             $"{GeneratePreemptiveActsText()}";
    }


    private string GenerateCurrentOwnershipText(RealEstate realEstate) {
      var ownershipAct = realEstate.TryGetLastDomainAct();

      if (ownershipAct == null) {
        return "<b>El predio no tiene registrado ningún acto de dominio. " +
               "Por lo tanto, no se pueden determinar sus propietarios o posesionarios.<br/>";
      }

      FixedList<RecordingActParty> owners = ownershipAct.Parties.PrimaryParties;

      var temp =$"ÚLTIMO ACTO DE DOMINIO REGISTRADO EL DÍA {ownershipAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy")}. " +
                $"SELLO REGISTRAL <b>{ownershipAct.LandRecord.UID}</b>.<br/>";

      foreach (RecordingActParty owner in owners) {
        temp += GetPartyText(owner, 0);
      }

      return $"<b>{temp}</b><br/>";
    }


    private string GeneratePreemptiveActsText() {

      if (!(_certificate.OnRecordableSubject is RealEstate realEstate)) {
        return string.Empty;
      }

      if (!realEstate.HasSoftLimitationActs) {
        return string.Empty;
      }

      var lastAct = realEstate.GetSoftLimitationActs().Reverse()[0];

      // TimeSpan.Today.AddDays(lastAct.LandRecord.PresentationTime.ToLocalTime RecordingActType.ValidityDays * -1);

      var actName = (lastAct.Kind.Length == 0) ? lastAct.DisplayName : lastAct.Kind;

      return $"ANOTACIONES PREVENTIVAS:<br/>" +
             $"<strong>{actName}</strong> " +
             $"REGISTRADO EL DÍA {lastAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy")}, " +
             $"{GetRecordingActAmountsText(lastAct)} " +
             $"BAJO EL SELLO REGISTRAL <b>{lastAct.LandRecord.UID}</b>.<br/>" +
             $"{lastAct.Summary}<br/>";
    }


    private string GenerateLimitacionAnotacionCertificateText() {
      return "GenerateLimitacionAnotacionCertificateText";
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
                 "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div><br/>" +
                 "{{CURRENT.OWNERSHIP}}" +
                 "{{ON.RESOURCE.TEXT}}" +
                 "{{ON.RESOURCE.METES.AND.BOUNDS}}<br/><br/>";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", _certificate.OnRecordableSubject.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", _certificate.OnRecordableSubject.AsText);

      if (_certificate.OnRecordableSubject is RealEstate realEstate) {

        x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));

        var metesAndBounds = realEstate.MetesAndBounds.Length != 0 ? realEstate.MetesAndBounds : "NO CONSTAN";
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}",
                      $"<br/><br/><b>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</b>:<br/>" +
                      $"{metesAndBounds}");
      } else {
        x = x.Replace("{{CURRENT.OWNERSHIP}}", string.Empty);
        x = x.Replace("{{ON.RESOURCE.METES.AND.BOUNDS}}", string.Empty);
      }

      return x.ToUpperInvariant();
    }

    #endregion Helpers

  }  // class CertificateTextBuilder

}  // namespace Empiria.Land.Certificates
