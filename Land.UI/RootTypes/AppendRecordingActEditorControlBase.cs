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

    #endregion Public properties

    #region Public methods

    public abstract RecordingAct[] CreateRecordingActs();

    public void Initialize(LRSTransaction transaction, RecordingDocument document) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(document, "document");

      this.Transaction = transaction;
      this.Document = document;
    }

    public bool IsReadyForEdition() {
      if (this.Transaction.IsEmptyInstance) {
        return false;
      }
      if (this.Transaction.Document.IsEmptyInstance) {
        return false;
      }
      if (!ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.Register")) {
        return false;
      }
      if (!(this.Transaction.Workflow.CurrentStatus == LRSTransactionStatus.Recording ||
            this.Transaction.Workflow.CurrentStatus == LRSTransactionStatus.Elaboration)) {
        return false;
      }
      if (this.Transaction.Document.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return true;
    }

    #endregion Public methods

  } // class RecordingActEditorControlBase

} // namespace Empiria.Land.UI
