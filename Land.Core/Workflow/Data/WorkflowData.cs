/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Data Services Layer                     *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data service                            *
*  Type     : WorkflowData                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides database read and write methods for Empiria Land micro-workflow services.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.Land.Transactions.Workflow.Data {

  /// <summary>Provides database read and write methods for Empiria Land micro-workflow services.</summary>
  static internal class WorkflowData {

    #region Methods

    static internal LRSWorkflowTask GetWorkflowLastTask(LRSTransaction transaction) {

      var sql = "SELECT * FROM LRSTransactionTrack " +
               $"WHERE TransactionId = {transaction.Id} AND " +
                "NextTrackId = -1 AND TrackStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<LRSWorkflowTask>(op);
    }


    static internal List<LRSWorkflowTask> GetWorkflowTrack(LRSTransaction transaction) {

      var sql = "SELECT * FROM LRSTransactionTrack " +
               $"WHERE TransactionId = {transaction.Id} AND TrackStatus <> 'X' " +
                "ORDER BY TrackId";

      var op = DataOperation.Parse(sql);

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

    #endregion Methods

  } // class WorkflowData

} // namespace Empiria.Land.Transactions.Workflow.Data
