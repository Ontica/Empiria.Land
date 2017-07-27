/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : DocumentRecordingActsGrid                       Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : HTML grid that displays the list of recordings acts of a recording document.                   *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
      string html = "<div class='tblContainer'>" +
                       "<div class='tbHeader'>" +
                          this.GetHeaderTable() +
                       "</div> " +
                       "<div class='tbBody'>" +
                          this.GetBodyTable() +
                       "</div>" +
                    "</div>";
      return html;
    }

    private string GetBodyTable() {
      FixedList<RecordingAct> recordingActsList = _document.RecordingActs;

      string rows = String.Empty;
      for (int i = 0; i < recordingActsList.Count; i++) {
        var recordingAct = recordingActsList[i];

        rows += this.GetRow(recordingAct, i);
      }
      if (recordingActsList.Count == 0) {
        rows += this.NoRecordsFoundRow();
      }

      return HtmlFormatters.TableWrapper(this.GetColGroup() +
                                          "<tbody>" +
                                            rows +
                                          "</tbody>", "details");
    }

    private string GetHeaderTable() {
      return HtmlFormatters.TableWrapper(this.GetColGroup() +
                                          "<tbody>" +
                                            this.GetTitleRow() +
                                            this.GetHeaderRow() +
                                          "</tbody>");
    }

    private string GetTitleRow() {
      string template = @"<tr>
                            <td class='tbTitle' colspan='4'>{{DOCUMENT.AS.TEXT}}</td>
                          </tr>";

      if (_document.IsHistoricDocument) {
        return template.Replace("{{DOCUMENT.AS.TEXT}}",
                      "Actos jurídicos en " + this._document.TryGetHistoricRecording().AsText);
      } else {
        return template.Replace("{{DOCUMENT.AS.TEXT}}",
                      "Actos jurídicos del documento " + this._document.UID);
      }
    }

    private string GetHeaderRow() {
      string template = @"<tr>
                            <th class='tbHeader'>Acto jurídico</th>
                            <th class='tbHeader'>Folio real</th>
                            <th class='tbHeader'>&#160;</th>
                            <th class='tbHeader'>Registró</th>
                          </tr>";

      return template;
    }

    private string GetColGroup() {
      string template = @"<colgroup>
                            <col width='250'/>
		                        <col width='220'/>
                            <col width='20'/>
		                        <col />
	                        </colgroup>";
      return template;
    }

    private string GetRow(RecordingAct recordingAct, int index) {
      const string template =
          "<tr class='{{CLASS}}'>" +
             "<td>{{RECORDING.ACT}}</td>" +
             "<td>" +
                "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{RESOURCE.UID}}</a></td>" +
             "<td><a href='javascript:copyToClipboard(\"{{RESOURCE.UID}}\");'>" +
                 "<img src='../themes/default/bullets/copy.gif' title='Copiar el folio real'></img></a></td>" +
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
          "<td colspan='4'>Este documento no tiene actos jurídicos</td>" +
        "</tr>";

      return template;
    }

    #endregion Private methods

  } // class DocumentRecordingActsGrid

} // namespace Empiria.Land.UI
