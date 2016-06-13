/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : WorkflowData                                 Pattern  : Data Services                         *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for Empiria Land micro-workflow services.            *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for Empiria Land micro-workflow services.</summary>
  static public class WorkflowData {

    #region Public methods

    static public FixedList<Contact> GetContactsWithWorkflowOutboxTasks() {
      var operation = DataOperation.Parse("qryLRSContactsWithWorkflowOutboxTasks");

      return DataReader.GetList(operation, (x) => BaseObject.ParseList<Contact>(x)).ToFixedList();
    }

    static public DataView GetResponsibleWorkflowInbox(Contact contact, WorkflowTaskStatus status,
                                                       string filter, string sort) {
      if (filter.Length == 0) {
        filter = "(1 = 1)";
      }
      if (sort.Length == 0) {
        sort = "PresentationTime, CheckInTime, TrackId";
      }
      var op = DataOperation.Parse("@qryLRSResponsibleWorkflowInbox",
                                   contact.Id, (char) status, filter, sort);

      return DataReader.GetDataView(op);
    }

    static public DataView GetWorkflowActiveTasksTotals() {
      string sql = "SELECT * FROM vwLRSWorkflowActiveTasksTotals ORDER BY Responsible";

      return DataReader.GetDataView(DataOperation.Parse(sql));
    }

    static public LRSWorkflowTask GetWorkflowLastTask(LRSTransaction transaction) {
      DataRow row = DataReader.GetDataRow(DataOperation.Parse("getLRSWorkflowLastTask", transaction.Id));

      return BaseObject.ParseDataRow<LRSWorkflowTask>(row);
    }

    static public List<LRSWorkflowTask> GetWorkflowTrack(LRSTransaction transaction) {
      var op = DataOperation.Parse("qryLRSWorkflowTrack", transaction.Id);

      return DataReader.GetList(op, (x) => BaseObject.ParseList<LRSWorkflowTask>(x));
    }

    static internal int WriteLRSWorkflowTask(LRSWorkflowTask o) {
      var operation = DataOperation.Parse("writeLRSWorkflowTask", o.Id, o.Transaction.Id,
                                          o.EventId, (char) o.Mode, o.AssignedBy.Id, o.Responsible.Id,
                                          o.NextContact.Id, (char) o.CurrentStatus, (char) o.NextStatus,
                                          o.CheckInTime, o.EndProcessTime, o.CheckOutTime, o.Notes,
                                          o.PreviousTask.Id, o.NextTask.Id, (char) o.Status, String.Empty);

      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class WorkflowData

} // namespace Empiria.Land.Registration.Data
