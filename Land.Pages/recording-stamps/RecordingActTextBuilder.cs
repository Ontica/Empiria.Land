/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Pages                         Component : Presentation Layer                      *
*  Assembly : Empiria.Land.Pages.dll                     Pattern   : Web Page                                *
*  Type     : RecordingActTextBuilder                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Text builder for recording acts.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Registration;

namespace Empiria.Land.Pages {

  /// <summary>Text builder for recording acts.</summary>
  internal class RecordingActTextBuilder {

    private readonly RecordingAct _recordingAct;

    internal RecordingActTextBuilder(RecordingAct recordingAct) {
      Assertion.Require(recordingAct, nameof(recordingAct));

      _recordingAct = recordingAct;
    }


    internal string GetNotesText() {
      if (_recordingAct.Summary.Length == 0) {
        return string.Empty;
      }

      const string t = "Notas: {NOTES}<br/>";

      return t.Replace("{NOTES}", _recordingAct.Summary);
    }


    internal string GetPartiesText() {
      var graph = new PartiesGraph(_recordingAct);

      var html = string.Empty;

      foreach (PartiesGraphNode node in graph.Roots) {
        html += GetPartyText(graph, node);
      }

      return html;
    }


    static private string GetPartyText(PartiesGraph graph, PartiesGraphNode node) {
      const string t = "{TAB}{PARTY-ROLE}: {PARTY-NAME} {OWNERSHIP}<br/>";

      var p = Reloaders.Reload(node.RecordingActParty);

      var html = t.Replace("{PARTY-ROLE}", p.PartyRole.Name);

      html = html.Replace("{TAB}", EmpiriaString.Duplicate(" &#160; &#160; &#160; ", node.Level - 1));

      html = html.Replace("{PARTY-NAME}", Reloaders.Reload(p.Party).FullName);

      if (p.OwnershipPart.Unit.IsEmptyInstance) {
        html = html.Replace("{OWNERSHIP}", string.Empty);

      } else if (p.OwnershipPart.Unit == DataTypes.Unit.FullUnit ||
                 p.OwnershipPart.Unit == DataTypes.Unit.UndividedUnit) {
        html = html.Replace("{OWNERSHIP}", $"({p.OwnershipPart.Unit.Name})");

      } else {
        html = html.Replace("{OWNERSHIP}", $"({ToPartAmountText(p.OwnershipPart)})");

      }

      foreach (PartiesGraphNode child in graph.GetChildren(node)) {
        html += GetPartyText(graph, child);
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


    internal string GetAmendmentActText(int index) {
      const string template = "{INDEX}.- <b style='text-transform:uppercase'>{AMENDMENT.ACT}</b> " +
                              "{AMENDMENT.ACT.RECORDING}, {RESOURCE.DATA}.<br/>";

      string x = template.Replace("{INDEX}", index.ToString());

      Assertion.Require(_recordingAct.RecordingActType.IsAmendmentActType,
                        "Recording act is not an amendment act.");

      x = x.Replace("{AMENDMENT.ACT}", this.GetAmendmentActTypeDisplayName());

      RecordingAct amendedAct = _recordingAct.AmendmentOf;

      amendedAct = Reloaders.Reload(amendedAct);

      if (amendedAct.IsEmptyInstance) {
        x = x.Replace(" {AMENDMENT.ACT.RECORDING},", " ");
      } else {

        var amendedActName = amendedAct.Kind.Length != 0 ? amendedAct.Kind : amendedAct.RecordingActType.DisplayName;

        if (amendedAct.OperationAmount != 0) {
          amendedActName += $" por {amendedAct.OperationAmount.ToString("C2")} {amendedAct.OperationCurrency.Name}, ";
        }

        var legend = amendedAct.RecordingActType.FemaleGenre ? "la cual está inscrito" : "el cual está inscrito";

        if (amendedAct.PhysicalRecording.IsEmptyInstance) {
          x = x.Replace("{AMENDMENT.ACT.RECORDING}",
                        "sobre " + amendedActName + " " +
                        $"{legend}  en el acto número {amendedAct.Index + 1} del documento <b> {amendedAct.Document.UID}</b> el día " +
                        CommonMethods.GetDateAsText(amendedAct.Document.AuthorizationTime));

        } else {
          x = x.Replace("{AMENDMENT.ACT.RECORDING}",
                        "sobre " + amendedActName + " " +
                        legend + " en la " + amendedAct.PhysicalRecording.AsText + " el día " +
                        CommonMethods.GetDateAsText(amendedAct.PhysicalRecording.MainDocument.AuthorizationTime));
        }

      }

      Resource resource = Reloaders.Reload(_recordingAct.Resource);

      if (resource is RealEstate) {
        x = x.Replace("{RESOURCE.DATA}", "sobre el bien inmueble con folio real " +
                      GetRealEstateTextWithAntecedentAndCadastralKey(_recordingAct));

      } else if (resource is Association) {
        x = x.Replace("{RESOURCE.DATA}", "sobre la sociedad o asociación denominada '" +
                      ((Association) resource).Name) + "' con folio electrónico <b class='bigger'>" + resource.UID + "</b>";

      } else if (resource is NoPropertyResource) {
        x = x.Replace("{RESOURCE.DATA}", "con identificador de inscripción <b class='bigger'>" + resource.UID + "</b>");

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unknown rule for resources with type {resource.GetType()}.");

      }

      return x;
    }

    private string GetAmendmentActTypeDisplayName() {
      Assertion.Require(_recordingAct.RecordingActType.IsAmendmentActType,
                       "_recordingAct.IsAmendment should be true.");


      if (_recordingAct.Kind.Length != 0) {
        return _recordingAct.Kind;
      }

      if (!_recordingAct.RecordingActType.RecordingRule.UseDynamicActNaming) {
        return _recordingAct.RecordingActType.DisplayName;
      }

      string x = _recordingAct.RecordingActType.RecordingRule.DynamicActNamePattern + " {AMENDED.ACT}";

      var amendedAct = _recordingAct.AmendmentOf;

      if (!_recordingAct.RecordingActType.AppliesToARecordingAct) {
        return x.Replace(" {AMENDED.ACT}", String.Empty);
      }

      if (amendedAct.RecordingActType.FemaleGenre) {
        return x.Replace("{AMENDED.ACT}", "DE LA " + amendedAct.RecordingActType.DisplayName);
      } else {
        return x.Replace("{AMENDED.ACT}", "DEL " + amendedAct.RecordingActType.DisplayName);
      }
    }


    internal string GetNoPropertyActText(int index) {
      const string template =
          "{INDEX}.- <b style='text-transform:uppercase'>{RECORDING.ACT}</b> " +
          "(folio electrónico <b>{RESOURCE.UID}</b>)<br/>";

      string x = template.Replace("{INDEX}", index.ToString());

      if (_recordingAct.Kind.Length != 0) {
        x = x.Replace("{RECORDING.ACT}", _recordingAct.Kind);
      } else {
        x = x.Replace("{RECORDING.ACT}", _recordingAct.DisplayName);
      }

      return x.Replace("{RESOURCE.UID}", _recordingAct.Resource.UID);
    }


    internal string GetRealEstateActText(int index) {
      Assertion.Require(_recordingAct.Resource is RealEstate,
                       $"Type mismatch parsing real estate with id {_recordingAct.Resource.Id}");

      RealEstate property = (RealEstate) Reloaders.Reload(_recordingAct.Resource);

      if (!property.IsPartitionOf.IsEmptyInstance &&
           property.IsInTheRankOfTheFirstDomainAct(_recordingAct)) {
        return this.GetRealEstateActTextOverNewPartition(property, index);

      } else {
        return this.GetRealEstateActOverTheWhole(index);

      }
    }


    internal string GetParentActText(int index, FixedList<RecordingAct> children) {
      const string overTheWhole =
          "{INDEX}.- {RECORDING.ACT} el cual se aplica sobre los siguientes <b>{CHILDREN.COUNT}</b> predios:<br/>";

      string x = String.Empty;

      x = overTheWhole.Replace("{INDEX}", index.ToString());
      x = x.Replace("{RECORDING.ACT}", this.GetRecordingActDisplayName());
      x = x.Replace("{CHILDREN.COUNT}", $"{children.Count} ({EmpiriaString.SpeechInteger(children.Count)})");

      for (int i = 0; i < children.Count; i++) {
        x += GetChildActText(i + 1, children[i]);
      }
      x += "<br/>";
      return x;
    }


    internal string GetChildActText(int index, RecordingAct child) {
      const string overTheWhole =
              "<div style='padding-left:20pt'>{INDEX}.- Bien inmueble de tipo <b>{PROPERTY.KIND}</b> con folio real {PROPERTY.UID}.</div>";

      string x = String.Empty;

      x = overTheWhole.Replace("{INDEX}", index.ToString());

      x = x.Replace("{PROPERTY.KIND}", _recordingAct.Resource.Kind);

      x = x.Replace("{PROPERTY.UID}", GetRealEstateTextWithAntecedentAndCadastralKey(child));

      return x;
    }


    private string GetRealEstateActOverTheWhole(int index) {
      const string overTheWhole =
          "{INDEX}.- {RECORDING.ACT} sobre el bien inmueble de tipo <b>{PROPERTY.KIND}</b> con folio real {PROPERTY.UID}.<br/>";

      string x = String.Empty;

      x = overTheWhole.Replace("{INDEX}", index.ToString());
      x = x.Replace("{RECORDING.ACT}", this.GetRecordingActDisplayName());

      x = x.Replace("{PROPERTY.KIND}", _recordingAct.Resource.Kind);

      x = x.Replace("{PROPERTY.UID}",
                    GetRealEstateTextWithAntecedentAndCadastralKey(_recordingAct));

      return x;
    }


    private string GetRealEstateActTextOverNewPartition(RealEstate newPartition, int index) {
      const string overPartition =
          "{INDEX}.- {RECORDING.ACT} sobre la " +
          "<b>{PARTITION.TEXT}</b> del bien inmueble con folio {PARTITION.OF}, misma a la que " +
          "se le asignó el folio real {PROPERTY.UID}.<br/>";

      const string overPartitionMale =
          "{INDEX}.- {RECORDING.ACT} sobre el " +
          "<b>{PARTITION.TEXT}</b> del bien inmueble con folio {PARTITION.OF}, mismo al que " +
          "se le asignó el folio real {PROPERTY.UID}.<br/>";

      const string overLot =
          "{INDEX}.- {RECORDING.ACT} sobre el " +
          "<b>{PARTITION.TEXT}</b> de la lotificación con folio {PARTITION.OF}, mismo al que " +
          "se le asignó el folio real {PROPERTY.UID}.<br/>";

      const string overApartment =
          "{INDEX}.- {RECORDING.ACT} sobre el " +
          "<b>{PARTITION.TEXT}</b> del condominio con folio {PARTITION.OF}, mismo a la que " +
          "se le asignó el folio real {PROPERTY.UID}.<br/>";

      const string overHouse =
          "{INDEX}.- {RECORDING.ACT} sobre la " +
          "<b>{PARTITION.TEXT}</b> del fraccionamiento con folio real {PARTITION.OF}, misma a la que " +
          "se le asignó el folio real {PROPERTY.UID}.<br/>";

      Assertion.Require(!newPartition.IsPartitionOf.IsEmptyInstance, "Property is not a partition.");

      string x = String.Empty;

      string partitionText;

      if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length == 0) {
        partitionText = $"FRACCIÓN O PARTE SIN IDENTIFICAR";

      } else if (newPartition.Kind.Length != 0 && newPartition.PartitionNo.Length == 0) {
        partitionText = $"{newPartition.Kind} SIN IDENTIFICAR";

      } else if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length != 0) {
        partitionText = $"FRACCIÓN O PARTE {newPartition.PartitionNo}";

      } else {
        partitionText = $"{newPartition.Kind} {newPartition.PartitionNo}";
      }

      if (newPartition.Kind.StartsWith("Fracción") ||
          newPartition.Kind.StartsWith("Bodega")) {
        x = overPartition.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);
        if (_recordingAct.RecordingActType.IsDomainActType) {
          x = x.Replace("sobre la", "de la");
        }

      } else if (newPartition.Kind.StartsWith("Estacionamiento") ||
                 newPartition.Kind.StartsWith("Local")) {
        x = overPartitionMale.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);
        if (_recordingAct.RecordingActType.IsDomainActType) {
          x = x.Replace("sobre el", "del");
        }

      } else if (newPartition.Kind.StartsWith("Lote")) {
        x = overLot.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);
        if (_recordingAct.RecordingActType.IsDomainActType) {
          x = x.Replace("sobre el", "del");
        }

      } else if (newPartition.Kind.StartsWith("Casa")) {
        x = overHouse.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);
        if (_recordingAct.RecordingActType.IsDomainActType) {
          x = x.Replace("sobre la", "de la");
        }

      } else if (newPartition.Kind.StartsWith("Departamento")) {
        x = overApartment.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);
        if (_recordingAct.RecordingActType.IsDomainActType) {
          x = x.Replace("sobre el", "del");
        }

      } else {
        x = overPartition.Replace("{INDEX}", index.ToString());
        x = x.Replace("{PARTITION.TEXT}", partitionText);

      }

      var parentAntecedent =
              newPartition.IsPartitionOf.Tract.GetRecordingAntecedent(_recordingAct.Document.PresentationTime);

      if (!parentAntecedent.PhysicalRecording.IsEmptyInstance) {
        x = x.Replace("{PARTITION.OF}", "<u>" + newPartition.IsPartitionOf.UID + "</u> " +
                      "y antecedente registral en " + parentAntecedent.PhysicalRecording.AsText);
      } else {
        x = x.Replace("{PARTITION.OF}", "<u>" + newPartition.IsPartitionOf.UID + "</u>");
      }

      x = x.Replace("{RECORDING.ACT}", GetRecordingActDisplayName());

      x = x.Replace("{PROPERTY.UID}", GetRealEstateTextWithCadastralKey(newPartition));

      return x;
    }

    static private string GetRealEstateTextWithAntecedentAndCadastralKey(RecordingAct recordingAct) {
      var domainAntecedent = recordingAct.Resource.Tract.GetRecordingAntecedent(recordingAct);

      domainAntecedent = Reloaders.Reload(domainAntecedent);

      var property = (RealEstate) Reloaders.Reload(recordingAct.Resource);

      string x = GetRealEstateTextWithCadastralKey(property);

      if (property.IsPartitionOf.IsEmptyInstance && domainAntecedent.Equals(RecordingAct.Empty)) {
        x += " sin antecedente registral";

      } else if (!property.IsPartitionOf.IsEmptyInstance && domainAntecedent.Equals(RecordingAct.Empty)) {
        // no-op

      } else if (!domainAntecedent.PhysicalRecording.IsEmptyInstance) {
        if (!recordingAct.AmendmentOf.PhysicalRecording.Equals(domainAntecedent.PhysicalRecording)) {
          x += ", con antecedente registral en " + domainAntecedent.PhysicalRecording.AsText;
        }

      } else if (domainAntecedent.Document.Equals(recordingAct.Document)) {
        x += ", registrado en este documento";

      } else if (!(domainAntecedent is DomainAct)) {   // TODO: this is very strange, is a special case
        x += String.Format(" el {0} bajo el número de documento electrónico {1}",
                           CommonMethods.GetDateAsText(domainAntecedent.Document.AuthorizationTime),
                           domainAntecedent.Document.UID);
      } else {
        x += String.Format(", con antecedente inscrito el {0} bajo el número de documento electrónico {1}",
                           CommonMethods.GetDateAsText(domainAntecedent.Document.AuthorizationTime),
                           domainAntecedent.Document.UID);
      }
      return x;
    }


    internal string GetAssociationActText(Association association, int index) {
      const string incorporationActText =
            "{INDEX}.- <b style='text-transform:uppercase'>CONSTITUCIÓN</b> de " +
            "la {ASSOCIATION.KIND} denominada <b>{ASSOCIATION.NAME}</b>, " +
            "misma a la que se le asignó el folio único <b class='bigger'>{ASSOCIATION.UID}</b>.<br/>";

      const string overAssociationWithIncorporationActInDigitalRecording =
          "{INDEX}.- <b style='text-transform:uppercase'>{RECORDING.ACT}</b> de " +
          "la {ASSOCIATION.KIND} denominada <b>{ASSOCIATION.NAME}</b>, " +
          "con folio único <b class='bigger'>{ASSOCIATION.UID}</b>.<br/>";

      const string overAssociationWithIncorporationActInPhysicalRecording =
          "{INDEX}.- <b style='text-transform:uppercase'>{RECORDING.ACT}</b> de " +
          "la {ASSOCIATION.KIND} denominada <b>{ASSOCIATION.NAME}</b>, " +
          "con folio único <b class='bigger'>{ASSOCIATION.UID}</b> y " +
          "antecedente registral en {ANTECEDENT}.<br/>";

      RecordingAct incorporationAct = association.GetIncorporationAct();

      string x = String.Empty;

      if (_recordingAct.Equals(incorporationAct)) {
        x = incorporationActText.Replace("{INDEX}", index.ToString());

      } else if (incorporationAct.PhysicalRecording.IsEmptyInstance) {
        x = overAssociationWithIncorporationActInDigitalRecording.Replace("{INDEX}", index.ToString());
        x = x.Replace("{RECORDING.ACT}", _recordingAct.DisplayName);

      } else if (!incorporationAct.PhysicalRecording.IsEmptyInstance) {
        x = overAssociationWithIncorporationActInPhysicalRecording.Replace("{INDEX}", index.ToString());
        x = x.Replace("{RECORDING.ACT}", _recordingAct.DisplayName);
        x = x.Replace("{ANTECEDENT}", incorporationAct.PhysicalRecording.AsText);

      } else {
        throw Assertion.EnsureNoReachThisCode();

      }

      x = x.Replace("{ASSOCIATION.UID}", association.UID);
      x = x.Replace("{ASSOCIATION.NAME}", association.Name);
      x = x.Replace("{ASSOCIATION.KIND}", association.Kind);

      return x;
    }


    private string GetRecordingActDisplayName() {
      var temp = string.Empty;

      if (_recordingAct.Kind.Length != 0) {
        temp = $"<b style='text-transform:uppercase'>{_recordingAct.Kind}</b>";
      } else {
        temp = $"<b style='text-transform:uppercase'>{_recordingAct.RecordingActType.DisplayName}</b>";
      }

      if (_recordingAct.Percentage != decimal.One) {
        temp += $" del <b>{(_recordingAct.Percentage * 100).ToString("N2")} por ciento,";
      }

      if (_recordingAct.OperationAmount != 0m) {
        temp += $" por {_recordingAct.OperationAmount.ToString("C2")} {_recordingAct.OperationCurrency.Name}, ";
      }

      return temp;
    }

    static private string GetRealEstateTextWithCadastralKey(RealEstate property) {
      string location = string.Empty;

      if (property.Lot.Length != 0) {
        location = $"Lote: <b>{property.Lot}</b>";
      }

      if (property.Block.Length != 0) {
        location += location.Length != 0 ? ", " : string.Empty;
        location += $"Manzana: <b>{property.Block}</b>";
      }

      if (property.Section.Length != 0) {
        location += location.Length != 0 ? ", " : string.Empty;
        location += $"Sección: <b>{property.Section}</b>";
      }

      if (property.CadastralKey.Length != 0) {
        location += location.Length != 0 ? ", " : string.Empty;
        location += $"Clave catastral: <b>{property.CadastralKey}</b>";
      } else {
        location += location.Length != 0 ? ", " : string.Empty;
        location += $"sin clave catastral";
      }

      string lotSize;

      if (property.LotSize.Unit.IsEmptyInstance || property.LotSize.Amount == 0) {
        lotSize = "<b>NO CONSTA</b>";
      } else {
        lotSize = $"<b>{property.LotSize}</b>";
      }


      if (property.BuildingArea != 0) {
        lotSize += $" y <b>{property.BuildingArea} M2</b> de construcción";
      } else {
        lotSize += $", sin superficie de construcción reportada";
      }


      if (property.UndividedPct != 0) {
        lotSize += $", y un indiviso de <b>{property.UndividedPct} por ciento</b>";
      }


      return $"<b class='bigger'>{property.UID}</b> ({location}, con una superficie de {lotSize})";
    }


  }  // class RecordingActTextBuilder

}  // namespace Empiria.Land.Pages
