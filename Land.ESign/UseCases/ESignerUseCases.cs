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
using System.Collections.Generic;

using Empiria.Security;
using Empiria.Services;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.ESign.Adapters;
using SeguriSign.Connector;
using SeguriSign.Connector.Adapters;


namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that performs electronic sign of documents.</summary>
  public class ESignerUseCases : UseCase {

    private readonly string ESIGN_SERVICE_PROVIDER_URL = ConfigurationData.GetString("ElectronicSignature.ServiceProvider.URL");

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

      ValidateCredentials(command);

      throw new NotImplementedException();
    }


    public void RevokeMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Revoke, true);

      ValidateCredentials(command);

      throw new NotImplementedException();
    }


    public void SignMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Sign, true);

      ValidateCredentials(command);

      FixedList<LRSTransaction> transactions = command.GetTransactions();

      FixedList<LandRecord> landRecords = GetTransactionDocumentsFor(transactions, ESignCommandType.Sign);

      var credentials = new SignerCredentialsDto {
        UserName = command.Credentials.UserID,
        Password = command.Credentials.Password
      };

      var service = new ESignService(ESIGN_SERVICE_PROVIDER_URL, credentials);

      foreach (var record in landRecords) {

        ESignDataDto signData = service.Sign($"{record.SecurityData.SecurityHash}{record.SecurityData.DigitalSeal}",
                                             $"CAT_sello_registral_{record.UID}_{record.SecurityData.SecurityHash}");

        var landSignData = new LandESignData {
          DocumentID = signData.DocumentID,
          DocumentName = signData.DocumentName,
          Digest = signData.Digest,
          Signature = signData.Signature,
          SignedTime = signData.LocalSignDate
        };

        record.Security.ElectronicSign(landSignData);
      }

    }


    public void UnrefuseMyTransactionDocuments(ESignCommand command) {
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid(ESignCommandType.Unrefuse, true);

      ValidateCredentials(command);

      throw new NotImplementedException();
    }

    #endregion Use cases

    #region Helpers

    private FixedList<LandRecord> GetTransactionDocumentsFor(FixedList<LRSTransaction> transactions,
                                                             ESignCommandType commandType) {
      var list = new List<LandRecord>(transactions.Count);

      foreach (var transaction in transactions) {
        list.Add(transaction.LandRecord);
      }

      return list.ToFixedList();
    }

    private void ValidateCredentials(ESignCommand command) {
      var entropy = SecurityTokenGenerator.PopToken(command.Credentials, SecurityTokenType.ElectronicSign);

      command.Credentials.Password = Cryptographer.Decrypt(command.Credentials.Password, entropy, true);
    }

    #endregion Helpers

  } // class ESignerUseCases

} // namespace Empiria.Land.ESign.UseCases
