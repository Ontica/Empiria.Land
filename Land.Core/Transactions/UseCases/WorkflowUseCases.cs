/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : WorkflowUseCases                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for get transaction workflow and invoke commands on it.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Transactions.Adapters;

using Empiria.Land.Registration.Transactions;
using Empiria.Contacts;

namespace Empiria.Land.Transactions.UseCases {

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


    public TransactionDto ExecuteWorkflowCommand(WorkflowCommand command) {
      ValidateCommand(command);

      var transaction = LRSTransaction.Parse(command.Payload.TransactionUID[0]);

      return TransactionDtoMapper.Map(transaction);
    }


    public WorkflowTaskDto ExecuteWorkflowCommand(string transactionUID, WorkflowCommand command) {
      Assertion.AssertObject(transactionUID, "transactionUID");
      ValidateCommand(command);

      command.Payload.TransactionUID = new string[] { transactionUID };

      var transaction = LRSTransaction.Parse(command.Payload.TransactionUID[0]);

      var workflow = transaction.Workflow;

      var status = TransactionDtoMapper.MapStatus(command.Payload.NextStatus);

      var assignTo = command.Payload.AssignTo;

      workflow.SetNextStatus(status, assignTo, command.Payload.Note);

      return WorkflowTaskMapper.Map(workflow.GetCurrentTask());
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

}  // namespace Empiria.Land.Transactions.UseCases
