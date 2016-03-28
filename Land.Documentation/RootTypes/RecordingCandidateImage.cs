/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : RecordingCandidateImage                        Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording book/recording document image.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.IO;
using System.Text.RegularExpressions;

using Empiria.Documents;
using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Represents a recording book/recording document image.</summary>
  public class RecordingCandidateImage : CandidateImage {

    #region Fields

    static readonly string rootTargetFolder = ConfigurationData.GetString("RecordingImage.RootTargetFolder");

    #endregion Fields

    #region Constructors and parsers

    private RecordingCandidateImage(FileInfo sourceFile) : base(sourceFile) {
      Initialize();
      if (this.IsFolderNameValid()) {
        LookupRecordingBook();
        LookupRecording();
      }
    }

    static public new RecordingCandidateImage Parse(FileInfo sourceFile) {
      Assertion.AssertObject(sourceFile, "sourceFile");
      Assertion.Assert(sourceFile.Exists, "File '{0}' does not exist.", sourceFile.FullName);

      return new RecordingCandidateImage(sourceFile);
    }

    #endregion Constructors and parsers

    #region Public properties

    public RecordingBook RecordingBook {
      get;
      private set;
    }

    public Recording Recording {
      get;
      private set;
    }

    public string RecordingNo {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    internal override void AssertCanBeProcessed(bool replaceDuplicated) {
      if (!this.IsFolderNameValid()) {
        throw new LandDocumentationException(LandDocumentationException.Msg.FolderNameBadFormed,
                                             this.FolderName);
      }

      if (this.RecordingBook.IsEmptyInstance) {
        throw new LandDocumentationException(LandDocumentationException.Msg.RecordingBookForFolderNameNotFound,
                                             this.FolderName);
      }
      base.AssertCanBeProcessed(replaceDuplicated);
    }

    internal override string GetTargetBaseFolderPath() {
      return rootTargetFolder;
    }

    internal override string GetTargetFolderName() {
      // E:\images\boooks\12312\12312.022.RP02WL-91PK57-TG62K5\RP02WL-91PK57-TG62K5_E.01_of_01.png
      // where 12312 is RecordingBookId and 022 is RecordingNo
      return this.GetTargetBaseFolderPath() + @"\" + this.RecordingBook.Id.ToString() + @"\" +
             this.RecordingBook.Id.ToString() + "." + this.RecordingNo + "." + this.Document.UID;
    }

    internal override bool IsAlreadyDigitalized() {
      return DataServices.DocumentWasDigitalized(this.Document, this.RecordingBook,
                                                 this.RecordingNo, this.DocumentImageType);
    }

    #endregion Public methods

    #region Private methods

    private void Initialize() {
      this.RecordingBook = RecordingBook.Empty;
      this.RecordingNo = String.Empty;
      this.Recording = Recording.Empty;
    }

    private bool IsFolderNameValid() {
      string[] directoryNameParts = this.FolderName.Split('_');

      if (directoryNameParts.Length != 3) {
        return false;
      }

      string districtName = directoryNameParts[0];
      string sectionNo = directoryNameParts[1];
      string bookNo = directoryNameParts[2];

      districtName = TryNormalizeDistrictName(districtName);

      if (districtName == null) {
        return false;
      }
      if (!EmpiriaString.IsInteger(sectionNo)) {
        return false;
      }
      if (!EmpiriaString.IsInteger(bookNo)) {
        return false;
      }
      return true;
    }

    private void LookupRecording() {
      string value = DataServices.TryGetOldRecordingNo(this.RecordingBook, this.Document);
      if (value != null) {
        this.RecordingNo = value;
      }
    }

    private void LookupRecordingBook() {
      string[] directoryNameParts = this.FolderName.Split('_');

      string districtName = directoryNameParts[0];
      string sectionNo = directoryNameParts[1];
      string bookNo = directoryNameParts[2];

      districtName = TryNormalizeDistrictName(districtName);
      Assertion.AssertObject(districtName, "districtName");
      if (EmpiriaString.IsInteger(sectionNo)) {
        sectionNo = int.Parse(sectionNo).ToString("00");
      }
      if (EmpiriaString.IsInteger(bookNo)) {
        bookNo = int.Parse(bookNo).ToString("0000");
      }
      this.RecordingBook = DataServices.TryGetRecordingBook(districtName, sectionNo, bookNo);
    }

    static private string TryNormalizeDistrictName(string districtName) {
      switch (districtName.ToUpperInvariant()) {
        case "CUAUHTEMOC":
        case "CUAHUTEMOC":
          return "Cuauhtémoc";
        case "HIDALGO":
          return "Hidalgo";
        case "JUAREZ":
          return "Juárez";
        case "LYU":
          return "Lardizábal y Uribe";
        case "MORELOS":
          return "Morelos";
        case "OCAMPO":
          return "Ocampo";
        case "XICOHTENCATL":
          return "Xicohténcatl";
        case "ZARAGOZA":
          return "Zaragoza";
        default:
          return null;
      }
    }

    #endregion Private methods

  }  // class RecordingCandidateImage

}  // namespace Empiria.Land.Documentation
