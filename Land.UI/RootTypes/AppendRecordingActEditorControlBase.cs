/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : RecordingActEditorControlBase                   Pattern  : User Control                        *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : User control to collect recording act information. This type should be derived in              *
*              a concrete aspx user control.                                                                  *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Presentation.Web;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.UI {

  /// <summary>User control to collect recording act information. This type should be derived in a
  /// concrete aspx user control.</summary>
  public abstract class AppendRecordingActEditorControlBase : WebUserControl {

    #region Public properties

    static private string _virtualPath = ConfigurationData.GetString("UIControls.AppendRecordingActEditorControl");
    static public string ControlVirtualPath {
      get {
        return _virtualPath;
      }
    }

    public RecordingDocument Document {
      get;
      private set;
    }

    public LRSTransaction Transaction {
      get;
      private set;
    }

    public bool IsHistoricEdition {
      get;
      private set;
    }

    public Recording HistoricRecording {
      get;
      private set;
    }


    #endregion Public properties

    #region Public methods

    public abstract RecordingAct[] CreateRecordingActs();

    public void Initialize(RecordingDocument document) {
      Assertion.AssertObject(document, "document");

      this.Document = document;
      this.Transaction = document.GetTransaction();

      this.IsHistoricEdition = this.Document.IsHistoricDocument;

      if (this.IsHistoricEdition) {
        this.HistoricRecording = this.Document.TryGetHistoricRecording();
      } else {
        this.HistoricRecording = Recording.Empty;
      }

      if (this.IsHistoricEdition) {
        Assertion.Assert(this.Transaction.IsEmptyInstance,
                        "For historic documents, transaction should be the empty instance.");
      }
    }

    public bool IsReadyForEdition() {
      return this.Document.IsReadyForEdition();
    }

    #endregion Public methods

  } // class RecordingActEditorControlBase

} // namespace Empiria.Land.UI
