/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : WorkflowUseCases                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get transaction workflow and invoke commands on it.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Workflow.Adapters;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Workflow.UseCases {

  /// <summary>Use cases for get transaction workflow and invoke commands on it.</summary>
  public partial class WorkflowUseCases : UseCase {

    #region Constructors and parsers

    protected WorkflowUseCases() {
      // no-op
    }

    static public WorkflowUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<WorkflowUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public WorkflowTaskDto CurrentTask(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var currentWorkflowTask = transaction.Workflow.GetCurrentTask();

      return WorkflowTaskMapper.Map(currentWorkflowTask);
    }


    public FixedList<WorkflowTaskDto> ExecuteWorkflowCommand(WorkflowCommand command) {
      ValidateCommand(command);

      var changesList = new List<WorkflowTaskDto>(command.Payload.TransactionUID.Length);

      foreach (var transactionUID in command.Payload.TransactionUID) {
        var transaction = LRSTransaction.Parse(transactionUID);

        var workflow = transaction.Workflow;

        var status = TransactionDtoMapper.MapStatus(command.Payload.NextStatus);

        var assignTo = command.Payload.AssignTo;

        workflow.SetNextStatus(status, assignTo, command.Payload.Note);

        var mapped = WorkflowTaskMapper.Map(workflow.GetCurrentTask());

        changesList.Add(mapped);
      }

      return changesList.ToFixedList();
    }


    public FixedList<WorkflowTaskDto> WorkflowHistory(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var workflowTasks = transaction.Workflow.Tasks;

      return WorkflowTaskMapper.Map(workflowTasks);
    }

    #endregion Use cases

    #region Helpers

    private void ValidateCommand(WorkflowCommand command) {
      Assertion.AssertObject(command, "command");

      Assertion.Assert(command.Payload.NextStatus != TransactionStatus.Undefined &&
                       command.Payload.NextStatus != TransactionStatus.All,
        $"Unrecognized value '{command.Payload.NextStatus}' for command.payload.nextStatus.");

    }

    #endregion Helpers

  }  // class WorkflowUseCases

}  // namespace Empiria.Land.Workflow.UseCases
