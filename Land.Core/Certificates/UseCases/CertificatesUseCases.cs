/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor                     *
*  Type     : CertificatesUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for retrive land certificates.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Certificates.Data;
using Empiria.Land.Certificates.Adapters;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Use cases for retrive land certificates.</summary>
  public class CertificatesUseCases : UseCase {

    #region Constructors and parsers

    static public CertificatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CertificatesUseCases>();
    }

    #endregion Constructors and parsers

    #region Query use cases

    public bool ExistsCertificateID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      return CertificatesData.TryGetCertificateWithID(certificateID) != null;
    }


    public CertificateDto GetCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      return CertificateMapper.Map(certificate);
    }


    public CertificateDto GetCertificateWithID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      var certificate = CertificatesData.TryGetCertificateWithID(certificateID);

      Assertion.Require(certificate, $"A certificate with number {certificateID} was not found.");

      return CertificateMapper.Map(certificate);
    }

    #endregion Query use cases

  }  // class CertificatesUseCases

}  // namespace Empiria.Land.Certificates.UseCases
