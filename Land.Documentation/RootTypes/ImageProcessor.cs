/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : ImageProcessor                                 Pattern  : Domain Service                      *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : It is the responsible of the image processing service.                                        *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Empiria.Documents.IO;
using Empiria.Security;

namespace Empiria.Land.Documentation {

  /// <summary>It is the responsible of the image processing service.</summary>
  static public class ImageProcessor {

    #region Fields

    static readonly int maxFilesToProcess = ConfigurationData.GetInteger("ImageProcessor.MaxFilesToProcess");

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

    static private string _substitutionsFolderPath = null;
    static public string SubstitutionsFolderPath {
      get {
        if (_substitutionsFolderPath == null) {
          _substitutionsFolderPath = GetImagingFolder("ImageProcessor.SubstitutionsFolderPath");
        }
        return _substitutionsFolderPath;
      }
    }

    #endregion Public properties

    #region Public methods

    static internal CandidateImage[] GetImagesToProcess() {
      CandidateImage[] files = ImageProcessor.GetImagesToProcess(ImageProcessor.SubstitutionsFolderPath, true);
      if (files.Length != 0) {
        return files;
      }

      return ImageProcessor.GetImagesToProcess(ImageProcessor.MainFolderPath, false);
    }

    private static void CleanFolders(string rootPath) {
      FileServices.DeleteEmptyDirectories(rootPath);
    }

