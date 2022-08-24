/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                        Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Command controller                    *
*  Type     : TransactionWorkflowExecutionController       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web api that executes land transactions workflow commands.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Transactions.UseCases;
using Empiria.Land.Transactions.Workflow.Services;

namespace Empiria.Land.Transactions.Workflow.WebApi {

  /// <summary>Command web api that executes land transactions workflow commands.</summary>
  public class TransactionWorkflowExecutionController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/workflow/search-and-assert-command-execution")]
    public SingleObjectModel AssertTransactionWorkflowCommandExecution([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      Assertion.Require(command.Payload.SearchUID, "payload.searchUID field must be provided.");

      TransactionDescriptor transaction;

      using (var usecase = TransactionUseCases.UseCaseInteractor()) {

        transaction = usecase.SearchTransaction(command.Payload.SearchUID);
      }

      using (var engine = TransactionWorkflowExecutionUseCases.UseCaseInteractor()) {

        command.Payload.TransactionUID = new string[] { transaction.UID };

        engine.AssertWorkflowCommandExecution(command);

        return new SingleObjectModel(this.Request, transaction);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/{transactionUID:length(19)}/execute-command")]
    public SingleObjectModel ExecuteWorkflowCommandForOneTransaction([FromUri] string transactionUID,
                                                                     [FromBody] WorkflowCommand command) {

      base.RequireBody(command);

      using (var engine = TransactionWorkflowExecutionUseCases.UseCaseInteractor()) {

        command.Payload.TransactionUID = new string[] { transactionUID };

        FixedList<WorkflowTaskDto> currentWorkflowTask = engine.ExecuteWorkflowCommand(command);

        return new SingleObjectModel(this.Request, currentWorkflowTask[0]);
      }
    }


    [HttpPost]
    [Route("v5/land/workflow/execute-command")]
    public CollectionModel ExecuteWorkflowCommandForMultipleTransactions([FromBody] WorkflowCommand command) {
      base.RequireBody(command);

      using (var engine = TransactionWorkflowExecutionUseCases.UseCaseInteractor()) {

        FixedList<WorkflowTaskDto> workflowTasks = engine.ExecuteWorkflowCommand(command);

        return new CollectionModel(this.Request, workflowTasks);
      }
    }


    #endregion Web Apis

  }  // class TransactionWorkflowExecutionController

}  //namespace Empiria.Land.Transactions.Workflow.WebApi
