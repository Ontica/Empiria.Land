/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Use case interactor class               *
*  Type     : CertificatesUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for Land certificates retrieving.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates.UseCases {

  /// <summary>Use cases for Land certificates retrieving.</summary>
  public class CertificatesUseCases : UseCase {

    #region Constructors and parsers

    static public CertificatesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CertificatesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public bool ExistsCertificateID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      var certificate = CertificatesData.TryGetCertificateWithID(certificateID);

      return (certificate != null);
    }


    public CertificateDto GetCertificateWithID(string certificateID) {
      Assertion.Require(certificateID, nameof(certificateID));

      var certificate = CertificatesData.TryGetCertificateWithID(certificateID);

      Assertion.Require(certificate,
                        $"A certificate with number {certificateID} was not found.");

      return CertificateMapper.Map(certificate);
    }


    #endregion Use cases

  }  // class CertificatesUseCases

}  // namespace Empiria.Land.Certificates.UseCases
