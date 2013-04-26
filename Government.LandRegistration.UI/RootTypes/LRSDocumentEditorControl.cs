/* Empiria® Land 2013 *****************************************************************************************
*                                                                                                             *
*  Solution  : Empiria® Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration.UI          Assembly : Empiria.Government.LandRegistration *
*  Type      : LRSGridControls                                 Pattern  : Presentation Services Static Class  *
*  Date      : 25/Jun/2013                                     Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                             *
*  Summary   : Static class that generates predefined grid content for Land Registration System data.         *
*                                                                                                             *
***************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System.Web.UI;

namespace Empiria.Government.LandRegistration.UI {

  public abstract class LRSDocumentEditorControl : UserControl {

    #region Abstract members

    static private string virtualPath = ConfigurationData.GetString("RecordingDocument.EditorControl");

    protected abstract void ImplementsFillRecordingDocument(RecordingDocumentType documentType);
    protected abstract void ImplementsLoadRecordingDocument();

    #endregion Abstract members

    #region Fields

    private RecordingDocument document = RecordingDocument.Empty;
    private Recording recording = Recording.Empty;

    #endregion Fields

    #region Public properties

    static public string ControlVirtualPath {
      get { return virtualPath; }
    }

    public RecordingDocument Document {
      get { return document; }
      set { document = value; }
    }

    public Recording Recording {
      get { return recording; }
      set { recording = value; }
    }

    #endregion Public properties

    #region Public methods

    public void FillRecordingDocument(RecordingDocumentType documentType) {
      ImplementsFillRecordingDocument(documentType);
    }

    public void LoadRecordingDocument() {
      ImplementsLoadRecordingDocument();
    }

    #endregion Public methods

  } // class LRSDocumentEditorControl

} // namespace Empiria.Government.LandRegistration.UI