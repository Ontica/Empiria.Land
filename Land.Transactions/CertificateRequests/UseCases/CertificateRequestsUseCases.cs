/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Use case interactor                     *
*  Type     : CertificateRequestsUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to request land certificates within a transaction context.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates;

using Empiria.Land.Transactions.CertificateRequests.Providers;

namespace Empiria.Land.Transactions.CertificateRequests.UseCases {

  /// <summary>Use cases used to request land certificates within a transaction context.</summary>
  public class CertificateRequestsUseCases : UseCase {

    #region Constructors and parsers

    static public CertificateRequestsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CertificateRequestsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public CertificateRequestDto CloseRequestedCertificate(string transactionID,
                                                           Guid certificateGuid) {
      throw new NotImplementedException();
    }


    public CertificateRequestDto EditRequestedCertificate(string transactionID,
                                                          Guid certificateGuid,
                                                          object fields) {
      throw new NotImplementedException();
    }


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

      FixedList<CertificateDto> certificates = CertificatesProvider.GetTransactionCertificates(transaction);

      return CertificateRequestMapper.Map(transaction, certificates);
    }


    public CertificateRequestDto OpenRequestedCertificate(string transactionID,
                                                          Guid certificateGuid) {
      throw new NotImplementedException();
    }


    public CertificateRequestDto RequestCertificate(string transactionID,
                                                    CertificateRequestCommand command) {
      Assertion.Require(transactionID, nameof(transactionID));
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var transaction = LRSTransaction.Parse(transactionID);

      var certificateType = command.GetCertificateType();

      var recordableSubject =  command.GetRecordableSubject();

      CertificateDto certificate = CertificatesProvider.CreateCertificate(certificateType,
                                                                          transaction,
                                                                          recordableSubject);

      return CertificateRequestMapper.Map(transaction, certificate);
    }


    #endregion Use cases

  }  // class CertificateRequestsUseCases

}  // namespace Empiria.Land.Transactions.CertificateRequests.UseCases
