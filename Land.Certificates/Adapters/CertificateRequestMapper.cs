/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests Management            Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Mapper                                  *
*  Type     : CertificateRequestMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for CertificateRequest instances.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Certificates;

namespace Empiria.Land.Transactions.CertificateRequests {

  /// <summary>Contains mapping methods for CertificateRequest instances.</summary>
  static internal class CertificateRequestMapper {

    static internal CertificateRequestDto Map(LRSTransaction transaction,
                                              CertificateDto certificate) {
      return new CertificateRequestDto {
         UID = certificate.UID,
         Type = certificate.Type,
         CertificateID = certificate.CertificateID,
         RecordableSubject = certificate.RecordableSubject,
         MediaLink = certificate.MediaLink,
         Status = certificate.Status,
         IssuingRecordingContext = certificate.IssuingRecordingContext,
         Actions = MapActions(transaction, certificate)
      };
    }


    static internal FixedList<CertificateRequestDto> Map(LRSTransaction transaction,
                                                         FixedList<CertificateDto> certificates) {
      return certificates.Select(certificate => Map(transaction, certificate))
                         .ToFixedList();
    }


    static private CertificateActions MapActions(LRSTransaction transaction,
                                                 CertificateDto certificate) {
      // ToDo: Complete with transaction's certificate request edition rules
      return certificate.Actions;
    }


  }  // class CertificateRequestMapper

}  // namespace Empiria.Land.Transactions.CertificateRequests
