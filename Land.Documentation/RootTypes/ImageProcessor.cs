/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : ImageProcessor                                 Pattern  : Domain Service                      *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Document imaging processing service for Empiria Land System.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Empiria.Documents.IO;
using Empiria.Json;

namespace Empiria.Land.Documentation {

  /// <summary>Document maging processing service for Empiria Land System.</summary>
  static public class ImageProcessor {

    #region Fields

    static readonly int maxFilesToProcess = ConfigurationData.GetInteger("ImageProcessor.MaxFilesToProcess");
    static readonly string imageFileExtensions = "*.tif";

    #endregion Fields

    #region Public properties

    static private string _errorsFolderPath = null;
    static public string ErrorsFolderPath {
      get {
        if (_errorsFolderPath == null) {
          _errorsFolderPath = GetImagingFolder("ImageProcessor.ErrorsFolderPath");
        }
        return _errorsFolderPath;
      }
    }

    static private string _mainFolderPath = null;
    static public string MainFolderPath {
      get {
        if (_mainFolderPath == null) {
          _mainFolderPath = GetImagingFolder("ImageProcessor.MainFolderPath");
        }
        return _mainFolderPath;
      }
    }

    static private string _mainFolderPathByBook = null;
    static public string MainFolderPathByBook {
      get {
        if (_mainFolderPathByBook == null) {
          _mainFolderPathByBook = GetImagingFolder("ImageProcessor.MainFolderPathByBook");
        }
        return _mainFolderPathByBook;
      }
    }

    static private string _substitutionsFolderPath = null;
    static public string SubstitutionsFolderPath {
      get {
        if (_substitutionsFolderPath == null) {
          _substitutionsFolderPath = GetImagingFolder("ImageProcessor.SubstitutionsFolderPath");
        }
        return _substitutionsFolderPath;
      }
    }

    static private string _substitutionsFolderPathByBook = null;
    static public string SubstitutionsFolderPathByBook {
      get {
        if (_substitutionsFolderPathByBook == null) {
          _substitutionsFolderPathByBook = GetImagingFolder("ImageProcessor.SubstitutionsFolderPathByBook");
        }
        return _substitutionsFolderPathByBook;
      }
    }

    #endregion Public properties

    #region Public methods

    static public CandidateImage[] GetImagesToProcess() {
      CandidateImage[] files = ImageProcessor.GetImagesToProcess(ImageProcessor.SubstitutionsFolderPath, true);
      if (files.Length != 0) {
        return files;
      }

      ImageProcessor.CleanFolders(ImageProcessor.SubstitutionsFolderPathByBook);
      files = ImageProcessor.GetImagesToProcessUsingBookFolder(ImageProcessor.SubstitutionsFolderPathByBook, true);
      if (files.Length != 0) {
        return files;
      }

      files = ImageProcessor.GetImagesToProcess(ImageProcessor.MainFolderPath, false);
      if (files.Length != 0) {
        return files;
      }

      ImageProcessor.CleanFolders(ImageProcessor.MainFolderPathByBook);
      return ImageProcessor.GetImagesToProcessUsingBookFolder(ImageProcessor.MainFolderPathByBook, false);
    }

    private static void CleanFolders(string rootPath) {
      FileServices.DeleteEmptyDirectories(rootPath);
    }

