﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Digitalization Services               Component : Domain Layer                            *
*  Assembly : Empiria.Land.Digitalization.dll            Pattern   : Information holder                      *
*  Type     : MediaFilesProcessorAuditTrail              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Audit trail services for Land file system operations.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Digitalization {

  /// <summary>Audit trail services for Land file system operations.</summary>
  internal class MediaFilesProcessorAuditTrail {

    #region Fields

    static private readonly MediaFilesProcessorAuditTrail _instance =
                                                   new MediaFilesProcessorAuditTrail();  // singleton element

    private bool isRunning = false;   // semaphore

    private string log = String.Empty;

    #endregion Fields

    #region Public methods

    private MediaFilesProcessorAuditTrail() {
      // Singleton pattern needs private constructor
    }

    static public MediaFilesProcessorAuditTrail GetInstance() {
      return _instance;
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

    public static void LogException(string exceptionText) {
      var auditTrail = MediaFilesProcessorAuditTrail.GetInstance();

      Assertion.Require(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      auditTrail.AddLog(exceptionText.Replace("\n", Environment.NewLine));
    }


    public static void LogText(string text) {
      var auditTrail = MediaFilesProcessorAuditTrail.GetInstance();

      Assertion.Require(auditTrail.isRunning, "FileAuditTrail is not running. Please start it first.");

      auditTrail.AddLog(text.Replace("\n", Environment.NewLine));
    }

    #endregion Public methods

    private void AddLog(string message) {
      this.log += message + Environment.NewLine;
    }

  } // class MediaFilesProcessorAuditTrail

} // namespace Empiria.Land.Digitalization
