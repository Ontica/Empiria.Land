/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query controller                      *
*  Type     : TransactionWorkflowQueryController           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api used to retrive transaction workflow data.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.Workflow.UseCases;

namespace Empiria.Land.Transactions.Workflow.WebApi {

  /// <summary>Query web api used to retrive transaction workflow data.</summary>
  public class TransactionWorkflowQueryController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/workflow/all-commands")]
    [Route("v5/land/workflow/all-command-types")]
    public CollectionModel AllApplicableUserCommandTypes() {

      using (var workflow = TransactionWorkflowQueryUseCases.UseCaseInteractor()) {

        FixedList<ApplicableCommandDto> commandTypes = workflow.GetAllApplicableUserCommands();

        return new CollectionModel(this.Request, commandTypes);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/applicable-commands")]
    [Route("v5/land/workflow/applicable-command-types")]
    public CollectionModel GetApplicableCommandTypesForMultipleTransactuions([FromBody] string[] transactions) {
      base.RequireBody(transactions);

      using (var workflow = TransactionWorkflowQueryUseCases.UseCaseInteractor()) {

        FixedList<ApplicableCommandDto> commandTypes = workflow.GetApplicableCommandsForMultipleTransactions(transactions);

        return new CollectionModel(this.Request, commandTypes);
      }
    }


    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/current-task")]
    public SingleObjectModel GetTransactionCurrentWorkflowTask([FromUri] string transactionUID) {

      using (var workflow = TransactionWorkflowQueryUseCases.UseCaseInteractor()) {

        WorkflowTaskDto currentWorkflowTask = workflow.CurrentTask(transactionUID);

        return new SingleObjectModel(this.Request, currentWorkflowTask);
      }
    }


    [HttpGet]
    [Route("v5/land/workflow/{transactionUID:length(19)}/history")]
    public CollectionModel GetTransactionWorkflowHistory([FromUri] string transactionUID) {

      using (var workflow = TransactionWorkflowQueryUseCases.UseCaseInteractor()) {
        FixedList<WorkflowTaskDto> history = workflow.WorkflowHistory(transactionUID);

        return new CollectionModel(this.Request, history);
      }
    }

    #endregion Web Apis

  }  // class TransactionWorkflowQueryController

}  //namespace Empiria.Land.Transactions.Workflow.WebApi
