/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : CertificateStatus                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the status of a land certificate.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/


namespace Empiria.Land.Certificates {

  /// <summary>Describes the status of a land certificate.</summary>
  public enum CertificateStatus {

    Pending = 'P',

    Closed = 'C',

    Deleted = 'X',

    Canceled = 'L',

  }  // enum CertificateStatus



  /// <summary>Extension methods for CertificateStatus enumeration.</summary>
  static public class CertificateStatusExtensions {

    static public string Name(this CertificateStatus certificateStatus) {
      switch (certificateStatus) {

        case CertificateStatus.Canceled:
          return "Cancelado";

        case CertificateStatus.Closed:
          return "Cerrado";

        case CertificateStatus.Deleted:
          return "Eliminado";

        case CertificateStatus.Pending:
          return "En elaboración";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized certificate status {certificateStatus}.");
      }
    }

  }  // class CertificateStatusExtensions

} // namespace Empiria.Land.Certificates
