/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateStatus                              Pattern  : Enumeration Type                    *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Certificate status enumeration.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Certification {

  public enum CertificateStatus {
    Pending = 'P',
    Closed = 'C',
    Deleted = 'X',
    Canceled = 'L',
  }

} // namespace Empiria.Land.Certification
