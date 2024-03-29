﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Assertion methods                       *
*  Type     : WorkflowAssertions                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Workflow's assertion check methods.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Workflow {

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

      if (task.NextStatus == TransactionStatus.EndPoint) {
        Assertion.RequireFail($"El trámite '{transaction.UID}' todavía no está listo para ser recibido.");
      }

      if (task.Responsible.Equals(user) && task.CurrentStatus != TransactionStatus.Reentry) {
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

      Assertion.Require(allNextStatusList.Contains(nextStatus),
                       $"No es posible mover el trámite '{transaction.UID}' a " +
                       $"'{nextStatus.GetStatusName()}', " +
                       $"debido a que se encuentra en '{task.CurrentStatusName}'.");
    }

  }  // class WorkflowAssertions

}  // namespace Empiria.Land.Transactions.Workflow
