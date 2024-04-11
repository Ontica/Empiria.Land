/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Enumeration                             *
*  Type     : CertificateIssueMode                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the issuing mode of a certificate.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Certificates {

  /// <summary>Enumerates the issuing mode of a certificate.</summary>
  public enum CertificateIssueMode {

    Automatic = 'A',

    Manual = 'M',

  }  // CertificateIssueMode

} // namespace Empiria.Land.Certification
