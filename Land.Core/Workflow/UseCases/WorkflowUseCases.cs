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

    public FixedList<ApplicableCommandDto> AllApplicableUserCommands() {
      var user = ExecutionServer.CurrentIdentity.User.AsContact();

      var workflowRules = new WorkflowRules();

      var aggregator = new WorkflowCommandsAggregator(workflowRules);

      return aggregator.GetAllApplicableUserCommands(user);
    }


    public FixedList<ApplicableCommandDto> ApplicableCommands(string[] transactionUIDs) {
      if (transactionUIDs == null || transactionUIDs.Length == 0) {
        return new FixedList<ApplicableCommandDto>();
      }

      var user = ExecutionServer.CurrentIdentity.User.AsContact();

      var workflowRules = new WorkflowRules();

      var aggregator = new WorkflowCommandsAggregator(workflowRules);

      foreach (var uid in transactionUIDs) {
        var transaction = LRSTransaction.Parse(uid);

        aggregator.Aggregate(transaction, user);
      }

      return aggregator.GetApplicableCommands();
    }


    public void AssertWorkflowCommandExecution(WorkflowCommand command) {
      ValidateCommand(command);

      var user = ExecutionServer.CurrentIdentity.User.AsContact();

      var workflowRules = new WorkflowRules();

      var assertions = new WorkflowAssertions(workflowRules);

      assertions.AssertExecution(command, user);
    }


    public WorkflowTaskDto CurrentTask(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var currentWorkflowTask = transaction.Workflow.GetCurrentTask();

      return WorkflowTaskMapper.Map(currentWorkflowTask);
    }


    public FixedList<WorkflowTaskDto> ExecuteWorkflowCommand(WorkflowCommand command) {
      ValidateCommand(command);

      var user = ExecutionServer.CurrentIdentity.User.AsContact();

      var workflowRules = new WorkflowRules();

      var workflowEngine = new WorkflowEngine(workflowRules);

      workflowEngine.Execute(command, user);

      return workflowEngine.GetChangesList();
    }


    public TransactionDescriptor SearchTransaction(string searchUID) {
      Assertion.Require(searchUID, "searchUID");

      var transaction = LRSTransaction.TryParseWitAnyKey(searchUID);

      if (transaction == null) {
        throw new ResourceNotFoundException("Transaction.NotFound", $"No encontré un trámite con clave '{searchUID}'.");
      }

      return TransactionMapper.MapToDescriptor(transaction);
    }


    public FixedList<WorkflowTaskDto> WorkflowHistory(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var workflowTasks = transaction.Workflow.Tasks;

      return WorkflowTaskMapper.Map(workflowTasks);
    }

    #endregion Use cases

    #region Helpers

    private void ValidateCommand(WorkflowCommand command) {
      Assertion.Require(command, "command");

      Assertion.Require(command.Payload.NextStatus != TransactionStatus.All,
        $"Unrecognized value '{command.Payload.NextStatus}' for command.payload.nextStatus.");

    }

    #endregion Helpers

  }  // class WorkflowUseCases

}  // namespace Empiria.Land.Workflow.UseCases
