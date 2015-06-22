/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImage                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a document image in Land Registration System.                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.IO;
using System.Text.RegularExpressions;

using Empiria.Documents;
using Empiria.Documents.IO;
using Empiria.Json;
using Empiria.Land.Registration;
using Empiria.Security;

namespace Empiria.Land.Documentation {

  public enum DocumentImageType {
    Unknown = 'U',
    MainDocument = 'E',
    Appendix = 'A',
    Folder = 'F'
  }

  /// <summary>Represents a document image in Land Registration System.</summary>
  public class DocumentImage : ImagingItem {

    #region Fields

    static readonly string rootTargetFolder = ConfigurationData.GetString("DocumentImage.RootTargetFolder");

    #endregion Fields

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
