/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Command Controller                    *
*  Type     : DocumentESignController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web api used to sign or unsign one or more documents.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;

namespace Empiria.Land.ESign.WebApi {

  /// <summary>Command web api used to sign or unsign one or more documents.</summary>
  public class DocumentESignController : WebApiController {

    #region Web apis

    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/documents/mine/refuse")]
    public NoDataModel RefuseDocumentsESign([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.RefuseMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/documents/mine/revoke")]
    public NoDataModel RevokeMyDocumentsESign([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.RevokeMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/documents/mine/sign")]
    public NoDataModel SignMyDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.SignMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/documents/mine/unrefuse")]
    public NoDataModel UnrefuseMyDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.UnrefuseMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }

    #endregion Web apis

    #region Helpers

    private void EnsureValidESignCommand(ESignCommand command) {
      base.RequireBody(command);
      Assertion.Require(command.Credentials, "credentials");

      command.Credentials.AppKey = base.GetClientApplication().Key;
      command.Credentials.UserHostAddress = base.GetClientIpAddress();
    }

    #endregion Helpers

  } // class DocumentESignController

} // namespace Empiria.Land.ESign.WebApi
