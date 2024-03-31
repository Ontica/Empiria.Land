/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Aggregator class                        *
*  Type     : WorkflowRules                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides specific rules for Empiria Land micro workflow engine.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Security;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Provides specific rules for Empiria Land micro workflow engine.</summary>
  public class WorkflowRules {


    public bool CanReceiveFor(Contact user, TransactionStatus nextStatus) {
      return nextStatus != TransactionStatus.EndPoint;
    }


    public bool IsApplicable(Contact user, WorkflowCommandType commandType) {
      Assertion.Require(user, nameof(user));
      Assertion.Require(commandType, nameof(commandType));

      switch (commandType) {
        // case WorkflowCommandType.AssignTo:
        case WorkflowCommandType.PullToControlDesk:
        case WorkflowCommandType.Unarchive:
          return IsInRole(user, WorkflowRole.ControlClerk);

        case WorkflowCommandType.Take:
          return true;

        case WorkflowCommandType.Reentry:
          return IsInRole(user, WorkflowRole.Supervisor);

        case WorkflowCommandType.SetNextStatus:
          return true;

        case WorkflowCommandType.Sign:
        case WorkflowCommandType.Unsign:
          return IsInRole(user, WorkflowRole.Signer);

        case WorkflowCommandType.Finish:
          return IsInRole(user, WorkflowRole.DeliveryClerk);

        default:
          return false;
      }
    }


    public bool IsApplicable(WorkflowCommandType commandType,
                             LRSTransaction transaction,
                             Contact user) {
      Assertion.Require(commandType, nameof(commandType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(user, nameof(user));

      var task = transaction.Workflow.GetCurrentTask();
      var nextStatus = transaction.Workflow.GetCurrentTask().NextStatus;

      switch (commandType) {
        // case WorkflowCommandType.AssignTo:
        case WorkflowCommandType.PullToControlDesk:
        case WorkflowCommandType.Unarchive:
          return IsInRole(user, WorkflowRole.ControlClerk);

        case WorkflowCommandType.Take:
          if (nextStatus == TransactionStatus.EndPoint) {
            return false;
          }
          if (task.Responsible.Equals(user) && task.CurrentStatus != TransactionStatus.Reentry) {
            return false;
          }
          return CanReceiveFor(user, nextStatus);

        case WorkflowCommandType.Reentry:
          return IsInRole(user, WorkflowRole.Supervisor);

        case WorkflowCommandType.ReturnToMe:
          return task.Responsible.Equals(user) && nextStatus != TransactionStatus.EndPoint;

        case WorkflowCommandType.SetNextStatus:
          return true;

        case WorkflowCommandType.Sign:
        case WorkflowCommandType.Unsign:
          return IsInRole(user, WorkflowRole.Signer);

        case WorkflowCommandType.Finish:
          if ((task.CurrentStatus == TransactionStatus.ToDeliver ||
              task.CurrentStatus == TransactionStatus.ToReturn)) {
            return IsInRole(user, WorkflowRole.DeliveryClerk);
          }
          return false;

        default:
          return false;
      }
    }


    internal bool IsInRole(Contact user, WorkflowRole role) {
      return AuthorizationService.IsSubjectInRole(user, $"{role}");
    }


    public FixedList<TransactionStatus> NextStatusList() {
      return new List<TransactionStatus> {
        TransactionStatus.Control,
        TransactionStatus.Qualification,
        TransactionStatus.Recording,
        TransactionStatus.Elaboration,
        TransactionStatus.Revision,
        TransactionStatus.OnSign,
        TransactionStatus.ToDeliver,
        TransactionStatus.ToReturn
      }.ToFixedList();
    }


    public FixedList<TransactionStatus> NextStatusList(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      var currentStatus = transaction.Workflow.GetCurrentTask().CurrentStatus;

      return NextStatusList(transaction, currentStatus).ToFixedList();
    }


    private List<TransactionStatus> NextStatusList(LRSTransaction transaction,
                                                   TransactionStatus currentStatus) {
      Assertion.Require(transaction.TransactionType, "type");
      Assertion.Require(transaction.DocumentType, "docType");

      List<TransactionStatus> list = new List<TransactionStatus>();

      switch (currentStatus) {

        case TransactionStatus.Payment:
          list.Add(TransactionStatus.Received);
          list.Add(TransactionStatus.Deleted);
          break;

        case TransactionStatus.Received:
        case TransactionStatus.Reentry:
          list.Add(TransactionStatus.Control);
          break;

        case TransactionStatus.Process:
        case TransactionStatus.Control:
          // Certificado || Cancelación || Copia simple
          if (LRSWorkflowRules.IsForElaborationOnly(transaction)) {
            list.Add(TransactionStatus.Elaboration);
          } else {
            list.Add(TransactionStatus.Qualification);
            list.Add(TransactionStatus.Recording);
            list.Add(TransactionStatus.Elaboration);
          }

          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.OnSign);

          list.Add(TransactionStatus.ToReturn);

          if (LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
            list.Add(TransactionStatus.ToDeliver);
          }
          break;

        case TransactionStatus.Juridic:
          break;

        case TransactionStatus.Qualification:       // Only used in Zacatecas

          list.Add(TransactionStatus.Recording);
          list.Add(TransactionStatus.Elaboration);
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.Qualification);
          list.Add(TransactionStatus.ToReturn);
          list.Add(TransactionStatus.Control);

          break;

        case TransactionStatus.Recording:
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.Recording);
          list.Add(TransactionStatus.Control);

          list.Add(TransactionStatus.ToReturn);

          if (LRSWorkflowRules.IsArchivable(transaction)) {
            list.Add(TransactionStatus.Archived);
          }
          if (transaction.DocumentType.Id == 728) {
            list.Add(TransactionStatus.OnSign);
          }

          break;

        case TransactionStatus.Elaboration:
          list.Add(TransactionStatus.Elaboration);
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.OnSign);
          list.Add(TransactionStatus.Control);
          list.Add(TransactionStatus.ToReturn);

          break;

        case TransactionStatus.Revision:

          list.Add(TransactionStatus.OnSign);

          if (LRSWorkflowRules.IsForElaborationOnly(transaction)) {
            list.Add(TransactionStatus.Elaboration);
          } else {
            list.Add(TransactionStatus.Qualification);
            list.Add(TransactionStatus.Recording);
            list.Add(TransactionStatus.Elaboration);
          }

          if (LRSWorkflowRules.IsArchivable(transaction)) {
            list.Add(TransactionStatus.Archived);
          }

          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.Control);
          list.Add(TransactionStatus.ToReturn);

          break;

        case TransactionStatus.OnSign:
          list.Add(TransactionStatus.ToDeliver);
          list.Add(TransactionStatus.Revision);

          if (LRSWorkflowRules.IsForElaborationOnly(transaction)) {
            list.Add(TransactionStatus.Elaboration);
          } else {
            list.Add(TransactionStatus.Qualification);
            list.Add(TransactionStatus.Recording);
            list.Add(TransactionStatus.Elaboration);
          }

          list.Add(TransactionStatus.Control);
          list.Add(TransactionStatus.ToReturn);

          break;

        case TransactionStatus.Digitalization:
          list.Add(TransactionStatus.Delivered);

          if (LRSWorkflowRules.IsDigitalizable(transaction)) {
            list.Add(TransactionStatus.Digitalization);
          }

          list.Add(TransactionStatus.Control);

          break;

        case TransactionStatus.ToDeliver:
          list.Add(TransactionStatus.Control);
          AddRecordingOrElaborationStatus(list, transaction);
          list.Add(TransactionStatus.Juridic);
          list.Add(TransactionStatus.OnSign);

          break;

        case TransactionStatus.Archived:
          list.Add(TransactionStatus.Control);
          break;

        case TransactionStatus.ToReturn:
          list.Add(TransactionStatus.Returned);
          list.Add(TransactionStatus.Control);
          break;
      }
      return list;
    }


    public bool MustBuildNextStatesList(WorkflowCommandType commandType) {
      return (commandType == WorkflowCommandType.AssignTo ||
              commandType == WorkflowCommandType.Take ||
              commandType == WorkflowCommandType.SetNextStatus);
    }


    public bool MustBuildNextUserArray(WorkflowCommandType commandType) {
      return (commandType == WorkflowCommandType.AssignTo ||
              commandType == WorkflowCommandType.Take);
    }


    public FixedList<WorkflowCommandType> WorkflowCommandTypeCandidates() {
      return BuildCommandTypeList(WorkflowCommandType.Take,
                                  WorkflowCommandType.SetNextStatus);
    }


    public FixedList<WorkflowCommandType> WorkflowCommandTypeCandidates(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      var currentTask = transaction.Workflow.GetCurrentTask();

      var currentStatus = currentTask.CurrentStatus;
      var nextStatus = currentTask.NextStatus;

      if (AnyOf(currentStatus, TransactionStatus.Payment, TransactionStatus.Deleted)) {
        return new FixedList<WorkflowCommandType>();
      }

      if (AnyOf(currentStatus, TransactionStatus.Returned, TransactionStatus.Delivered)) {
        return BuildCommandTypeList(WorkflowCommandType.Reentry);
      }

      if (currentStatus == TransactionStatus.Archived) {
        return BuildCommandTypeList(WorkflowCommandType.Unarchive);
      }

      if (AnyOf(currentStatus, TransactionStatus.ToDeliver, TransactionStatus.ToReturn)) {
        if (nextStatus == TransactionStatus.EndPoint) {
          return BuildCommandTypeList(WorkflowCommandType.Finish,
                                      WorkflowCommandType.SetNextStatus);
        } else {
          return BuildCommandTypeList(WorkflowCommandType.Take,
                                      WorkflowCommandType.ReturnToMe,
                                      WorkflowCommandType.SetNextStatus);
        }
      }

      if (currentStatus == TransactionStatus.Control) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == TransactionStatus.Qualification) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == TransactionStatus.OnSign) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.Sign,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == TransactionStatus.Revision) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == TransactionStatus.Received) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == TransactionStatus.Reentry) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (AnyOf(currentStatus, TransactionStatus.Recording, TransactionStatus.Elaboration) &&
          nextStatus == TransactionStatus.EndPoint) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (nextStatus != TransactionStatus.EndPoint) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.ReturnToMe);
      }

      return new FixedList<WorkflowCommandType>();
    }


    #region Helper methods

    private void AddRecordingOrElaborationStatus(List<TransactionStatus> list,
                                                 LRSTransaction transaction) {
      if (LRSWorkflowRules.IsRecordingDocumentCase(transaction)) {
        list.Add(TransactionStatus.Recording);

      } else if (LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
        list.Add(TransactionStatus.Elaboration);

      } else {
        list.Add(TransactionStatus.Elaboration);
        list.Add(TransactionStatus.Recording);
      }
    }


    private bool AnyOf<T>(T value, params T[] matchTo) {
      foreach (var item in matchTo) {
        if (value.Equals(item)) {
          return true;
        }
      }
      return false;
    }


    static private FixedList<WorkflowCommandType> BuildCommandTypeList(params WorkflowCommandType[] list) {
      return new FixedList<WorkflowCommandType>(list);
    }

    #endregion Helper methods

  }  // class WorkflowRules

}  // namespace Empiria.Land.Transactions.Workflow
