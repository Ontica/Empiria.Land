/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Domain Layer                            *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Service provider                        *
*  Type     : LandDocumentsSigner                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides electrnic sign services for Empiria Land documents.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using SeguriSign.Connector.Adapters;
using SeguriSign.Connector;

using Empiria.Land.ESign.Adapters;
using System.Collections.Generic;

namespace Empiria.Land.ESign {

  internal class LandDocumentsSigner {

    #region Fields

    private readonly string ESIGN_SERVICE_PROVIDER_URL =
                                     ConfigurationData.GetString("ElectronicSignature.ServiceProvider.URL");

    private readonly ESignService _signServiceProvider;

    #endregion Fields

    #region Constructors and parsers

    internal LandDocumentsSigner(SignCredentialsDto credentials) {
      Assertion.Require(credentials, nameof(credentials));

      _signServiceProvider = new ESignService(ESIGN_SERVICE_PROVIDER_URL,
                                              new SignerCredentialsDto {
                                                UserName = credentials.UserID,
                                                Password = credentials.Password
                                              });
    }

    #endregion Constructors and parsers

    #region Methods


    internal void RevokeSignForLandRecord(LandRecord record) {
      record.Security.RevokeSign();
    }


    internal void RevokeSignForTransactionDocuments(FixedList<LRSTransaction> transactions) {
      FixedList<LandRecord> landRecords = GetTransactionDocumentsFor(transactions, ESignCommandType.Revoke);

      foreach (var record in landRecords) {
        RevokeSignForLandRecord(record);
      }
    }


    internal void SignLandRecord(LandRecord record) {
      var contentToSign = $"{record.SecurityData.SecurityHash}{record.SecurityData.DigitalSeal}";

      var documentUID = $"CAT_sello_registral_{record.UID}_{record.SecurityData.SecurityHash}";

      ESignDataDto signData = _signServiceProvider.Sign(contentToSign, documentUID);

      LandESignData landSignData = MapToLandESignData(signData);

      record.Security.ElectronicSign(landSignData);
    }


    internal void SignTransactionDocuments(FixedList<LRSTransaction> transactions) {
      FixedList<LandRecord> landRecords = GetTransactionDocumentsFor(transactions, ESignCommandType.Sign);

      foreach (var record in landRecords) {
        SignLandRecord(record);
      }
    }

    #endregion Methods

    #region Helpers

    private FixedList<LandRecord> GetTransactionDocumentsFor(FixedList<LRSTransaction> transactions,
                                                             ESignCommandType commandType) {
      var list = new List<LandRecord>(transactions.Count);

      foreach (var transaction in transactions) {
        var landRecord = transaction.LandRecord;

        if (landRecord.IsEmptyInstance ||
           !landRecord.IsClosed ||
            landRecord.IsHistoricRecord) {
          continue;
        }

        if (commandType == ESignCommandType.Sign && landRecord.SecurityData.IsUnsigned) {
          list.Add(transaction.LandRecord);
        } else if (commandType == ESignCommandType.Revoke && landRecord.SecurityData.IsSigned) {
          list.Add(transaction.LandRecord);
        }
      }

      return list.ToFixedList();
    }


    private LandESignData MapToLandESignData(ESignDataDto signData) {
      return new LandESignData {
        DocumentID = signData.DocumentID,
        DocumentName = signData.DocumentName,
        Digest = signData.Digest,
        Signature = signData.Signature,
        SignedTime = signData.LocalSignDate
      };
    }

    #endregion Helpers

  }  // class LandDocumentsSigner

}  // namespace Empiria.Land.ESign
