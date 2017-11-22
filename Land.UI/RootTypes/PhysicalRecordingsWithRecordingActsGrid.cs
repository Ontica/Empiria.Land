/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : PhysicalRecordingsWithRecordingActsGrid         Pattern  : Standard class                      *
*  Version   : 3.0                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : HTML grid that displays a list of physical recordings with their recording acts.               *
*                                                                                                             *
********************************** Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>HTML grid that displays a list of physical recordings with their recording acts.</summary>
  public class PhysicalRecordingsWithRecordingActsGrid {

    #region Fields

    private RecordingBook _recordingBook = null;

    #endregion Fields

    #region Constructors and parsers

    private PhysicalRecordingsWithRecordingActsGrid(RecordingBook recordingBook) {
      _recordingBook = recordingBook;
    }

    static public string Parse(RecordingBook recordingBook) {
      var grid = new PhysicalRecordingsWithRecordingActsGrid(recordingBook);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetHtml() {
      FixedList<Recording> physicalRecordingsList = _recordingBook.GetRecordings();

      string html = this.GetTitle() + this.GetHeader();
      for (int i = 0; i < physicalRecordingsList.Count; i++) {
        var physicalRecording = physicalRecordingsList[i];

        html += this.GetRow(physicalRecording, i);
      }
      return HtmlFormatters.TableWrapper(html);
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='5'>Partidas registradas en el libro {{RECORDING.BOOK.AS.TEXT}}</td>" +
              "<td><a href='javascript:doOperation(\"showRecordingBookAnalyzer\", {{RECORDING.BOOK.ID}});'>Captura histórica</a></td>" +
            "</tr>";

      template = template.Replace("{{RECORDING.BOOK.ID}}", this._recordingBook.Id.ToString());

      return template.Replace("{{RECORDING.BOOK.AS.TEXT}}", this._recordingBook.AsText);
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Partida</td>" +
              "<td style='width:260px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Folio real</td>" +
              "<td style='white-space:nowrap'>&nbsp;</td>" +
              "<td style='width:200px'>Present/Registro</td>" +
              "<td style ='width:160px'>Registró</td>" +
            "</tr>";
      return template;
    }

    private string GetRow(Recording physicalRecording, int index) {
      const string template =
           "<tr class='{{CLASS}}'>" +
             "<td style='white-space:nowrap'>" +
                "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{RECORDING.NUMBER}}</a></td>" +
             "<td style='white-space:normal;width:260px;'>{{RECORDING.ACT}}</td>" +
             "<td style='white-space:nowrap;'>" +
                "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{RESOURCE.UID}}</a></td>" +
             "<td><a href='javascript:copyToClipboard(\"{{RESOURCE.UID}}\");'>" +
                 "<img src='../themes/default/bullets/copy.gif' title='Copiar el folio real'></a></td>" +
             "<td style='white-space:nowrap;'>{{PRESENTATION.DATE}}<br/>{{AUTHORIZATION.DATE}}</td>" +
             "<td>{{REGISTERED.BY}}</td>" +
           "</tr>";

      FixedList<RecordingAct> recordingActs = physicalRecording.RecordingActs;
      if (recordingActs.Count == 0) {
        return String.Empty;
      }

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");
      row = row.Replace("{{RECORDING.NUMBER}}", physicalRecording.Number);
      row = row.Replace("{{DOCUMENT.ID}}", physicalRecording.MainDocument.Id.ToString());
      row = HtmlFormatters.SetPresentationAndAuthorizationDates(row, physicalRecording.MainDocument);

      for (int i = 0; i < recordingActs.Count; i++) {
        var recordingAct = recordingActs[i];
        if (i == 0) {   // For the first recording act just parse the main recording row
          row = FillRecordingActsFields(row, recordingAct);
        } else {        // For each subsequent recording act add a new secondary row
          var secondaryRow = GetSecondaryRow(recordingAct, index);
          row += secondaryRow;
        }
      }
      return row;
    }

    private string GetSecondaryRow(RecordingAct recordingAct, int index) {
      const string template =
          "<tr class='{{CLASS}}'>" +
             "<td>&nbsp;</td>" +
             "<td style='white-space:normal'>{{RECORDING.ACT}}</td>" +
             "<td style='white-space:nowrap;'>" +
                "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{RESOURCE.UID}}</a></td>" +
             "<td>&nbsp;</td>" +
             "<td>{{REGISTERED.BY}}</td>" +
           "</tr>";

      var secondaryRow = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");
      secondaryRow = FillRecordingActsFields(secondaryRow, recordingAct);

      return secondaryRow;
    }

    #endregion Private methods

    #region Auxiliar methods

    private string FillRecordingActsFields(string row, RecordingAct recordingAct) {
      row = row.Replace("{{RESOURCE.ID}}", recordingAct.Resource.Id.ToString());
      row = row.Replace("{{RESOURCE.UID}}", recordingAct.Resource.UID);
      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());
      row = row.Replace("{{RECORDING.ACT}}", recordingAct.DisplayName);
      row = row.Replace("{{REGISTERED.BY}}", recordingAct.RegisteredBy.Nickname);

      return row;
    }

    #endregion Auxiliar methods

  } // class PhysicalRecordingsWithRecordingActsGrid

} // namespace Empiria.Land.UI
