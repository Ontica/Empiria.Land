/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImage                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
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

  /// <summary>Represents a document image in Land Registration System.</summary>
  public class CandidateImage {

    #region Fields

    static readonly string rootTargetFolder = ConfigurationData.GetString("DocumentImage.RootTargetFolder");

    #endregion Fields

    #region Constructors and parsers

    protected CandidateImage(FileInfo sourceFile) {
      Initialize();
      this.SourceFile = sourceFile;
      if (this.IsFileNameValid()) {
        LoadDocumentData();
      }
    }

    static public CandidateImage Parse(FileInfo sourceFile) {
      Assertion.AssertObject(sourceFile, "sourceFile");
      Assertion.Assert(sourceFile.Exists,
                       String.Format("File '{0}' does not exist.", sourceFile.FullName));

      return new CandidateImage(sourceFile);
    }

    #endregion Constructors and parsers

    #region Public properties

    public FileInfo SourceFile {
      get;
      private set;
    }

    public ImagingFolder BaseFolder {
      get;
      private set;
    }

    public RecordingDocument Document {
      get;
      private set;
    }

    public DocumentImageType DocumentImageType {
      get;
      private set;
    }

    public string FolderName {
      get {
        return this.SourceFile.Directory.Name;
      }
    }

    public string FileName {
      get {
        return this.SourceFile.Name;
      }
    }

    internal virtual bool ReadyToCreate {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    internal virtual void AssertCanBeProcessed(bool replaceDuplicated) {
      if (!this.IsFileNameValid()) {
        throw new LandDocumentationException(LandDocumentationException.Msg.FileNameBadFormed,
                                             this.FileName);
      }
      if (this.Document.IsEmptyInstance) {
        throw new LandDocumentationException(LandDocumentationException.Msg.DocumentForFileNameNotFound,
                                             this.FileName);
      }
      if (!replaceDuplicated && this.IsAlreadyDigitalized()) {
        throw new LandDocumentationException(LandDocumentationException.Msg.DocumentAlreadyDigitalized,
                                             this.FileName);
      }
      if (replaceDuplicated) {
        throw new NotImplementedException("Replace duplicated candidate images is not yet implemented.");
      }
      this.AssertBaseImagingFolderExists();
    }

    internal virtual string GetTargetBaseFolderPath() {
      return rootTargetFolder + @"\" + this.Document.PresentationTime.Year;
    }

    internal virtual string GetTargetFolderName() {
      DateTime date = this.Document.PresentationTime;

      // E:\images\2015\2015-01\2015-01-04\RP02WL-91PK57-TG62K5\RP02WL-91PK57-TG62K5_E.01_of_01.png
      return rootTargetFolder + @"\" + date.Year + @"\" + date.ToString("yyyy-MM") + @"\" +
             date.ToString("yyyy-MM-dd") + @"\" + this.Document.UID;
    }

    internal string GetTargetPngFileName(int frameNumber, int totalFrames) {
      Assertion.Assert(frameNumber >= 0, "frameNumber should be not negative.");
      Assertion.Assert(totalFrames >= 1, "totalFrames should be greater than zero.");
      Assertion.Assert(frameNumber < totalFrames, "totalFrames should be greater than frameNumber.");

      string fileNameWithoutExtension = this.FileName.TrimEnd(this.SourceFile.Extension.ToCharArray());

      string targetFileName = this.GetTargetFolderName() + @"\" + fileNameWithoutExtension;

      frameNumber++;    // Image index based on 1
      if (totalFrames < 100) {
        return String.Concat(targetFileName, ".", frameNumber.ToString("00"),
                             "_of_" + totalFrames.ToString("00") + ".png");
      } else {
        return String.Concat(targetFileName, ".", frameNumber.ToString("000"),
                             "_of_" + totalFrames.ToString("000") + ".png");
      }
    }

    internal virtual bool IsAlreadyDigitalized() {
      return DataServices.DocumentWasDigitalized(this.Document, this.DocumentImageType);
    }

    internal DocumentImage ConvertToDocumentImage() {
      string destinationFolder = this.GetTargetFolderName();
      string destinationFileName = FileServices.MoveFileTo(this.SourceFile, destinationFolder);

      AuditTrail.WriteOperation("SendImageToFinished", "MoveTiffFile",
                                new JsonRoot() { new JsonItem("Source", this.SourceFile.FullName),
                                                 new JsonItem("Destination", destinationFileName) } );
      this.ReadyToCreate = true;

      DocumentImage documentImage;
      if (this is RecordingCandidateImage) {
        documentImage = new RecordingImage((RecordingCandidateImage) this);
      } else {
        documentImage = new DocumentImage(this);
      }
      documentImage.Save();

      return documentImage;
    }

    #endregion Public methods

    #region Private methods

    private void AssertBaseImagingFolderExists() {
      string folderPath = this.GetTargetBaseFolderPath();

      var imagingFolder = ImagingFolder.TryParse(folderPath);

      if (imagingFolder != null) {
        this.BaseFolder = imagingFolder;
      } else {
        throw new LandDocumentationException(LandDocumentationException.Msg.ImagingFolderNotExists,
                                             folderPath);
      }
    }

    private string GetDocumentIDFromFileName() {
      return this.FileName.Substring(0, this.FileName.IndexOf('_'));
    }

    private void Initialize() {
      this.Document = RecordingDocument.Empty;
      this.DocumentImageType = DocumentImageType.Unknown;
      this.BaseFolder = ImagingFolder.Empty;
    }

    private bool IsFileNameValid() {
      string regex = "^RP\\d{2}[A-Z]{2}-\\d{2}[A-Z]{2}\\d{2}-[A-Z]{2}\\d{2}[A-Z|0-9]{2}_[AE].TIF$";

      return Regex.IsMatch(this.FileName.ToUpperInvariant(), regex);
    }

    private void LoadDocumentData() {
      string documentUID = this.GetDocumentIDFromFileName();

      var document = RecordingDocument.TryParse(documentUID);
      if (document != null) {
        this.Document = document;
      }
      this.DocumentImageType =
            (DocumentImageType) Convert.ToChar(this.FileName.Substring(this.FileName.Length - 5, 1));
    }

    #endregion Private methods

  }  // class CandidateImage

}  // namespace Empiria.Land.Documentation
