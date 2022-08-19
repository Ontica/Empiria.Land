/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Providers Layer                         *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Service proxy                           *
*  Type     : CertificatesProvider                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Service proxy for land certificates issuing services.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates;
using Empiria.Land.Certificates.Services;

namespace Empiria.Land.Transactions.CertificateRequests.Providers {

  /// <summary>Provides services for land certificates issuing.</summary>
  static public class CertificatesProvider {

    #region Services


    static internal CertificateDto CreateCertificate(CertificateType certificateType,
                                                     LRSTransaction transaction,
                                                     Resource recordableSubject) {

      using (var certificateCreator = CertificateIssuingServices.ServiceInteractor()) {

        return certificateCreator.CreateCertificate(certificateType, transaction, recordableSubject);
      }
    }


    static internal CertificateDto GetTransactionCertificate(LRSTransaction transaction,
                                                             Guid certificateGuid) {

      using (var service = SearchCertificatesServices.ServiceInteractor()) {
        CertificateDto certificate = service.GetCertificate(certificateGuid);

        EnsureTransactionHasCertificate(transaction, certificate);

        return certificate;
      }
    }


    static internal FixedList<CertificateDto> GetTransactionCertificates(LRSTransaction transaction) {

      using (var searcher = SearchCertificatesServices.ServiceInteractor()) {

        return searcher.GetTransactionCertificates(transaction);
      }
    }


    #endregion Services

    #region Helpers

    static private void EnsureTransactionHasCertificate(LRSTransaction transaction,
                                                        CertificateDto certificate) {
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(certificate, nameof(certificate));

      //Assertion.Require(certificate.Transaction.Equals(transaction),
      //                  "The certificate was not requested on the given transaction.");
    }

    #endregion Helpers

  }  // class CertificatesProvider

}  //namespace Empiria.Land.Transactions.CertificateRequests.Providers