    static public void ProcessTiffImage(CandidateImage candidateImage) {
      string resourceFolder = candidateImage.SourceFile.Directory.FullName;

      //// Open a Stream and decode a TIFF image
      //Stream imageStreamSource = new FileStream("tulipfarm.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
      //TiffBitmapDecoder decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
      //BitmapSource bitmapSource = decoder.Frames[0];
      //Bitmap myImage = new Bitmap(bitmapSource);
      //myImage.Source = bitmapSource;
      //myImage.Stretch = Stretch.None;
      //myImage.Margin = new Thickness(20);

      try {
        using (Image tiffImage = Image.FromFile(candidateImage.SourceFile.FullName)) {
          Empiria.Messaging.Publisher.Publish("ProcessTiffImage B");
          var frameDimensions = new FrameDimension(tiffImage.FrameDimensionsList[0]);
          Empiria.Messaging.Publisher.Publish("ProcessTiffImage C");
          // Gets the number of pages (frames) from the tiff image
          int totalFrames = tiffImage.GetFrameCount(frameDimensions);
          Empiria.Messaging.Publisher.Publish("ProcessTiffImage D");
          for (int frameIndex = 0; frameIndex < totalFrames; frameIndex++) {
            // Selects one frame at a time and save the bitmap as a gif but with png name
            tiffImage.SelectActiveFrame(frameDimensions, frameIndex);
            Empiria.Messaging.Publisher.Publish("ProcessTiffImage E");
            using (Bitmap bmp = new Bitmap(tiffImage)) {
              Empiria.Messaging.Publisher.Publish("ProcessTiffImage F");
              string pngImageFileName = candidateImage.GetTargetPngFileName(frameIndex, totalFrames);
              FileServices.AssureDirectoryForFile(pngImageFileName);
              bmp.Save(pngImageFileName, ImageFormat.Gif);
              Empiria.Messaging.Publisher.Publish("ProcessTiffImage G");
            }
          }  // for
        }   //using Image
        candidateImage.ConvertToDocumentImage();
      } catch (OutOfMemoryException exception) {
        ImageProcessor.SendCandidateImageToOutOfMemoryErrorsBin(candidateImage, exception);
      } catch (Exception exception) {
        ImageProcessor.SendCandidateImageToErrorsBin(candidateImage, exception);
      }
    }

    #endregion Public methods

    #region Private methods

    static private void AssertFileExists(string sourceFileName) {
      if (!File.Exists(sourceFileName)) {
        Assertion.AssertFail(new LandDocumentationException(LandDocumentationException.Msg.FileNotExists,
                                                            sourceFileName));
      }
    }

    static private CandidateImage[] GetImagesToProcess(string rootFolderPath, bool replaceDuplicated) {
      FileInfo[] filesInDirectory = FileServices.GetFiles(rootFolderPath, imageFileExtensions);

      var candidateImages =
                  new List<CandidateImage>(Math.Min(filesInDirectory.Length, maxFilesToProcess));
      foreach (FileInfo file in filesInDirectory) {
        var candidate = CandidateImage.Parse(file);
        try {
          candidate.AssertCanBeProcessed(replaceDuplicated);

          candidateImages.Add(candidate);
          if (candidateImages.Count > maxFilesToProcess) {
            break;
          }
        } catch (Exception exception) {
          SendCandidateImageToErrorsBin(candidate, exception);
        }
      } // foreach
      return candidateImages.ToArray();
    }

    static private CandidateImage[] GetImagesToProcessUsingBookFolder(string rootFolderPath,
                                                                      bool replaceDuplicated) {
      DirectoryInfo root = new DirectoryInfo(rootFolderPath);

      DirectoryInfo[] subdirectories = root.GetDirectories();
      var candidateImages = new List<CandidateImage>(maxFilesToProcess);
      foreach (DirectoryInfo subdirectory in subdirectories) {

        FileInfo[] filesInDirectory = FileServices.GetFiles(subdirectory.FullName, imageFileExtensions);
        foreach (FileInfo file in filesInDirectory) {
          var candidate = RecordingCandidateImage.Parse(file);
          try {
            candidate.AssertCanBeProcessed(replaceDuplicated);
            candidateImages.Add(candidate);
            if (candidateImages.Count > maxFilesToProcess) {
              break;
            }
          } catch (Exception exception) {
            SendCandidateImageToErrorsBin(candidate, exception);
          }
        } // foreach file;

      }  // foreach directory;

      return candidateImages.ToArray();
    }

