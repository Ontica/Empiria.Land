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
      const string t = "QUE, HABIÉNDOSE REALIZADO UNA MINUCIOSA BÚSQUEDA EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE A LOS ÚLTIMOS VEINTE AÑOS RESPECTO AL FOLIO ELECTRÓNICO: {{REAL.ESTATE.UID}}, " +
                       "SE ENCONTRÓ QUE EL BIEN INMUEBLE ESTÁ :<br/>" +
                       "<div style='text-align:center;font-size:16pt'><strong>G R A V A D O</strong></div>" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{CURRENT.OWNERSHIP}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.UID}}", realEstate.UID);

      x = x.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildInscripcionCertificateText() {
      const string t = "QUE, HABIÉNDOSE REALIZADO UNA MINUCIOSA BÚSQUEDA EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE A LOS ÚLTIMOS VEINTE AÑOS RESPECTO AL FOLIO ELECTRÓNICO: {{REAL.ESTATE.UID}}, " +
                       "SE ENCONTRÓ QUE EL BIEN INMUEBLE ESTÁ:<br/>" +
                       "<div style='text-align:center;font-size:16pt'><strong>I N S C R I T O</strong></div>" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.UID}}", realEstate.UID);
      x = x.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildLibertadGravamenCertificateText() {
      const string t = "QUE, HABIÉNDOSE REALIZADO UNA MINUCIOSA BÚSQUEDA EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE A LOS ÚLTIMOS VEINTE AÑOS RESPECTO AL FOLIO ELECTRÓNICO: {{REAL.ESTATE.UID}}, " +
                       "SE ENCONTRÓ QUE EL BIEN INMUEBLE ESTÁ:<br/>" +
                       "<div style='text-align:center;font-size:16pt'><strong>L I B R E &#160; &#160; D E  &#160; &#160; G R A V A M E N</strong></div>" +
                       "{{REAL.ESTATE.TEXT}}" +
                       "{{CURRENT.OWNERSHIP}}" +
                       "{{RECORDING.ACTS}}" +
                       "<br/><br/>";

      RealEstate realEstate = (RealEstate) _certificate.OnRecordableSubject;

      var x = t.Replace("{{REAL.ESTATE.UID}}", realEstate.UID);
      x = x.Replace("{{REAL.ESTATE.TEXT}}", GenerateRealEstateText(realEstate));

      x = x.Replace("{{CURRENT.OWNERSHIP}}", GenerateCurrentOwnershipText(realEstate));
      x = x.Replace("{{RECORDING.ACTS}}", GenerateRecordingActsText(realEstate));

      return x.ToUpperInvariant();
    }


    private string BuildLimitacionAnotacionCertificateText() {
      return "GenerateLimitacionAnotacionCertificateText";
    }


    private string BuildNoInscripcionCertificateText() {
      const string t = "QUE, HABIÉNDOSE REALIZADO UNA MINUCIOSA BÚSQUEDA EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE A LOS ÚLTIMOS VEINTE AÑOS, RESPECTO AL SIGUIENTE BIEN INMUEBLE, SE ENCONTRÓ:<br/>" +
                       "<div style='text-align:center;font-size:16pt'><strong>N O &#160; &#160; I N S C R I T O</strong></div><br/>" +
                       "<div><strong>DATOS MANIFESTADOS EN LA CONSTANCIA DE INSCRIPCIÓN EXPEDIDA POR EL AYUNTAMIENTO:</strong></div>" +
                       "{{ON.REAL.STATE.DESCRIPTION}}";

      string x = t.Replace("{{ON.REAL.STATE.DESCRIPTION}}", _certificate.OnRealEstateDescription);

      return x.ToUpperInvariant();
    }


    private string BuildNoPropertyCertificateText() {
      const string t = "QUE, HABIÉNDOSE REALIZADO UNA MINUCIOSA BÚSQUEDA EN LOS ARCHIVOS QUE OBRAN EN ESTA OFICIALÍA A MI CARGO, " +
                       "POR UN LAPSO CORRESPONDIENTE A LOS ÚLTIMOS VEINTE AÑOS, NO SE ENCONTRÓ REGISTRADO NINGÚN BIEN INMUEBLE A " +
                       "NOMBRE DE LA SIGUIENTE PERSONA:<br/>" +
                       "<div style='text-align:center;font-size:16pt'><strong>N O &#160; &#160; P R O P I E D A D</strong></div>" +
                       "<div style='text-align:center;font-size:12pt'><strong>{{ON.PERSON.NAME}}</strong></div>";

      string x = t.Replace("{{ON.PERSON.NAME}}", _certificate.OnPersonName);

      return x.ToUpperInvariant();
    }


    #endregion Builders

    #region Helpers

    static private string FormatDate(DateTime date) {
      return date.ToString("dd \\de MMMM \\de yyyy");
    }


    static private string GeneratePartiesText(RecordingAct recordingAct) {
      var primaryParties = recordingAct.Parties.PrimaryParties;

      var html = string.Empty;

      foreach (var primaryParty in primaryParties) {

        html += GeneratePartyText(primaryParty, 0);

        var secondaryParties = recordingAct.Parties.GetSecondaryPartiesOf(primaryParty.Party);

        foreach (var secondaryParty in secondaryParties) {
          html += GeneratePartyText(secondaryParty, 1);
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
                 "<strong>{{RECORDING.ACT.NAME}}</strong>, CON FECHA DE REGISTRO EL DÍA {{RECORDING.ACT.DATE}}, {{RECORDING.TEXT}}.<br/>";

      temp = temp.Replace("{{RECORDING.ACT.NAME}}", ownershipAct.KindOrDisplayName);
      temp = temp.Replace("{{RECORDING.ACT.DATE}}", FormatDate(ownershipAct.RegistrationTime));

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
                       "{{ON.RESOURCE.FIELDS}} " +
                       "<br/><br/><strong>DESCRIPCIÓN</strong>:<br/>{{ON.RESOURCE.TEXT}}" +
                       "{{REAL.ESTATE.METES.AND.BOUNDS}}";

      string x = t.Replace("{{ON.RESOURCE.CODE}}", realEstate.UID);

      x = x.Replace("{{ON.RESOURCE.TEXT}}", realEstate.AsText);
      x = x.Replace("{{ON.RESOURCE.FIELDS}}", GenerateRealEstateFieldsText(realEstate));

      return x.Replace("{{REAL.ESTATE.METES.AND.BOUNDS}}", GenerateRealEstateMetesAndBounds(realEstate));
    }

    private string GenerateRealEstateFieldsText(RealEstate realEstate) {
      const string t = "Clave catastral: <strong>{{CADASTRAL.KEY}}</strong>, " +
                       "Municipio: <strong>{{MUNICIPALITY}}</strong>, " +
                       "Tipo de predio: <strong>{{REAL.ESTATE.KIND}}</strong>, " +
                       "Superficie de terreno: <strong>{{LOT.SIZE}}</strong>, " +
                       "Superficie de construcción: <strong>{{BUILDING_AREA}} M2</strong>, " +
                       "Indiviso: <strong>{{UNDIVIDED_PCT}}</strong>, " +
                       "Lote: <strong>{{LOT}}</strong>, " +
                       "Manzana: <strong>{{BLOCK}}</strong>, " +
                       "Sección: <strong>{{SECTION}}</strong>, " +
                       "Fracción: <strong>{{PARTITION_NO}}</strong>";

      string x = t.Replace("{{CADASTRAL.KEY}}", realEstate.CadastralKey.Length != 0 ? realEstate.CadastralKey : "no registrada");

      x = x.Replace("{{MUNICIPALITY}}", realEstate.Municipality.Name);
      x = x.Replace("{{REAL.ESTATE.KIND}}", realEstate.Kind);
      x = x.Replace("{{LOT.SIZE}}", realEstate.LotSize.ToString());
      x = x.Replace("{{BUILDING_AREA}}", realEstate.BuildingArea.ToString());
      x = x.Replace("{{UNDIVIDED_PCT}}", realEstate.UndividedPct != 0 ? $"{realEstate.UndividedPct} por ciento" : "sin indiviso");
      x = x.Replace("{{LOT}}", realEstate.Lot.Length != 0 ? realEstate.Lot : "No registrado");
      x = x.Replace("{{BLOCK}}", realEstate.Block.Length != 0 ? realEstate.Block : "No registrado");
      x = x.Replace("{{SECTION}}", realEstate.Section.Length != 0 ? realEstate.Section : "No registrada");
      x = x.Replace("{{PARTITION_NO}}", realEstate.PartitionNo.Length != 0 ? realEstate.PartitionNo : "No registrada");

      return x;
    }

    private string GenerateRecordingActsText(RealEstate realEstate) {

      FixedList<RecordingAct> recordingActs = GetRecordingActs(realEstate);

      string x;

      if (_certificate.CertificateType == CertificateType.Inscripcion) {
        x = "<br/><strong>CONTIENE LAS SIGUIENTES ANOTACIONES:</strong><br/>";
      } else {

        x = "<br/><strong>ANOTACIONES VIGENTES:</strong><br/>";
      }

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

      return $"<strong>{recordingAct.KindOrDisplayName}</strong> " +
             $"REGISTRADO EL DÍA {FormatDate(recordingAct.RegistrationTime)}, " +
             $"{GenerateRecordingActAmountsText(recordingAct)} " +
             $"BAJO EL SELLO REGISTRAL <b>{recordingAct.LandRecord.UID}</b>.<br/>" +
             $"{recordingAct.Summary}";
    }


    private string GenerateRecordingActText(RecordingAct recordingAct) {
      const string t = "<strong>{{RECORDING.ACT}}</strong>. {{RECORDING.SEAL}}, de fecha {{RECORDING.DATE}}. {{PRIMARY.PARTIES}}";

      var temp = t.Replace("{{RECORDING.ACT}}", recordingAct.KindOrDisplayName);

      if (recordingAct.HasBookEntry) {
        temp = temp.Replace("{{RECORDING.SEAL}}", recordingAct.BookEntry.AsText);
      } else {
        temp = temp.Replace("{{RECORDING.SEAL}}", $"Sello registral <strong>{recordingAct.LandRecord.UID}</strong>");
      }

      temp = temp.Replace("{{RECORDING.DATE}}", FormatDate(recordingAct.RegistrationTime));

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

      return $"<strong>{recordingAct.KindOrDisplayName}</strong> " +
            $"REGISTRADO EL DÍA {FormatDate(recordingAct.RegistrationTime)}," +
            $"{GenerateRecordingActAmountsText(recordingAct)}" +
            $"BAJO EL SELLO REGISTRAL <strong>{recordingAct.LandRecord.UID}</strong>.<br/>" +
            $"{GeneratePartiesText(recordingAct)}";
    }


    private string GeneratePartitionActText(RecordingAct recordingAct) {

      var temp = $"<strong>{recordingAct.KindOrDisplayName}</strong>.";

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

      return $"<br/><br/><strong>CON LAS SIGUIENTES MEDIDAS Y COLINDANCIAS</strong>:<br/>{metesAndBounds}<br/><br/>";
    }


    private string GenerateRegistrationText(RecordingAct recordingAct) {

      string temp;

      if (recordingAct.LandRecord.IsRegisteredInRecordingBook) {
        temp = recordingAct.LandRecord.TryGetBookEntry().AsText;
      } else {
        temp = $"Sello registral <strong>{recordingAct.LandRecord.UID}</strong>";
      }

      return $"{temp} de fecha {FormatDate(recordingAct.RegistrationTime)}.";
    }


    private FixedList<RecordingAct> GetRecordingActs(RealEstate realEstate) {
      if (_certificate.CertificateType != CertificateType.Inscripcion) {
        return realEstate.GetAliveRecordingActsWithPartitionActs();
      }

      FixedList<RecordingAct> recordingActs = realEstate.GetAliveRecordingActsWithPartitionActs();

      recordingActs = FixedList<RecordingAct>.Merge(recordingActs, realEstate.GetDomainActs());

      return recordingActs.Sort((x, y) => x.CompareToString.CompareTo(y.CompareToString));
    }

    #endregion Helpers

  }  // class CertificateTextBuilder

}  // namespace Empiria.Land.Certificates
