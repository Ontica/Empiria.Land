/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Use cases Layer                         *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Use case interactor class               *
*  Type     : ESignerUseCases                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that performs electronic sign of documents.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Security;
using Empiria.Services;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.ESign.Adapters;

namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that performs electronic sign of documents.</summary>
  public class ESignerUseCases : UseCase {

    #region Constructors and parsers

    protected ESignerUseCases() {
      // no-op
    }

    static public ESignerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ESignerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public string GenerateESignCommandSecurityToken(UserCredentialsDto credentials) {
      Assertion.Require(credentials, nameof(credentials));

      return SecurityTokenGenerator.GenerateToken(credentials, SecurityTokenType.ElectronicSign);
    }


    public void RefuseMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Refuse, true);

      PrepareCredentials(command);

      throw new NotImplementedException();
    }


    public void RevokeMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Revoke, true);

      PrepareCredentials(command);

      FixedList<LRSTransaction> transactions = command.GetTransactions();

      var signer = new LandDocumentsSigner(command.Credentials);

      signer.RevokeSignForTransactionDocuments(transactions);
    }


    public void SignMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Sign, true);

      PrepareCredentials(command);

      FixedList<LRSTransaction> transactions = command.GetTransactions();

      var signer = new LandDocumentsSigner(command.Credentials);

      signer.SignTransactionDocuments(transactions);

    }


    public void UnrefuseMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Unrefuse, true);

      PrepareCredentials(command);

      throw new NotImplementedException();
    }

    #endregion Use cases

    #region Helpers

    private void PrepareCredentials(ESignCommand command) {
      var entropy = SecurityTokenGenerator.PopToken(command.Credentials, SecurityTokenType.ElectronicSign);

      command.Credentials.Password = Cryptographer.Decrypt(command.Credentials.Password, entropy, true);
    }

    #endregion Helpers

  } // class ESignerUseCases

} // namespace Empiria.Land.ESign.UseCases
