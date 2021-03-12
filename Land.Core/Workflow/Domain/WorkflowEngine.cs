/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Coordinator                             *
*  Type     : WorkflowEngine                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Performs execution of Empiria Land micro workflow commands.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Workflow.Adapters;

namespace Empiria.Land.Workflow {

  /// <summary>Performs execution of Empiria Land micro workflow commands.</summary>
  internal class WorkflowEngine {

    private readonly WorkflowRules _rules;

    private readonly List<WorkflowTaskDto> _changesList = new List<WorkflowTaskDto>(4);


    internal WorkflowEngine(WorkflowRules rules) {
      Assertion.AssertObject(rules, "rules");

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

    #region Private methods


    private void Execute(LRSTransaction transaction, WorkflowCommand command) {
      var workflow = transaction.Workflow;

      var assignTo = command.Payload.AssignTo();

      switch (command.Type) {
        case WorkflowCommandType.Take:
          workflow.Take(command.Payload.Note);
          break;

        case WorkflowCommandType.SetNextStatus:
          var status = TransactionDtoMapper.MapStatus(command.Payload.NextStatus);

          workflow.SetNextStatus(status, assignTo, command.Payload.Note);
          break;

        case WorkflowCommandType.ReturnToMe:
          workflow.ReturnToMe();
          break;

        case WorkflowCommandType.Finish:
          if (workflow.CurrentStatus == LRSTransactionStatus.ToDeliver) {
            workflow.SetNextStatus(LRSTransactionStatus.Delivered, Contact.Parse(-6),
                                   command.Payload.Note);
          } else if (workflow.CurrentStatus == LRSTransactionStatus.ToReturn) {
            workflow.SetNextStatus(LRSTransactionStatus.Returned, Contact.Parse(-6),
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
          throw Assertion.AssertNoReachThisCode();
      }

      var mapped = WorkflowTaskMapper.Map(workflow.GetCurrentTask());

      _changesList.Add(mapped);
    }

    #endregion Private methods

  }  // class WorkflowEngine

}  // namespace Empiria.Land.Workflow
