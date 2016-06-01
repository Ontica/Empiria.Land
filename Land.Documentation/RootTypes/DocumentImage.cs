/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImage                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a processed and ready to use document image.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Documents;
using Empiria.Json;
using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Represents a processed and ready to use document image.</summary>
  public class DocumentImage : ImagingItem {

    #region Constructors and parsers

    internal DocumentImage(CandidateImage candidateImage) {
      Assertion.AssertObject(candidateImage, "candidateImage");
      Assertion.Assert(candidateImage.ReadyToCreate,
                       "CandidateImage is not ready to be created.");

      this.Document = candidateImage.Document;
      this.DocumentImageType = candidateImage.DocumentImageType;
      this.BaseFolder = candidateImage.BaseFolder;
      base.ItemPath = this.GetRelativePath(candidateImage.SourceFile.FullName);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("BaseFolderId")]
    public ImagingFolder BaseFolder {
      get;
      private set;
    }

    [DataField("DocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }

    [DataField("DocumentImageType", Default = DocumentImageType.Unknown)]
    public DocumentImageType DocumentImageType {
      get;
      private set;
    }

    [DataField("ImagingItemExtData")]
    protected internal JsonObject ImagingItemExtData {
      get;
      set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      DataServices.WriteImagingItem(this);
    }

    #endregion Public methods

    #region Private methods

    protected sealed override void GetFilesCounters(out int filesCount, out int totalSize) {
      filesCount = 0;
      totalSize = 0;
    }

    ///<summary>Builds the relative path from the BaseFolder given an absolute path.</summary>
    private string GetRelativePath(string absolutePath) {
      string baseFolder = this.BaseFolder.ItemPath.Trim('\\');

      return absolutePath.Replace(baseFolder, "~");
    }

    #endregion Private methods

  }  // class DocumentImage

}  // namespace Empiria.Land.Documentation
