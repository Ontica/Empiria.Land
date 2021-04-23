/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Registration System          *
*  Namespace : Empiria.Land.Documentation                       Assembly : Empiria.Land.Documentation        *
*  Type      : ImageProcessingEngine                            Pattern  : Singleton Service                 *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Performs image processing services.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Runtime.Remoting.Messaging;

namespace Empiria.Land.Documentation {

  public class ImageProcessingEngine {


    #region Delegates

    private delegate int ProcessImagesDelegate();

    #endregion


    #region Fields

    static private readonly ImageProcessingEngine instance = new ImageProcessingEngine();  // singleton

    private readonly string logFilePath = ConfigurationData.GetString("ImageProcessor.LogFilesPath");

    private IAsyncResult asyncResult = null;

    private string logText = String.Empty;

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
      get;
      private set;
    } = false;


    public int TotalJobs {
      get;
      private set;
    } = 0;


    public int CompletedJobs {
      get;
      private set;
    } = 0;


    public void Start() {
      if (this.IsRunning) {
        return;
      }
      this.ProcessImages();
    }


    private void ProcessImages() {
      WriteLog(String.Empty);
      WriteLog($"Proceso iniciado a las: {DateTime.Now.ToLongTimeString()}");
      WriteLog(String.Empty);

      asyncResult = BeginProcessImages(EndProcessImages);
      IsRunning = true;
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
      WriteLog($"Proceso terminado a las: {DateTime.Now.ToLongTimeString()}");
      IsRunning = false;
      WriteLogToDisk();
    }


    private int DoProcessImages() {
      var auditTrail = FileAuditTrail.GetInstance();
      try {
        auditTrail.Start();
        var imagesToProcess = ImageProcessor.GetImagesToProcess();
        this.TotalJobs = imagesToProcess.Length;

        FileAuditTrail.LogText(String.Empty);

        FileAuditTrail.LogText("Se procesarán en total " + this.TotalJobs.ToString("N0") + " imágenes ... \n");

        foreach (var image in imagesToProcess) {
          ImageProcessor.ProcessTiffImage(image);
        }

        WriteLog(auditTrail.GetLogs());

        auditTrail.Clean();
        auditTrail.End();

        return imagesToProcess.Length;
      } catch (Exception exception) {
        IsRunning = false;
        string msg = auditTrail.GetLogs() + "\n\n" +
                      "Ocurrió un problema en la conversión y procesamiento de imágenes:\n" +
                      exception.ToString() + "\n\n" +
                      "Proceso terminado a las : " + DateTime.Now.ToLongTimeString();

        return -1;
      }
    }


    private void WriteLog(string text) {
      logText += text + Environment.NewLine;
    }


    private void WriteLogToDisk() {
      string message = "Tarea de conversión y procesamiento de imágenes";
      message += Environment.NewLine;

      message += logText;

      message += Environment.NewLine;

      System.IO.File.WriteAllText(logFilePath + @"\imaging.processing." +
                                  DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss.ffff") + ".log",
                                  message);

      logText = String.Empty;
    }


    #endregion Private and internal methods

  } // class ImageProcessingEngine

} // namespace Empiria.Land.Documentation
