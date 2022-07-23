/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Assertion methods                       *
*  Type     : WorkflowAssertions                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Workflow's assertion check methods.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Workflow.Adapters;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Workflow {

  /// <summary>Workflow's assertion check methods.</summary>
  internal class WorkflowAssertions {

    private readonly WorkflowRules _rules;


    internal WorkflowAssertions(WorkflowRules rules) {
      Assertion.Require(rules, "rules");

      _rules = rules;
    }


    internal void AssertExecution(WorkflowCommand command, Contact user) {
      Assertion.Require(command, "command");
      Assertion.Require(user, "user");

      foreach (var transactionUID in command.Payload.TransactionUID) {
        var transaction = LRSTransaction.Parse(transactionUID);

        this.AssertExecution(transaction, command, user);
      }
    }


    internal void AssertExecution(LRSTransaction transaction,
                                  WorkflowCommand command,
                                  Contact user) {
      switch (command.Type) {
        case WorkflowCommandType.Take:
          this.CanTake(transaction, user);
          break;

        case WorkflowCommandType.SetNextStatus:
          this.CanSetNextStatus(transaction, command.Payload.NextStatus, user);
          break;

        default:
          break;
      }
    }


    internal void CanTake(LRSTransaction transaction, Contact user) {
      var task = transaction.Workflow.GetCurrentTask();

      if (task.NextStatus == LRSTransactionStatus.EndPoint) {
        Assertion.RequireFail($"El trámite '{transaction.UID}' todavía no está listo para ser recibido.");
      }

      if (task.Responsible.Id == user.Id && task.CurrentStatus != LRSTransactionStatus.Reentry) {
        Assertion.RequireFail($"El trámite '{transaction.UID}' todavía no está listo para ser recibido.");
      }

      if (!_rules.CanReceiveFor(user, task.NextStatus)) {
        Assertion.RequireFail($"La cuenta de usuario no tiene permisos para recibir el trámite " +
                              $"'{transaction.UID}' en el estado '{task.NextStatusName}'.");
      }
    }


    internal void CanSetNextStatus(LRSTransaction transaction,
                                   TransactionStatus nextStatus,
                                   Contact user) {
      var task = transaction.Workflow.GetCurrentTask();

      var allNextStatusList = _rules.NextStatusList(transaction);

      LRSTransactionStatus mappedStatus = TransactionMapper.MapStatus(nextStatus);

      Assertion.Require(allNextStatusList.Contains(mappedStatus),
                       $"No es posible mover el trámite '{transaction.UID}' a " +
                       $"'{LRSWorkflowRules.GetStatusName(mappedStatus)}', " +
                       $"debido a que se encuentra en '{task.CurrentStatusName}'.");
    }

  }  // class WorkflowAssertions

}  // namespace Empiria.Land.Workflow
