/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording Services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Separated entity                      *
*  Type     : RecordingDocumentImaging                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains security methods used to protect the integrity of recording documents.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Documents;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Contains security methods used to protect the integrity of recording documents.</summary>
  public class RecordingDocumentImaging {

    #region Constructors and parsers


    private RecordingDocumentImaging() {

    }

    public RecordingDocumentImaging(RecordingDocument document) {
      this.Document = document;
    }


    #endregion Constructors and parsers

    #region Public properties


    public RecordingDocument Document {
      get;
    }


    public int AuxiliarImageSetId {
      get {
        return this.Document.ExtensionData.AuxiliarImageSetId;
      }
    }


    public bool HasAuxiliarImageSet {
      get {
        return (this.Document.ExtensionData.AuxiliarImageSetId != -1);
      }
    }


    public bool HasImageSet {
      get {
        return (this.Document.ExtensionData.DocumentImageSetId != -1);
      }
    }


    public int ImageSetId {
      get {
        return this.Document.ExtensionData.DocumentImageSetId;
      }
    }


    [DataField("ImagingControlID")]
    public string ImagingControlID {
      get;
      private set;
    } = string.Empty;


    #endregion Public properties

    #region Public methods


    public bool CanGenerateImagingControlID() {
      if (this.Document.IsEmptyInstance) {
        return false;
      }
      if (this.ImagingControlID.Length != 0) {
        return false;
      }
      if (!this.Document.IsClosed) {
        return false;
      }
      if (this.Document.RecordingActs.Count == 0) {
        return false;
      }
      if (this.Document.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) != 0) {
        return false;
      }
      return true;
    }


    public void GenerateImagingControlID() {
      Assertion.Require(!this.Document.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Require(this.Document.IsClosed, "Document is not closed.");

      Assertion.Require(this.ImagingControlID.Length == 0,
                        "Document has already assigned an imaging control number.");

      Assertion.Require(this.Document.RecordingActs.Count > 0, "Document should have recording acts.");
      Assertion.Require(this.Document.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) == 0,
                        "Document can't have any recording acts that are related to physical book entries.");


      this.ImagingControlID = DocumentsData.GetNextImagingControlID(this.Document);

      DocumentsData.SaveImagingControlID(this.Document);
    }


    public void SetAuxiliarImageSet(ImagingItem image) {
      Assertion.Require(image, "image");

      this.Document.ExtensionData.AuxiliarImageSetId = image.Id;

      this.Document.Save();
    }


    public void SetImageSet(ImagingItem image) {
      Assertion.Require(image, "image");

      this.Document.ExtensionData.DocumentImageSetId = image.Id;

      this.Document.Save();
    }


    public ImagingItem TryGetAuxiliarImageSet() {
      if (this.Document.ExtensionData.AuxiliarImageSetId != -1) {
        return ImagingItem.Parse(this.Document.ExtensionData.AuxiliarImageSetId);
      } else {
        return null;
      }
    }


    public ImagingItem TryGetImageSet() {
      if (this.Document.ExtensionData.DocumentImageSetId != -1) {
        return ImagingItem.Parse(this.Document.ExtensionData.DocumentImageSetId);
      } else {
        return null;
      }
    }


    #endregion Public methods

  } // class RecordingDocumentImaging

} // namespace Empiria.Land.Registration
