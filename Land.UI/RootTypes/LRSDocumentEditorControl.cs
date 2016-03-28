/* Empiria Land ***********************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : LRSGridControls                                 Pattern  : User Control                        *
*  Version   : 2.1                                             License  : Please read license.txt file        *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
********************************** Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Presentation.Web;
using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  public abstract class LRSDocumentEditorControl : WebUserControl {

    #region Public properties

    static private string _virtualPath = ConfigurationData.GetString("RecordingDocument.EditorControl");
    static public string ControlVirtualPath {
      get {
        return _virtualPath;
      }
    }

    public RecordingDocument Document {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    public RecordingDocument FillRecordingDocument(RecordingDocumentType documentType) {
      if (this.Document.IsEmptyInstance) {
        this.Document = new RecordingDocument(documentType);
      }
      return ImplementsFillRecordingDocument(documentType);
    }

    protected abstract RecordingDocument ImplementsFillRecordingDocument(RecordingDocumentType documentType);

    protected abstract void ImplementsLoadRecordingDocument();

    public void LoadRecordingDocument(RecordingDocument document) {
      this.Document = document;
      if (!IsPostBack) {
        ImplementsLoadRecordingDocument();
      }
    }

    #endregion Public methods

  } // class LRSDocumentEditorControl

} // namespace Empiria.Land.UI
