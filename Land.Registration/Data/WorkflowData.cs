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

    static public DataView GetLRSResponsibleTransactionInbox(Contacts.Contact contact, TrackStatus status,
                                                     string filter, string sort) {
      DataOperation op = DataOperation.Parse("qryLRSResponsibleTransactionInbox", contact.Id, (char) status);
      return DataReader.GetDataView(op, filter, sort);
    }

    static public LRSWorkflowTask GetLRSWorkflowLastTask(LRSTransaction transaction) {
      DataRow row = DataReader.GetDataRow(DataOperation.Parse("getLRSLastTransactionTrack", transaction.Id));

      return BaseObject.ParseDataRow<LRSWorkflowTask>(row);
    }

    static public List<LRSWorkflowTask> GetLRSWorkflowTrack(LRSTransaction transaction) {
      var op = DataOperation.Parse("qryLRSWorkflowTrack", transaction.Id);

      return DataReader.GetList(op, (x) => BaseObject.ParseList<LRSWorkflowTask>(x));
    }

    static public DataView GetContactsWithActiveTransactions() {
      return DataReader.GetDataView(DataOperation.Parse("SELECT * FROM vwLRSTransactionsTotals"));
    }

    static public FixedList<Contact> GetContactsWithOutboxDocuments() {
      var operation = DataOperation.Parse("qryLRSContactsWithOutboxDocuments");

      return DataReader.GetList(operation, (x) => BaseObject.ParseList<Contact>(x)).ToFixedList();
    }

    static internal int WriteLRSWorkflowTask(LRSWorkflowTask o) {
      var operation = DataOperation.Parse("writeLRSWorkflowTrack", o.Id, o.Transaction.Id,
                                          o.EventId, (char) o.Mode, o.AssignedBy.Id, o.Responsible.Id,
                                          o.NextContact.Id, (char) o.CurrentStatus, (char) o.NextStatus,
                                          o.CheckInTime, o.EndProcessTime, o.CheckOutTime, o.Notes,
                                          o.PreviousTask.Id, o.NextTask.Id, (char) o.Status, String.Empty);

      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class WorkflowData

} // namespace Empiria.Land.Registration.Data
