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
      // Used by Empiria Framework
    }

    public RecordingDocumentImaging(RecordingDocument landRecord) {
      this.LandRecord = landRecord;
    }


    #endregion Constructors and parsers

    #region Public properties


    public RecordingDocument LandRecord {
      get;
    }


    public int AuxiliarImageSetId {
      get {
        return this.LandRecord.ExtensionData.AuxiliarImageSetId;
      }
    }


    public bool HasAuxiliarImageSet {
      get {
        return (this.LandRecord.ExtensionData.AuxiliarImageSetId != -1);
      }
    }


    public bool HasImageSet {
      get {
        return (this.LandRecord.ExtensionData.DocumentImageSetId != -1);
      }
    }


    public int ImageSetId {
      get {
        return this.LandRecord.ExtensionData.DocumentImageSetId;
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
      if (this.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (this.ImagingControlID.Length != 0) {
        return false;
      }
      if (!this.LandRecord.IsClosed) {
        return false;
      }
      if (this.LandRecord.RecordingActs.Count == 0) {
        return false;
      }
      if (this.LandRecord.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) != 0) {
        return false;
      }
      return true;
    }


    public void GenerateImagingControlID() {
      Assertion.Require(!this.LandRecord.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Require(this.LandRecord.IsClosed, "Document is not closed.");

      Assertion.Require(this.ImagingControlID.Length == 0,
                        "Document has already assigned an imaging control number.");

      Assertion.Require(this.LandRecord.RecordingActs.Count > 0, "Document should have recording acts.");
      Assertion.Require(this.LandRecord.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) == 0,
                        "Document can't have any recording acts that are related to physical book entries.");


      this.ImagingControlID = DocumentsData.GetNextImagingControlID(this.LandRecord);

      DocumentsData.SaveImagingControlID(this.LandRecord);
    }


    public void SetAuxiliarImageSet(ImagingItem image) {
      Assertion.Require(image, "image");

      this.LandRecord.ExtensionData.AuxiliarImageSetId = image.Id;

      this.LandRecord.Save();
    }


    public void SetImageSet(ImagingItem image) {
      Assertion.Require(image, "image");

      this.LandRecord.ExtensionData.DocumentImageSetId = image.Id;

      this.LandRecord.Save();
    }


    public ImagingItem TryGetAuxiliarImageSet() {
      if (this.LandRecord.ExtensionData.AuxiliarImageSetId != -1) {
        return ImagingItem.Parse(this.LandRecord.ExtensionData.AuxiliarImageSetId);
      } else {
        return null;
      }
    }


    public ImagingItem TryGetImageSet() {
      if (this.LandRecord.ExtensionData.DocumentImageSetId != -1) {
        return ImagingItem.Parse(this.LandRecord.ExtensionData.DocumentImageSetId);
      } else {
        return null;
      }
    }


    #endregion Public methods

  } // class RecordingDocumentImaging

} // namespace Empiria.Land.Registration
