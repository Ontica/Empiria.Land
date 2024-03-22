/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign Services                   Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests class                   *
*  Type     : ESignerUseCasesTests                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for electronic sign of documents.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Security;
using Empiria.Tests;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.UseCases;

namespace Empiria.Land.Tests.ESign {

  /// <summary>Test cases for electronic sign of documents.</summary>
  public class ESignerUseCasesTests {

    #region Use cases initialization

    private readonly ESignerUseCases _usecases;

    public ESignerUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = ESignerUseCases.UseCaseInteractor();
    }

    ~ESignerUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization

    #region Facts

    [Fact]
    public void Should_Generate_ESign_Command_Security_Token() {
      string sut = GenerateESignCommandSecurityToken();

      Assert.NotEmpty(sut);
    }


    [Fact]
    public void Should_Refuse_My_Transaction_Documents() {
      ESignCommand command = CreateESignCommand(ESignCommandType.Refuse);

      _usecases.RefuseMyTransactionDocuments(command);

      Assert.True(true);
    }


    [Fact]
    public void Should_Revoke_My_Transaction_Documents() {
      ESignCommand command = CreateESignCommand(ESignCommandType.Revoke);

      _usecases.RevokeMyTransactionDocuments(command);

      Assert.True(true);
    }


    [Fact]
    public void Should_Sign_My_Transaction_Documents() {
      ESignCommand command = CreateESignCommand(ESignCommandType.Sign);

      _usecases.SignMyTransactionDocuments(command);

      Assert.True(true);
    }


    [Fact]
    public void Should_Unrefuse_My_Transaction_Documents() {
      ESignCommand command = CreateESignCommand(ESignCommandType.Unrefuse);

      _usecases.UnrefuseMyTransactionDocuments(command);

      Assert.True(true);
    }

    #endregion Facts

    #region Helpers

    private ESignCommand CreateESignCommand(ESignCommandType commandType) {
      var token = GenerateESignCommandSecurityToken();

      var password = Cryptographer.Encrypt(EncryptionMode.Pure, TestingConstants.ESIGN_PASSWORD, token);

      return new ESignCommand {
        CommandType = commandType,
        Credentials = new SignCredentialsDto {
           AppKey = ExecutionServer.CurrentPrincipal.ClientApp.Key,
           UserID = TestingConstants.ESIGN_USERID,
           Password = password,
           UserHostAddress = ExecutionServer.CurrentPrincipal.Session.UserHostAddress,
        },
        TransactionUIDs = new FixedList<string>(new[] { "TR-ZS-37XE8-9YJ23-0", "TR-ZS-92WB8-5FS47-7" })
      };
    }

    private string GenerateESignCommandSecurityToken() {
      var credentials = new UserCredentialsDto {
        AppKey = ExecutionServer.CurrentPrincipal.ClientApp.Key,
        UserID = TestingConstants.ESIGN_USERID,
        UserHostAddress = ExecutionServer.CurrentPrincipal.Session.UserHostAddress,
      };
      return _usecases.GenerateESignCommandSecurityToken(credentials);
    }

    #endregion Helpers

  }  // class ESignerUseCasesTests

}  // namespace Empiria.Land.Tests.ESign
