/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowCommandsAggregator                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Aggregates a set of commands for a given transaction and user, that can be executed by         *
*             the workflow engine.                                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Workflow.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Workflow {

  /// <summary>Aggregates a set of commands for a given transaction and user, that can be
  /// executed by the workflow engine.</summary>
  internal class WorkflowCommandsAggregator {

    private readonly List<LRSTransaction> _transactions = new List<LRSTransaction>();
    private readonly Contact _user;

    private List<ApplicableCommandDto> _actions = new List<ApplicableCommandDto>();

    #region Public methods

    public WorkflowCommandsAggregator(Contact user) {
      _user = user;
    }


    internal void Aggregate(LRSTransaction transaction) {
      var actions = GetApplicableActionsForTransaction(transaction);

      AppendActions(actions);

      _transactions.Add(transaction);
    }


    internal FixedList<ApplicableCommandDto> GetApplicableCommands() {
      return _actions.ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private void AppendActions(List<ApplicableCommandDto> actionsToAppend) {
      var temp = new List<ApplicableCommandDto>(_actions);

      foreach (var currentAction in _actions) {
        if (actionsToAppend.Exists(x => x.Type == currentAction.Type)) {
          temp.Remove(currentAction);
        }
      }

      foreach (var toAppend in actionsToAppend) {
        if (!_actions.Exists(x => x.Type == toAppend.Type)) {
          temp.Add(toAppend);
        }

      }  // foreach

      _actions = temp;
    }


    private List<ApplicableCommandDto> GetApplicableActionsForTransaction(LRSTransaction transaction) {
      WorkflowCommandType[] commandTypes = {
        WorkflowCommandType.AssignTo, WorkflowCommandType.PullToControlDesk,
        WorkflowCommandType.Receive, WorkflowCommandType.Reentry,
        WorkflowCommandType.ReturnToMe, WorkflowCommandType.SetNextStatus,
        WorkflowCommandType.Sign, WorkflowCommandType.Unsign,
      };

      var actionBuilder = new WorkflowCommandBuilder(transaction, _user);

      var actionsList = new List<ApplicableCommandDto>();

      ApplicableCommandDto action;

      foreach (var commandType in commandTypes) {
        if (actionBuilder.IsApplicable(commandType)) {
          action = actionBuilder.BuildActionFor(commandType);
          actionsList.Add(action);
        }
      }

      return actionsList;
    }

    #endregion Private methods

  }  // class WorkflowCommandsAggregator

}  // namespace Empiria.Land.Workflow.UseCases
