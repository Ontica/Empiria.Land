/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : WorkflowCommand                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains information about a transaction workflow command than can be executed.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Workflow.Adapters {

  /// <summary>Contains information about a transaction workflow command than can be executed.</summary>
  public class WorkflowCommand {

    public WorkflowCommandType Type {
      get; set;
    } = WorkflowCommandType.Undefined;


    public WorkflowPayload Payload {
      get; set;
    } = new WorkflowPayload();

  }  // class WorkflowCommand


  /// <summary>Holds data needed to execute a given workflow command.</summary>
  public class WorkflowPayload {

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


  }  // class WorkflowPayload

}  // namespace Empiria.Land.Workflow.Adapters
