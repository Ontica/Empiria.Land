﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Empiria.Land.Registration;

namespace Empiria.Land.UI.Utilities {

  internal class DomainActGridRow : RecordingActGridRow {

    #region Constructors and parsers

    internal DomainActGridRow(RecordingDocument document,
                              DomainAct domainAct): base(document, domainAct) {

    }

    #endregion Constructors and parsers

    #region Public methods

    internal string GetRecordingActRow(ResourceTarget target) {
      string row = base.GetRowTemplate();

      row = row.Replace("{{STATUS}}", this.RecordingAct.StatusName);
      row = row.Replace("{{RECORDING.ACT.URL}}", this.RecordingAct.DisplayName);
      row = row.Replace("{{RESOURCE.URL}}", target.Resource.UID);

      row = row.Replace("{{ID}}", this.RecordingAct.Id.ToString());
      row = row.Replace("{{TARGET.ID}}", this.RecordingAct.Id.ToString());

      row = row.Replace("{{RESOURCE.ID}}", this.RecordingAct.Id.ToString());
      row = row.Replace("{{ANTECEDENT}}", this.RecordingAct.Id.ToString());
      row = row.Replace("{{OPTIONS.COMBO}}", this.GetOptionsCombo(target));

      return row;
    }

    #endregion Public methods

    #region Private methods

    private string GetOptionsCombo(ResourceTarget target) {
      const string template =
        "<select id='cboRecordingOptions_{{TARGET.ID}}' class='selectBox' style='width:148px'>" +
        "<option value='selectRecordingActOperation'>( Seleccionar )</option>" +
        "<option value='modifyRecordingActType'>Modificar este acto</option>" +
        "<option value='deleteRecordingAct'>Eliminar este acto</option>" +
        "<option value='viewResourceTract'>Ver la historia de este predio</option>" +
        "</select><img class='comboExecuteImage' src='../themes/default/buttons/next.gif' " +
        "alt='' title='Ejecuta la operación seleccionada' onclick='" +
        "doOperation(getElement(\"cboRecordingOptions_{{TARGET.ID}}\").value, {{ID}}, {{RESOURCE.ID}});'/>";

      string html = template.Replace("{{ID}}", target.RecordingAct.Id.ToString());
      html = html.Replace("{{RESOURCE.ID}}", target.Resource.Id.ToString());
      html = html.Replace("{{TARGET.ID}}", target.Id.ToString());

      return html;
    }

    #endregion Private methods

  }  // class DomainActGridRow

}  // namespace Empiria.Land.UI.Utilities