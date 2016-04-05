/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : RecordingActsGrid                               Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates the grid HTML content for a document's recording acts.                               *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Generates the grid HTML content for a document's recording acts.</summary>
  public class RecordingActsGrid {

    #region Fields

    private RecordingDocument document = null;

    #endregion Fields

    #region Constructors and parsers

    private RecordingActsGrid(RecordingDocument document) {
      // TODO: Complete member initialization
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

        for (int j = 0; j < recordingAct.TractIndex.Count; j++) {
          if (j == 0) {
            html += this.GetRecordingActRow(recordingAct, recordingAct.TractIndex[j]);
          } else {
            html += this.GetAdditionalTargetRow(recordingAct, recordingAct.TractIndex[j]);
          }
        } // for int i
      } // for int j
      return html;
    }

    private string GetAdditionalTargetRow(RecordingAct recordingAct, TractItem baseTarget) {
 	    throw new NotImplementedException();
    }

    Dictionary<string, int> antecedentsDictionary = new Dictionary<string, int>();

    private string GetRecordingActRow(RecordingAct recordingAct, TractItem tractItem) {
      string row = GetRowTemplate(recordingAct);

      row = row.Replace("{{STATUS}}", recordingAct.StatusName);
      row = row.Replace("{{RECORDING.ACT.URL}}", recordingAct.DisplayName);
      row = row.Replace("{{RESOURCE.URL}}", GetResourceCell(tractItem));

      var antecedent = GetAntecedentOrTargetCell(tractItem);
      if (antecedentsDictionary.ContainsKey(antecedent)) {
        int recordingActIndex = antecedentsDictionary[antecedent];
        row = row.Replace("{{ANTECEDENT}}", "Igual que el acto " +
                                            recordingActIndex.ToString("00"));
      } else {
        antecedentsDictionary.Add(antecedent, tractItem.RecordingAct.Index + 1);
        row = row.Replace("{{ANTECEDENT}}", antecedent);
      }
      row = row.Replace("{{OPTIONS.COMBO}}", GetOptionsCombo(tractItem));

      row = row.Replace("{{TARGET.ID}}", recordingAct.Id.ToString());
      row = row.Replace("{{RESOURCE.ID}}", recordingAct.Id.ToString());
      row = row.Replace("{{ID}}", recordingAct.Id.ToString());

      return row;
    }

    #endregion Constructors and parsers

    #region Public properties

    #endregion Public properties

    #region Private auxiliar methods

    static private string GetResourceCell(TractItem tractItem) {
      if (tractItem.Resource is RealEstate) {
        var realEstate = (RealEstate) tractItem.Resource;
        if (!realEstate.IsPartitionOf.IsEmptyInstance &&
             realEstate.FirstRecordingAct.Equals(tractItem.RecordingAct)) {
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
      } else if (tractItem.Resource is Association) {
        return tractItem.Resource.UID + "<br />" +
               ((Association) tractItem.Resource).Name;
      } else {
        return "Referencia registral:<br />" +
               tractItem.Resource.UID;
      }
    }

    static private string GetAntecedentOrTargetCell(TractItem tractItem) {
      if (tractItem.RecordingAct.RecordingActType.IsAmendmentActType) {
        return GetAmendedItemCell(tractItem);
      }
      var antecedent = tractItem.GetRecordingAntecedent();
      if (antecedent.IsEmptyInstance) {
        return "Sin antecedente registral";
      } else if (!antecedent.PhysicalRecording.IsEmptyInstance) {
        return antecedent.PhysicalRecording.AsText + "<br />" +
               GetRecordingDates(antecedent.Document);
      } else if (antecedent.Document.Equals(tractItem.RecordingAct.Document)) {
        return String.Format("Folio real creado en el acto {0}",
                             (antecedent.Index + 1).ToString("00"));
      } else {
        return antecedent.Document.UID + "<br />" +
               GetRecordingDates(antecedent.Document);
      }
    }

    private static string GetRecordingDates(RecordingDocument document) {
      return "Present: " + GetDateAsText(document.PresentationTime) + " &nbsp; " +
             "Reg: " + GetDateAsText(document.AuthorizationTime);
    }

    static private string GetAmendedItemCell(TractItem tractItem) {
      var amendedAct = tractItem.RecordingAct.AmendmentOf;

      if (!amendedAct.PhysicalRecording.IsEmptyInstance) {
        return amendedAct.RecordingActType.DisplayName +
               (amendedAct.RecordingActType.FemaleGenre ?
                                            " registrada en<br/>" : " registrado en<br/>") +
               amendedAct.PhysicalRecording.AsText + "<br />" +
               GetRecordingDates(amendedAct.Document);
      } else {
        return amendedAct.RecordingActType.DisplayName +
               (amendedAct.RecordingActType.FemaleGenre ?
                                            " registrada en<br/>" : " registrado en<br/>") +
               "Doc: " + amendedAct.Document.UID + "<br />" +
               GetRecordingDates(amendedAct.Document);
      }
    }

    static private string GetRowTemplate(RecordingAct recordingAct) {
      const string template = "<tr class='{{CLASS}}'>" +
                              "<td><b id='ancRecordingActIndex_{{ID}}'>{{INDEX}}</b></td>" +
                              "<td style='white-space:normal'>{{RECORDING.ACT.URL}}</td>" +
                              "<td style='white-space:nowrap'>{{RESOURCE.URL}}</td>" +
                              "<td style='white-space:normal'>{{ANTECEDENT}}</td>" +
                              "<td>{{OPTIONS.COMBO}}</td></tr>";

      int index = recordingAct.Index + 1;

      string html = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");
      html = html.Replace("{{INDEX}}", index.ToString("00"));

      return html;
    }

    static private string GetOptionsCombo(TractItem tractItem) {
      const string template =
        "<select id='cboRecordingOptions_{{TRACT-ITEM.ID}}' class='selectBox' style='width:130px'>" +
        "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
        "<option value='modifyRecordingActType'>Modificar este acto</option>" +
        "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
        "<option value='viewResourceTract'>Ver la historia</option>" +
        "</select><img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
        "alt='' title='Ejecuta la operación seleccionada' onclick='" +
        "doOperation(getElement(\"cboRecordingOptions_{{TRACT-ITEM.ID}}\").value, {{ID}}, {{RESOURCE.ID}});'/>";

      string html = template.Replace("{{ID}}", tractItem.RecordingAct.Id.ToString());
      html = html.Replace("{{RESOURCE.ID}}", tractItem.Resource.Id.ToString());
      html = html.Replace("{{TRACT-ITEM.ID}}", tractItem.Id.ToString());

      return html;
    }

    private static string GetDateAsText(DateTime date) {
      if (date == ExecutionServer.DateMinValue || date == ExecutionServer.DateMaxValue) {
        return "N/D";
      } else {
        return date.ToString("dd/MMM/yyyy");
      }
    }

    #endregion Private auxiliar methods

  } // class LRSGridControls

} // namespace Empiria.Land.UI
