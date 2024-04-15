/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Coordinator                             *
*  Type     : WorkflowEngine                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs execution of Empiria Land micro workflow commands.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Transactions.Workflow.Adapters;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Performs execution of Empiria Land micro workflow commands.</summary>
  internal class WorkflowEngine {

    private readonly WorkflowRules _rules;

    private readonly List<WorkflowTaskDto> _changesList = new List<WorkflowTaskDto>(4);


    internal WorkflowEngine(WorkflowRules rules) {
      Assertion.Require(rules, "rules");

      _rules = rules;
    }


    internal void Execute(WorkflowCommand command, Contact user) {
      var assertions = new WorkflowAssertions(_rules);

      foreach (var transactionUID in command.Payload.TransactionUID) {
        var transaction = LRSTransaction.Parse(transactionUID);

        assertions.AssertExecution(transaction, command, user);

        Execute(transaction, command);
      }
    }


    internal FixedList<WorkflowTaskDto> GetChangesList() {
      return _changesList.ToFixedList();
    }

    #region Helpers

    private void Execute(LRSTransaction transaction, WorkflowCommand command) {
      var workflow = transaction.Workflow;

      var assignTo = command.Payload.AssignTo();

      switch (command.Type) {
        case WorkflowCommandType.Take:
          workflow.Take(command.Payload.Note);
          break;

        case WorkflowCommandType.SetNextStatus:
          var status = command.Payload.NextStatus;

          workflow.SetNextStatus(status, assignTo, command.Payload.Note);
          break;

        case WorkflowCommandType.ReturnToMe:
          workflow.ReturnToMe();
          break;

        case WorkflowCommandType.Finish:
          if (workflow.CurrentStatus == TransactionStatus.ToDeliver) {
            workflow.SetNextStatus(TransactionStatus.Delivered, LRSWorkflowRules.InterestedContact,
                                   command.Payload.Note);
          } else if (workflow.CurrentStatus == TransactionStatus.ToReturn) {
            workflow.SetNextStatus(TransactionStatus.Returned, LRSWorkflowRules.InterestedContact,
                                   command.Payload.Note);
          }

          break;

        case WorkflowCommandType.Reentry:
          workflow.Reentry();
          break;

        case WorkflowCommandType.AssignTo:
        case WorkflowCommandType.PullToControlDesk:
        case WorkflowCommandType.Sign:
        case WorkflowCommandType.Unarchive:
        case WorkflowCommandType.Unsign:
          break;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }

      var mapped = WorkflowTaskMapper.Map(workflow.GetCurrentTask());

      _changesList.Add(mapped);
    }

    #endregion Helpers

  }  // class WorkflowEngine

}  // namespace Empiria.Land.Transactions.Workflow
