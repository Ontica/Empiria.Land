/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Services Layer                   *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Domain service interactor               *
*  Type     : SearchCertificatesServices                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for land certificates searching.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates.Services {

  /// <summary>Provides services for land certificates searching.</summary>
  public class SearchCertificatesServices : Service {

    #region Constructors and parsers

    static public SearchCertificatesServices ServiceInteractor() {
      return Service.CreateInstance<SearchCertificatesServices>();
    }

    #endregion Constructors and parsers

    #region Services

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

    #endregion Services

  }  // class SearchCertificatesService

}  // namespace Empiria.Land.Certificates.Services
