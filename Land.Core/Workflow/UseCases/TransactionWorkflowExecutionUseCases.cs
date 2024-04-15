/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionWorkflowExecutionUseCases       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that execute workflow commands for land transactions.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Transactions.Workflow.Adapters;

namespace Empiria.Land.Transactions.Workflow.UseCases {

  /// <summary>Use cases that execute workflow commands for land transactions.</summary>
  public class TransactionWorkflowExecutionUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionWorkflowExecutionUseCases() {
      // no-op
    }

    static public TransactionWorkflowExecutionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionWorkflowExecutionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public void AssertWorkflowCommandExecution(WorkflowCommand command) {
      ValidateCommand(command);

      var user = ExecutionServer.CurrentContact;

      var workflowRules = new WorkflowRules();

      var assertions = new WorkflowAssertions(workflowRules);

      assertions.AssertExecution(command, user);
    }


    public FixedList<WorkflowTaskDto> ExecuteWorkflowCommand(WorkflowCommand command) {
      ValidateCommand(command);

      var user = ExecutionServer.CurrentContact;

      var workflowRules = new WorkflowRules();

      var workflowEngine = new WorkflowEngine(workflowRules);

      workflowEngine.Execute(command, user);

      return workflowEngine.GetChangesList();
    }


    #endregion Use cases

    #region Helpers

    private void ValidateCommand(WorkflowCommand command) {
      Assertion.Require(command, "command");

      Assertion.Require(command.Payload.NextStatus != TransactionStatus.All,
                        $"Unrecognized value '{command.Payload.NextStatus}' for command.payload.nextStatus.");

    }

    #endregion Helpers

  }  // class WorkflowServices

}  // namespace Empiria.Land.Transactions.Workflow.UseCases
