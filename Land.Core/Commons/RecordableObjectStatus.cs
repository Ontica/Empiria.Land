/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Enumeration Type                      *
*  Type     : RecordableObjectStatus                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary   : Enumerates the status for recordable objects inside Empiria Land.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Enumerates the status for recordable objects inside Empiria Land.</summary>
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

  }  // enum RecordableObjectStatus



  /// <summary>Extension methods for RecordableObjectStatus enumeration instances.</summary>
  static public class RecordableObjectStatusExtensionMethods {

    static public string StatusName(this RecordableObjectStatus status) {
      switch (status) {
        case RecordableObjectStatus.Obsolete:
          return "No vigente";
        case RecordableObjectStatus.NotLegible:
          return "No legible";
        case RecordableObjectStatus.Incomplete:
          return "Incompleto";
        case RecordableObjectStatus.Pending:
          return "Pendiente";
        case RecordableObjectStatus.Registered:
          return "Registrado";
        case RecordableObjectStatus.Closed:
          return "Cerrado";
        case RecordableObjectStatus.Deleted:
          return "Eliminado";
        default:
          return "No determinado";
      }
    }

  }  // RecordableObjectStatusExtensionMethods

} // namespace Empiria.Land.Registration
