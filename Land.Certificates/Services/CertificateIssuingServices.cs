/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Services Layer                   *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Domain service interactor               *
*  Type     : CertificateIssuingServices                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for land certificates issuing.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;
using Empiria.Land.Registration;

namespace Empiria.Land.Certificates.Services {

  /// <summary>Provides services for land certificates issuing.</summary>
  public class CertificateIssuingServices : Service {

    #region Constructors and parsers

    static public CertificateIssuingServices ServiceInteractor() {
      return Service.CreateInstance<CertificateIssuingServices>();
    }

    #endregion Constructors and parsers

    #region Services

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

    #endregion Services

  }  // class CertificateIssuingServices

}  // namespace Empiria.Land.Certificates.Services
