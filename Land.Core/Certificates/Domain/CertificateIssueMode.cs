/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
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

} // namespace Empiria.Land.Certificates
