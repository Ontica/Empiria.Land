/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowCommandBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Creates a workflow command that can be invoked by the workflow engine.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Workflow.Adapters;

using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Workflow {

  /// <summary>Creates a workflow command that can be invoked by the workflow engine.</summary>
  internal class WorkflowCommandBuilder {

    private readonly LRSTransaction _transaction;
    private readonly Contact _user;

    internal WorkflowCommandBuilder(LRSTransaction transaction, Contact user) {
      _transaction = transaction;
      _user = user;
    }

    internal ApplicableCommandDto BuildActionFor(WorkflowCommandType commandType) {
      var command = new ApplicableCommandDto();

      command.Type = commandType;
      command.Name = GetCommandTypeName(commandType);
      command.NextStatus = GetNextStates(commandType);
      command.NextUsers = GetNextUsers(commandType);

      return command;
    }


    internal bool IsApplicable(WorkflowCommandType commandType) {
      var task = _transaction.Workflow.GetCurrentTask();
      var nextStatus = _transaction.Workflow.GetCurrentTask().CurrentStatus;

      switch (commandType) {
        // case WorkflowCommandType.AssignTo:
        case WorkflowCommandType.PullToControlDesk:
        case WorkflowCommandType.Unarchive:
          return WorkflowUsers.IsInRole(_user, WorkflowRole.ControlClerk);

        case WorkflowCommandType.Receive:
          if (nextStatus == LRSTransactionStatus.EndPoint) {
            return false;
          }
          if (task.Responsible.Id == _user.Id && task.CurrentStatus != LRSTransactionStatus.Reentry) {
            return false;
          }
          return WorkflowUsers.CanReceiveFor(_user, nextStatus);

        case WorkflowCommandType.Reentry:
          return WorkflowUsers.IsInRole(_user, WorkflowRole.Supervisor);

        case WorkflowCommandType.ReturnToMe:
          return task.Responsible.Equals(_user) && nextStatus != LRSTransactionStatus.EndPoint;

        case WorkflowCommandType.SetNextStatus:
          return true;

        case WorkflowCommandType.Sign:
        case WorkflowCommandType.Unsign:
          return WorkflowUsers.IsInRole(_user, WorkflowRole.Signer);

        case WorkflowCommandType.Finish:
          if ((task.CurrentStatus == LRSTransactionStatus.ToDeliver ||
              task.CurrentStatus == LRSTransactionStatus.ToReturn)) {
            return WorkflowUsers.IsInRole(_user, WorkflowRole.DeliveryClerk);
          }
          return false;

        default:
          return false;
      }
    }


    #region Private methods

    private string GetCommandTypeName(WorkflowCommandType commandType) {
      switch (commandType) {

        case WorkflowCommandType.AssignTo:
          return "Asignar a";

        case WorkflowCommandType.Finish:
          return "Entregar o devolver al interesado";

        case WorkflowCommandType.PullToControlDesk:
          return "Traer a la mesa de control";

        case WorkflowCommandType.Receive:
          return "Recibir en el siguiente estado";

        case WorkflowCommandType.Reentry:
          return "Reingresar el trámite";

        case WorkflowCommandType.ReturnToMe:
          return "Regresar a mi bandeja de trabajo";

        case WorkflowCommandType.SetNextStatus:
          return "Mover al siguiente estado";

        case WorkflowCommandType.Sign:
          return "Firmar electrónicamente";

        case WorkflowCommandType.Unarchive:
          return "Desarchivar y moverlo a  la mesa de control";

        case WorkflowCommandType.Unsign:
          return "Cancelar firma electrónica";

        default:
          throw Assertion.AssertNoReachThisCode();

      }
    }


    private NextStateDto[] GetNextStates(WorkflowCommandType commandType) {
      if (!NeedBuildNextStates(commandType)) {
        return new NextStateDto[0];
      }

      var nextStateRules = WorkflowRules.GetNextStatusList(_transaction);

      var nextStates = new List<NextStateDto>();

      foreach (var rule in nextStateRules) {
        var nextState = GetNextStateDto(rule);
        nextState.Users = GetNextUsers(commandType);
        nextStates.Add(nextState);
      }

      return nextStates.ToArray();
    }


    private NextStateDto GetNextStateDto(LRSTransactionStatus status) {
      var nextState = new NextStateDto();

      nextState.Type = status.ToString();
      nextState.Name = LRSWorkflowRules.GetStatusName(status);

      return nextState;
    }


    private NamedEntityDto[] GetNextUsers(WorkflowCommandType commandType) {
      return new NamedEntityDto[0];
    }


    private bool NeedBuildNextStates(WorkflowCommandType commandType) {
      return (commandType == WorkflowCommandType.AssignTo ||
              commandType == WorkflowCommandType.Receive ||
              commandType == WorkflowCommandType.SetNextStatus);
    }


    private bool NeedBuildNextUsers(WorkflowCommandType commandType) {
      return (commandType == WorkflowCommandType.AssignTo ||
              commandType == WorkflowCommandType.Receive);
    }


    #endregion Private methods

  }  // class WorkflowCommandBuilder

}  // namespace Empiria.Land.Workflow
