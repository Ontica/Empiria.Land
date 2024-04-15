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
using System.Collections.Generic;

using Empiria.Json;

using Empiria.Land.Registration;
using Empiria.Land.Transactions;

using SeguriSign.Connector;
using SeguriSign.Connector.Adapters;

using Empiria.Land.ESign.Adapters;

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

      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
                        "El servicio de firma electrónica no está habilitado.");

      _signServiceProvider = new ESignService(ESIGN_SERVICE_PROVIDER_URL,
                                              new SignerCredentialsDto {
                                                UserName = credentials.UserID,
                                                Password = credentials.Password
                                              });
    }

    #endregion Constructors and parsers

    #region Methods

    internal void RevokeLandRecordSign(LandRecord record) {
      var validator = new LandRecordValidator(record);

      validator.AssertCanRevokeSign();

      record.Security.RevokeSign();
    }


    internal void RevokeTransactionDocumentsSigns(FixedList<LRSTransaction> transactions) {
      Assertion.Require(transactions, nameof(transactions));

      FixedList<LandRecord> landRecords = GetTransactionDocumentsFor(transactions, ESignCommandType.Revoke);

      foreach (var record in landRecords) {
        RevokeLandRecordSign(record);
      }
    }


    internal void SignLandRecord(LandRecord record) {
      var validator = new LandRecordValidator(record);

      validator.AssertCanBeElectronicallySigned();

      record.Security.PrepareForElectronicSign();

      var contentToSign = GetContentToSign(record);

      var documentUID = $"CAT_sello_registral_{record.UID}_{record.SecurityData.SecurityHash}";

      ESignDataDto signData = _signServiceProvider.Sign(contentToSign, documentUID);

      LandESignData landSignData = MapToLandESignData(signData);

      record.Security.ElectronicSign(landSignData);
    }


    internal void SignTransactionDocuments(FixedList<LRSTransaction> transactions) {
      Assertion.Require(transactions, nameof(transactions));

      FixedList<LandRecord> landRecords = GetTransactionDocumentsFor(transactions, ESignCommandType.Sign);

      foreach (var record in landRecords) {
        SignLandRecord(record);
      }
    }

    #endregion Methods

    #region Helpers

    private string GetContentToSign(LandRecord record) {
      return new JsonObject {
        { "Documento", record.UID },
        { "Tipo de documento", "Sello registral" },
        { "Identificador de firmado", record.SecurityData.SignGuid },
        { "Código de verificación", record.SecurityData.SecurityHash },
        { "Sello digital", record.SecurityData.DigitalSeal },
        { "Trámite", record.Transaction.UID },
        { "Fecha de presentación", record.Transaction.PresentationTime },
        { "Fecha de registro", record.AuthorizationTime }
      }.ToString(true);
    }


    private FixedList<LandRecord> GetTransactionDocumentsFor(FixedList<LRSTransaction> transactions,
                                                             ESignCommandType commandType) {
      var list = new List<LandRecord>(transactions.Count);

      foreach (var transaction in transactions) {
        var landRecord = transaction.LandRecord;

        var validator = new LandRecordValidator(landRecord);

        if (commandType == ESignCommandType.Sign) {

          validator.AssertCanBeElectronicallySigned();
          list.Add(transaction.LandRecord);

        } else if (commandType == ESignCommandType.Revoke) {

          validator.AssertCanRevokeSign();
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
