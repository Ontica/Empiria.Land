/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Services Layer                          *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Services provider                       *
*  Type     : TransactionWorkflowDataServices            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Services for get land transactions workflow data.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Workflow.Services {

  /// <summary>Services for get land transactions workflow data.</summary>
  public class TransactionWorkflowDataServices : Service {

    #region Constructors and parsers

    protected TransactionWorkflowDataServices() {
      // no-op
    }

    static public TransactionWorkflowDataServices Provider() {
      return Service.CreateInstance<TransactionWorkflowDataServices>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<ApplicableCommandDto> GetAllApplicableUserCommands() {
      var user = ExecutionServer.CurrentContact;

      var workflowRules = new WorkflowRules();

      var aggregator = new WorkflowCommandsAggregator(workflowRules);

      return aggregator.GetAllApplicableUserCommands(user);
    }


    public FixedList<ApplicableCommandDto> GetApplicableCommandsForMultipleTransactions(string[] transactionUIDs) {
      if (transactionUIDs == null || transactionUIDs.Length == 0) {
        return new FixedList<ApplicableCommandDto>();
      }

      var user = ExecutionServer.CurrentContact;

      var workflowRules = new WorkflowRules();

      var aggregator = new WorkflowCommandsAggregator(workflowRules);

      foreach (var uid in transactionUIDs) {
        var transaction = LRSTransaction.Parse(uid);

        aggregator.Aggregate(transaction, user);
      }

      return aggregator.GetApplicableCommands();
    }


    public WorkflowTaskDto CurrentTask(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var currentWorkflowTask = transaction.Workflow.GetCurrentTask();

      return WorkflowTaskMapper.Map(currentWorkflowTask);
    }

    public FixedList<WorkflowTaskDto> WorkflowHistory(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var workflowTasks = transaction.Workflow.Tasks;

      return WorkflowTaskMapper.Map(workflowTasks);
    }

    #endregion Use cases

  }  // class TransactionWorkflowDataServices

}  // namespace Empiria.Land.Transactions.Workflow.Services
