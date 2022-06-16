/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : DocumentRecordingActsGrid                       Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : HTML grid that displays the list of recordings acts of a recording document.                   *
*                                                                                                             *
************************** Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>HTML grid that displays the list of recordings acts of a recording document.</summary>
  public class DocumentRecordingActsGrid {

    #region Fields

    private RecordingDocument _document = null;

    #endregion Fields

    #region Constructors and parsers

    private DocumentRecordingActsGrid(RecordingDocument document) {
      _document = document;
    }

    static public string Parse(RecordingDocument document) {
      var grid = new DocumentRecordingActsGrid(document);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetHtml() {
      FixedList<RecordingAct> recordingActsList = _document.RecordingActs;

      string html = this.GetTitle() + this.GetHeader();
      for (int i = 0; i < recordingActsList.Count; i++) {
        var recordingAct = recordingActsList[i];

        html += this.GetRow(recordingAct, i);
      }
      if (recordingActsList.Count == 0) {
        html += this.NoRecordsFoundRow();
      }
      return HtmlFormatters.TableWrapper(html);
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='4'>{{DOCUMENT.AS.TEXT}}</td>" +
            "</tr>";

      if (_document.IsHistoricDocument) {
        return template.Replace("{{DOCUMENT.AS.TEXT}}",
                      "Actos jurídicos en " + this._document.TryGetHistoricRecording().AsText);
      } else {
        return template.Replace("{{DOCUMENT.AS.TEXT}}",
                      "Actos jurídicos del documento " + this._document.UID);
      }
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td style='width:260px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Folio real</td>" +
              "<td style='white-space:nowrap'>&nbsp;</td>" +
              "<td style ='width:160px'>Registró</td>" +
            "</tr>";
      return template;
    }

    private string GetRow(RecordingAct recordingAct, int index) {
      const string template =
          "<tr class='{{CLASS}}'>" +
             "<td style='white-space:normal;width:260px'>{{RECORDING.ACT}}</td>" +
             "<td style='white-space:nowrap;'>" +
                "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{RESOURCE.UID}}</a></td>" +
             "<td><a href='javascript:copyToClipboard(\"{{RESOURCE.UID}}\");'>" +
                 "<img src='../themes/default/bullets/copy.gif' title='Copiar el folio electrónico'></a></td>" +
             "<td>{{REGISTERED.BY}}</td>" +
           "</tr>";

      var row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      row = row.Replace("{{RESOURCE.ID}}", recordingAct.Resource.Id.ToString());
      row = row.Replace("{{RESOURCE.UID}}", recordingAct.Resource.UID);
      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());
      row = row.Replace("{{RECORDING.ACT}}", recordingAct.DisplayName);
      row = row.Replace("{{REGISTERED.BY}}", recordingAct.RegisteredBy.Nickname);
      return row;
    }

    private string NoRecordsFoundRow() {
      const string template =
        "<tr class='detailsItem'>" +
          "<td colspan='3'>Este documento no tiene actos jurídicos</td>" +
        "<tr>";

      return template;
    }

    #endregion Private methods

  } // class DocumentRecordingActsGrid

} // namespace Empiria.Land.UI
