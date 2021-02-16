/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                          Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : WorkflowController                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Workflow.Adapters;
using Empiria.Land.Workflow.UseCases;

namespace Empiria.Land.Workflow.WebApi {

  /// <summary>Web API used to retrive transaction workflow data and invoke commands on it.</summary>
  public class WorkflowController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/current-task")]
    public SingleObjectModel GetTransactionCurrentWorkflowTask([FromUri] string transactionUID) {

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        WorkflowTaskDto currentWorkflowTask = usecases.CurrentTask(transactionUID);

        return new SingleObjectModel(this.Request, currentWorkflowTask);
      }
    }


    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/history")]
    public CollectionModel GetTransactionWorkflowHistory([FromUri] string transactionUID) {

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        FixedList<WorkflowTaskDto> history = usecases.WorkflowHistory(transactionUID);

        return new CollectionModel(this.Request, history);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/{transactionUID:length(19)}/execute-command")]
    public SingleObjectModel ExecuteWorkflowCommandForATransaction([FromUri] string transactionUID,
                                                                   [FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        command.Payload.TransactionUID = new string[] { transactionUID };

        FixedList<WorkflowTaskDto> currentWorkflowTask = usecases.ExecuteWorkflowCommand(command);

        return new SingleObjectModel(this.Request, currentWorkflowTask[0]);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/execute-command")]
    public CollectionModel ExecuteWorkflowCommandForMultipleTransactions([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        FixedList<WorkflowTaskDto> workflowTasks = usecases.ExecuteWorkflowCommand(command);

        return new CollectionModel(this.Request, workflowTasks);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/available-operations")]
    public CollectionModel GetAvailableOperationsForMultipleTransactions([FromBody] string[] transactions) {
      base.RequireBody(transactions);

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        // FixedList<WorkflowTaskDto> workflowTasks = usecases.ExecuteWorkflowCommand(command);

        return new CollectionModel(this.Request, transactions);
      }
    }

    #endregion Web Apis

  }  // class WorkflowController

}  //namespace Empiria.Land.Workflow.WebApi
