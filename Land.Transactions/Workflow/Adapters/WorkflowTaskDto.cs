/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Data Transfer Object                    *
*  Type     : WorkflowTaskDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that represents a workflow task.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Output DTO that represents a workflow task.</summary>
  public class WorkflowTaskDto {

    public string TaskName {
      get; internal set;
    }

    public string AssigneeName {
      get; internal set;
    }

    public DateTime CheckInTime {
      get; internal set;
    }

    public DateTime CheckOutTime {
      get; internal set;
    }

    public DateTime EndProcessTime {
      get; internal set;
    }

    public string ElapsedTime {
      get; internal set;
    }

    public TransactionStatus NextTask {
      get; internal set;
    }

    public string NextTaskName {
      get; internal set;
    }

    public string NextAssigneeUID {
      get; internal set;
    }

    public string NextAssigneeName {
      get; internal set;
    }

    public string Notes {
      get; internal set;
    }

    public string StatusName {
      get; internal set;
    }

  }  // class WorkflowTaskDto

}  // namespace Empiria.Land.Transactions.Workflow
