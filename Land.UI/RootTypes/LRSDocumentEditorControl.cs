/* Empiria Land 2014 ******************************************************************************************
*                                                                                                             *
*  Solution  : Empiria Land                                    System   : Land Registration System            *
*  Namespace : Empiria.Land.UI                                 Assembly : Empiria.Land.UI                     *
*  Type      : LRSGridControls                                 Pattern  : User Control                        *
*  Version   : 1.5        Date: 28/Mar/2014                    License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
********************************** Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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