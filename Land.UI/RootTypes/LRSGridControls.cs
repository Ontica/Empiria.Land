/* Empiria Land 2014 ******************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land                        *
*  Type      : LRSGridControls                                 Pattern  : Presentation Services Static Class  *
*  Version   : 1.5        Date: 28/Mar/2014                    License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
********************************** Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Web.UI.WebControls;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.UI {

  /// <summary>Static class that generates predefined grid content for Land Registration System data.</summary>
  static public class LRSGridControls {

    #region Public methods

    static public PagedDataSource GetPagedDataSource(System.Collections.IEnumerable view, int pageSize, int pageIndex) {
      PagedDataSource pagedDataSource = new PagedDataSource();
      pagedDataSource.DataSource = view;

      pagedDataSource.AllowPaging = true;
      pagedDataSource.PageSize = pageSize;
      pagedDataSource.CurrentPageIndex = pageIndex;

      return pagedDataSource;
    }

    static public string GetRecordingActPartiesGrid(RecordingAct recordingAct, bool readOnly) {
      if (!recordingAct.IsAnnotation) {
        ObjectList<RecordingActParty> primaryParties = RecordingActParty.GetDomainPartyList(recordingAct);

        return GetDomainActPartiesGrid(recordingAct, primaryParties, readOnly);
      } else {
        ObjectList<RecordingActParty> annotationParties = RecordingActParty.GetList(recordingAct);

        return GetAnnotationPartiesGrid(recordingAct, annotationParties, readOnly);
      }
    }

    static private string GetAnnotationPartiesGrid(RecordingAct recordingAct, ObjectList<RecordingActParty> annotationParties, bool readOnly) {
      string html = String.Empty;
      string temp = String.Empty;

      string processed = String.Empty;
      for (int i = 0; i < annotationParties.Count; i++) {
        RecordingActParty recordingActParty = annotationParties[i];
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
        temp = temp.Replace("{DATE.ID}", recordingActParty.Party.RegistryID);
        temp = temp.Replace("{ROLE}", recordingActParty.SecondaryPartyRole.InverseRoleName);
        temp = temp.Replace("{DOMAIN}", recordingActParty.DomainName);
        temp = temp.Replace("{DOMAIN.PART}", recordingActParty.DomainPartName);
        temp = temp.Replace("{SECONDARY.TABLE}", GetRecordingActSecondaryPartiesTable(recordingAct, recordingActParty));
        html += temp;
      }
      return html;
    }

    static private string GetDomainActPartiesGrid(RecordingAct recordingAct, ObjectList<RecordingActParty> domainParties, bool readOnly) {
      string html = String.Empty;
      string temp = String.Empty;
      for (int i = 0; i < domainParties.Count; i++) {
        RecordingActParty recordingActParty = domainParties[i];
        if (i % 2 == 0) {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsItem");
        } else {
          temp = GetRecordingActPartyRowTemplate(readOnly).Replace("{CLASS}", "detailsOddItem");
        }
        temp = temp.Replace("{ID}", recordingActParty.Id.ToString());
        temp = temp.Replace("{PARTY.ID}", recordingActParty.Party.Id.ToString());
        temp = temp.Replace("{NAME}", recordingActParty.Party.FullName);
        temp = temp.Replace("{DATE.ID}", recordingActParty.Party.RegistryID);
        temp = temp.Replace("{ROLE}", recordingActParty.PartyRole.Name);
        temp = temp.Replace("{DOMAIN}", recordingActParty.DomainName);
        temp = temp.Replace("{DOMAIN.PART}", recordingActParty.DomainPartName);
        temp = temp.Replace("{SECONDARY.TABLE}", GetRecordingActSecondaryPartiesTable(recordingAct, recordingActParty));
        html += temp;
      }
      return html;
    }

    static private string GetRecordingActSecondaryPartiesTable(RecordingAct recordingAct,
                                                               RecordingActParty baseRecordingActParty) {
      string html = String.Empty;

      if (baseRecordingActParty.PartyRole == DomainActPartyRole.Usufructuary) {
        html += GetSecondaryPartyRow(baseRecordingActParty);
      }

      ObjectList<RecordingActParty> secondaryParties = RecordingActParty.GetSecondaryPartiesList(recordingAct);
      for (int i = 0; i < secondaryParties.Count; i++) {
        if (!secondaryParties[i].Party.Equals(baseRecordingActParty.Party)) {
          continue;
        }
        html += GetSecondaryPartyRow(secondaryParties[i]);
      }
      return html;
    }

    static private string GetSecondaryPartyRow(RecordingActParty secondaryParty) {
      const string row = "<table class='ghostTable' style='margin:8px;'>" +
                         "<tr><td>{ROLE}:&nbsp;&nbsp;</td><td style='white-space:normal'>" +
                         "<a href='javascript:doOperation(\"selectParty\", {PARTY.ID})'><i>{NAME}</i></a>" +
                         " <a href='javascript:doOperation(\"deleteParty\", {ID})'>(supr)</a>" +
                         "</td></tr>" +
                         "</table>";

      string html = row.Replace("{ID}", secondaryParty.Id.ToString());

      html = html.Replace("{PARTY.ID}", secondaryParty.SecondaryParty.Id.ToString());
      html = html.Replace("{NAME}", secondaryParty.SecondaryParty.FullName);

      if (secondaryParty.PartyRole == DomainActPartyRole.Usufructuary) {
        return html.Replace("{ROLE}", "Nudo propietario");
      } else {
        return html.Replace("{ROLE}", secondaryParty.SecondaryPartyRole.Name);
      }
    }

    static public string GetRecordingAnnotationsGrid(Recording recording) {
      const string row = "<tr class='{CLASS}'>" +
                         "<td><a href='javascript:showRecordingImages({RECORDING.ID})' title='Muestra la imagen del libro asociado a esta anotación o limitación'><b>{NUMBERING}</b></a></td>" +
                         "<td><b id='ancAnnotationProperty_{PTY_ID}_{REC_ACT_ID}'>{PROPERTY.TRACT.NUMBER}</b></a></td>" +
                         "<td style='white-space:normal'>" +
                         "<a href='javascript:doOperation(\"editAnnotation\", {RECORDING.ID}, {REC_ACT_ID})' title='Permite editar la información específica de este acto jurídico'><b id='ancAnnotation_{PTY_ID}_{REC_ACT_ID}'>{RECORDING.ACT.TYPE}</b></a></td>" +
                         "<td><b id='ancAnnotationBook_{PTY_ID}_{REC_ACT_ID}'}>{IMAGING.FILES.FOLDER}</b> &nbsp; &nbsp;<a href='javascript:gotoRecordingBook({RECORDING.BOOK.ID}, {RECORDING.ID})'>Editar</a><br />Insc: {ANNOTATION.NUMBER}</td>" +
                         "<td><b>{ANNOTATION.PRESENTATION}</b></td>" +
                         "<td>{ANNOTATION.STATUS.NAME}</td>" +
                         "<td><img src='../themes/default/buttons/trash.gif' alt='' onclick='return doOperation(\"deleteAnnotation\", {PTY_ID}, {REC_ACT_ID})'></td></tr>";

      string html = String.Empty;
      string temp = String.Empty;
      ObjectList<PropertyEvent> annotations = recording.GetPropertiesAnnotationsList();
      for (int i = 0; i < annotations.Count; i++) {
        PropertyEvent association = annotations[i];
        if (i % 2 == 0) {
          temp = row.Replace("{CLASS}", "detailsItem");
        } else {
          temp = row.Replace("{CLASS}", "detailsOddItem");
        }

        temp = temp.Replace("{NUMBERING}", Char.ConvertFromUtf32(65 + i));
        temp = temp.Replace("{PROPERTY.TRACT.NUMBER}", association.Property.UniqueCode);
        temp = temp.Replace("{RECORDING.ACT.TYPE}", association.RecordingAct.RecordingActType.DisplayName);
        if (!association.RecordingAct.Recording.Equals(recording)) {
          temp = temp.Replace("{IMAGING.FILES.FOLDER}", association.RecordingAct.Recording.RecordingBook.ImagingFilesFolder.DisplayName);
          if (association.RecordingAct.Recording.StartImageIndex != 0) {
            temp = temp.Replace("{ANNOTATION.NUMBER}", "<span id='ancAnnotationNumber_{PTY_ID}_{REC_ACT_ID}'><b>" + association.RecordingAct.Recording.Number + "</b></span>" +
                                                       "&nbsp; Img: <b>" + association.RecordingAct.Recording.StartImageIndex.ToString() + " - " +
                                                       association.RecordingAct.Recording.EndImageIndex.ToString() + "</b>");
          } else {
            temp = temp.Replace("{ANNOTATION.NUMBER}", "<span id='ancAnnotationNumber_{PTY_ID}_{REC_ACT_ID}'><b>" + association.RecordingAct.Recording.Number + "</b></span>");
          }
        } else {
          temp = temp.Replace("{IMAGING.FILES.FOLDER}", "Este libro");
          temp = temp.Replace("{ANNOTATION.NUMBER}", "<span id='ancAnnotationNumber_{PTY_ID}_{REC_ACT_ID}'><b>Esta inscripción</b></span>");
        }
        if (association.RecordingAct.Recording.PresentationTime != ExecutionServer.DateMaxValue) {
          temp = temp.Replace("{ANNOTATION.PRESENTATION}", "<span id='ancAnnotationPresentation_{PTY_ID}_{REC_ACT_ID}'>" +
                                                           association.RecordingAct.Recording.PresentationTime.ToString("dd/MMM/yyyy HH:mm") + "</span><br />" +
                                                           "Aut: " + association.RecordingAct.Recording.AuthorizedTime.ToString("dd/MMM/yyyy"));
        } else {
          temp = temp.Replace("{ANNOTATION.PRESENTATION}", "<span id='ancAnnotationPresentation_{PTY_ID}_{REC_ACT_ID}'>No determinada</span>");
        }
        temp = temp.Replace("{RECORDING.ID}", association.RecordingAct.Recording.Id.ToString());
        temp = temp.Replace("{RECORDING.BOOK.ID}", association.RecordingAct.Recording.RecordingBook.Id.ToString());
        temp = temp.Replace("{ANNOTATION.STATUS.NAME}", association.RecordingAct.StatusName);
        temp = temp.Replace("{PTY_ID}", association.Property.Id.ToString());
        temp = temp.Replace("{REC_ACT_ID}", association.RecordingAct.Id.ToString());

        html += temp;
      } // for (i)
      return html;
    }

    static public string GetRecordingsSummaryTable(RecordingBook recordingBook, int pageSize, int pageIndex) {
      ObjectList<Recording> recordings = RecordingBooksData.GetRecordings(recordingBook);

      return GetRecordingsSummaryTable(recordings, pageSize, pageIndex);
    }


    static public string GetBatchCaptureRecordingActsGrid(Recording recording) {
      const string row = "<tr class='{CLASS}'><td><b id='ancRecordingActIndex_{ID}_{PROPERTY.ID}'>{RECORDING.ACT.INDEX}</b></td>" +
                         "<td style='white-space:normal'>{RECORDING.ACT.URL}</td>" +
                        "<td>{RECORDING.ACT.STATUS}</td>" +
                         "<td>{PROPERTY.URL}</td>" +
                         "<td>{PROPERTY.STATUS}</td>" +
                         "<td>{OPTIONS.COMBO}<img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
                         "alt='' title='Ejecuta la operación seleccionada' " +
                         "onclick='doOperation(getElement(\"cboRecordingOptions_{ID}_{PROPERTY.ID}\").value, {ID}, {PROPERTY.ID});'/>" +
                         "</td></tr>";

      const string editURL = "<a href='javascript:doOperation(\"editRecordingAct\", {ID})'>" +
                             "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b></a>";
      const string idemURL = "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b>";

      const string propertyURL = "<a href='javascript:doOperation(\"editProperty\", {PROPERTY.ID}, {ID})'>" +
                                 "<b id='ancRecordingActProperty_{ID}_{PROPERTY.ID}'>{PROPERTY.TRACT}</b></a>";

      const string optionsCombo = "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
                                  "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
                                  "{INCREMENT_INDEX}" +
                                  "{DECREMENT_INDEX}" +
                                  "<option value='selectRecordingActOperation'></option>" +
                                  "<option value='addPropertyToRecordingAct'>Agregar otro predio</option>" +
                                  "<option value='modifyRecordingActType'>Modificar este acto</option>" +
                                  "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
                                  "</select>";

      const string propertyOptionsCombo = "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
                                          "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
                                          "<option value='deleteRecordingActProperty'>Eliminar este predio</option>" +
                                          "</select>";
      string html = String.Empty;
      string temp = String.Empty;
      ObjectList<RecordingAct> recordingActs = recording.GetNoAnnotationActs();
      for (int i = 0; i < recordingActs.Count; i++) {
        RecordingAct recordingAct = recordingActs[i];
        for (int j = 0; j < recordingActs[i].PropertiesEvents.Count; j++) {
          PropertyEvent propertyEvent = recordingAct.PropertiesEvents[j];
          if (i % 2 == 0) {
            temp = row.Replace("{CLASS}", "detailsItem");
          } else {
            temp = row.Replace("{CLASS}", "detailsOddItem");
          }
          if (j == 0) {
            temp = temp.Replace("{RECORDING.ACT.INDEX}", recordingAct.Index.ToString("00"));
            if (recordingAct.RecordingActType.Autoregister) {
              temp = temp.Replace("{RECORDING.ACT.URL}", idemURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                         recordingAct.RecordingActType.DisplayName));
            } else {
              temp = temp.Replace("{RECORDING.ACT.URL}", editURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                         recordingAct.RecordingActType.DisplayName));
            }
            temp = temp.Replace("{RECORDING.ACT.DISPLAY.NAME}", recordingAct.RecordingActType.DisplayName);
          } else {
            temp = temp.Replace("{RECORDING.ACT.INDEX}", "<i>" + recordingAct.Index.ToString("00") + "</i>");
            temp = temp.Replace("{RECORDING.ACT.URL}", idemURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                       "<i>ídem</i>"));
          }
          temp = temp.Replace("{PROPERTY.URL}", propertyURL.Replace("{PROPERTY.TRACT}", propertyEvent.Property.UniqueCode));
          if (propertyEvent.Property.Status == PropertyStatus.Registered && propertyEvent.Status != PropertyEventStatus.Registered) {
            temp = temp.Replace("{PROPERTY.STATUS}", "Parcial");
          } else {
            temp = temp.Replace("{PROPERTY.STATUS}", propertyEvent.StatusName);
          }
          temp = temp.Replace("{PROPERTY.STATUS}", propertyEvent.Property.StatusName);
          temp = temp.Replace("{RECORDING.ACT.STATUS}", recordingAct.StatusName);
          if (j == 0) {
            temp = temp.Replace("{OPTIONS.COMBO}", optionsCombo);
            if (recordingActs.Count > 1) {
              if (i != 0) {
                temp = temp.Replace("{INCREMENT_INDEX}", "<option value='upwardRecordingAct'>Subir en la secuencia</option>");
              } else {
                temp = temp.Replace("{INCREMENT_INDEX}", String.Empty);
              }
              if (i != recordingActs.Count - 1) {
                temp = temp.Replace("{DECREMENT_INDEX}", "<option value='downwardRecordingAct'>Bajar en la secuencia</option>");
              } else {
                temp = temp.Replace("{DECREMENT_INDEX}", String.Empty);
              }
            } else { // recording.RecordingActs.Count <= 1
              temp = temp.Replace("{INCREMENT_INDEX}", String.Empty);
              temp = temp.Replace("{DECREMENT_INDEX}", String.Empty);
            }
          } else { // j != 0
            temp = temp.Replace("{OPTIONS.COMBO}", propertyOptionsCombo);
          }
          temp = temp.Replace("{ID}", recordingAct.Id.ToString());
          temp = temp.Replace("{PROPERTY.ID}", propertyEvent.Property.Id.ToString());
          html += temp;
        }  // for j
      } // for i
      return html;
    }

    static public string GetRecordingActsGrid(ObjectList<Recording> recordingList) {
      string html = String.Empty;
      int counter = 0;

      foreach (var recording in recordingList) {
        ObjectList<RecordingAct> recordingActs = recording.RecordingActs;
        int recordingActIndex = 0;
        foreach (var recordingAct in recordingActs) {
          var propertyEvents = recordingAct.PropertiesEvents;
          if (propertyEvents.Count != 0) {
            int propertyEventIndex = 0;
            foreach (var propertyEvent in propertyEvents) {
              if (recordingActIndex == 0 && propertyEventIndex == 0) {
                counter++;
              }
              html += GetRecordingActGridRow(recordingAct, propertyEvent, counter, 
                                             recordingActIndex, propertyEventIndex);
              propertyEventIndex++;
            }
          } else {
            counter++;
            html += GetRecordingActGridRow(recordingAct, PropertyEvent.Empty, counter, 
                                           recordingActIndex, 0);
          }
          recordingActIndex++;
        } // foreach RecordingAct
      } // foreach Recording
      return html;
    }

    private static string GetRecordingActGridRow(RecordingAct recordingAct, PropertyEvent propertyEvent, int counter, 
                                                 int recordingActIndex, int propertyEventIndex) {
      const string row = "<tr class='{CLASS}'>" +
                           "<td><b id='ancRecordingActIndex_{ID}_{PROPERTY.ID}'>{RECORDING.ACT.INDEX}</b><br/>" +
                           "<img src='../themes/default/buttons/document.sm.gif' alt=''" +
                           " title='Imprime el sello específico para esta partida'" +
                           " onclick='return doOperation(\"viewRecordingSeal\", {RECORDING.ID})' /></td>" +
                           "<td style='white-space:normal'>{RECORDING.BOOK}</td>" +
                           "<td>{RECORDING.NUMBER}</td>" +
                           "<td style='white-space:normal'>{RECORDING.ACT.URL}</td>" +
                           "<td>{RECORDING.ACT.STATUS}</td>" +
                           "<td style='white-space:normal'>{PROPERTY.URL}<br/>{ANTECEDENT.TAG} {ANTECEDENT.STATUS}</td>" +
                           "<td>{PROPERTY.STATUS}</td>" +
                           "<td>{OPTIONS.COMBO}<img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
                           "alt='' title='Ejecuta la operación seleccionada' " +
                           "onclick='doOperation(getElement(\"cboRecordingOptions_{ID}_{PROPERTY.ID}\").value, {ID}, {PROPERTY.ID});'/>" +
                           "</td></tr>";
      const string editURL = "<a href='javascript:doOperation(\"editRecordingAct\", {RECORDING.BOOK.ID}, {RECORDING.ID})'>" +
                             "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b></a>";
      const string idemURL = "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b>";

      const string propertyURL = "<a href='javascript:doOperation(\"editProperty\", {RECORDING.BOOK.ID}, {RECORDING.ID}, {PROPERTY.ID})'>" +
                                 "<b id='ancRecordingActProperty_{ID}_{PROPERTY.ID}'>{PROPERTY.TRACT}</b></a>";

      const string optionsCombo = "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
                                  "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
                                  "{INCREMENT_INDEX}" +
                                  "{DECREMENT_INDEX}" +
                                  "<option value='selectRecordingActOperation'></option>" +
                                  "<option value='addPropertyToRecordingAct'>Agregar otro predio</option>" +
                                  "<option value='addAnotherRecordingActToRecording'>Agregar otro acto</option>" +
                                  "<option value='modifyRecordingActType'>Modificar este acto</option>" +
                                  "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
                                  "</select>";
      const string propertyOptionsCombo = "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
                                          "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
                                          "<option value='deleteRecordingActProperty'>Eliminar este predio</option>" +
                                          "<option value='viewPropertyTract'>Ver la historia de este predio</option>" +
                                          "</select>";
      string temp = row.Replace("{CLASS}", (counter % 2 == 0) ? "detailsItem" : "detailsOddItem");
      if (propertyEventIndex == 0) {
        temp = temp.Replace("{RECORDING.ACT.INDEX}", counter.ToString("00"));
        if (recordingAct.RecordingActType.Autoregister) {
          temp = temp.Replace("{RECORDING.ACT.URL}", idemURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                      recordingAct.RecordingActType.DisplayName));
        } else {
          temp = temp.Replace("{RECORDING.ACT.URL}", editURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                      recordingAct.RecordingActType.DisplayName));
        }
        temp = temp.Replace("{RECORDING.ACT.DISPLAY.NAME}", recordingAct.RecordingActType.DisplayName);
        temp = temp.Replace("{RECORDING.ACT.STATUS}", recordingAct.StatusName);
      } else {
        temp = temp.Replace("{RECORDING.ACT.INDEX}", "<i>" + counter.ToString("00") + "</i>");
        temp = temp.Replace("{RECORDING.ACT.URL}", idemURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                    "<i>ídem</i>"));
        temp = temp.Replace("{RECORDING.ACT.STATUS}", @"&nbsp;");
      }

      temp = temp.Replace("{PROPERTY.URL}", propertyURL.Replace("{PROPERTY.TRACT}", propertyEvent.Property.UniqueCode));
      if (recordingAct.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.Property ||
          recordingAct.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.Structure) {
        temp = temp.Replace("{PROPERTY.STATUS}", propertyEvent.StatusName);
      } else {
        temp = temp.Replace("{PROPERTY.STATUS}", "N/A");
      }

      RecordingAct antecedent = propertyEvent.Property.GetDomainAntecedent(recordingAct);
      if (propertyEvent.Property.IsEmptyInstance) {
        temp = temp.Replace("{ANTECEDENT.TAG}", String.Empty);
        temp = temp.Replace("{ANTECEDENT.STATUS}", String.Empty);
      } else if (antecedent.Equals(InformationAct.Empty)) {
        temp = temp.Replace("{ANTECEDENT.TAG}", "Sin antecedente");
        temp = temp.Replace("{ANTECEDENT.STATUS}", String.Empty);
      } else if (recordingAct.RecordingActType.RecordingRule.IsAnnotation) {
        temp = temp.Replace("{ANTECEDENT.TAG}", "*Anotación*");
        temp = temp.Replace("{ANTECEDENT.STATUS}", String.Empty);
      } else {
        temp = temp.Replace("{ANTECEDENT.TAG}", "Antecedente: " + antecedent.Recording.FullNumber);
        temp = temp.Replace("{ANTECEDENT.STATUS}", "(" + antecedent.StatusName + ")");
      }

      if (propertyEventIndex == 0) {
        temp = temp.Replace("{OPTIONS.COMBO}", optionsCombo);
        if (recordingAct.Recording.RecordingActs.Count > 1) {
          if (recordingActIndex != 0) {
            temp = temp.Replace("{INCREMENT_INDEX}", "<option value='upwardRecordingAct'>Subir en la secuencia</option>");
          } else {
            temp = temp.Replace("{INCREMENT_INDEX}", String.Empty);
          }
          if (recordingActIndex != recordingAct.Recording.RecordingActs.Count - 1) {
            temp = temp.Replace("{DECREMENT_INDEX}", "<option value='downwardRecordingAct'>Bajar en la secuencia</option>");
          } else {
            temp = temp.Replace("{DECREMENT_INDEX}", String.Empty);
          }
        } else { // recording.RecordingActs.Count <= 1
          temp = temp.Replace("{INCREMENT_INDEX}", String.Empty);
          temp = temp.Replace("{DECREMENT_INDEX}", String.Empty);
        }
      } else { // propertyEventIndex != 0
        temp = temp.Replace("{OPTIONS.COMBO}", propertyOptionsCombo);
      }
      temp = temp.Replace("{ID}", recordingAct.Id.ToString());
      temp = temp.Replace("{PROPERTY.ID}", propertyEvent.Property.Id.ToString());
      temp = temp.Replace("{RECORDING.ID}", recordingAct.Recording.Id.ToString());
      temp = temp.Replace("{RECORDING.BOOK}", recordingAct.Recording.RecordingBook.FullName);
      temp = temp.Replace("{RECORDING.BOOK.ID}", recordingAct.Recording.RecordingBook.Id.ToString());
      temp = temp.Replace("{RECORDING.NUMBER}", recordingAct.Recording.Number);
      return temp;
    }

    static public string GetRecordingsSummaryTable(ObjectList<Recording> recordings, int pageSize, int pageIndex) {
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

        if (recordings[i].BaseRecordingId != -1) {
          temp = temp.Replace("{RECORDING.IMAGES}", "*Anotación*");
        } else if (recordings[i].StartImageIndex <= 0 && recordings[i].EndImageIndex <= 0) {
          temp = temp.Replace("{RECORDING.IMAGES}", "Sin imagen");
        } else {
          temp = temp.Replace("{RECORDING.IMAGES}", "De la " + recordings[i].StartImageIndex.ToString() + " a la " + recordings[i].EndImageIndex);
        }
        if (recordings[i].PresentationTime == Empiria.ExecutionServer.DateMaxValue) {
          temp = temp.Replace("{RECORDING.PRESENTATION.TIME}", "No consta");
        } else {
          temp = temp.Replace("{RECORDING.PRESENTATION.TIME}", recordings[i].PresentationTime.ToString("dd/MMM/yyyy HH:mm"));
        }
        if (recordings[i].AuthorizedTime == Empiria.ExecutionServer.DateMaxValue) {
          temp = temp.Replace("{RECORDING.AUTHORIZATION.TIME}", "No consta");
        } else {
          temp = temp.Replace("{RECORDING.AUTHORIZATION.TIME}", recordings[i].AuthorizedTime.ToString("dd/MMM/yyyy"));
        }
        if (recordings[i].Notes.Length == 0) {
          temp = temp.Replace("{RECORDING.NOTES}", "Ninguna");
        } else {
          temp = temp.Replace("{RECORDING.NOTES}", recordings[i].Notes);
        }
        temp = temp.Replace("{RECORDING.STATUS}", recordings[i].StatusName);
        temp = temp.Replace("{CAPTURED.BY}", recordings[i].CapturedBy.Alias);

        temp = temp.Replace("{RECORDING.ID}", recordings[i].Id.ToString());
        html += temp;
      }
      return header + html + "</table>";
    }

    static private string GetRecordingActPartyRowTemplate(bool readOnly) {
      const string recordingActPartyRow =
            "<tr class='{CLASS}'><td style='white-space:normal'>" +
            "<a href='javascript:doOperation(\"{ON.CLICK.EVENT}\", {PARTY.ID})'><b id='ancRecordingAct_{ID}'>{NAME}</b></a><br />{SECONDARY.TABLE}</td>" +
            "<td>{DATE.ID}</td>" +
            "<td style='white-space:normal'>{ROLE}</td>" +
            "<td style='white-space:normal'>{DOMAIN}<br>{DOMAIN.PART}</td>";
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
