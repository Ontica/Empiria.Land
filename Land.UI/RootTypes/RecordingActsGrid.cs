/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : RecordingActsGrid                               Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates an HTML grid with the recording acts of a given document.                            *
*                                                                                                             *
************************** Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Generates an HTML grid with the recording acts of a given document.</summary>
  public class RecordingActsGrid {

    #region Fields

    private RecordingDocument document = null;
    private Dictionary<string, int> antecedentsDictionary = new Dictionary<string, int>();

    #endregion Fields

    #region Constructors and parsers

    private RecordingActsGrid(RecordingDocument document) {
      this.document = document;
    }

    static public string Parse(RecordingDocument document) {
      var grid = new RecordingActsGrid(document);

      return grid.GetHtml();
    }

    private string GetHtml() {
      string html = String.Empty;

      for (int i = 0; i < document.RecordingActs.Count; i++) {
        var recordingAct = document.RecordingActs[i];

        html += this.GetRecordingActRow(recordingAct);
      }
      return html;
    }

    private string GetRecordingActRow(RecordingAct recordingAct) {
      string row = GetRowTemplate(recordingAct);

      row = row.Replace("{{STATUS}}", recordingAct.StatusName);
      row = row.Replace("{{RECORDING.ACT.URL}}", recordingAct.DisplayName);
      row = row.Replace("{{RESOURCE.URL}}", GetResourceCell(recordingAct));

      var antecedentText = GetAntecedentOrTargetCell(recordingAct);
      if (antecedentText.Length == 0) {
        row = row.Replace("{{ANTECEDENT}}", "Sin antecedente registral");
      } else if (antecedentsDictionary.ContainsKey(antecedentText)) {
        int recordingActIndex = antecedentsDictionary[antecedentText];
        row = row.Replace("{{ANTECEDENT}}", "Igual que el acto " +
                                            recordingActIndex.ToString("00"));
      } else {
        antecedentsDictionary.Add(antecedentText, recordingAct.Index + 1);
        row = row.Replace("{{ANTECEDENT}}", antecedentText);
      }
      if (this.document.Security.IsReadyForEdition()) {
        row = row.Replace("{{OPTIONS.LINKS}}", GetDeleteLink(recordingAct));
      } else {
        row = row.Replace("{{OPTIONS.LINKS}}", "&nbsp;");
      }
      row = row.Replace("{{RESOURCE.ID}}", recordingAct.Resource.Id.ToString());
      row = row.Replace("{{ID}}", recordingAct.Id.ToString());

      return row;
    }

    #endregion Constructors and parsers

    #region Private auxiliar methods

    static private string GetResourceCell(RecordingAct recordingAct) {
      if (recordingAct.Resource is RealEstate) {
        var realEstate = (RealEstate) recordingAct.Resource;
        if (!realEstate.IsPartitionOf.IsEmptyInstance &&
             realEstate.Tract.FirstRecordingAct.Equals(recordingAct)) {
          return realEstate.UID + "<br />" +
                 (realEstate.CadastralKey.Length != 0 ?
                 "<i>Catastro: " + realEstate.CadastralKey + "</i><br />" : String.Empty) +
                 "creado como <b>" + realEstate.PartitionNo + "</b> del<br />predio " +
                 realEstate.IsPartitionOf.UID;
        } else {
          return realEstate.UID + "<br />" +
                 (realEstate.CadastralKey.Length != 0 ?
                 "<i>Catastro: " + realEstate.CadastralKey + "</i><br />" : String.Empty);
        }

      } else if (recordingAct.Resource is Association) {
        return recordingAct.Resource.UID + "<br />" +
               ((Association) recordingAct.Resource).Name;

      } else if (recordingAct.Resource is NoPropertyResource) {
        return "Referencia registral:<br />" +
               recordingAct.Resource.UID;

      } else {
        throw Assertion.AssertNoReachThisCode();

      }
    }

    static private string GetAntecedentOrTargetCell(RecordingAct recordingAct) {
      if (recordingAct.RecordingActType.IsAmendmentActType) {
        return GetAmendedItemCell(recordingAct);
      }
      var antecedent = recordingAct.GetRecordingAntecedent();
      if (antecedent.IsEmptyInstance) {
        return String.Empty;

      } else if (!antecedent.PhysicalRecording.IsEmptyInstance) {
        return antecedent.PhysicalRecording.AsText + "<br />" +
               GetRecordingDates(antecedent.Document);

      } else if (antecedent.Document.Equals(recordingAct.Document)) {
        return String.Format("Folio real creado en el acto {0}",
                             (antecedent.Index + 1).ToString("00"));

      } else {
        return antecedent.Document.UID + "<br />" +
               GetRecordingDates(antecedent.Document);

      }
    }

    static private string GetAmendedItemCell(RecordingAct recordingAct) {
      var amendedAct = recordingAct.AmendmentOf;

      if (amendedAct.IsEmptyInstance) {
        return recordingAct.GetRecordingAntecedent().Document.UID;

      } else if (amendedAct.PhysicalRecording.IsEmptyInstance) {
        return amendedAct.RecordingActType.DisplayName +
               (amendedAct.RecordingActType.FemaleGenre ?
                                            " registrada en<br/>" : " registrado en<br/>") +
               "Doc: " + amendedAct.Document.UID + "<br />" +
               GetRecordingDates(amendedAct.Document);

      } else {
        return amendedAct.RecordingActType.DisplayName +
               (amendedAct.RecordingActType.FemaleGenre ?
                                            " registrada en<br/>" : " registrado en<br/>") +
               amendedAct.PhysicalRecording.AsText + "<br />" +
               GetRecordingDates(amendedAct.Document);

      }
    }

    static private string GetRecordingDates(RecordingDocument document) {
      return "Presentación: " + HtmlFormatters.GetDateAsText(document.PresentationTime) + " &nbsp; " +
             "Registro: " + HtmlFormatters.GetDateAsText(document.AuthorizationTime);
    }

    static private string GetRowTemplate(RecordingAct recordingAct) {
      const string template =
          "<tr class='{{CLASS}}'>" +
            "<td><b id='ancRecordingActIndex_{{ID}}'>{{INDEX}}</b></td>" +
            "<td style='white-space:normal'>" +
              "<a {{RECORDING.ACT.CLASS}} href='javascript:doOperation(\"editResource\", {{RESOURCE.ID}}, {{ID}});'>" +
                "{{RECORDING.ACT.URL}}</a></td>" +
            "<td style='white-space:nowrap'>" +
            "<a {{RESOURCE.CLASS}} href='javascript:doOperation(\"editResource\", {{RESOURCE.ID}}, {{ID}});'>" +
                "{{RESOURCE.URL}}</a></td>" +
            "<td style='white-space:normal'>{{ANTECEDENT}}</td>" +
            "<td>{{OPTIONS.LINKS}}</td></tr>";

      int index = recordingAct.Index + 1;

      string html = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");
      html = html.Replace("{{INDEX}}", index.ToString("00"));

      if (!recordingAct.IsCompleted) {
        html = html.Replace("{{RECORDING.ACT.CLASS}}", "class='pending-edition'");
      }
      if (!recordingAct.Resource.IsCompleted) {
        html = html.Replace("{{RESOURCE.CLASS}}", "class='pending-edition'");
      }
      return html;
    }

    static private string GetDeleteLink(RecordingAct recordingAct) {
      const string template =
        "<a href='javascript:doOperation(\"deleteRecordingAct\", {{ID}});' title='Elimina este acto jurídico'>" +
                "<img src='../themes/default/buttons/trash.gif'></a>";

      return template.Replace("{{ID}}", recordingAct.Id.ToString());
    }

    #endregion Private auxiliar methods

  } // class LRSGridControls

} // namespace Empiria.Land.UI
