/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : WorkflowController                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Workflow.Services;

namespace Empiria.Land.Transactions.Workflow.WebApi {

  /// <summary>Web API used to retrive transaction workflow data and invoke commands on it.</summary>
  public class WorkflowController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/workflow/all-commands")]
    [Route("v5/land/workflow/all-command-types")]
    public CollectionModel AllApplicableUserCommandTypes() {

      using (var workflow = WorkflowServices.Provider()) {

        FixedList<ApplicableCommandDto> commandTypes = workflow.AllApplicableUserCommands();

        return new CollectionModel(this.Request, commandTypes);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/applicable-commands")]
    [Route("v5/land/workflow/applicable-command-types")]
    public CollectionModel ApplicableCommandTypes([FromBody] string[] transactions) {
      base.RequireBody(transactions);

      using (var workflow = WorkflowServices.Provider()) {

        FixedList<ApplicableCommandDto> commandTypes = workflow.ApplicableCommands(transactions);

        return new CollectionModel(this.Request, commandTypes);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/search-and-assert-command-execution")]
    public SingleObjectModel AssertWorkflowCommandExecutionForMultipleTransactions([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      Assertion.Require(command.Payload.SearchUID, "payload.searchUID field must be provided.");

      using (var workflow = WorkflowServices.Provider()) {

        TransactionDescriptor transaction = workflow.SearchTransaction(command.Payload.SearchUID);

        command.Payload.TransactionUID = new string[] { transaction.UID };

        workflow.AssertWorkflowCommandExecution(command);

        return new SingleObjectModel(this.Request, transaction);
      }
    }


    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/current-task")]
    public SingleObjectModel GetTransactionCurrentWorkflowTask([FromUri] string transactionUID) {

      using (var workflow = WorkflowServices.Provider()) {

        WorkflowTaskDto currentWorkflowTask = workflow.CurrentTask(transactionUID);

        return new SingleObjectModel(this.Request, currentWorkflowTask);
      }
    }


    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/history")]
    public CollectionModel GetTransactionWorkflowHistory([FromUri] string transactionUID) {

      using (var workflow = WorkflowServices.Provider()) {
        FixedList<WorkflowTaskDto> history = workflow.WorkflowHistory(transactionUID);

        return new CollectionModel(this.Request, history);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/{transactionUID:length(19)}/execute-command")]
    public SingleObjectModel ExecuteWorkflowCommandForATransaction([FromUri] string transactionUID,
                                                                   [FromBody] WorkflowCommand command) {

      base.RequireBody(command);

      using (var workflow = WorkflowServices.Provider()) {

        command.Payload.TransactionUID = new string[] { transactionUID };

        FixedList<WorkflowTaskDto> currentWorkflowTask = workflow.ExecuteWorkflowCommand(command);

        return new SingleObjectModel(this.Request, currentWorkflowTask[0]);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/execute-command")]
    public CollectionModel ExecuteWorkflowCommandForMultipleTransactions([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      using (var workflow = WorkflowServices.Provider()) {

        FixedList<WorkflowTaskDto> workflowTasks = workflow.ExecuteWorkflowCommand(command);

        return new CollectionModel(this.Request, workflowTasks);
      }
    }


    #endregion Web Apis

  }  // class WorkflowController

}  //namespace Empiria.Land.Transactions.Workflow.WebApi
