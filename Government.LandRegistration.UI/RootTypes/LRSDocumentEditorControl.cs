﻿/* Empiria® Land 2014 *****************************************************************************************
*                                                                                                             *
*  Solution  : Empiria® Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land                        *
*  Type      : LRSGridControls                                 Pattern  : Presentation Services Static Class  *
*  Date      : 28/Mar/2014                                     Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
***************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System.Web.UI;

using Empiria.Land.Registration;

namespace Empiria.Land.UI {

  public abstract class LRSDocumentEditorControl : UserControl {

    #region Abstract members

    static private string virtualPath = ConfigurationData.GetString("RecordingDocument.EditorControl");

    protected abstract RecordingDocument ImplementsFillRecordingDocument(RecordingDocumentType documentType);
    protected abstract void ImplementsLoadRecordingDocument();

    #endregion Abstract members

    #region Fields

    private RecordingDocument document = RecordingDocument.Empty;

    #endregion Fields

    #region Public properties

    static public string ControlVirtualPath {
      get { return virtualPath; }
    }

    public RecordingDocument Document {
      get { return document; }
    }

    #endregion Public properties

    #region Public methods

    public RecordingDocument FillRecordingDocument(RecordingDocumentType documentType) {
      if (this.Document == RecordingDocument.Empty) {
        this.document = RecordingDocument.Create(documentType);
      }
      return ImplementsFillRecordingDocument(documentType);
    }

    public void LoadRecordingDocument(RecordingDocument document) {
      this.document = document;
      if (!IsPostBack) {
        ImplementsLoadRecordingDocument();
      }
    }

    #endregion Public methods

  } // class LRSDocumentEditorControl

} // namespace Empiria.Land.UI
