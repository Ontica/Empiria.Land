/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Service provider                        *
*  Type     : CertificateIssuingService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for land certificates issuing.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

using Empiria.Land.Transactions;

using Empiria.Land.Certificates.Adapters;
using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates {

  /// <summary>Provides services for land certificates issuing.</summary>
  static internal class CertificateIssuingService {

    #region Services

    static internal CertificateDto CloseCertificate(Guid certificateGuid) {

      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.Close();

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    static internal CertificateDto CreateCertificate(CertificateType certificateType,
                                                     LRSTransaction transaction,
                                                     Resource recordableSubject) {

      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(recordableSubject, nameof(recordableSubject));

      var certificate = Certificate.Create(certificateType, transaction, recordableSubject);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    static internal CertificateDto CreateOnPersonName(CertificateType certificateType,
                                                     LRSTransaction transaction,
                                                     string personName) {

      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(personName, nameof(personName));

      var certificate = Certificate.CreateOnPersonName(certificateType, transaction, personName);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    static internal CertificateDto CreateOnRealEstateDescription(CertificateType certificateType,
                                                                 LRSTransaction transaction,
                                                                 string realEstateDescription) {

      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(realEstateDescription, nameof(realEstateDescription));

      var certificate = Certificate.CreateOnRealEstateDescription(certificateType, transaction,
                                                                  realEstateDescription);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    static internal void DeleteCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.Delete();

      certificate.Save();
    }


    static internal FixedList<CertificateDto> GetRecordableSubjectIssuedCertificates(Resource recordableSubject) {
      if (recordableSubject.IsEmptyInstance) {
        return new FixedList<CertificateDto>();
      }

      var certificates = CertificatesData.GetRecordableSubjectIssuedCertificates(recordableSubject);

      return CertificateMapper.Map(certificates);
    }


    static internal FixedList<CertificateDto> GetTransactionCertificates(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<CertificateDto>();
      }

      var certificates = CertificatesData.GetTransactionCertificates(transaction);

      return CertificateMapper.Map(certificates);
    }


    static internal CertificateDto OpenCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.Open();

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }

    #endregion Services

  }  // class CertificateIssuingService

}  //namespace Empiria.Land.Certificates
