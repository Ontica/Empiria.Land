using System;

using Empiria.Land.Registration;

namespace Empiria.Land.UI.Utilities {

  internal abstract class RecordingActGridRow {

    #region Constructors and parsers

    internal protected RecordingActGridRow(RecordingDocument document, RecordingAct recordingAct) {
      Assertion.AssertObject(document, "document");
      Assertion.AssertObject(recordingAct, "recordingAct");

      this.Document = document;
      this.RecordingAct = recordingAct;
    }

    #endregion Constructors and parsers

    #region Public properties

    protected RecordingDocument Document {
      get;
      private set;
    }

    protected RecordingAct RecordingAct {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected string GetRowTemplate() {
      const string template = "<tr class='{{CLASS}}'>" +
                              "<td><b id='ancRecordingActIndex_{{TARGET.ID}}'>{{INDEX}}</b><br/>" +
                              "<td style='white-space:normal'>{{RECORDING.ACT.URL}}</td>" +
                              "<td style='white-space:nowrap'>{{RESOURCE.URL}}</td>" +
                              "<td style='white-space:normal'>{{ANTECEDENT}}</td>" +
                              "<td>{{STATUS}}</td>" +
                              "<td>{{OPTIONS.COMBO}}</td></tr>";

      int index = this.RecordingAct.Index + 1;

      string html = template.Replace("{{CLASS}}", (index % 2 == 0) ? "detailsItem" : "detailsOddItem");
      html = html.Replace("{{INDEX}}", index.ToString("00"));

      return html;
    }

    #endregion Public methods

  }  // abstract class RecordingActGridRow

}  // namespace Empiria.Land.UI.Utilities
