/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Use case interactor                     *
*  Type     : RequestCertificatesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to request land certificates within a transaction context.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates;
using Empiria.Land.Certificates.Services;

namespace Empiria.Land.Transactions.CertificateRequests.UseCases {

  /// <summary>Use cases used to request land certificates within a transaction context.</summary>
  public class RequestCertificatesUseCases : UseCase {

    #region Constructors and parsers

    static public RequestCertificatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<RequestCertificatesUseCases>();
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

      using (var searcher = SearchCertificatesServices.ServiceInteractor()) {
        FixedList<CertificateDto> certificates = searcher.GetTransactionCertificates(transaction);

        return CertificateRequestMapper.Map(transaction, certificates);
      }
    }


    public CertificateRequestDto RequestCertificate(string transactionID,
                                                    CertificateRequestCommand command) {
      Assertion.Require(transactionID, nameof(transactionID));
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var transaction = LRSTransaction.Parse(transactionID);

      var certificateType = (CertificateType) CertificateType.Parse(command.Payload.CertificateTypeUID);

      Resource recordableSubject;

      if (command.Type == CertificateRequestCommandType.OverRealEstateAntecedent) {
        recordableSubject = CreateRecordableSubjectInAntecedent(command.Payload);
      } else {
        recordableSubject = (RealEstate) Resource.ParseGuid(command.Payload.RecordableSubjectUID);
      }

      using (var certificateCreator = CertificateIssuingServices.ServiceInteractor()) {
        CertificateDto certificate = certificateCreator.CreateCertificate(certificateType, transaction, recordableSubject);

        return CertificateRequestMapper.Map(transaction, certificate);
      }
    }


    public CertificateRequestDto OpenRequestedCertificate(string transactionID,
                                                          Guid certificateGuid) {
      throw new NotImplementedException();
    }

    #endregion Use cases

    #region Helpers

    private Resource CreateRecordableSubjectInAntecedent(CertificateRequestCommandPayload payload) {
      return RealEstate.Parse(1885);
    }

    #endregion Helpers

  }  // class CertificateRequestUseCases

}  // namespace Empiria.Land.Transactions.CertificateRequests.UseCases
