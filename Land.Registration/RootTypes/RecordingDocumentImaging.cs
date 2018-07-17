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

    internal RecordingDocumentImaging(RecordingDocument document) {
      this.Document = document;
    }

    #endregion Constructors and parsers

    #region Public properties
    internal RecordingDocument Document {
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
    }


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
      if (this.Document.RecordingActs.CountAll((x) => !x.PhysicalRecording.IsEmptyInstance) != 0) {
        return false;
      }
      return true;
    }

    public void GenerateImagingControlID() {
      Assertion.Assert(!this.Document.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Assert(this.Document.IsClosed, "Document is not closed.");
      Assertion.Assert(this.ImagingControlID.Length == 0,
                       "Document has already assigned an imaging control number.");
      Assertion.Assert(this.Document.RecordingActs.Count > 0, "Document should have recording acts.");
      Assertion.Assert(this.Document.RecordingActs.CountAll((x) => !x.PhysicalRecording.IsEmptyInstance) == 0,
                       "Document can't have any recording acts that are related to physical recordings.");


      this.ImagingControlID = DocumentsData.GetNextImagingControlID(this.Document);
      DocumentsData.SaveImagingControlID(this.Document);
    }


    public void SetAuxiliarImageSet(ImagingItem image) {
      Assertion.AssertObject(image, "image");

      this.Document.ExtensionData.AuxiliarImageSetId = image.Id;
      this.Document.Save();
    }

    public void SetImageSet(ImagingItem image) {
      Assertion.AssertObject(image, "image");

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
