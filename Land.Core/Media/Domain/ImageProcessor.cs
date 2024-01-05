/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : ImageProcessor                                 Pattern  : Domain Service                      *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : It is the responsible of the image processing service.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Documents.IO;

namespace Empiria.Land.Media {

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

    //static internal CandidateImage[] GetImagesToProcess() {
    //  CandidateImage[] files = ImageProcessor.GetImagesToProcess(ImageProcessor.SubstitutionsFolderPath, true);
    //  if (files.Length != 0) {
    //    return files;
    //  }

    //  return ImageProcessor.GetImagesToProcess(ImageProcessor.MainFolderPath, false);
    //}

    private static void CleanFolders(string rootPath) {
      FileServices.DeleteEmptyDirectories(rootPath);
    }

    #endregion Public methods

    #region Private methods

    static private void AssertFileExists(string sourceFileName) {
      if (!File.Exists(sourceFileName)) {
        throw new LandMediaException(LandMediaException.Msg.FileNotExists,
                                     sourceFileName);
      }
    }


    static void DeleteSourceFolderIfEmpty(string sourceFolderToDelete) {
      if (sourceFolderToDelete != MainFolderPath) {
        FileServices.DeleteWhenIsEmpty(sourceFolderToDelete);
      }
    }

    internal static FileInfo[] GetImagesToProcess() {
      throw new NotImplementedException();
    }

    internal static void ProcessMediaFile(FileInfo image) {
      throw new NotImplementedException();
    }


    //static private CandidateImage[] GetImagesToProcess(string rootFolderPath, bool replaceDuplicated) {
    //  FileInfo[] filesInDirectory = FileServices.GetFiles(rootFolderPath);

    //  MediaFilesProcessorAuditTrail.LogText("Se leyeron " + filesInDirectory.Length +
    //                                        " archivos del directorio " + rootFolderPath);
    //  MediaFilesProcessorAuditTrail.LogText("Revisando y descartando archivos previo al procesamiento ...\n");
    //  var candidateImages =
    //              new List<CandidateImage>(Math.Min(filesInDirectory.Length, maxFilesToProcess));
    //  foreach (FileInfo file in filesInDirectory) {
    //    var candidate = CandidateImage.Parse(file);
    //    try {
    //      candidate.AssertCanBeProcessed(replaceDuplicated);

    //      candidateImages.Add(candidate);
    //      if (candidateImages.Count > maxFilesToProcess) {
    //        break;
    //      }
    //    } catch (Exception exception) {
    //      SendCandidateImageToErrorsBin(candidate, exception);
    //    }
    //  } // foreach
    //  return candidateImages.ToArray();
    //}


    static private string GetImagingFolder(string folderName) {
      string path = ConfigurationData.GetString(folderName);

      path = path.TrimEnd('\\');

      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
        MediaFilesProcessorAuditTrail.LogText("MSG: Se creó el directorio '" + path + "'");
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

      throw Assertion.EnsureNoReachThisCode(folderPath + " doesn't start with a recognized path pattern.");
    }


    //static private void SendCandidateImageToErrorsBin(CandidateImage image, Exception exception) {
    //  string sourceFolderPath = image.SourceFile.DirectoryName;
    //  try {
    //    var destinationFolder = ReplaceImagingFolder(sourceFolderPath,
    //                                                 ImageProcessor.ErrorsFolderPath + GetSpecialErrorFolder(exception));

    //    FileServices.MoveFileTo(image.SourceFile, destinationFolder);

    //    MediaFilesProcessorAuditTrail.LogException(image, exception, GetShortExceptionMessage(exception),
    //                                "ERR: " + image.SourceFile.Name +
    //                                " se envió a la bandeja de errores debido a:\n\t" + exception.Message + "\n\t" +
    //                                "Origen:  " + sourceFolderPath + "\n\t" +
    //                                "Destino: " + destinationFolder);

    //    DeleteSourceFolderIfEmpty(sourceFolderPath);
    //  } catch (Exception e) {
    //    MediaFilesProcessorAuditTrail.LogException(image, exception, GetShortExceptionMessage(exception),
    //                                "ERR: " + image.SourceFile.Name + " no se pudo procesar debido a:\n\t" +
    //                                exception.Message + "\n\t" +
    //                                "Tampoco se pudo enviar a la bandeja de errores por:\n\t" + e.Message + "\n\t" +
    //                                "Origen: " + sourceFolderPath);
    //  }
    //}

    private static string GetShortExceptionMessage(Exception exception) {
      if (!(exception is LandMediaException)) {
        return exception.Message;
      }
      string exceptionTag = ((LandMediaException) exception).ExceptionTag;

      if (exceptionTag == LandMediaException.Msg.FileNameBadFormed.ToString()) {
        return "Archivo mal nombrado";
      } else if (exceptionTag == LandMediaException.Msg.DocumentAlreadyDigitalized.ToString()) {
        return "Ya fue digitalizado";
      } else if (exceptionTag == LandMediaException.Msg.DocumentForFileNameNotFound.ToString()) {
        return "El documento registral no existe";
      } else {
        return exception.Message;
      }
    }

    private static string GetSpecialErrorFolder(Exception exception) {
      if (!(exception is LandMediaException)) {
        return @"\otros.errores";
      }
      string exceptionTag = ((LandMediaException) exception).ExceptionTag;

      if (exceptionTag == LandMediaException.Msg.FileNameBadFormed.ToString()) {
        return @"\mal.nombrados";
      } else if (exceptionTag == LandMediaException.Msg.DocumentAlreadyDigitalized.ToString()) {
        return @"\duplicados";
      } else if (exceptionTag == LandMediaException.Msg.DocumentForFileNameNotFound.ToString()) {
        return @"\sin.documento";
      } else {
        return @"\otros.errores";
      }
    }

    #endregion Private methods

  } // class ImageProcessor

} // namespace Empiria.Land.Documentation
