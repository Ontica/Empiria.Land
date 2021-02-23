/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowCommandsAggregator                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Aggregates a set of commands for a given transaction and user, that can be                     *
*             executed by the workflow engine.                                                               *
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
      var commands = GetApplicableCommandsForTransaction(transaction);

      AppendCommands(commands);

      _transactions.Add(transaction);
    }


    internal FixedList<ApplicableCommandDto> GetApplicableCommands() {
      return _actions.ToFixedList();
    }


    #endregion Public methods

    #region Private methods

    private void AppendCommands(List<ApplicableCommandDto> actionsToAppend) {
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


    private List<ApplicableCommandDto> GetApplicableCommandsForTransaction(LRSTransaction transaction) {
      var commandTypes = GetCandidates(transaction);

      var commandBuilder = new WorkflowCommandBuilder(transaction, _user);

      var applicableCommands = new List<ApplicableCommandDto>();

      ApplicableCommandDto action;

      foreach (var commandType in commandTypes) {
        if (commandBuilder.IsApplicable(commandType)) {
          action = commandBuilder.BuildActionFor(commandType);
          applicableCommands.Add(action);
        }
      }

      return applicableCommands;
    }

    private FixedList<WorkflowCommandType> GetCandidates(LRSTransaction transaction) {
      var currentTask = transaction.Workflow.GetCurrentTask();

      var currentStatus = currentTask.CurrentStatus;
      var nextStatus = currentTask.NextStatus;

      if (AnyOf(currentStatus, LRSTransactionStatus.Payment, LRSTransactionStatus.Deleted)) {
        return new FixedList<WorkflowCommandType>();
      }

      if (AnyOf(currentStatus, LRSTransactionStatus.Returned, LRSTransactionStatus.Delivered)) {
        return BuildCommandList(WorkflowCommandType.Reentry);
      }

      if (currentStatus == LRSTransactionStatus.Archived) {
        return BuildCommandList(WorkflowCommandType.Unarchive);
      }

      if (AnyOf(currentStatus, LRSTransactionStatus.ToDeliver, LRSTransactionStatus.ToReturn)) {
        return BuildCommandList(WorkflowCommandType.Finish,
                                WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Control) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus,
                                WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.Qualification) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus,
                                WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.OnSign) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.Sign,
                                WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Revision) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Received) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus,
                                WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.Reentry) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus,
                                WorkflowCommandType.AssignTo);
      }

      if (AnyOf(currentStatus, LRSTransactionStatus.Recording, LRSTransactionStatus.Elaboration) &&
          nextStatus == LRSTransactionStatus.EndPoint) {
        return BuildCommandList(WorkflowCommandType.Receive,
                                WorkflowCommandType.SetNextStatus);
      }

      if (nextStatus != LRSTransactionStatus.EndPoint) {
        return BuildCommandList(WorkflowCommandType.Receive, WorkflowCommandType.ReturnToMe);
      }

      return new FixedList<WorkflowCommandType>();
    }

    #endregion Private methods

    #region Helper methods

    private bool AnyOf<T>(T value, params T[] matchTo) {
      foreach (var item in matchTo) {
        if (value.Equals(item)) {
          return true;
        }
      }
      return false;
    }

    private FixedList<WorkflowCommandType> BuildCommandList(params WorkflowCommandType[] list) {
      return new FixedList<WorkflowCommandType>(list);
    }

    #endregion Helper methods

  }  // class WorkflowCommandsAggregator

}  // namespace Empiria.Land.Workflow.UseCases
