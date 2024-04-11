﻿/* Empiria Land **********************************************************************************************
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
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates.Adapters;

namespace Empiria.Land.Certificates {

  /// <summary>Provides services for land certificates issuing.</summary>
  static internal class CertificateIssuingService {

    #region Services

    static internal CertificateDto CloseCertificate(Guid certificateGuid) {

      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Closed);

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


    static internal void DeleteCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Deleted);

      certificate.Save();
    }


    static internal CertificateDto OpenCertificate(Guid certificateGuid) {
      var certificate = Certificate.Parse(certificateGuid.ToString());

      certificate.SetStatus(CertificateStatus.Pending);

      certificate.Save();

      return CertificateMapper.Map(certificate);
    }


    #endregion Services

  }  // class CertificateIssuingService

}  //namespace Empiria.Land.Certificates
