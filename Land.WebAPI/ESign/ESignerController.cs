/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Command Controller                    *
*  Type     : ESignerController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command web api used to sign documents.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Security;
using Empiria.WebApi;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;

namespace Empiria.Land.ESign.WebAPI {

  /// <summary>Command web api used to sign documents.</summary>
  public class ESignerController : WebApiController {

    #region Web apis

    [HttpPost]
    [Route("v5/land/electronic-sign/generate-esign-command-security-token")]
    public SingleObjectModel GenerateESignCommandSecurityToken([FromBody] UserCredentialsDto credentials) {

      PrepareAuthenticationFields(credentials);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {
        string token = usecases.GenerateESignCommandSecurityToken(credentials);

        return new SingleObjectModel(base.Request, token);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/transactions/mine/refuse")]
    public NoDataModel RefuseMyTransactionDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.RefuseMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/transactions/mine/revoke")]
    public NoDataModel RevokeMyTransactionDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.RevokeMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/transactions/mine/sign")]
    public NoDataModel SignMyTransactionDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.SignMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }


    [HttpPost]
    [Route("v5/land/electronic-sign/execute-task/transactions/mine/unrefuse")]
    public NoDataModel UnrefuseMyTransactionDocuments([FromBody] ESignCommand command) {

      EnsureValidESignCommand(command);

      using (var usecases = ESignerUseCases.UseCaseInteractor()) {

        usecases.UnrefuseMyTransactionDocuments(command);

        return new NoDataModel(this.Request);
      }
    }

    #endregion Web apis

    #region Helpers

    private void PrepareAuthenticationFields(UserCredentialsDto credentials) {
      base.RequireBody(credentials);

      credentials.AppKey = base.GetClientApplication().Key;
      credentials.UserHostAddress = base.GetClientIpAddress();
    }


    private void EnsureValidESignCommand(ESignCommand command) {
      base.RequireBody(command);
      Assertion.Require(command.Credentials, "credentials");

      command.Credentials.AppKey = base.GetClientApplication().Key;
      command.Credentials.UserHostAddress = base.GetClientIpAddress();
    }

    #endregion Helpers

  } // class ESignerController

} // namespace Empiria.Land.ESign.WebAPI
