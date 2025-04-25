/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Builder                                 *
*  Type     : CertificateTextBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds the text for a land certificate.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

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
        return BuildGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.Inscripcion)) {
        return BuildInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LibertadGravamen)) {
        return BuildLibertadGravamenCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.LimitacionAnotacion)) {
        return BuildLimitacionAnotacionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoInscripcion)) {
        return BuildNoInscripcionCertificateText();

      } else if (this.CertificateType.Equals(CertificateType.NoPropiedad)) {
        return BuildNoPropertyCertificateText();

      } else {

        throw Assertion.EnsureNoReachThisCode($"Unhandled certificate type {this.CertificateType.Name}.");
      }
    }

    #region Builders

    private string BuildGravamenCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "SOBRE EL BIEN INMUEBLE CON EL SIGUIENTE <strong>FOLIO REAL ELECTRÓNICO</strong>, " +
                       "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>SÍ SE ENCUENTRA GRAVADO</b>:" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{CURRENT.OWNERSHIP}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildInscripcionCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "SE DETERMINA QUE EL BIEN INMUEBLE REFERIDO <strong>SÍ ESTÁ INSCRITO</strong>, " +
                       "Y QUE TIENE ASIGNADO EL SIGUIENTE <strong>FOLIO REAL ELECTRÓNICO</strong>:" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{CURRENT.OWNERSHIP}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildLibertadGravamenCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "SOBRE EL BIEN INMUEBLE CON EL SIGUIENTE <strong>FOLIO REAL ELECTRÓNICO</strong>, " +
                       "PARA DETERMINAR SI TIENE O NO GRAVÁMENES, RESULTÓ QUE <b>ESTÁ LIBRE DE GRAVÁMENES</b>:" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{CURRENT.OWNERSHIP}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildLimitacionAnotacionCertificateText() {
      return "GenerateLimitacionAnotacionCertificateText";
    }


    private string BuildNoInscripcionCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ INSCRITO</b> EL BIEN INMUEBLE:" +
                       "<div style='font-size:12pt'><strong>{{ON.REAL.STATE.DESCRIPTION}}</strong></div>";

      string x = t.Replace("{{ON.REAL.STATE.DESCRIPTION}}", _certificate.OnRealEstateDescription);

      return x.ToUpperInvariant();
    }


    private string BuildNoPropertyCertificateText() {
      const string t = "QUE HABIENDO INVESTIGADO EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE DE LOS ÚLTIMOS <b>20 AÑOS</b> A LA FECHA, " +
                       "<b>NO SE ENCONTRÓ REGISTRADO</b> NINGÚN BIEN INMUEBLE A NOMBRE DE:" +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.PERSON.NAME}}</strong></div>";

      string x = t.Replace("{{ON.PERSON.NAME}}", _certificate.OnPersonName);

      return x.ToUpperInvariant();
    }


    #endregion Builders

    #region Helpers

    static private string GeneratePartiesText(RecordingAct recordingAct) {
      var primaryParties = recordingAct.Parties.PrimaryParties;

      var html = string.Empty;

      foreach (var primaryParty in primaryParties) {

        html += GeneratePartyText(primaryParty, 0);

        var secondaryParties = recordingAct.Parties.GetSecondaryPartiesOf(primaryParty.Party);

        foreach (var secondaryParty in secondaryParties) {
          html += GeneratePartyText(secondaryParty, -1);
        }
      }

      return html;
    }


    static private string GeneratePartyText(RecordingActParty party, int level) {
      const string t = "{TAB}{PARTY-ROLE}: {PARTY-NAME} {OWNERSHIP}";

      var html = t.Replace("{PARTY-ROLE}", party.PartyRole.Name);

      html = html.Replace("{TAB}", EmpiriaString.Duplicate(" &#160; &#160; &#160; ", level));

      html = html.Replace("{PARTY-NAME}", party.Party.FullName);

      Unit unit = party.OwnershipPart.Unit;

      if (unit.IsEmptyInstance) {
        html = html.Replace("{OWNERSHIP}", string.Empty);

      } else if (unit.IsIndivisible) {
        html = html.Replace("{OWNERSHIP}", $"({unit.Name})");

      } else {
        html = html.Replace("{OWNERSHIP}", $"({GenerateToPartAmountText(party.OwnershipPart)})");

      }

      if (level == -1) {
        return html;
      } else {
        return html + "<br/>";
      }
    }


    static private string GenerateToPartAmountText(Quantity ownershipPart) {
      if (ownershipPart.Unit.UID == "Unit.Fraction") {
        var fractionParts = ownershipPart.Amount.ToString().Split('.');

        return $"{fractionParts[0]}/{fractionParts[1].TrimEnd('0')} parte";
      }

      return ownershipPart.ToString();
    }


    static private string GenerateRecordingActAmountsText(RecordingAct recordingAct) {
      var temp = string.Empty;

      if (recordingAct.Percentage != decimal.One) {
        temp += $" del <b>{(recordingAct.Percentage * 100).ToString("N2")} por ciento,";
      }

      if (recordingAct.OperationAmount != 0m) {
        temp += $" por <b>{recordingAct.OperationCurrency.Format(recordingAct.OperationAmount)}</b>, ";
      }
      return temp;
    }


    private string GenerateCurrentOwnershipText(RealEstate realEstate) {
      var ownershipAct = realEstate.TryGetLastDomainAct();

      if (ownershipAct == null) {
        return "<strong>El predio no tiene registrado ningún acto de dominio. " +
               "Por lo tanto no se pueden determinar sus propietarios o posesionarios.</strong><br/>";
      }

      var temp = "<strong>ÚLTIMO ACTO DE DOMINIO:</strong><br/>" +
                 "<u>{{RECORDING.ACT.NAME}}</u>, CON FECHA DE REGISTRO EL DÍA {{RECORDING.ACT.DATE}}, {{RECORDING.TEXT}}.<br/>";

      temp = temp.Replace("{{RECORDING.ACT.NAME}}", ownershipAct.Kind.Length != 0 ? ownershipAct.Kind : ownershipAct.DisplayName);
      temp = temp.Replace("{{RECORDING.ACT.DATE}}", ownershipAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy"));

      if (ownershipAct.HasBookEntry) {
        temp = temp.Replace("{{RECORDING.TEXT}}", $"INSCRITO EN {ownershipAct.BookEntry.AsText}");
      } else {
        temp = temp.Replace("{{RECORDING.TEXT}}", $"SELLO REGISTRAL {ownershipAct.LandRecord.UID}");
      }

      FixedList<RecordingActParty> owners = ownershipAct.Parties.PrimaryParties;

      foreach (RecordingActParty owner in owners) {
        temp += GeneratePartyText(owner, 0);
      }

      return temp;
    }

    private string GenerateRealEstateText(RealEstate realEstate) {
      const string t = "<div style='text-align:center;font-size:12pt'><strong>{{ON.RESOURCE.CODE}}</strong></div><br/>" +
                       "{{ON.RESOURCE.TEXT}}" +
                       "{{REAL.ESTATE.METES.AND.BOUNDS}}";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", realEstate.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", realEstate.AsText);

      return x.Replace("{{REAL.ESTATE.METES.AND.BOUNDS}}", GenerateRealEstateMetesAndBounds(realEstate));
    }


    private string GenerateRecordingActsText(RealEstate realEstate) {

      FixedList<RecordingAct> recordingActs = realEstate.GetAliveRecordingActsWithPartitionActs();

      var x = "<br/><strong>ANOTACIONES VIGENTES:</strong><br/>";

      if (recordingActs.Count == 0) {
        return x + "NO PRESENTA.";
      }

      for (int i = 0; i < recordingActs.Count; i++) {
        RecordingAct recordingAct = recordingActs[i];

        if (recordingAct.IsHardLimitation) {
          x += $"{i + 1}.- {GenerateHardLimitationActText(recordingAct)}<br/>";

        } else if (recordingAct.IsPreemptiveAct) {
          x += $"{i + 1}.- {GeneratePreemptiveActText(recordingAct)}<br/>";

        } else if (recordingAct.IsAppliedOverNewPartition) {
          x += $"{i + 1}.- {GeneratePartitionActText(recordingAct)}<br/>";

        } else {
          x += $"{i + 1}.- {GenerateRecordingActText(recordingAct)}<br/>";
        }

      }  // for

      return x;
    }


    private string GeneratePreemptiveActText(RecordingAct recordingAct) {

      var actName = (recordingAct.Kind.Length != 0) ? recordingAct.Kind : recordingAct.DisplayName;

      return $"<strong>{actName}</strong> " +
             $"REGISTRADO EL DÍA {recordingAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy")}, " +
             $"{GenerateRecordingActAmountsText(recordingAct)} " +
             $"BAJO EL SELLO REGISTRAL <b>{recordingAct.LandRecord.UID}</b>.<br/>" +
             $"{recordingAct.Summary}";
    }


    private string GenerateRecordingActText(RecordingAct recordingAct) {
      const string t = "<u>{{RECORDING.ACT}}</u>. {{RECORDING.SEAL}}, de fecha {{RECORDING.DATE}}. {{PRIMARY.PARTIES}}";

      var temp = t.Replace("{{RECORDING.ACT}}", recordingAct.Kind.Length != 0 ? recordingAct.Kind : recordingAct.DisplayName);

      if (recordingAct.HasBookEntry) {
        temp = temp.Replace("{{RECORDING.SEAL}}", recordingAct.BookEntry.AsText);
      } else {
        temp = temp.Replace("{{RECORDING.SEAL}}", "Sello registral " + recordingAct.LandRecord.UID);
      }

      temp = temp.Replace("{{RECORDING.DATE}}", recordingAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy"));

      FixedList<RecordingActParty> primaryParties = recordingAct.Parties.PrimaryParties;

      var partiesText = string.Empty;

      foreach (RecordingActParty party in primaryParties) {
        if (partiesText.Length != 0) {
          partiesText += "; ";
        }
        partiesText += GeneratePartyText(party, -1);
      }

      return temp.Replace("{{PRIMARY.PARTIES}}", partiesText);
    }


    static private string GenerateHardLimitationActText(RecordingAct recordingAct) {
      var actName = (recordingAct.Kind.Length != 0) ? recordingAct.Kind : recordingAct.DisplayName;

      return $"<strong>{actName}</strong> " +
            $"REGISTRADO EL DÍA {recordingAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy")}," +
            $"{GenerateRecordingActAmountsText(recordingAct)}" +
            $"BAJO EL SELLO REGISTRAL <strong>{recordingAct.LandRecord.UID}</strong>.<br/>" +
            $"{GeneratePartiesText(recordingAct)}";
    }


    private string GeneratePartitionActText(RecordingAct recordingAct) {

      var temp = recordingAct.Kind.Length != 0 ? recordingAct.Kind : recordingAct.DisplayName;

      temp = $"<strong>{temp}</strong>.";

      var partition = (RealEstate) recordingAct.Resource;

      if (partition.LotSize.Amount != 0) {
        temp += $"Superficie: {partition.LotSize.ToString()}. ";
      } else {
        temp += $"Superficie NO CONSTA. ";
      }

      FixedList<RecordingActParty> owners = recordingAct.Parties.PrimaryParties;

      foreach (RecordingActParty owner in owners) {
        temp += GeneratePartyText(owner, -1) + ", ";
      }

      temp += GenerateRegistrationText(recordingAct);

      return temp;
    }


    private string GenerateRealEstateMetesAndBounds(RealEstate realEstate) {
      var metesAndBounds = realEstate.MetesAndBounds.Length != 0 ? realEstate.MetesAndBounds : "NO CONSTAN";

      return $"<br/><br/><b>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</b>:<br/>{metesAndBounds}<br/><br/>";
    }


    private string GenerateRegistrationText(RecordingAct recordingAct) {
      var temp = recordingAct.LandRecord.IsRegisteredInRecordingBook ?
              recordingAct.LandRecord.TryGetBookEntry().AsText :
              $"Sello registral <strong>{recordingAct.LandRecord.UID}</strong>";

      return temp + " de fecha " + recordingAct.RegistrationTime.ToString("dd \\de MMMM \\de yyyy") + ".";
    }

    #endregion Helpers

  }  // class CertificateTextBuilder

}  // namespace Empiria.Land.Certificates
