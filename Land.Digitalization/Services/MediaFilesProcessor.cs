/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Digitalization Services               Component : Services Layer                          *
*  Assembly : Empiria.Land.Digitalization.dll            Pattern   : Service provider                        *
*  Type     : MediaFilesProcessor                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Image processing engine.                                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using System.Runtime.Remoting.Messaging;

namespace Empiria.Land.Digitalization {

  /// <summary>Image processing engine.</summary>
  public class MediaFilesProcessor {

    #region Fields

    static private readonly MediaFilesProcessor _instance = new MediaFilesProcessor();  // singleton

    private delegate int ProcessImagesDelegate();

    private readonly string logFilePath = ConfigurationData.GetString("ImageProcessor.LogFilesPath");

    private IAsyncResult asyncResult = null;

    private string logText = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    private MediaFilesProcessor() {
      // Singleton pattern needs private constructor
    }

    public static MediaFilesProcessor GetInstance() {
      return _instance;
    }

    #endregion Constructors and parsers

    #region Public members

    public bool IsRunning {
      get; private set;
    } = false;


    public int TotalJobs {
      get; private set;
    } = 0;


    public int CompletedJobs {
      get; private set;
    } = 0;


    public void Start() {
      if (this.IsRunning) {
        return;
      }
      this.ProcessImages();
    }


    public void ProcessImages() {
      WriteLog(String.Empty);
      WriteLog("Proceso iniciado a las: " + DateTime.Now.ToLongTimeString());
      WriteLog(String.Empty);

      asyncResult = BeginProcessImages(EndProcessImages);
      IsRunning = true;
    }

    #endregion Public members

    #region Private and internal methods

    private IAsyncResult BeginProcessImages(AsyncCallback callback) {
      var processImageDelegate = new ProcessImagesDelegate(DoProcessImages);

      return processImageDelegate.BeginInvoke(callback, null);
    }


    private void EndProcessImages(IAsyncResult asyncResult) {
      var processImagesDelegate = (ProcessImagesDelegate) ((AsyncResult) asyncResult).AsyncDelegate;

      processImagesDelegate.EndInvoke(asyncResult);

      WriteLog(String.Empty);
      WriteLog("Proceso terminado a las: " + DateTime.Now.ToLongTimeString());
      IsRunning = false;
      WriteLogToDisk();
    }


    private int DoProcessImages() {
      var auditTrail = MediaFilesProcessorAuditTrail.GetInstance();

      try {

        auditTrail.Start();

        var imagesToProcess = ImageProcessor.GetImagesToProcess();

        this.TotalJobs = imagesToProcess.Length;

        MediaFilesProcessorAuditTrail.LogText(String.Empty);

        MediaFilesProcessorAuditTrail.LogText($"Se procesarán en total {this.TotalJobs.ToString("N0")} " +
                                                $"documentos u archivos de medios ... \n");

        foreach (var image in imagesToProcess) {
          ImageProcessor.ProcessMediaFile(image);
        }

        WriteLog(auditTrail.GetLogs());

        auditTrail.Clean();
        auditTrail.End();

        return imagesToProcess.Length;

      } catch (Exception exception) {
        IsRunning = false;
        string msg = $"{auditTrail.GetLogs()} \n\n" +
                    "Ocurrió un problema en la conversión y procesamiento de imágenes:\n" +
                     $"{exception.ToString()} \n\n" +
                     $"Proceso terminado a las : {DateTime.Now.ToLongTimeString()}.";
        MediaFilesProcessorAuditTrail.LogException(msg);

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

      File.WriteAllText($@"{logFilePath}\imaging.processing.{DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss.ffff")}.log",
                        message);

      logText = String.Empty;
    }


    #endregion Private and internal methods

  }  // class RecordingBookMediaUseCases

}  // namespace Empiria.Land.Digitalization
