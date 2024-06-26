﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Data Services Layer                     *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data service                            *
*  Type     : WorkflowTaskMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides database read and write methods for Empiria Land micro-workflow services.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.Land.Transactions.Workflow.Data {

  /// <summary>Provides database read and write methods for Empiria Land micro-workflow services.</summary>
  static public class WorkflowData {

    #region Public methods

    static public LRSWorkflowTask GetWorkflowLastTask(LRSTransaction transaction) {
      var op = DataOperation.Parse("getLRSWorkflowLastTask", transaction.Id);

      return DataReader.GetObject<LRSWorkflowTask>(op);
    }


    static public List<LRSWorkflowTask> GetWorkflowTrack(LRSTransaction transaction) {
      var op = DataOperation.Parse("qryLRSWorkflowTrack", transaction.Id);

      return DataReader.GetList<LRSWorkflowTask>(op);
    }


    static internal void WriteWorkflowTask(LRSWorkflowTask o) {
      var op = DataOperation.Parse("writeLRSWorkflowTask", o.Id, o.Transaction.Id,
                               o.EventId, (char) o.Mode, o.AssignedBy.Id, o.Responsible.Id,
                               o.NextContact.Id, (char) o.CurrentStatus, (char) o.NextStatus,
                               o.CheckInTime, o.EndProcessTime, o.CheckOutTime, o.Notes,
                               o.PreviousTask.Id, o.NextTask.Id, (char) o.Status, String.Empty);

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class WorkflowData

} // namespace Empiria.Land.Transactions.Workflow.Data
