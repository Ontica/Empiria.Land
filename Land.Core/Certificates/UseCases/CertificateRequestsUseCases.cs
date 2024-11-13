/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor                     *
*  Type     : CertificateRequestsUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to request land certificates within a transaction context.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;
using Empiria.Land.Transactions;

using Empiria.Land.Certificates.Data;
using Empiria.Land.Certificates.Adapters;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Use cases used to request land certificates within a transaction context.</summary>
  public class CertificateRequestsUseCases : UseCase {

    #region Constructors and parsers

    static public CertificateRequestsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CertificateRequestsUseCases>();
    }

    #endregion Constructors and parsers

    #region Query use cases

    public FixedList<CertificateRequestTypeDto> GetCertificateRequestTypes(string transactionID) {
      Assertion.Require(transactionID, nameof(transactionID));

      var transaction = LRSTransaction.Parse(transactionID);

      var wholeList = CertificateType.GetList();

      var builder = new ApplicableCertificateTypesBuilder(wholeList);

      return builder.BuildFor(transaction);
    }


    public FixedList<CertificateRequestDto> GetRequestedCertificates(string transactionID) {
      Assertion.Require(transactionID, nameof(transactionID));

      var transaction = LRSTransaction.Parse(transactionID);

      FixedList<Certificate> certificates = CertificatesData.GetTransactionCertificates(transaction);

      FixedList<CertificateDto> mappedCertificates = CertificateMapper.Map(certificates);

      return CertificateRequestMapper.Map(transaction, mappedCertificates);
    }

    #endregion Query use cases

    #region Command use cases

    public CertificateRequestDto CloseRequestedCertificate(string transactionID,
                                                           Guid certificateGuid) {
      Assertion.Require(transactionID, nameof(transactionID));

      var transaction = LRSTransaction.Parse(transactionID);

      EnsureTransactionHasCertificate(transaction, certificateGuid);

      // ToDo: control transaction / certificate edition

      CertificateDto certificate = CertificateIssuingService.CloseCertificate(certificateGuid);

      return CertificateRequestMapper.Map(transaction, certificate);
    }


    public void DeleteCertificateRequest(string transactionID, Guid certificateGuid) {
      Assertion.Require(transactionID, nameof(transactionID));

      var transaction = LRSTransaction.Parse(transactionID);

      // ToDo: control transaction / certificate edition

      EnsureTransactionHasCertificate(transaction, certificateGuid);

      CertificateIssuingService.DeleteCertificate(certificateGuid);
    }


    public CertificateRequestDto EditRequestedCertificate(string transactionID,
                                                          Guid certificateGuid,
                                                          object fields) {
      throw new NotImplementedException();
    }


    public CertificateRequestDto OpenRequestedCertificate(string transactionID,
                                                          Guid certificateGuid) {
      Assertion.Require(transactionID, nameof(transactionID));

      var transaction = LRSTransaction.Parse(transactionID);

      // ToDo: control transaction / certificate edition

      EnsureTransactionHasCertificate(transaction, certificateGuid);

      CertificateDto certificate = CertificateIssuingService.OpenCertificate(certificateGuid);

      return CertificateRequestMapper.Map(transaction, certificate);
    }


    public CertificateRequestDto RequestCertificate(string transactionID,
                                                    CertificateRequestCommand command) {
      Assertion.Require(transactionID, nameof(transactionID));
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var transaction = LRSTransaction.Parse(transactionID);

      // ToDo: control transaction / certificate edition

      CertificateType certificateType = command.GetCertificateType();

      CertificateDto certificate;

      if (command.Type == CertificateRequestCommandType.OverPersonName) {

        certificate = CertificateIssuingService.CreateOnPersonName(certificateType,
                                                                  transaction,
                                                                  command.Payload.PersonName);

      } else {
        Resource recordableSubject = command.GetRecordableSubject();

        certificate = CertificateIssuingService.CreateCertificate(certificateType,
                                                                  transaction,
                                                                  recordableSubject);
      }

      return CertificateRequestMapper.Map(transaction, certificate);
    }


    #endregion Command use cases

    #region Helpers

    static internal void EnsureTransactionHasCertificate(LRSTransaction transaction,
                                                         Guid certificateGuid) {
      Assertion.Require(transaction, nameof(transaction));

      // Assertion.Require(certificate.Transaction.Equals(transaction),
      //                  "The certificate was not requested on the given transaction.");
    }

    #endregion Helpers

  }  // class CertificateRequestsUseCases

}  // namespace Empiria.Land.Certificates.UseCases
