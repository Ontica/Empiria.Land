/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Use case interactor class               *
*  Type     : TransactionCertificatesUseCases            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that links transactions with Land certificates.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Use cases that links transactions with certificates.</summary>
  public class TransactionCertificatesUseCases : UseCase {

    #region Constructors and parsers

    static public TransactionCertificatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionCertificatesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<CertificateTypeDto> CertificateTypes(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.Parse(transactionUID);

      var defaultList = CertificateType.GetList();

      var builder = new ApplicableCertificateTypesBuilder(defaultList);

      return builder.BuildFor(transaction);
    }


    public CertificateDto CreateCertificate(string transactionUID, CreateCertificateCommand command) {
      Assertion.Require(transactionUID, nameof(transactionUID));
      Assertion.Require(command, nameof(command));

      command.EnsureIsValid();

      var transaction = LRSTransaction.Parse(transactionUID);

      var certificateType = CertificateType.Parse(command.Payload.CertificateTypeUID);

      Resource recordableSubject;

      if (command.Type == CreateCertificateCommandType.OverRealEstateAntecedent) {
        recordableSubject = CreateRecordableSubjectInAntecedent(command.Payload);
      } else {
        recordableSubject = (RealEstate) Resource.ParseGuid(command.Payload.RecordableSubjectUID);
      }

      var certificate = Certificate.Create(certificateType, transaction, recordableSubject);

      return CertificateMapper.Map(certificate);
    }

    #endregion Use cases

    #region Helpers

    private Resource CreateRecordableSubjectInAntecedent(CreateCertificateCommandPayload payload) {
      return RealEstate.Parse(1885);
    }

    #endregion Helpers

  }  // class TransactionCertificatesUseCases

}  // namespace Empiria.Land.Certificates.UseCases
