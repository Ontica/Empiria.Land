/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Use case interactor                     *
*  Type     : CertificateIssuingUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for land certificates issuing and retrieving.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Provides services for land certificates issuing and retrieving.</summary>
  public class CertificateIssuingUseCases : UseCase {

    #region Constructors and parsers

    static public CertificateIssuingUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CertificateIssuingUseCases>();
    }

    #endregion Constructors and parsers

    #region Query use cases

    public bool ExistsCertificateID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      var certificate = CertificatesData.TryGetCertificateWithID(certificateID);

      return (certificate != null);
    }


    public CertificateDto GetCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      return CertificateMapper.Map(certificate);
    }


    public CertificateDto GetCertificateWithID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      var certificate = CertificatesData.TryGetCertificateWithID(certificateID);

      Assertion.Require(certificate,
                        $"A certificate with number {certificateID} was not found.");

      return CertificateMapper.Map(certificate);
    }


    public FixedList<CertificateDto> GetTransactionCertificates(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      FixedList<Certificate> certificates = CertificatesData.GetTransactionCertificates(transaction);

      return CertificateMapper.Map(certificates);
    }

    #endregion Query use cases

    #region Command use cases

    public CertificateDto CreateCertificate(CertificateType certificateType,
                                            LRSTransaction transaction,
                                            Resource recordableSubject) {
      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(recordableSubject, nameof(recordableSubject));

      var certificate = Certificate.Create(certificateType, transaction, recordableSubject);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    public CertificateDto CloseCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Closed);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    public void DeleteCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Deleted);

      certificate.Save();
    }


    public CertificateDto OpenCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Pending);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }

    #endregion Command use cases

  }  // class CertificateIssuingUseCases

}  // namespace Empiria.Land.Certificates.Services
