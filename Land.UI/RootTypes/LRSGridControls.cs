﻿/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : LRSGridControls                                 Pattern  : Static Class                        *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections;
using System.Web.UI.WebControls;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.UI {

  /// <summary>Static class that generates predefined grid content for Land Registration System data.</summary>
  static public class LRSGridControls {

    #region Public methods

    static public PagedDataSource GetPagedDataSource(IEnumerable view, int pageSize, int pageIndex) {
      PagedDataSource pagedDataSource = new PagedDataSource();
      pagedDataSource.DataSource = view;

      pagedDataSource.AllowPaging = true;
      pagedDataSource.PageSize = pageSize;
      pagedDataSource.CurrentPageIndex = pageIndex;

      return pagedDataSource;
    }

    static public string GetRecordingActPartiesGrid(RecordingAct recordingAct, bool readOnly) {
      FixedList<RecordingActParty> allInvolvedParties = PartyData.GetRecordingPartyList(recordingAct);

      var primaryParties = allInvolvedParties.FindAll((x) => x.PartyOf.IsEmptyInstance);

      string html = GetPrimaryPartiesGridItems(recordingAct, primaryParties, readOnly);

      var secondaryAdditionalParties =
                allInvolvedParties.FindAll((x) => !x.PartyOf.IsEmptyInstance &&
                                                  !primaryParties.Exists((y) => y.Party.Equals(x.PartyOf)));

      html += GetSecondaryPartiesGridItems(recordingAct, secondaryAdditionalParties, readOnly);

      return html;
    }

    static private string GetPrimaryPartiesGridItems(RecordingAct recordingAct,
                                                     FixedList<RecordingActParty> primaryParties,
                                                     bool readOnly) {
      string html = String.Empty;
      string temp = String.Empty;
      for (int i = 0; i < primaryParties.Count; i++) {
        RecordingActParty primaryPartyItem = primaryParties[i];
        if (i % 2 == 0) {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsItem");
        } else {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsOddItem");
        }
        temp = temp.Replace("{ID}", primaryPartyItem.Id.ToString());
        temp = temp.Replace("{PARTY.ID}", primaryPartyItem.Party.Id.ToString());
        temp = temp.Replace("{NAME}", primaryPartyItem.Party.FullName);
        temp = temp.Replace("{UNIQUE.ID}", primaryPartyItem.Party.FullUID);
        temp = temp.Replace("{ROLE}", primaryPartyItem.PartyRole.Name);
        temp = temp.Replace("{DOMAIN.PART}", primaryPartyItem.GetOwnershipPartAsText());

        FixedList<RecordingActParty> relatedParties = PartyData.GetSecondaryPartiesList(recordingAct);
        relatedParties = relatedParties.FindAll((x) => x.PartyOf.Equals(primaryPartyItem.Party));


        temp = temp.Replace("{SECONDARY.TABLE}", GetRecordingActRelatedPartiesTable(recordingAct,
                                                                                    primaryPartyItem,
                                                                                    relatedParties));
        html += temp;
      }
      return html;
    }

    static private string GetSecondaryPartiesGridItems(RecordingAct recordingAct,
                                                       FixedList<RecordingActParty> secondaryParties,
                                                       bool readOnly) {
      string html = String.Empty;
      string temp = String.Empty;

      string processed = String.Empty;
      for (int i = 0; i < secondaryParties.Count; i++) {
        RecordingActParty recordingActParty = secondaryParties[i];
        if (processed.Contains("|" + recordingActParty.Party.Id.ToString())) {
          continue;
        } else {
          processed += "|" + recordingActParty.Party.Id.ToString();
        }
        if (i % 2 == 0) {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsItem");
        } else {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsOddItem");
        }
        temp = temp.Replace("{ID}", recordingActParty.Id.ToString());
        temp = temp.Replace("{PARTY.ID}", recordingActParty.Party.Id.ToString());
        temp = temp.Replace("{NAME}", recordingActParty.Party.FullName);
        temp = temp.Replace("{UNIQUE.ID}", recordingActParty.Party.FullUID);
        temp = temp.Replace("{ROLE}", String.Empty);
        temp = temp.Replace("{DOMAIN.PART}", String.Empty);

        FixedList<RecordingActParty> relatedParties = PartyData.GetSecondaryPartiesList(recordingAct);
        relatedParties = relatedParties.FindAll((x) => x.Party.Equals(recordingActParty.Party));

        temp = temp.Replace("{SECONDARY.TABLE}", GetRecordingActRelatedPartiesTable(recordingAct,
                                                                                    recordingActParty,
                                                                                    relatedParties, true));
        html += temp;
      }
      return html;
    }

    static private string GetRecordingActRelatedPartiesTable(RecordingAct recordingAct,
                                                             RecordingActParty baseRecordingActParty,
                                                             FixedList<RecordingActParty> secondaryParties,
                                                             bool displayPartyOf = false) {
      string html = String.Empty;

      foreach (var recordingActPartyItem in secondaryParties) {
        html += GetRelatedPartyOnRoleRow(recordingActPartyItem, displayPartyOf);
      }
      return html;
    }

    static private string GetRelatedPartyOnRoleRow(RecordingActParty recordingActParty, bool displayPartyOf) {
      const string row = "<table class='ghostTable' style='margin:8px;'>" +
                         "<tr><td>{ROLE}:&nbsp;&nbsp;</td><td style='white-space:normal'>" +
                         "<a href='javascript:doOperation(\"selectParty\", {PARTY.ID})'><i>{NAME}</i></a>" +
                         " <a href='javascript:doOperation(\"deleteParty\", {ID})'>(supr)</a>" +
                         "</td></tr>" +
                         "</table>";

      string html = row.Replace("{ID}", recordingActParty.Id.ToString());

      if (displayPartyOf) {
        html = html.Replace("{PARTY.ID}", recordingActParty.PartyOf.Id.ToString());
        html = html.Replace("{NAME}", recordingActParty.PartyOf.FullName);
        html = html.Replace("{ROLE}", recordingActParty.PartyRole.Name + " de");
      } else {
        html = html.Replace("{PARTY.ID}", recordingActParty.Party.Id.ToString());
        html = html.Replace("{NAME}", recordingActParty.Party.FullName);
        html = html.Replace("{ROLE}", recordingActParty.PartyRole.Name);
      }
      return html;
    }

    static public string GetRecordingsSummaryTable(RecordingBook recordingBook, int pageSize, int pageIndex) {
      FixedList<Recording> recordings = RecordingBooksData.GetRecordings(recordingBook);

      return GetRecordingsSummaryTable(recordings, pageSize, pageIndex);
    }

    static public string GetRecordingsSummaryTable(FixedList<Recording> recordings, int pageSize, int pageIndex) {
      PagedDataSource pageView = GetPagedDataSource(recordings, pageSize, pageIndex);

      const string header = @"<table id='tblRecordingsViewer' class='details' style='width:658px'>" +
                              "<tr class='detailsHeader'><td>Partida</td><td>Imágenes</td>" +
                              "<td>Presentación</td><td>Autorización</td><td>Observaciones</td>" +
                              "<td>Estado</td><td width='200px'>Registró</td></tr>";

      const string row = @"<tr class='{CLASS}'><td><a href=""javascript:doOperation('moveToRecording', {RECORDING.ID})"">{RECORDING.NUMBER}</a></td>" +
                           "<td>{RECORDING.IMAGES}</td><td>{RECORDING.PRESENTATION.TIME}</td><td>{RECORDING.AUTHORIZATION.TIME}</td>" +
                           "<td style='white-space:normal;'>{RECORDING.NOTES}</td><td>{RECORDING.STATUS}</td><td style='white-space:normal;width:200px'>{CAPTURED.BY}</td></tr>";
      string html = String.Empty;
      string temp = String.Empty;
      for (int i = pageView.FirstIndexInPage; i < Math.Min(pageView.FirstIndexInPage + pageSize, recordings.Count); i++) {
        if (i % 2 == 0) {
          temp = row.Replace("{CLASS}", "detailsOddItem");
        } else {
          temp = row.Replace("{CLASS}", "detailsItem");
        }
        temp = temp.Replace("{RECORDING.NUMBER}", recordings[i].Number);

        if (recordings[i].GetAnnotationActs().Count != 0) {     // No guarantee that the recording IS AN annotation. Annotation isn't a recording property.
          temp = temp.Replace("{RECORDING.IMAGES}", "*¿¿ Puede ser anotación ??*");
        } else if (recordings[i].StartImageIndex <= 0 && recordings[i].EndImageIndex <= 0) {
          temp = temp.Replace("{RECORDING.IMAGES}", "Sin imagen");
        } else {
          temp = temp.Replace("{RECORDING.IMAGES}", "De la " + recordings[i].StartImageIndex.ToString() +
                              " a la " + recordings[i].EndImageIndex);
        }
        if (recordings[i].Document.PresentationTime == ExecutionServer.DateMinValue) {
          temp = temp.Replace("{RECORDING.PRESENTATION.TIME}", "No consta");
        } else {
          temp = temp.Replace("{RECORDING.PRESENTATION.TIME}",
                                recordings[i].Document.PresentationTime.ToString("dd/MMM/yyyy HH:mm"));
        }
        if (recordings[i].AuthorizationTime == Empiria.ExecutionServer.DateMaxValue) {
          temp = temp.Replace("{RECORDING.AUTHORIZATION.TIME}", "No consta");
        } else {
          temp = temp.Replace("{RECORDING.AUTHORIZATION.TIME}",
                                recordings[i].AuthorizationTime.ToString("dd/MMM/yyyy"));
        }
        if (recordings[i].Notes.Length == 0) {
          temp = temp.Replace("{RECORDING.NOTES}", "Ninguna");
        } else {
          temp = temp.Replace("{RECORDING.NOTES}", recordings[i].Notes);
        }
        temp = temp.Replace("{RECORDING.STATUS}", recordings[i].StatusName);
        temp = temp.Replace("{CAPTURED.BY}", recordings[i].RecordedBy.Alias);

        temp = temp.Replace("{RECORDING.ID}", recordings[i].Id.ToString());
        html += temp;
      }
      return header + html + "</table>";
    }

    static private string GetRecordingActPartyRowTemplate(bool readOnly) {
      const string recordingActPartyRow =
            "<tr class='{CLASS}'><td style='white-space:normal'>" +
              "<a href='javascript:doOperation(\"{ON.CLICK.EVENT}\", {PARTY.ID})'>" +
              "<b id='ancRecordingAct_{ID}'>{NAME}</b></a><br />{SECONDARY.TABLE}</td>" +
            "<td style='white-space:normal'>{UNIQUE.ID}</td>" +
            "<td style='white-space:normal'>{ROLE}</td>" +
            "<td style='white-space:normal'>{DOMAIN.PART}</td>";
      const string deleteCell =
            "<td><img class='comboExecuteImage' src='../themes/default/buttons/trash.gif' alt='' title='Elimina a la persona del acto jurídico' " +
            "onclick='doOperation(\"deleteParty\", {ID});'/></td>";

      string html = String.Empty;
      if (readOnly) {
        return recordingActPartyRow.Replace("{ON.CLICK.EVENT}", "viewParty") + "</tr>";
      } else {
        return recordingActPartyRow.Replace("{ON.CLICK.EVENT}", "selectParty") + deleteCell + "</tr>";
      }
    }

    #endregion Public methods

  } // class LRSGridControls

} // namespace Empiria.Land.UI
