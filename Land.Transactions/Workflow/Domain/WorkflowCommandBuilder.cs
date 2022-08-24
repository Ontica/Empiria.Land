/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Data structure builder                  *
*  Type     : WorkflowCommandBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Creates workflow commands that can be invoked by the workflow engine.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Land.Workflow.Adapters;

using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Workflow {

  /// <summary>Creates workflow commands that can be invoked by the workflow engine.</summary>
  internal class WorkflowCommandBuilder {

    private readonly WorkflowRules _rules;

    internal WorkflowCommandBuilder() {
      _rules = new WorkflowRules();
    }


    internal ApplicableCommandDto BuildActionFor(WorkflowCommandType commandType,
                                                 LRSTransaction transaction) {
      var command = new ApplicableCommandDto();

      command.Type = commandType;
      command.Name = GetCommandTypeName(commandType);
      command.NextStatus = BuildNextStates(commandType, transaction);
      command.NextUsers = BuildNextUsersArray(commandType);

      return command;
    }


    internal ApplicableCommandDto BuildUserActionFor(WorkflowCommandType commandType, Contact user) {
      var command = new ApplicableCommandDto();

      command.Type = commandType;
      command.Name = GetCommandTypeName(commandType);
      command.NextStatus = BuildAllNextStates(commandType);

      return command;
    }


    #region Private methods


    private NextStateDto[] BuildAllNextStates(WorkflowCommandType commandType) {
      if (!_rules.MustBuildNextStatesList(commandType)) {
        return new NextStateDto[0];
      }

      var nextStateRules = _rules.NextStatusList();

      var nextStates = new List<NextStateDto>();

      foreach (var rule in nextStateRules) {
        var nextState = BuildNextStateDto(rule);
        nextState.Users = BuildNextUsersArray(commandType);
        nextStates.Add(nextState);
      }

      return nextStates.ToArray();
    }


    private NextStateDto[] BuildNextStates(WorkflowCommandType commandType,
                                           LRSTransaction transaction) {
      if (!_rules.MustBuildNextStatesList(commandType)) {
        return new NextStateDto[0];
      }

      var nextStateRules = _rules.NextStatusList(transaction);

      var nextStates = new List<NextStateDto>();

      foreach (var rule in nextStateRules) {
        var nextState = BuildNextStateDto(rule);
        nextState.Users = BuildNextUsersArray(commandType);
        nextStates.Add(nextState);
      }

      return nextStates.ToArray();
    }


    private NextStateDto BuildNextStateDto(LRSTransactionStatus status) {
      var nextState = new NextStateDto();

      nextState.Type = status.ToString();
      nextState.Name = LRSWorkflowRules.GetStatusName(status);

      return nextState;
    }


    private NamedEntityDto[] BuildNextUsersArray(WorkflowCommandType commandType) {
      if (!_rules.MustBuildNextUserArray(commandType)) {
        return new NamedEntityDto[0];
      }

      return new NamedEntityDto[0];
    }


    static private string GetCommandTypeName(WorkflowCommandType commandType) {
      switch (commandType) {

        case WorkflowCommandType.AssignTo:
          return "Asignar a";

        case WorkflowCommandType.Finish:
          return "Entregar o devolver al interesado";

        case WorkflowCommandType.PullToControlDesk:
          return "Traer a la mesa de control";

        case WorkflowCommandType.Take:
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
          return "Desarchivar y moverlo a la mesa de control";

        case WorkflowCommandType.Unsign:
          return "Cancelar firma electrónica";

        default:
          throw Assertion.EnsureNoReachThisCode();

      }
    }

    #endregion Private methods

  }  // class WorkflowCommandBuilder

}  // namespace Empiria.Land.Workflow