    static internal void ProcessTiffImage(CandidateImage candidateImage) {
      int totalFrames = 0;
      string[] securityHashCodes = new string[0];

      try {
        using (Image tiffImage = Image.FromFile(candidateImage.SourceFile.FullName)) {

          var frameDimensions = new FrameDimension(tiffImage.FrameDimensionsList[0]);

          // Gets the number of pages (frames) from the tiff image
          totalFrames = tiffImage.GetFrameCount(frameDimensions);
          securityHashCodes = new string[totalFrames];

          for (int frameIndex = 0; frameIndex < totalFrames; frameIndex++) {
            // Selects one frame at a time and save the bitmap as a gif but with png name
            tiffImage.SelectActiveFrame(frameDimensions, frameIndex);

            using (Bitmap bmp = new Bitmap(tiffImage)) {
              string pngImageFileName = candidateImage.GetTargetPngFileName(frameIndex, totalFrames);
              FileServices.AssureDirectoryForFile(pngImageFileName);
              bmp.Save(pngImageFileName, ImageFormat.Gif);
              string hashCode = Cryptographer.CreateHashCode(File.ReadAllBytes(pngImageFileName),
                                                             pngImageFileName);
              securityHashCodes[frameIndex] =
                         hashCode.Substring(0, totalFrames <= 20 ? 32 : (totalFrames <= 100 ? 16 : 8));
            }

          }  // for

        }   //using Image

        string sourceFolderPath = candidateImage.SourceFile.DirectoryName;

        candidateImage.MoveToDestinationFolder();

        DocumentImageSet documentImageSet = candidateImage.ConvertToDocumentImage(securityHashCodes);

        FileAuditTrail.LogOperation(documentImageSet, "Procesada correctamente",
                                    "OK:  " + candidateImage.SourceFile.Name + " se procesó correctamente.\n\t" +
                                    "Origen:  " + sourceFolderPath + "\n\t" +
                                    "Destino: " + documentImageSet.FullPath);

        DeleteSourceFolderIfEmpty(sourceFolderPath);
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


    static void DeleteSourceFolderIfEmpty(string sourceFolderToDelete) {
      if (sourceFolderToDelete != MainFolderPath) {
        FileServices.DeleteWhenIsEmpty(sourceFolderToDelete);
      }
    }


    static private CandidateImage[] GetImagesToProcess(string rootFolderPath, bool replaceDuplicated) {
      FileInfo[] filesInDirectory = FileServices.GetFiles(rootFolderPath);

      FileAuditTrail.LogText("Se leyeron " + filesInDirectory.Length +
                             " archivos del directorio " + rootFolderPath);
      FileAuditTrail.LogText("Revisando y descartando archivos previo al procesamiento ...\n");
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

    static private string GetImagingFolder(string folderName) {
      string path = ConfigurationData.GetString(folderName);

      path = path.TrimEnd('\\');

      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
        FileAuditTrail.LogText("MSG: Se creó el directorio '" + path + "'");
      }
      return path;
    }

    static private string ReplaceImagingFolder(string folderPath, string replacedPath) {
      if (folderPath.StartsWith(ImageProcessor.ErrorsFolderPath)) {
        return folderPath.Replace(ImageProcessor.ErrorsFolderPath, replacedPath);
      }

      if (folderPath.StartsWith(ImageProcessor.MainFolderPath)) {
        return folderPath.Replace(ImageProcessor.MainFolderPath, replacedPath);
      }

      if (folderPath.StartsWith(ImageProcessor.SubstitutionsFolderPath)) {
        return folderPath.Replace(ImageProcessor.SubstitutionsFolderPath, replacedPath);
      }

      throw Assertion.AssertNoReachThisCode(folderPath + " doesn't start with a recognized path pattern.");
    }

    static private void SendCandidateImageToErrorsBin(CandidateImage image, Exception exception) {
      string sourceFolderPath = image.SourceFile.DirectoryName;
      try {
        var destinationFolder = ReplaceImagingFolder(sourceFolderPath,
                                                     ImageProcessor.ErrorsFolderPath + GetSpecialErrorFolder(exception));

        FileServices.MoveFileTo(image.SourceFile, destinationFolder);

        FileAuditTrail.LogException(image, exception, GetShortExceptionMessage(exception),
                                    "ERR: " + image.SourceFile.Name +
                                    " se envió a la bandeja de errores debido a:\n\t" + exception.Message + "\n\t" +
                                    "Origen:  " + sourceFolderPath + "\n\t" +
                                    "Destino: " + destinationFolder);

        DeleteSourceFolderIfEmpty(sourceFolderPath);
      } catch (Exception e) {
        FileAuditTrail.LogException(image, exception, GetShortExceptionMessage(exception),
                                    "ERR: " + image.SourceFile.Name + " no se pudo procesar debido a:\n\t" +
                                    exception.Message + "\n\t" +
                                    "Tampoco se pudo enviar a la bandeja de errores por:\n\t" + e.Message + "\n\t" +
                                    "Origen: " + sourceFolderPath);
      }
    }

    static private void SendCandidateImageToOutOfMemoryErrorsBin(CandidateImage image, OutOfMemoryException exception) {
      string sourceFolderPath = image.SourceFile.DirectoryName;
      try {
        var destinationFolder = ReplaceImagingFolder(sourceFolderPath,
                                                     ImageProcessor.ErrorsFolderPath + @"\\out.of.memory");

        FileServices.MoveFileTo(image.SourceFile, destinationFolder);

        FileAuditTrail.LogException(image, exception, "Problema de memoria",
                                    "ERR: " + image.SourceFile.Name +
                                    " se envió a la bandeja de errores de memoria para su procesamiento posterior.\n\t" +
                                    "Origen:  " + sourceFolderPath + "\n\t" +
                                    "Destino: " + destinationFolder);

        DeleteSourceFolderIfEmpty(sourceFolderPath);
      } catch (Exception e) {
        FileAuditTrail.LogException(image, e, "Problema de memoria. No se pudo enviar a la bandeja de errores.",
                                    "ERR: " + image.SourceFile.Name +
                                    " no se pudo procesar debido a problemas de memoria, pero no se pudo enviar a" +
                                    " la bandeja de problemas de memoria debido a:\n\t" + e.Message + "\n\t" +
                                    "Origen: " + sourceFolderPath);
      }
    }

    private static string GetShortExceptionMessage(Exception exception) {
      if (!(exception is LandDocumentationException)) {
        return exception.Message;
      }
      string exceptionTag = ((LandDocumentationException) exception).ExceptionTag;

      if (exceptionTag == LandDocumentationException.Msg.FileNameBadFormed.ToString()) {
        return "Archivo mal nombrado";
      } else if (exceptionTag == LandDocumentationException.Msg.DocumentAlreadyDigitalized.ToString()) {
        return "Ya fue digitalizado";
      } else if (exceptionTag == LandDocumentationException.Msg.DocumentForFileNameNotFound.ToString()) {
        return "El documento registral no existe";
      } else {
        return exception.Message;
      }
    }

    private static string GetSpecialErrorFolder(Exception exception) {
      if (!(exception is LandDocumentationException)) {
        return @"\otros.errores";
      }
      string exceptionTag = ((LandDocumentationException) exception).ExceptionTag;

      if (exceptionTag == LandDocumentationException.Msg.FileNameBadFormed.ToString()) {
        return @"\mal.nombrados";
      } else if (exceptionTag == LandDocumentationException.Msg.DocumentAlreadyDigitalized.ToString()) {
        return @"\duplicados";
      } else if (exceptionTag == LandDocumentationException.Msg.DocumentForFileNameNotFound.ToString()) {
        return @"\sin.documento";
      } else {
        return @"\otros.errores";
      }
    }

    #endregion Private methods

  } // class ImageProcessor

} // namespace Empiria.Land.Documentation
