/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DocumentImage                                  Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Candidate images are processed images that not necessarily will become to a recording image,  *
*              because sometimes some of them are unreadable and must be replaced.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Text.RegularExpressions;

using Empiria.Documents;
using Empiria.Documents.IO;
using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Candidate images are processed images that not necessarily will become to a recording image,
  /// because sometimes some of them are unreadable and must be replaced.</summary>
  internal class CandidateImage {

    #region Fields

    static readonly string rootTargetFolder = ConfigurationData.GetString("DocumentImage.RootTargetFolder");

    #endregion Fields

    #region Constructors and parsers

    protected CandidateImage(FileInfo sourceFile) {
      Initialize();
      this.SourceFile = sourceFile;
      this.ChangeFileNameIfPossible();
      if (this.IsFileNameValid()) {
        LoadDocumentData();
      }
    }

    static public CandidateImage Parse(FileInfo sourceFile) {
      Assertion.Require(sourceFile, "sourceFile");
      Assertion.Require(sourceFile.Exists, $"File '{sourceFile.FullName}' does not exist.");

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

      return rootTargetFolder + @"\" + date.Year + @"\" + date.ToString("yyyy-MM") + @"\" +
             date.ToString("yyyy-MM-dd") + @"\" + this.Document.UID;
    }

    internal string GetTargetPngFileName(int frameNumber, int totalFrames) {
      Assertion.Require(frameNumber >= 0, "frameNumber should be not negative.");
      Assertion.Require(totalFrames >= 1, "totalFrames should be greater than zero.");
      Assertion.Require(frameNumber < totalFrames, "totalFrames should be greater than frameNumber.");

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

    internal void MoveToDestinationFolder() {
      string destinationFolder = this.GetTargetFolderName();

      FileServices.MoveFileTo(this.SourceFile, destinationFolder);

      this.ReadyToCreate = true;
    }

    internal DocumentImageSet ConvertToDocumentImage(string[] imagesHashCodes) {
      var imageSet = new DocumentImageSet(this, imagesHashCodes);
      imageSet.Save();

      if (imageSet.DocumentImageType == DocumentImageType.MainDocument) {
        imageSet.Document.Imaging.SetImageSet(imageSet);
      } else if (imageSet.DocumentImageType == DocumentImageType.Appendix) {
        imageSet.Document.Imaging.SetAuxiliarImageSet(imageSet);
      }

      return imageSet;
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

    private void ChangeFileNameIfPossible() {
      if (this.IsFileNameValid()) {
        return;
      }
      string currentFileName = this.FileName.ToUpperInvariant();
      string renamedFileName = String.Empty;

      string regex = "^RP\\d{2}[A-Z]{2}-\\d{2}[A-Z]{2}\\d{2}-[A-Z]{2}\\d{2}[A-Z|0-9]{2}-[AE].TIF$";
      if (Regex.IsMatch(currentFileName, regex)) {
        if (currentFileName.EndsWith("-A.TIF")) {
          renamedFileName = currentFileName.Replace("-A.TIF", "_A.TIF");
        } else if (currentFileName.EndsWith("-E.TIF")) {
          renamedFileName = currentFileName.Replace("-E.TIF", "_E.TIF");
        }
      }
      regex = "^RP\\d{2}[A-Z]{2}-\\d{2}[A-Z]{2}\\d{2}-[A-Z]{2}\\d{2}[A-Z|0-9]{2}.TIF$";
      if (Regex.IsMatch(currentFileName, regex)) {
        renamedFileName = currentFileName.Replace(".TIF", "_E.TIF");
      }

      if (renamedFileName.Length == 0) {
        return;
      }

      var newFileFullPath =
              this.SourceFile.FullName.ToUpperInvariant().Replace(currentFileName, renamedFileName);

      if (!File.Exists(newFileFullPath)) {
        this.SourceFile.MoveTo(newFileFullPath);
      }
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
