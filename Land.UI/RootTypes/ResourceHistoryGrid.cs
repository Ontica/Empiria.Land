/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : ResourceHistoryGrid                             Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content that display the resource's history.                             *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content that display the resource's history.</summary>
  public class ResourceHistoryGrid {

    #region Fields

    private Resource _resource = null;

    #endregion Fields

    #region Constructors and parsers

    private ResourceHistoryGrid(Resource resource) {
      _resource = resource;
    }

    static public string Parse(Resource resource) {
      var grid = new ResourceHistoryGrid(resource);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetHtml() {
      FixedList<RecordingAct> resourceHistory = _resource.GetFullRecordingActsTract();

      string html = this.GetTitle() + this.GetHeader();
      for (int i = resourceHistory.Count - 1; 0 <= i; i--) {
        var recordingAct = resourceHistory[i];

        html += this.GetRow(recordingAct, i);
      }
      return html;
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='5'>Historia del predio {{RESOURCE.UID}}</td>" +
            "</tr>";

      return template.Replace("{{RESOURCE.UID}}", this._resource.UID);
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Present/Registro</td>" +
              "<td style='width:160px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Antecedente / Fracción</td>" +
              "<td style='width:200px'>Registrado en</td>" +
              "<td style ='width:160px'>Registró</ td >" +
            "</tr>";
      return template;
    }

    private string GetRow(RecordingAct recordingAct, int index) {
      const string template =
           "<tr class='{{CLASS}}'>" +
             "<td>{{PRESENTATION.DATE}}<br/>{{AUTHORIZATION.DATE}}</td>" +
             "<td style='white-space:normal'>{{RECORDING.ACT}}</td>" +
             "<td style='white-space:normal;'>{{PARTITION}}</td>" +
             "<td style='white-space:{{WHITE-SPACE}};'>" +
               "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{DOCUMENT.OR.RECORDING}}</a>" +
               "<br>{{TRANSACTION}}</td>" +
             "<td>{{RECORDED.BY}}</td>" +
           "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      row = row.Replace("{{RECORDING.ACT}}", recordingAct.DisplayName);
      row = row.Replace("{{PARTITION}}", this.GetPartitionOrAntecedentCell(recordingAct));
      if (!recordingAct.PhysicalRecording.IsEmptyInstance) {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", recordingAct.PhysicalRecording.AsText);
        row = row.Replace("{{TRANSACTION}}", this.OnSelectDocumentButton(recordingAct));
        row = row.Replace("{{WHITE-SPACE}}", "normal");
      } else {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", recordingAct.Document.UID);
        row = row.Replace("{{TRANSACTION}}", "Trámite:" + recordingAct.Document.GetTransaction().UID);
        row = row.Replace("{{WHITE-SPACE}}", "nowrap");
      }

      row = row.Replace("{{PRESENTATION.DATE}}", GetDateAsText(recordingAct.Document.PresentationTime));
      row = row.Replace("{{AUTHORIZATION.DATE}}", GetDateAsText(recordingAct.Document.AuthorizationTime));
      row = row.Replace("{{RECORDED.BY}}", recordingAct.RegisteredBy.Nickname);

      row = row.Replace("{{DOCUMENT.ID}}", recordingAct.Document.Id.ToString());
      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());

      return row;
    }

    private string GetPartitionOrAntecedentCell(RecordingAct recordingAct) {
      if (Resource.IsCreationalRole(recordingAct.ResourceRole)) {

        var realEstate = (RealEstate) recordingAct.Resource;

        if (recordingAct.ResourceRole == ResourceRole.Created) {
          return "Sin antecedente registral";
        } else if (realEstate.Equals(this._resource)) {
          return "Creado como <b>" + realEstate.PartitionNo +
                 "</b> del predio " + NoWrap(recordingAct.RelatedResource.UID);
        } else {
          return "Sobre <b>" + realEstate.PartitionNo + "</b> con folio real " + NoWrap(realEstate.UID);
        }
      }
      return "&nbsp;";
    }

    private string OnSelectDocumentButton(RecordingAct recordingAct) {

      return String.Empty;

      //const string template =
      //    "<a href='javascript:doOperation(\"onSelectRecordingAct\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
      //        "Editar este acto</a>";

      //string x = template.Replace("{{DOCUMENT.ID}}", recordingAct.Document.Id.ToString());
      //x = x.Replace("{{RECORDING.ACT.ID}}}", recordingAct.Id.ToString());

      //return x;
    }

    #endregion Private methods

    #region Auxiliar methods

    private string GetDateAsText(DateTime date) {
      if (date == ExecutionServer.DateMinValue || date == ExecutionServer.DateMaxValue) {
        return "No consta";
      } else {
        return date.ToString(@"dd/MMM/yyyy");
      }
    }

    private string NoWrap(string text) {
      return "<span style='white-space:nowrap;'>" + text + "</span>";
    }

    #endregion Auxiliar methods

  } // class ResourceHistoryGrid

} // namespace Empiria.Land.UI
