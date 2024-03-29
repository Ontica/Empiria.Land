﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : FormerCertificateStatus                        Pattern  : Enumeration Type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Former Certificate status enumeration.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Certification {

  /// <summary>Former Certificate status enumeration.</summary>
  public enum FormerCertificateStatus {
    Pending = 'P',
    Closed = 'C',
    Deleted = 'X',
    Canceled = 'L',
  }

} // namespace Empiria.Land.Certification
