/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : LandImaging                                    Pattern  : Domain Service                      *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Imaging service for Empiria Land System.                                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Empiria.Documents.IO;
using Empiria.Json;
using Empiria.Security;

namespace Empiria.Land.Documentation {

  /// <summary>Imaging service for Empiria Land System.</summary>
  static public class LandImaging {

    static readonly int maxFilesToProcess = ConfigurationData.GetInteger("LandImaging.MaxFilesToProcess");
    static readonly string imageFileExtensions = "*.tif";

    #region Public properties

    static private string _errorsFolderPath = null;
    static public string ErrorsFolderPath {
      get {
        if (_errorsFolderPath == null) {
          _errorsFolderPath = GetImagingFolder("LandImaging.ErrorsFolderPath");
        }
        return _errorsFolderPath;
      }
    }

    static private string _mainFolderPath = null;
    static public string MainFolderPath {
      get {
        if (_mainFolderPath == null) {
          _mainFolderPath = GetImagingFolder("LandImaging.MainFolderPath");
        }
        return _mainFolderPath;
      }
    }

    static private string _mainFolderPathByBook = null;
    static public string MainFolderPathByBook {
      get {
        if (_mainFolderPathByBook == null) {
          _mainFolderPathByBook = GetImagingFolder("LandImaging.MainFolderPathByBook");
        }
        return _mainFolderPathByBook;
      }
    }

    static private string _substitutionsFolderPath = null;
    static public string SubstitutionsFolderPath {
      get {
        if (_substitutionsFolderPath == null) {
          _substitutionsFolderPath = GetImagingFolder("LandImaging.SubstitutionsFolderPath");
        }
        return _substitutionsFolderPath;
      }
    }

    static private string _substitutionsFolderPathByBook = null;
    static public string SubstitutionsFolderPathByBook {
      get {
        if (_substitutionsFolderPathByBook == null) {
          _substitutionsFolderPathByBook = GetImagingFolder("LandImaging.SubstitutionsFolderPathByBook");
        }
        return _substitutionsFolderPathByBook;
      }
    }

    #endregion Public properties

    #region Public methods

    static public string[] GetFilesToProcess() {
      string[] files = LandImaging.GetFilesToProcess(LandImaging.SubstitutionsFolderPath, true);
      if (files.Length != 0) {
        return files;
      }

      files = LandImaging.GetFilesToProcessUsingBookFolder(LandImaging.SubstitutionsFolderPathByBook, true);
      if (files.Length != 0) {
        return files;
      }

      files = LandImaging.GetFilesToProcess(LandImaging.MainFolderPath, false);
      if (files.Length != 0) {
        return files;
      }

      files = LandImaging.GetFilesToProcessUsingBookFolder(LandImaging.MainFolderPathByBook, false);

      return files;
    }

    static public string GetTargetPngFileName(string sourceFileName, int frameNumber, int totalFrames) {
      Assertion.AssertObject(sourceFileName, "sourceFileName");
      Assertion.Assert(frameNumber >= 0, "frameNumber should be not negative.");
      Assertion.Assert(totalFrames >= 1, "totalFrames should be greater than zero.");
      Assertion.Assert(frameNumber < totalFrames, "totalFrames should be greater than frameNumber.");
      AssertFileExists(sourceFileName);

      var documentFile = ManualRecordingImageFile.Parse(new FileInfo(sourceFileName));

      var fileName = documentFile.GetTargetPngFileName(frameNumber, totalFrames);
      FileServices.AssureDirectoryForFile(fileName);

      return fileName;
    }

    static public void SendFileToErrorsBin(string sourceFileName, Exception exception) {
      Assertion.AssertObject(sourceFileName, "sourceFileName");
      Assertion.AssertObject(exception, "exception");
      AssertFileExists(sourceFileName);

      string destinationFileName = String.Empty;
      try {
        FileInfo file = new FileInfo(sourceFileName);

        var destinationFolder = ReplaceImagingFolder(file.Directory.FullName, LandImaging.ErrorsFolderPath);
        destinationFileName = FileServices.MoveFileTo(file, destinationFolder);
        AuditTrail.WriteOperation("SendFileToErrorsBin", "MoveFile",
                                  new JsonRoot() { new JsonItem("Source", sourceFileName),
                                                   new JsonItem("Destination", destinationFileName),
                                                   new JsonItem("Reason", exception)});
      } catch (Exception e) {
        AuditTrail.WriteException("SendFileToErrorsBin", "MoveFile",
                                  new JsonRoot() { new JsonItem("Source", sourceFileName),
                                                   new JsonItem("Destination", destinationFileName),
                                                   new JsonItem("Reason", exception),
                                                   new JsonItem("OperationException", e)});
      }
    }

    static public void SendToFinished(string sourceFileName) {
      Assertion.AssertObject(sourceFileName, "sourceFileName");
      AssertFileExists(sourceFileName);

      ManualRecordingImageFile documentFile = ManualRecordingImageFile.Parse(new FileInfo(sourceFileName));

      string destinationFolder = documentFile.GetTargetFolderName();
      string destinationFileName = FileServices.MoveFileTo(documentFile.SourceFile,
                                                           destinationFolder);

      //var imagingItem = ImagingItem.Create(ManualRecordingImageFile data, sourceFileName)
      AuditTrail.WriteOperation("SendToFinished", "MoveTiffFile",
                                new JsonRoot() { new JsonItem("Source", sourceFileName),
                                                 new JsonItem("Destination", destinationFileName)});
    }

