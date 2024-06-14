/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service  provider                       *
*  Type     : WorkflowRules                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides specific rules for Empiria Land micro workflow engine.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;

using Empiria.Security;

using Empiria.Land.Transactions.Workflow.Adapters;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Provides specific rules for Empiria Land micro workflow engine.</summary>
  public class WorkflowRules {

    public bool AllowAutoTake(LRSTransaction transaction) {
      var model = WorkflowModel.Parse(transaction.RecorderOffice.WorkflowModelId);

      return model.AllowAutoTake;
    }

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

          if (AllowAutoTake(transaction)) {
            return CanReceiveFor(user, nextStatus);
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


    public TransactionStatus NextStatusAfterReceived(LRSTransaction transaction) {
      if (transaction.RecorderOffice.Id == 101 || transaction.RecorderOffice.Id == 102) {
        return TransactionStatus.Control;
      }
      if (LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
        return TransactionStatus.Elaboration;
      } else {
        return TransactionStatus.Recording;
      }
    }


    public TransactionStatus NextStatusAfterReentry(LRSTransaction transaction) {
      if (transaction.RecorderOffice.Id == 101 || transaction.RecorderOffice.Id == 102) {
        return TransactionStatus.Control;
      }
      if (transaction.LandRecord.SecurityData.IsSigned) {
        return TransactionStatus.OnSign;
      }
      if (LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
        return TransactionStatus.Elaboration;
      } else {
        return TransactionStatus.Recording;
      }
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

      var model = WorkflowModel.Parse(transaction.RecorderOffice.WorkflowModelId);

      List <TransactionStatus> list = new List<TransactionStatus>();

      var rules = model.Rules.FindAll(x => x.From == currentStatus);

      foreach (var rule in rules) {
        if (rule.If == string.Empty && rule.IfNot == string.Empty) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.If == "IsCertificateIssueCase" && LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.If == "IsForElaborationOnly" && LRSWorkflowRules.IsForElaborationOnly(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.If == "IsArchivable" && LRSWorkflowRules.IsArchivable(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.IfNot == "IsCertificateIssueCase" && !LRSWorkflowRules.IsCertificateIssueCase(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.IfNot == "IsForElaborationOnly" && !LRSWorkflowRules.IsForElaborationOnly(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }
        if (rule.IfNot == "IsArchivable" && !LRSWorkflowRules.IsArchivable(transaction)) {
          foreach (TransactionStatus to in rule.To) {
            list.Add(to);
          }
          continue;
        }

      } // foreach rule

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
        if (nextStatus == TransactionStatus.EndPoint ||
            nextStatus == TransactionStatus.Delivered ||
            nextStatus == TransactionStatus.Returned) {
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
                                    WorkflowCommandType.ReturnToMe,
                                    WorkflowCommandType.Sign,
                                    WorkflowCommandType.SetNextStatus);
      }

      if (currentStatus == TransactionStatus.Revision) {
        return BuildCommandTypeList(WorkflowCommandType.Take,
                                    WorkflowCommandType.ReturnToMe,
                                    WorkflowCommandType.SetNextStatus,
                                    WorkflowCommandType.AssignTo);
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


    #region Helpers

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


    #endregion Helpers

  }  // class WorkflowRules

}  // namespace Empiria.Land.Transactions.Workflow
