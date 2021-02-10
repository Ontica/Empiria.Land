/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                       Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : WorkflowController                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Transactions.UseCases;

namespace Empiria.Land.Transactions.WebApi {

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
        WorkflowTaskDto currentWorkflowTask = usecases.ExecuteWorkflowCommand(transactionUID, command);

        return new SingleObjectModel(this.Request, currentWorkflowTask);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/execute-command")]
    public SingleObjectModel ExecuteWorkflowCommandForMultipleTransactions([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      using (var usecases = WorkflowUseCases.UseCaseInteractor()) {
        TransactionDto transactionDto = usecases.ExecuteWorkflowCommand(command);

        return new SingleObjectModel(this.Request, transactionDto);
      }
    }

    #endregion Web Apis

  }  // class WorkflowController

}  //namespace Empiria.Land.Transactions.WebApi
