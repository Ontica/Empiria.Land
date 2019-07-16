/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : OwnershipMode                                  Pattern  : Enumeration Type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Indicates the type of ownership that a party has with respect of a real estate.               *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Instruments {

  /// <summary>Indicates the status of an instrument.</summary>
  public enum InstrumentStatus {
    All = '?',
    Pending = 'P',
    Signed = 'S',
    Requested = 'R',
    Delivered = 'C',
    Returned = 'T',
    Deleted = 'X'
  }

} // namespace Empiria.Land.Instruments
