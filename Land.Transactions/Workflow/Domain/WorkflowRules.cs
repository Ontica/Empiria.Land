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


    public bool CanReceiveFor(Contact user, LRSTransactionStatus nextStatus) {
      return nextStatus != LRSTransactionStatus.EndPoint;
    }


    internal bool IsInRole(Contact user, WorkflowRole role) {
      return AuthorizationService.IsSubjectInRole(user, $"{role}");
    }


    public bool IsApplicable(Contact user, WorkflowCommandType commandType) {
      Assertion.Require(user, "user");
      Assertion.Require(commandType, "commandType");

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
      Assertion.Require(commandType, "commandType");
      Assertion.Require(transaction, "transaction");
      Assertion.Require(user, "user");

      var task = transaction.Workflow.GetCurrentTask();
      var nextStatus = transaction.Workflow.GetCurrentTask().NextStatus;

      switch (commandType) {
        // case WorkflowCommandType.AssignTo:
        case WorkflowCommandType.PullToControlDesk:
        case WorkflowCommandType.Unarchive:
          return IsInRole(user, WorkflowRole.ControlClerk);

        case WorkflowCommandType.Take:
          if (nextStatus == LRSTransactionStatus.EndPoint) {
            return false;
          }
          if (task.Responsible.Id == user.Id && task.CurrentStatus != LRSTransactionStatus.Reentry) {
            return false;
          }
          return CanReceiveFor(user, nextStatus);

        case WorkflowCommandType.Reentry:
          return IsInRole(user, WorkflowRole.Supervisor);

        case WorkflowCommandType.ReturnToMe:
          return task.Responsible.Equals(user) && nextStatus != LRSTransactionStatus.EndPoint;

        case WorkflowCommandType.SetNextStatus:
          return true;

        case WorkflowCommandType.Sign:
        case WorkflowCommandType.Unsign:
          return IsInRole(user, WorkflowRole.Signer);

        case WorkflowCommandType.Finish:
          if ((task.CurrentStatus == LRSTransactionStatus.ToDeliver ||
              task.CurrentStatus == LRSTransactionStatus.ToReturn)) {
            return IsInRole(user, WorkflowRole.DeliveryClerk);
          }
          return false;

        default:
          return false;
      }
    }


    public FixedList<LRSTransactionStatus> NextStatusList() {
      List<LRSTransactionStatus> list = new List<LRSTransactionStatus>();

      list.Add(LRSTransactionStatus.Control);
      list.Add(LRSTransactionStatus.Qualification);
      list.Add(LRSTransactionStatus.Recording);
      list.Add(LRSTransactionStatus.Elaboration);
      list.Add(LRSTransactionStatus.Revision);
      list.Add(LRSTransactionStatus.OnSign);
      list.Add(LRSTransactionStatus.ToDeliver);
      list.Add(LRSTransactionStatus.ToReturn);

      return list.ToFixedList();
    }


    public FixedList<LRSTransactionStatus> NextStatusList(LRSTransaction transaction) {
      Assertion.Require(transaction, "transaction");

      var currentStatus = transaction.Workflow.GetCurrentTask().CurrentStatus;

      return NextStatusList(transaction.TransactionType,
                               transaction.DocumentType,
                               currentStatus).ToFixedList();
    }


    public List<LRSTransactionStatus> NextStatusList(LRSTransactionType type,
                                                     LRSDocumentType docType,
                                                     LRSTransactionStatus currentStatus) {
      Assertion.Require(type, "type");
      Assertion.Require(docType, "docType");

      List<LRSTransactionStatus> list = new List<LRSTransactionStatus>();

      switch (currentStatus) {

        case LRSTransactionStatus.Payment:
          list.Add(LRSTransactionStatus.Received);
          list.Add(LRSTransactionStatus.Deleted);
          break;

        case LRSTransactionStatus.Received:
        case LRSTransactionStatus.Reentry:
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.Process:
        case LRSTransactionStatus.Control:
          // Certificado || Cancelación || Copia simple
          if (type.Id == 701 || docType.Id == 734) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else {
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Elaboration);
          }

          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.OnSign);

          list.Add(LRSTransactionStatus.ToReturn);

          if (LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
            list.Add(LRSTransactionStatus.ToDeliver);
          }
          break;

        case LRSTransactionStatus.Juridic:
          break;

        case LRSTransactionStatus.Qualification:       // Only used in Zacatecas

          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Elaboration);
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Qualification);
          list.Add(LRSTransactionStatus.ToReturn);
          list.Add(LRSTransactionStatus.Control);

          break;

        case LRSTransactionStatus.Recording:
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Control);

          list.Add(LRSTransactionStatus.ToReturn);
          if (LRSWorkflowRules.IsArchivable(type, docType)) {
            list.Add(LRSTransactionStatus.Archived);
          }
          if (docType.Id == 728) {
            list.Add(LRSTransactionStatus.OnSign);
          }

          break;

        case LRSTransactionStatus.Elaboration:
          list.Add(LRSTransactionStatus.Elaboration);
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.OnSign);
          list.Add(LRSTransactionStatus.Control);
          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.Revision:

          list.Add(LRSTransactionStatus.OnSign);

          if (type.Id == 701) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else {
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Elaboration);
          }

          if (LRSWorkflowRules.IsArchivable(type, docType)) {
            list.Add(LRSTransactionStatus.Archived);
          }

          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Control);
          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.OnSign:
          list.Add(LRSTransactionStatus.ToDeliver);
          list.Add(LRSTransactionStatus.Revision);
          if (type.Id == 701) {
            list.Add(LRSTransactionStatus.Elaboration);
          } else {
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Elaboration);
          }

          list.Add(LRSTransactionStatus.Control);
          list.Add(LRSTransactionStatus.ToReturn);

          break;

        case LRSTransactionStatus.Digitalization:
          list.Add(LRSTransactionStatus.Delivered);
          if (LRSWorkflowRules.IsDigitalizable(type, docType)) {
            list.Add(LRSTransactionStatus.Digitalization);
          }
          list.Add(LRSTransactionStatus.Control);

          break;

        case LRSTransactionStatus.ToDeliver:
          list.Add(LRSTransactionStatus.Control);
          AddRecordingOrElaborationStatus(list, type, docType);
          list.Add(LRSTransactionStatus.Juridic);
          list.Add(LRSTransactionStatus.OnSign);

          break;

        case LRSTransactionStatus.Archived:
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToReturn:
          list.Add(LRSTransactionStatus.Returned);
          list.Add(LRSTransactionStatus.Control);
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
      Assertion.Require(transaction, "transaction");

      var currentTask = transaction.Workflow.GetCurrentTask();

      var currentStatus = currentTask.CurrentStatus;

      if (AnyOf(currentStatus, LRSTransactionStatus.Payment, LRSTransactionStatus.Deleted)) {
        return new FixedList<WorkflowCommandType>();
      }

      if (AnyOf(currentStatus, LRSTransactionStatus.Returned, LRSTransactionStatus.Delivered)) {
        return BuildCommandTypeList(WorkflowCommandType.Reentry);
      }

      if (currentStatus == LRSTransactionStatus.Archived) {
        return BuildCommandTypeList(WorkflowCommandType.Unarchive);
      }

      if (AnyOf(currentStatus, LRSTransactionStatus.ToDeliver, LRSTransactionStatus.ToReturn)) {
        return BuildCommandTypeList(WorkflowCommandType.Finish,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Control) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.Qualification) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.OnSign) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                WorkflowCommandType.Sign,
                                WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Revision) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == LRSTransactionStatus.Received) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      if (currentStatus == LRSTransactionStatus.Reentry) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
      }

      var nextStatus = currentTask.NextStatus;

      if (AnyOf(currentStatus, LRSTransactionStatus.Recording, LRSTransactionStatus.Elaboration) &&
          nextStatus == LRSTransactionStatus.EndPoint) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (nextStatus != LRSTransactionStatus.EndPoint) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.ReturnToMe);
      }

      return new FixedList<WorkflowCommandType>();
    }


    #region Helper methods

    private void AddRecordingOrElaborationStatus(List<LRSTransactionStatus> list,
                                                 LRSTransactionType type,
                                                 LRSDocumentType docType) {
      if (LRSWorkflowRules.IsRecordingDocumentCase(type, docType)) {
        list.Add(LRSTransactionStatus.Recording);

      } else if (LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
        list.Add(LRSTransactionStatus.Elaboration);

      } else {
        list.Add(LRSTransactionStatus.Elaboration);
        list.Add(LRSTransactionStatus.Recording);
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
