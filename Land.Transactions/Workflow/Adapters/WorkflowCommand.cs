/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Command payload                         *
*  Type     : WorkflowCommand                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about a transaction workflow command than can be executed.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Contains information about a transaction workflow command than can be executed.</summary>
  public class WorkflowCommand {

    #region Fields

    public WorkflowCommandType Type {
      get; set;
    } = WorkflowCommandType.Undefined;


    public WorkflowCommandPayload Payload {
      get; set;
    } = new WorkflowCommandPayload();

    #endregion Fields


    /// <summary>Holds data needed to execute a given workflow command.</summary>
    public class WorkflowCommandPayload {

      internal WorkflowCommandPayload() {
        // no-op
      }

      public string[] TransactionUID {
        get; set;
      } = new string[0];


      public string AssignToUID {
        get; set;
      } = string.Empty;


      public TransactionStatus NextStatus {
        get; set;
      } = TransactionStatus.Undefined;


      public string SearchUID {
        get; set;
      } = string.Empty;


      public string Note {
        get; set;
      } = string.Empty;


      internal Contact AssignTo() {
        if (!string.IsNullOrWhiteSpace(this.AssignToUID)) {
          return Contact.Parse(this.AssignToUID);
        }
        return Contact.Empty;
      }

    }  // inner class WorkflowCommandPayload


  }  // class WorkflowCommand

}  // namespace Empiria.Land.Transactions.Workflow
