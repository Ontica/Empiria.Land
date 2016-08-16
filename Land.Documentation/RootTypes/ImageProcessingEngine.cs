/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Registration System          *
*  Namespace : Empiria.Land.Documentation                       Assembly : Empiria.Land.Documentation        *
*  Type      : ImageProcessingEngine                            Pattern  : Singleton Service                 *
*  Version   : 2.1                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Performs image processing services.                                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Runtime.Remoting.Messaging;

using Empiria.Documents.IO;

namespace Empiria.Land.Documentation {

  public class ImageProcessingEngine {

    #region Delegates

    private delegate int ProcessImagesDelegate();

    #endregion

    #region Fields

    static private readonly ImageProcessingEngine instance = new ImageProcessingEngine();  // singleton element

    static private string logText = String.Empty;
    static private string errorLogText = String.Empty;

    private readonly string logFilePath = ConfigurationData.GetString("ImageProcessor.LogFilesPath");

    private IAsyncResult asyncResult = null;

    private bool isRunning = false;   // semaphore
    private int totalJobs = 0;        // total jobs count;
    private int completedJobs = 0;    // completed jobs count;

    #endregion Fields

    #region Constructors and parsers

    private ImageProcessingEngine() {
      // Singleton pattern needs private constructor
    }

    static public ImageProcessingEngine GetInstance() {
      return instance;
    }

    #endregion Constructors and parsers

    #region Public methods

    public bool IsRunning {
      get {
        return isRunning;
      }
    }

    public int TotalJobs {
      get {
        return totalJobs;
      }
    }

    public int CompletedJobs {
      get {
        return completedJobs;
      }
    }

    public void Start() {
      if (this.isRunning) {
        return;
      }
      this.ProcessImages();
    }

    private void ProcessImages() {
      WriteLog(String.Empty);
      WriteLog("Proceso iniciado a las: " + DateTime.Now.ToLongTimeString());
      WriteLog(String.Empty);

      asyncResult = BeginProcessImages(EndProcessImages);
      isRunning = true;
    }

    #endregion Public methods

    #region Private and internal methods

    private IAsyncResult BeginProcessImages(AsyncCallback callback) {
      var processImageDelegate = new ProcessImagesDelegate(DoProcessImages);

      return processImageDelegate.BeginInvoke(callback, null);
    }

    private void EndProcessImages(IAsyncResult asyncResult) {
      var processImagesDelegate = (ProcessImagesDelegate) ((AsyncResult) asyncResult).AsyncDelegate;

      processImagesDelegate.EndInvoke(asyncResult);

      WriteLog(String.Empty);
      WriteLog("Terminado a las: " + DateTime.Now.ToLongTimeString());
      isRunning = false;
      WriteLogToDisk();
    }

    private int DoProcessImages() {
      try {
        var auditTrail = FileAuditTrail.GetInstance();
        auditTrail.Start();
        var imagesToProcess = ImageProcessor.GetImagesToProcess();
        this.totalJobs = imagesToProcess.Length;

        WriteLog(String.Empty);
        WriteLog("Se encontraron " + this.TotalJobs.ToString("N0") + " imágenes para procesar.");

        foreach (var image in imagesToProcess) {
          ImageProcessor.ProcessTiffImage(image);
        }
        WriteDataErrorLog("Proceso terminado a las : " + DateTime.Now.ToLongTimeString());

        auditTrail.End();
        WriteLog(auditTrail.GetLogs());

        var exceptions = auditTrail.GetExceptions();
        if (exceptions.Length != 0) {
          WriteLog("NOTA: Ver el detalle con los errores encontrados en la conversión al final de este archivo.");
          WriteDataErrorLog(String.Empty);
          WriteDataErrorLog(exceptions);
        }
        auditTrail.Clean();

        return imagesToProcess.Length;
      } catch (Exception exception) {
        isRunning = false;
        WriteLog("NOTA: Ver el detalle con los errores encontrados en la conversión al final de este archivo.");
        WriteDataErrorLog(String.Empty);
        WriteDataErrorLog("Ocurrió un problema en la conversión y procesamiento de imágenes:");
        WriteDataErrorLog(exception.ToString());
        WriteDataErrorLog(String.Empty);
        WriteDataErrorLog(String.Empty);
        WriteDataErrorLog("Proceso terminado a las : " + DateTime.Now.ToLongTimeString());
        return -1;
      }
    }

    private void WriteLog(string text) {
      logText += text + System.Environment.NewLine;
    }

    private void WriteDataErrorLog(string text) {
      errorLogText += text + System.Environment.NewLine;
    }

    private void WriteLogToDisk() {
      string message = "Tarea de conversión y procesamiento de imágenes";
      message += System.Environment.NewLine;

      message += logText;

      message += System.Environment.NewLine;
      message += System.Environment.NewLine;

      if (errorLogText.Length == 0) {
        message += "No se detectaron problemas en la conversión";
      } else {
        message += "***************************************************" + System.Environment.NewLine;
        message += "   Problemas encontrados en la conversión y procesamiento" + System.Environment.NewLine;
        message += "***************************************************" + System.Environment.NewLine;
        message += System.Environment.NewLine;
        message += errorLogText;
      }

      System.IO.File.WriteAllText(logFilePath + @"\imaging.processing." +
                                  DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss.ffff") + ".log",
                                  message);

      errorLogText = String.Empty;
      logText = String.Empty;
    }

    #endregion Private and internal methods

  } // class ImageProcessingEngine

} // namespace Empiria.Land.Documentation
