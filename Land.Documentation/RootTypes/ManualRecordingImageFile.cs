/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : ManualRecordingImageFile                       Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents an imaging item in Land Registration System.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Empiria.Land.Documentation {

  internal class ManualRecordingImageFile {

    #region Constructors and parsers

    private ManualRecordingImageFile(FileInfo sourceFile) {
      this.SourceFile = sourceFile;
      Initialize();
      LoadRecordingData();
    }

    static internal ManualRecordingImageFile Parse(FileInfo sourceFile) {
      Assertion.AssertObject(sourceFile, "sourceFile");

      return new ManualRecordingImageFile(sourceFile);
    }

    #endregion Constructors and parsers

    #region Public properties

    public FileInfo SourceFile {
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

    public int DocumentId {
      get;
      private set;
    }

    public string DocumentKey {
      get;
      private set;
    }

    public int RecordingBookId {
      get;
      private set;
    }

    public int OldRecordingId {
      get;
      private set;
    }

    public string RecordingNo {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    internal string GetTargetPngFileName(int frameNumber, int totalFrames) {
      string fileNameWithoutExtension = this.FileName.TrimEnd(this.SourceFile.Extension.ToCharArray());
      string targetFileName = this.GetTargetFolderName() + fileNameWithoutExtension;

      frameNumber++;    // Image index based on 1
      if (totalFrames < 100) {
        return String.Concat(targetFileName, ".", frameNumber.ToString("00"),
                             "_of_" + totalFrames.ToString("00") + ".png");
      } else {
        return String.Concat(targetFileName, ".", frameNumber.ToString("000"),
                             "_of_" + totalFrames.ToString("000") + ".png");
      }
    }

    internal string GetTargetFolderName() {
      string targetDirectory =
              @"E:\tlaxcala.imaging\books\" +
              this.RecordingBookId.ToString() + @"\" +
              this.RecordingBookId.ToString() + "." + this.RecordingNo + "." + this.DocumentKey + @"\";

      return targetDirectory;
    }

    internal bool IsAlreadyDigitalized() {
      string documentKey = this.FileName.Substring(0, this.FileName.IndexOf('_') + 1);

      return DataServices.DocumentWasDigitalized(documentKey);
    }

    public bool IsFileNameValid() {
      string regex = "^RP\\d{2}[A-Z]{2}-\\d{2}[A-Z]{2}\\d{2}-[A-Z]{2}\\d{2}[A-Z|0-9]{2}_[AE].TIF$";

      return Regex.IsMatch(this.FileName.ToUpperInvariant(), regex);
    }

    public bool IsFolderNameValid() {
      string[] directoryNameParts = this.FolderName.Split('_');

      if (directoryNameParts.Length != 3) {
        return false;
      }

      string districtName = directoryNameParts[0];
      string sectionNo = directoryNameParts[1];
      string bookNo = directoryNameParts[2];

      districtName = NormalizeDistrictName(districtName);

      if (districtName.Length == 0) {
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

    #endregion Public methods

    #region Private methods

    private void Initialize() {
      this.RecordingBookId = -1;
      this.DocumentId = -1;
      this.DocumentKey = String.Empty;
      this.OldRecordingId = -1;
      this.RecordingNo = String.Empty;
    }

    private void LoadRecordingData() {
      if (this.IsFileNameValid()) {
        LookupDocument();
      }
      if (this.IsFolderNameValid()) {
        LookupRecordingBook();
        LookupRecording();
      }
    }

    private void LookupDocument() {
      string documentKey = this.FileName.Substring(0, this.FileName.IndexOf('_'));

      this.DocumentId = DataServices.TryGetDocumentId(documentKey);
      if (this.DocumentId != -1) {
        this.DocumentKey = documentKey;
      }
    }

    private void LookupRecording() {
      var data = DataServices.TryGetOldRecording(this.RecordingBookId, this.DocumentId);

      if (data != null) {
        this.OldRecordingId = data.Item1;
        this.RecordingNo = data.Item2;
      }
    }

    private void LookupRecordingBook() {
      string[] directoryNameParts = this.FolderName.Split('_');

      string districtName = directoryNameParts[0];
      string sectionNo = directoryNameParts[1];
      string bookNo = directoryNameParts[2];

      districtName = NormalizeDistrictName(districtName);
      if (EmpiriaString.IsInteger(sectionNo)) {
        sectionNo = int.Parse(sectionNo).ToString("00");
      }
      if (EmpiriaString.IsInteger(bookNo)) {
        bookNo = int.Parse(bookNo).ToString("0000");
      }
      this.RecordingBookId = DataServices.TryGetRecordingBookId(districtName, sectionNo, bookNo);
    }

    static private string NormalizeDistrictName(string districtName) {
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
          return String.Empty;
      }
    }

    #endregion Private methods

  }  // class ManualRecordingImageFile

}  // namespace Empiria.Land.Documentation
