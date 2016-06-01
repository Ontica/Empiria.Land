/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : PartyRecordingActsGrid                          Pattern  : Standard class                      *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates a grid HTML content with the recording acts associated with a human                  *
*              or organization party.                                                                         *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  /// <summary>Generates a grid HTML content with the recording acts associated with a human
  /// or organization party.</summary>
  public class PartyRecordingActsGrid {

    #region Fields

    private Party _party = null;

    #endregion Fields

    #region Constructors and parsers

    private PartyRecordingActsGrid(Party party) {
      _party = party;
    }

    static public string Parse(Party party) {
      var grid = new PartyRecordingActsGrid(party);

      return grid.GetHtml();
    }

    #endregion Constructors and parsers

    #region Private methods

    private string GetHtml() {
      FixedList<RecordingActParty> recordingActs = _party.GetRecordingActs();

      string html = this.GetTitle() + this.GetHeader();
      for (int i = recordingActs.Count - 1; 0 <= i; i--) {
        RecordingActParty item = recordingActs[i];

        html += this.GetRecordingActPartyRow(item, i);
      }
      return HtmlFormatters.TableWrapper(html);
    }

    private string GetTitle() {
      string template =
            "<tr class='detailsTitle'>" +
              "<td colspan='5' style='white-space:normal'>Historial de movimientos <b>{{PARTY}}</b></td>" +
            "</tr>";

      return template.Replace("{{PARTY}}", this._party.ExtendedName);
    }

    private string GetHeader() {
      string template =
            "<tr class='detailsHeader'>" +
              "<td>Present/Registro</td>" +
              "<td style='width:160px'>Acto jurídico</td>" +
              "<td style='white-space:nowrap'>Rol</td>" +
              "<td style='width:200px'>Registrado en</td>" +
              "<td style ='width:160px'>Registró</ td >" +
            "</tr>";
      return template;
    }

    private string GetRecordingActPartyRow(RecordingActParty recordingActParty, int index) {
      const string template =
           "<tr class='{{CLASS}}'>" +
             "<td>{{PRESENTATION.DATE}}<br/>{{AUTHORIZATION.DATE}}</td>" +
             "<td style='white-space:normal;width:260px'>{{RECORDING.ACT}}</td>" +
             "<td style='white-space:normal;'>{{ROLE}}<br/>" +
                 "<a href='javascript:doOperation(\"displayResourcePopupWindow\", {{RESOURCE.ID}}, {{RECORDING.ACT.ID}});'>" +
                 "{{RESOURCE.UID}}</a></td>" +
             "<td style='white-space:{{WHITE-SPACE}};'>" +
               "<a href='javascript:doOperation(\"onSelectDocument\", {{DOCUMENT.ID}}, {{RECORDING.ACT.ID}});'>" +
                   "{{DOCUMENT.OR.RECORDING}}</a>" +
               "<br>{{TRANSACTION}}</td>" +
             "<td>{{RECORDED.BY}}</td>" +
           "</tr>";

      string row = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");

      var document = recordingActParty.RecordingAct.Document;
      var recordingAct = recordingActParty.RecordingAct;
      var isMainParty = recordingActParty.Party.Equals(this._party);

      row = HtmlFormatters.SetPresentationAndAuthorizationDates(row, document);
      row = row.Replace("{{RECORDING.ACT}}", recordingAct.DisplayName);

      if (isMainParty) {
        row = row.Replace("{{ROLE}}", recordingActParty.PartyRole.Name);
      } else {
        row = row.Replace("{{ROLE}}", ((SecondaryPartyRole) recordingActParty.PartyRole).InverseRoleName);
      }
      if (!recordingAct.PhysicalRecording.IsEmptyInstance) {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", recordingAct.PhysicalRecording.AsText);
        row = row.Replace("{{TRANSACTION}}", String.Empty);
        row = row.Replace("{{WHITE-SPACE}}", "normal");
      } else {
        row = row.Replace("{{DOCUMENT.OR.RECORDING}}", document.UID);
        row = row.Replace("{{TRANSACTION}}", "Trámite:" + document.GetTransaction().UID);
        row = row.Replace("{{WHITE-SPACE}}", "nowrap");
      }

      row = row.Replace("{{RECORDING.ACT.ID}}", recordingAct.Id.ToString());
      row = row.Replace("{{RESOURCE.ID}}", recordingAct.Resource.Id.ToString());
      row = row.Replace("{{RESOURCE.UID}}", HtmlFormatters.NoWrap(recordingAct.Resource.UID));
      row = row.Replace("{{RECORDED.BY}}", recordingAct.RegisteredBy.Nickname);
      row = row.Replace("{{DOCUMENT.ID}}", document.Id.ToString());

      return row;
    }

    #endregion Private methods

  } // class PartyRecordingActsGrid

} // namespace Empiria.Land.UI
