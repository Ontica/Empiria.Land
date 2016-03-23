using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI.Utilities {

  internal class DocumentActGridRow : RecordingActGridRow {

    #region Constructors and parsers

    internal DocumentActGridRow(RecordingDocument document,
                                RecordingAct recordingAct) : base(document, recordingAct) {

    }

    #endregion Constructors and parsers

    #region Public methods

    internal override string GetRecordingActRow(TractItem tractItem) {
      string row = base.GetRowTemplate();

      row = row.Replace("{{STATUS}}", base.RecordingAct.StatusName);
      row = row.Replace("{{RECORDING.ACT.URL}}", base.RecordingAct.DisplayName);
      row = row.Replace("{{RESOURCE.URL}}", "No aplica");

      row = row.Replace("{{ID}}", base.RecordingAct.Id.ToString());
      row = row.Replace("{{TARGET.ID}}", base.RecordingAct.Id.ToString());

      row = row.Replace("{{RESOURCE.ID}}", base.RecordingAct.Id.ToString());
      row = row.Replace("{{ANTECEDENT}}", base.RecordingAct.Id.ToString());
      row = row.Replace("{{OPTIONS.COMBO}}", this.GetOptionsCombo(tractItem));

      return row;
    }

    #endregion Public methods

    #region Private methods

    private string GetOptionsCombo(TractItem tractItem) {
      const string template =
        "<select id='cboRecordingOptions_{{TARGET.ID}}' class='selectBox' style='width:148px'>" +
        "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
        "<option value='modifyRecordingActType'>Modificar este acto</option>" +
        "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
        "</select><img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
        "alt='' title='Ejecuta la operación seleccionada' onclick='" +
        "doOperation(getElement(\"cboRecordingOptions_{{TARGET.ID}}\").value, {{ID}}, {{RESOURCE.ID}});'/>";

      string html = template.Replace("{{ID}}", tractItem.RecordingAct.Id.ToString());
      html = html.Replace("{{RESOURCE.ID}}", "-1");
      html = html.Replace("{{TARGET.ID}}", tractItem.Id.ToString());

      return html;
    }

    #endregion Private methods

  }  // class DocumentActGridRow

}  // namespace Empiria.Land.UI.Utilities