    static private string GetImagingFolder(string folderName) {
      string path = ConfigurationData.GetString(folderName);

      path = path.TrimEnd('\\');
      if (Directory.Exists(path)) {
        return path;
      }
      throw new LandDocumentationException(LandDocumentationException.Msg.ImagingFolderNotExists,
                                           folderName, path);
    }

    static private string ReplaceImagingFolder(string folderPath, string replacedPath) {
      if (folderPath.StartsWith(ImageProcessor.ErrorsFolderPath)) {
        return folderPath.Replace(ImageProcessor.ErrorsFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(ImageProcessor.MainFolderPath)) {
        return folderPath.Replace(ImageProcessor.MainFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(ImageProcessor.MainFolderPathByBook)) {
        return folderPath.Replace(ImageProcessor.MainFolderPathByBook, replacedPath);
      }
      if (folderPath.StartsWith(ImageProcessor.SubstitutionsFolderPath)) {
        return folderPath.Replace(ImageProcessor.SubstitutionsFolderPath, replacedPath);
      }
      if (folderPath.StartsWith(ImageProcessor.SubstitutionsFolderPathByBook)) {
        return folderPath.Replace(ImageProcessor.SubstitutionsFolderPathByBook, replacedPath);
      }
      throw Assertion.AssertNoReachThisCode();
    }

    static private void SendCandidateImageToErrorsBin(CandidateImage image, Exception exception) {
      Assertion.AssertObject(image, "imageFile");
      Assertion.AssertObject(exception, "exception");

      string destinationFileName = String.Empty;
      string sourceFile = image.SourceFile.FullName;
      try {
        var destinationFolder = ReplaceImagingFolder(image.SourceFile.Directory.FullName,
                                                     ImageProcessor.ErrorsFolderPath);
        destinationFileName = FileServices.MoveFileTo(image.SourceFile, destinationFolder);
        FileAuditTrail.WriteOperation("SendImageToErrorsBin", "MoveFile",
                                      new JsonObject() { new JsonItem("Source", sourceFile),
                                                     new JsonItem("Destination", destinationFileName),
                                                     new JsonItem("Reason", exception)});
      } catch (Exception e) {
        FileAuditTrail.WriteException("SendImageToErrorsBin", "MoveFile",
                                      new JsonObject() { new JsonItem("Source", sourceFile),
                                                     new JsonItem("Destination", destinationFileName),
                                                     new JsonItem("Reason", exception),
                                                     new JsonItem("OperationException", e)});
      }
    }

    static private void SendCandidateImageToOutOfMemoryErrorsBin(CandidateImage image, OutOfMemoryException exception) {
      Assertion.AssertObject(image, "image");
      Assertion.AssertObject(exception, "exception");

      string destinationFileName = String.Empty;
      string sourceFile = image.SourceFile.FullName;
      try {
        var destinationFolder = ReplaceImagingFolder(image.SourceFile.Directory.FullName,
                                                     ImageProcessor.ErrorsFolderPath + @"\\out.of.memory");
        destinationFileName = FileServices.MoveFileTo(image.SourceFile, destinationFolder);
        FileAuditTrail.WriteOperation("SendImageToOutOfMemoryErrorsBin", "MoveFile",
                                      new JsonObject() { new JsonItem("Source", sourceFile),
                                                         new JsonItem("Destination", destinationFileName),
                                                         new JsonItem("Reason", exception)});
      } catch (Exception e) {
        FileAuditTrail.WriteException("SendImageToOutOfMemoryErrorsBin", "MoveFile",
                                      new JsonObject() { new JsonItem("Source", sourceFile),
                                                         new JsonItem("Destination", destinationFileName),
                                                         new JsonItem("Reason", exception),
                                                         new JsonItem("OperationException", e)});
      }
    }

    #endregion Private methods

  } // class ImageProcessor

} // namespace Empiria.Land.Documentation
