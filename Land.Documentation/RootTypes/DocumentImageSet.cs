/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImageSet                               Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a processed and ready to use document image set.                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Documents;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Represents a processed and ready to use document image set.</summary>
  public class DocumentImageSet : ImageSet, IProtected {

    #region Constructors and parsers

    private DocumentImageSet() {
      // Required by Empiria Framework
    }

    internal DocumentImageSet(CandidateImage candidateImage, string[] imagesHashCodes) {
      Assertion.AssertObject(candidateImage, "candidateImage");
      Assertion.AssertObject(imagesHashCodes, "imagesHashCodes");

      Assertion.Assert(candidateImage.ReadyToCreate,
                       "CandidateImage is not ready to be created.");
      Assertion.Assert(imagesHashCodes.Length > 0,
                       "CandidateImage has no inner images.");
      this.Document = candidateImage.Document;
      this.DocumentImageType = candidateImage.DocumentImageType;
      base.BaseFolder = candidateImage.BaseFolder;
      base.ItemPath = this.GetRelativePath(candidateImage.SourceFile.FullName);
      base.FilesCount = imagesHashCodes.Length;

      this.SetImagesHashCodes(imagesHashCodes);
    }

    static public new DocumentImageSet Parse(int id) {
      return BaseObject.ParseId<DocumentImageSet>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("DocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }

    [DataField("ImageType", Default = DocumentImageType.Unknown)]
    public DocumentImageType DocumentImageType {
      get;
      private set;
    }

    internal string MainImageFileName {
      get {
        return base.ItemPath.Substring(base.ItemPath.LastIndexOf('\\') + 1);
      }
    }

    internal string MainImageFilePath {
      get {
        return base.ItemPath.Substring(0, base.ItemPath.LastIndexOf('\\'))
                            .Replace("~", this.BaseFolder.FullPath);
      }
    }

    public string UrlRelativePath {
      get {
        return base.ItemPath.Substring(0, base.ItemPath.LastIndexOf('\\') + 1)
                            .Replace("~", this.BaseFolder.UrlRelativePath).Replace('\\', '/');
      }
    }


    private string[] _imagesFileNamesArray = null;
    public string[] ImagesNamesArray {
      get {
        if (_imagesFileNamesArray == null) {
          _imagesFileNamesArray = this.GetImagesFileNamesArray();
        }
        return _imagesFileNamesArray;
      }
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "DocumentUID", this.Document.UID, "ImageSetType", (char) this.DocumentImageType,
          "BaseFolder", this.BaseFolder.Id, "ItemPath", base.ItemPath, "ExtData",
          this.ImagingItemExtData.ToString(), "FilesCount", this.FilesCount
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      DataServices.WriteImagingItem(this);
    }

    #endregion Public methods

    #region Private methods

    ///<summary>Builds the relative path from the BaseFolder given an absolute path.</summary>
    private string GetRelativePath(string absolutePath) {
      string baseFolder = this.BaseFolder.ItemPath.Trim('\\');

      return absolutePath.Replace(baseFolder, "~");
    }

    private string[] GetImagesFileNamesArray() {
      string[] array = new string[base.FilesCount];

      string baseFileName = MainImageFileName.Replace(".tif", String.Empty);

      for (int i = 0; i < base.FilesCount; i++) {
        if (base.FilesCount < 100) {
          array[i] = String.Concat(baseFileName, ".", (i + 1).ToString("00"),
                                   "_of_" + base.FilesCount.ToString("00") + ".png");
        } else {
          array[i] = String.Concat(baseFileName, ".", (i + 1).ToString("000"),
                                   "_of_" + base.FilesCount.ToString("000") + ".png");
        }
      }
      return array;
    }

    private void SetImagesHashCodes(string[] imagesHashCodes) {
      var json = new JsonObject();

      json.Add(new JsonItem("imageHashCodes", imagesHashCodes));

      this.ImagingItemExtData = json;
    }

    #endregion Private methods

  }  // class DocumentImageSet

}  // namespace Empiria.Land.Documentation