    #endregion Public methods

    #region Private methods

    static private void AssertFileExists(string sourceFileName) {
      if (!File.Exists(sourceFileName)) {
        Assertion.AssertFail(new LandDocumentationException(LandDocumentationException.Msg.FileNotExists,
                                                            sourceFileName));
      }
    }

    static private bool CheckFileToProcess(string fullFileName, bool replaceDuplicated) {
      var documentFile = ManualRecordingImageFile.Parse(new FileInfo(fullFileName));

      if (!documentFile.IsFileNameValid()) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                                              LandDocumentationException.Msg.FileNameBadFormed,
                                              documentFile.FileName));
        return false;
      }
      if (documentFile.DocumentId == -1) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                                          LandDocumentationException.Msg.DocumentForFileNameNotFound,
                                          documentFile.FileName));
        return false;
      }
      if (!replaceDuplicated && documentFile.IsAlreadyDigitalized()) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                                              LandDocumentationException.Msg.DocumentAlreadyDigitalized,
                                              documentFile.FileName));
        return false;
      }
      return true;
    }

    static private bool CheckFileToProcessUsingBookFolder(string fullFileName, bool replaceDuplicated) {
      var documentFile = ManualRecordingImageFile.Parse(new FileInfo(fullFileName));

      if (!documentFile.IsFolderNameValid()) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                                              LandDocumentationException.Msg.FolderNameBadFormed,
                                              documentFile.FolderName));
        return false;
      }

      if (documentFile.RecordingBookId == -1) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                                          LandDocumentationException.Msg.RecordingBookForFolderNameNotFound,
                                          documentFile.FolderName));
        return false;
      }
      if (!documentFile.IsFileNameValid()) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                            LandDocumentationException.Msg.FileNameBadFormed, documentFile.FileName));
        return false;
      }
      if (documentFile.DocumentId == -1) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                            LandDocumentationException.Msg.DocumentForFileNameNotFound, documentFile.FileName));
        return false;
      }
      if (!replaceDuplicated && documentFile.IsAlreadyDigitalized()) {
        SendFileToErrorsBin(fullFileName, new LandDocumentationException(
                            LandDocumentationException.Msg.DocumentAlreadyDigitalized, documentFile.FileName));
        return false;
      }
      return true;
    }

    static private string[] GetFilesToProcess(string rootFolder, bool replaceDuplicated) {
      string[] fileNames = FileServices.GetFileNames(rootFolder, imageFileExtensions);

      var filesToReturn = new List<string>(Math.Min(fileNames.Length, maxFilesToProcess));
      foreach (string fileName in fileNames) {
        if (!CheckFileToProcess(fileName, replaceDuplicated)) {
          continue;
        }
        filesToReturn.Add(fileName);
        if (filesToReturn.Count > maxFilesToProcess) {
          break;
        }
      }
      return filesToReturn.ToArray();
    }

    static private string[] GetFilesToProcessUsingBookFolder(string rootFolder, bool replaceDuplicated) {
      DirectoryInfo root = new DirectoryInfo(rootFolder);

      DirectoryInfo[] subdirectories = root.GetDirectories();
      var filesToReturn = new List<string>(maxFilesToProcess);
      foreach (DirectoryInfo subdirectory in subdirectories) {
        string[] fileNames = FileServices.GetFileNames(subdirectory.FullName, imageFileExtensions);
        foreach (string fileName in fileNames) {
          if (!CheckFileToProcessUsingBookFolder(fileName, replaceDuplicated)) {
            continue;
          }
          filesToReturn.Add(fileName);
          if (filesToReturn.Count > maxFilesToProcess) {
            break;
          }
        }
      }
      return filesToReturn.ToArray();
    }

    static private string GetImagingFolder(string folderName) {
      string path = ConfigurationData.GetString(folderName);

      path = path.TrimEnd('\\') + @"\";
      if (Directory.Exists(path)) {
        return path;
      }
      throw new LandDocumentationException(LandDocumentationException.Msg.ImagingFolderNotExists,
                                           folderName, path);
    }

    static private string ReplaceImagingFolder(string folderPath, string replacedPath) {
      if (folderPath.StartsWith(LandImaging.ErrorsFolderPath)) {
        return folderPath.Replace(LandImaging.ErrorsFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(LandImaging.MainFolderPath)) {
        return folderPath.Replace(LandImaging.MainFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(LandImaging.MainFolderPathByBook)) {
        return folderPath.Replace(LandImaging.MainFolderPathByBook, replacedPath);
      }
      if (folderPath.StartsWith(LandImaging.SubstitutionsFolderPath)) {
        return folderPath.Replace(LandImaging.SubstitutionsFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(LandImaging.SubstitutionsFolderPathByBook)) {
        return folderPath.Replace(LandImaging.SubstitutionsFolderPathByBook, replacedPath);
      }
      throw Assertion.AssertNoReachThisCode();
    }

    #endregion Private methods

  } // class LandImaging

} // namespace Empiria.Land.Documentation
