/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordableObjectStatus                         Pattern  : Enumeration Type                    *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Enumerates the statuses for recordable documents and objects in the Land Registration System. *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Enumerates the statuses for recordable documents and objects in the
  /// Land Registration System.</summary>
  public enum RecordableObjectStatus {
    Obsolete = 'S',
    NoLegible = 'L',
    Incomplete = 'I',
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

} // namespace Empiria.Land.Registration
