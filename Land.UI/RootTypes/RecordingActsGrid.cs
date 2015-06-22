/* Empiria Land 2015 ******************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : RecordingActsGrid                               Pattern  : Standard class                      *
*  Version   : 2.0        Date: 25/Jun/2015                    License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Generates the grid HTML content for a document's recording acts.                               *
*                                                                                                             *
********************************** Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections;
using System.Web.UI.WebControls;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Data;
using Empiria.Land.UI.Utilities;

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

        for (int j = 0; j < recordingAct.Targets.Count; j++) {
          if (j == 0) {
            html += this.GetRecordingActRow(recordingAct, recordingAct.Targets[j]);
          } else {
            html += this.GetAdditionalTargetRow(recordingAct, recordingAct.Targets[j]);
          }
        } // for int i
      } // for int j
      return html;
    }

    private string GetAdditionalTargetRow(RecordingAct recordingAct, RecordingActTarget recordingActTarget) {
 	    throw new NotImplementedException();
    }

    private string GetRecordingActRow(RecordingAct recordingAct, RecordingActTarget recordingActTarget) {
      if (recordingAct is AssociationAct) {
        var row = new AssociationActGridRow(document, (AssociationAct) recordingAct);
        return row.GetRecordingActRow((ResourceTarget) recordingActTarget);
      } else if (recordingAct is DocumentAct) {
        var row = new DocumentActGridRow(document, (DocumentAct) recordingAct);
        return row.GetRecordingActRow((DocumentTarget) recordingActTarget);
      } else if (recordingAct is DomainAct) {
        var row = new DomainActGridRow(document, (DomainAct) recordingAct);
        return row.GetRecordingActRow((ResourceTarget) recordingActTarget);
      //} else if (recordingAct is LimitationAct) {
      //  html += GetLimitationActGridRow(recordingAct, counter);
      //} else if (recordingAct is InformationAct) {
      //  html += GetInformationActGridRow(recordingAct, counter);
      //} else if (recordingAct is ModificationAct) {
      //  html += GetModificationActGridRow(recordingAct, counter);
      //} else if (recordingAct is CancelationAct) {
      //  html += GetCancelationActGridRow(recordingAct, counter);
      //} else if (recordingAct is StructureAct) {
      //  html += GetStructureActGridRow(recordingAct, counter);
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    #endregion Public properties

    #region Private auxiliar methods

    static private string GetAdditionalPropertyOptionsCombo(ResourceTarget target) {
      const string template =
        "<select id='cboRecordingOptions_{{TARGET.ID}}' class='selectBox' style='width:148px'>" +
        "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
        "<option value='deleteRecordingActProperty'>Eliminar este predio</option>" +
        "<option value='viewPropertyTract'>Ver la historia de este predio</option>" +
        "</select><img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
        "alt='' title='Ejecuta la operación seleccionada' onclick='" +
        "doOperation(getElement(\"cboRecordingOptions_{{TARGET.ID}}\").value, {{ID}}, {{RESOURCE.ID}});'/>";

      string html = template.Replace("{{TARGET.ID}}", target.Id.ToString());

      html = html.Replace("{{ID}}", target.RecordingAct.Id.ToString());
      html = html.Replace("{{RESOURCE.ID}}", target.Resource.Id.ToString());

      return html;
    }

    static private string GetPropertyOptionsCombo(ResourceTarget target,
                                                  int recordingActsCount, int recordingActIndex) {
      const string template =
        "<select id='cboRecordingOptions_{{TARGET.ID}}' class='selectBox' style='width:148px'>" +
        "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
        "{{INCREMENT_INDEX}}" +
        "{{DECREMENT_INDEX}}" +
        "<option value='selectRecordingActOperation'></option>" +
        "<option value='addPropertyToRecordingAct'>Agregar otro predio</option>" +
        "<option value='addAnotherRecordingActToRecording'>Agregar otro acto</option>" +
        "<option value='modifyRecordingActType'>Modificar este acto</option>" +
        "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
        "<option value='viewPropertyTract'>Ver la historia de este predio</option>" +
        "</select><img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
        "alt='' title='Ejecuta la operación seleccionada' onclick='" +
        "doOperation(getElement(\"cboRecordingOptions_{{TARGET.ID}}\").value, {{ID}}, {{RESOURCE.ID}});'/>";

      string html = template.Replace("{{TARGET.ID}}", target.Id.ToString());
      if (recordingActsCount > 1) {
        if (recordingActIndex != 0) {
          html = html.Replace("{{INCREMENT_INDEX}}",
                  "<option value='upwardRecordingAct'>Subir en la secuencia</option>");
        } else {
          html = html.Replace("{{INCREMENT_INDEX}}", String.Empty);
        }
        if (recordingActIndex != recordingActsCount - 1) {
          html = html.Replace("{{DECREMENT_INDEX}}",
                  "<option value='downwardRecordingAct'>Bajar en la secuencia</option>");
        } else {
          html = html.Replace("{{DECREMENT_INDEX}}", String.Empty);
        }
      } else { // recording.RecordingActs.Count <= 1
        html = html.Replace("{{INCREMENT_INDEX}}", String.Empty);
        html = html.Replace("{{DECREMENT_INDEX}}", String.Empty);
      }
      html = html.Replace("{{ID}}", target.RecordingAct.Id.ToString());
      html = html.Replace("{{RESOURCE.ID}}", target.Resource.Id.ToString());

      return html;
    }

    #endregion Private auxiliar methods

    #region OLD Private methods

    static private string FillAssociationGridRow(RecordingAct recordingAct, Association association, string temp) {
      RecordingAct antecedent = association.GetDomainAntecedent(recordingAct);

      if (recordingAct.IsAmendment && !recordingAct.AmendmentOf.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", recordingAct.AmendmentOf.PhysicalRecording.FullNumber);
      } else if (recordingAct.IsAmendment && recordingAct.AmendmentOf.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Documento: " + recordingAct.AmendmentOf.Document.UID);
      } else if (association.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "&nbsp;");
      } else if (antecedent.Equals(InformationAct.Empty)) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Sin antecedente registral");
      } else if (!antecedent.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Entidad inscrita en: " +
                             antecedent.PhysicalRecording.FullNumber);
      } else if (antecedent.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Documento: " + antecedent.Document.UID);
      }
      return temp;
    }

    static private string FillPropertyGridRow(RecordingAct recordingAct, Property property, string temp) {
      RecordingAct antecedent = property.GetDomainAntecedent(recordingAct);

      if (recordingAct.IsAmendment && !recordingAct.AmendmentOf.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", recordingAct.AmendmentOf.PhysicalRecording.FullNumber);
      } else if (recordingAct.IsAmendment && recordingAct.AmendmentOf.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Documento: " + recordingAct.AmendmentOf.Document.UID);
      } else if (property.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "&nbsp;");
      } else if (!property.IsPartitionOf.IsEmptyInstance) {
        var partitionAntecedent = property.IsPartitionOf.GetDomainAntecedent(recordingAct);
        if (property.PartitionNo.StartsWith("Lote") ||
            property.PartitionNo.StartsWith("Casa") ||
            property.PartitionNo.StartsWith("Departamento")) {
          temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", property.PartitionNo +
                     (property.IsPartitionOf.MergedInto.Equals(property) ? " y última" : String.Empty)
                     + " del predio inscrito en " + partitionAntecedent.PhysicalRecording.FullNumber);
        } else {
          temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Fracción " + property.PartitionNo +
                     (property.IsPartitionOf.MergedInto.Equals(property) ? " y última" : String.Empty)
                     + " del predio inscrito en " + partitionAntecedent.PhysicalRecording.FullNumber);
        }
      } else if (antecedent.Equals(InformationAct.Empty)) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Sin antecedente registral");
      } else if (!antecedent.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Predio inscrito en: " +
                             antecedent.PhysicalRecording.FullNumber);
      } else if (antecedent.PhysicalRecording.IsEmptyInstance) {
        temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", "Documento: " + antecedent.Document.UID);
      }
      return temp;
    }

    static private string GetRecordingActGridRow(RecordingAct recordingAct, ResourceTarget tractItem, int counter,
                                                 int recordingActIndex, int tractItemIndex, int recordingActsCount) {

      const string row = "<tr class='{CLASS}'>" +
          "<td><b id='ancRecordingActIndex_{ID}_{PROPERTY.ID}'>{RECORDING.ACT.INDEX}</b><br/>" +
          "<td style='white-space:normal'>{RECORDING.ACT.URL}</td>" +
          "<td style='white-space:nowrap'>{PROPERTY.URL}</td>" +
          "<td style='white-space:normal'>{PHYSICAL.RECORDING.DATA}</td>" +
          "<td>{RECORDING.ACT.STATUS}</td>" +
          "<td>{OPTIONS.COMBO}<img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
          "alt='' title='Ejecuta la operación seleccionada' " +
          "onclick='doOperation(getElement(\"cboRecordingOptions_{ID}_{PROPERTY.ID}\").value, {ID}, {PROPERTY.ID});'/>" +
          "</td></tr>";
      const string editableURL = "<a href='javascript:doOperation(\"editRecordingAct\", {RECORDING.BOOK.ID}, {RECORDING.ID})'>" +
                                 "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b></a>";
      const string readonlyURL = "<b id='ancRecordingAct_{ID}'>{RECORDING.ACT.DISPLAY.NAME}</b>";

      const string propertyURL = "<a href='javascript:doOperation(\"editProperty\", {ID}, {PROPERTY.ID})'>" +
                                 "<b id='ancRecordingActProperty_{ID}_{PROPERTY.ID}'>{PROPERTY.TRACT}</b></a>";

      const string optionsCombo =
          "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
          "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
          "{INCREMENT_INDEX}" +
          "{DECREMENT_INDEX}" +
          "<option value='selectRecordingActOperation'></option>" +
          "<option value='addPropertyToRecordingAct'>Agregar otro predio</option>" +
          "<option value='addAnotherRecordingActToRecording'>Agregar otro acto</option>" +
          "<option value='modifyRecordingActType'>Modificar este acto</option>" +
          "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
          "</select>";

      const string propertyOptionsCombo =
          "<select id='cboRecordingOptions_{ID}_{PROPERTY.ID}' class='selectBox' style='width:148px'>" +
          "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
          "<option value='deleteRecordingActProperty'>Eliminar este predio</option>" +
          "<option value='viewPropertyTract'>Ver la historia de este predio</option>" +
          "</select>";

      string temp = row.Replace("{CLASS}", (counter % 2 == 0) ? "detailsItem" : "detailsOddItem");

      if (tractItemIndex == 0) {
        temp = temp.Replace("{RECORDING.ACT.INDEX}", counter.ToString("00"));
        if (recordingAct.RecordingActType.Autoregister) {
          temp = temp.Replace("{RECORDING.ACT.URL}", readonlyURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                         recordingAct.DisplayName));
        } else {
          temp = temp.Replace("{RECORDING.ACT.URL}", editableURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                         recordingAct.DisplayName));
        }
        temp = temp.Replace("{RECORDING.ACT.DISPLAY.NAME}", recordingAct.DisplayName);
        temp = temp.Replace("{RECORDING.ACT.STATUS}", recordingAct.StatusName);
      } else {
        temp = temp.Replace("{RECORDING.ACT.INDEX}", "<i>" + counter.ToString("00") + "</i>");
        temp = temp.Replace("{RECORDING.ACT.URL}", readonlyURL.Replace("{RECORDING.ACT.DISPLAY.NAME}",
                                                                       "<i>ídem</i>"));
        temp = temp.Replace("{RECORDING.ACT.STATUS}", @"&nbsp;");
      }

      if (tractItem.Resource is Property && ((Property) tractItem.Resource).CadastralKey.Length != 0) {
        temp = temp.Replace("{PROPERTY.URL}", propertyURL.Replace("{PROPERTY.TRACT}", tractItem.Resource.UID
                                              + " <br />" + ((Property) tractItem.Resource).CadastralKey));
      } else {
        temp = temp.Replace("{PROPERTY.URL}", propertyURL.Replace("{PROPERTY.TRACT}", tractItem.Resource.UID));
      }
      if (recordingAct.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.Property ||
          recordingAct.RecordingActType.RecordingRule.AppliesTo == RecordingRuleApplication.Structure) {
        temp = temp.Replace("{PROPERTY.STATUS}", tractItem.StatusName);
      } else {
        temp = temp.Replace("{PROPERTY.STATUS}", "N/A");
      }

      if (tractItem.Resource is Association) {
        temp = FillAssociationGridRow(recordingAct, (Association) tractItem.Resource, temp);
      } else {
        temp = FillPropertyGridRow(recordingAct, (Property) tractItem.Resource, temp);
      }

      temp = temp.Replace("{PHYSICAL.RECORDING.DATA}", recordingAct.Id.ToString());

      if (tractItemIndex == 0) {
        temp = temp.Replace("{OPTIONS.COMBO}", optionsCombo);
        if (recordingActsCount > 1) {
          if (recordingActIndex != 0) {
            temp = temp.Replace("{INCREMENT_INDEX}",
                   "<option value='upwardRecordingAct'>Subir en la secuencia</option>");
          } else {
            temp = temp.Replace("{INCREMENT_INDEX}", String.Empty);
          }
          if (recordingActIndex != recordingActsCount - 1) {
            temp = temp.Replace("{DECREMENT_INDEX}",
                   "<option value='downwardRecordingAct'>Bajar en la secuencia</option>");
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
      temp = temp.Replace("{PROPERTY.ID}", tractItem.Resource.Id.ToString());
      temp = temp.Replace("{RECORDING.ID}", recordingAct.PhysicalRecording.Id.ToString());
      temp = temp.Replace("{RECORDING.BOOK}", recordingAct.PhysicalRecording.RecordingBook.AsText);
      temp = temp.Replace("{RECORDING.BOOK.ID}", recordingAct.PhysicalRecording.RecordingBook.Id.ToString());
      temp = temp.Replace("{RECORDING.NUMBER}", recordingAct.PhysicalRecording.Number);

      return temp;
    }

    #endregion OLD Private methods

  } // class LRSGridControls

} // namespace Empiria.Land.UI
