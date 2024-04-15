/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator                              *
*  Type     : WorkflowCommandsAggregator                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Aggregates a set of commands for a given transaction and user, that can be                     *
*             executed by the workflow engine.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Transactions.Workflow.Adapters;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Aggregates a set of commands for a given transaction and user, that can be
  /// executed by the workflow engine.</summary>
  internal class WorkflowCommandsAggregator {

    private readonly WorkflowRules _rules;

    private readonly List<LRSTransaction> _transactions = new List<LRSTransaction>();

    private List<ApplicableCommandDto> actions = new List<ApplicableCommandDto>();


    #region Public methods

    public WorkflowCommandsAggregator(WorkflowRules rules) {
      Assertion.Require(rules, "rules");

      _rules = rules;
    }


    internal void Aggregate(LRSTransaction transaction, Contact user) {
      var commands = GetApplicableCommandsForTransaction(transaction, user);

      AppendCommands(commands);

      _transactions.Add(transaction);
    }


    internal FixedList<ApplicableCommandDto> GetAllApplicableUserCommands(Contact user) {
      FixedList<WorkflowCommandType> candidates = _rules.WorkflowCommandTypeCandidates();

      var commandBuilder = new WorkflowCommandBuilder();

      List<ApplicableCommandDto> list = new List<ApplicableCommandDto>(candidates.Count);

      foreach (var candidate in candidates) {
        if (_rules.IsApplicable(user, candidate)) {
          var command = commandBuilder.BuildUserActionFor(candidate, user);

          list.Add(command);
        }
      }

      return list.ToFixedList();
    }


    internal FixedList<ApplicableCommandDto> GetApplicableCommands() {
      return this.actions.ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private void AppendCommands(List<ApplicableCommandDto> actionsToAppend) {
      var temp = new List<ApplicableCommandDto>(this.actions);

      foreach (var currentAction in this.actions) {
        if (!actionsToAppend.Exists(x => x.Type == currentAction.Type)) {
          temp.Remove(currentAction);
        }
      }

      foreach (var toAppend in actionsToAppend) {
        if (!this.actions.Exists(x => x.Type == toAppend.Type)) {
          temp.Add(toAppend);
        }
      }

      this.actions = temp;
    }


    private List<ApplicableCommandDto> GetApplicableCommandsForTransaction(LRSTransaction transaction,
                                                                           Contact user) {
      var commandTypes = _rules.WorkflowCommandTypeCandidates(transaction);

      var commandBuilder = new WorkflowCommandBuilder();

      var applicableCommands = new List<ApplicableCommandDto>();

      foreach (var commandType in commandTypes) {
        if (_rules.IsApplicable(commandType, transaction, user)) {
          ApplicableCommandDto action = commandBuilder.BuildActionFor(commandType, transaction);

          applicableCommands.Add(action);
        }
      }

      return applicableCommands;
    }


    #endregion Private methods

  }  // class WorkflowCommandsAggregator

}  // namespace Empiria.Land.Transactions.Workflow
