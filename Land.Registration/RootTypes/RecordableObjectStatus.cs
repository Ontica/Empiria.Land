/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordableObjectStatus                         Pattern  : Enumeration Type                    *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Enumerates the statuses for recordable documents and objects inside Empiria Land.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Enumerates the statuses for recordable documents and objects inside Empiria Land.</summary>
  public enum RecordableObjectStatus {

    /// <summary>Element exists and it was noted down, but requires further revision to
    /// determinate its right status. It's common to mark recordable items as pending on those
    /// cases where the system generates them.</summary>
    Pending = 'P',


    /// <summary>Element exists and it was noted down, but it can't be recorded because its source
    /// information is no legible. Useful when the data capture is drived by document images.</summary>
    NotLegible = 'L',


    /// <summary>The recordable element exists and it was noted down, but is not worth
    /// complete it because it's obsolete. (Spanish translation: 'no-vigente')</summary>
    Obsolete = 'S',


    /// <summary>Element exist but its information is incomplete.
    /// Frequently is the status before Registered.</summary>
    Incomplete = 'I',


    /// <summary>Element was registered with complete information but is still possible to change it.
    /// Commonly is the status before Closed and after Incomplete.</summary>
    Registered = 'R',


    /// <summary>Element was closed and it is not possible to change it.
    /// Typically is the last step, after Registered.</summary>
    Closed = 'C',


    /// <summary>The recordable element was deleted.</summary>
    Deleted = 'X'

  }

} // namespace Empiria.Land.Registration
