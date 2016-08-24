/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : FileAuditTrail                                 Pattern  : Service provider                    *
*  Version   : 6.7                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Audit trail services for Land file system operations.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Documentation {

  /// <summary>Audit trail services for Land file system operations.</summary>
  internal class FileAuditTrail {

    #region Fields

    static private readonly FileAuditTrail instance = new FileAuditTrail();  // singleton element
    private bool isRunning = false;   // semaphore
    private string log = String.Empty;

    #endregion Fields

    #region Public methods

    private FileAuditTrail() {
      // Singleton pattern needs private constructor
    }

    static public FileAuditTrail GetInstance() {
      return instance;
    }

    public void Start() {
      this.isRunning = true;
    }

    public string GetLogs() {
      return this.log;
    }

    public void End() {
      this.isRunning = false;
    }

    public void Clean() {
      this.log = String.Empty;
    }

    /// <summary>Adds the exception text to the exception log.</summary>
    /// <param name="exceptionText">The exception to log.</param>
    public static void LogException(string exceptionText) {
      var auditTrail = FileAuditTrail.GetInstance();

      Assertion.Assert(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      auditTrail.AddLog(exceptionText.Replace("\n", Environment.NewLine));
    }

    /// <summary>Adds the text to the text log.</summary>
    public static void LogException(CandidateImage image, Exception exception,
                                    string message, string fullTextToLog) {
      var auditTrail = FileAuditTrail.GetInstance();

      Assertion.Assert(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      DataServices.WriteImageProcessingLogException(image, message, exception);

      auditTrail.AddLog(fullTextToLog.Replace("\n", Environment.NewLine));
    }

    /// <summary>Adds the text to the text log.</summary>
    public static void LogOperation(DocumentImageSet image, string message, string fullTextToLog) {
      var auditTrail = FileAuditTrail.GetInstance();

      Assertion.Assert(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      DataServices.WriteImageProcessingLog(image, message);

      auditTrail.AddLog(fullTextToLog.Replace("\n", Environment.NewLine));
    }

    /// <summary>Adds the text to the text log.</summary>
    /// <param name="text">The text to log.</param>
    public static void LogText(string text) {
      var auditTrail = FileAuditTrail.GetInstance();

      Assertion.Assert(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      auditTrail.AddLog(text.Replace("\n", Environment.NewLine));
    }

    #endregion Public methods

    private void AddLog(string message) {
      this.log += message + Environment.NewLine;
    }

  } // class FileAuditTrail

} // namespace Empiria.Land.Documentation
