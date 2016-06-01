﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingExtData                               Pattern  : IExtensibleData class               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that describes recording document types.                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>In the future, use this type to hold extended data for physical recordings.</summary>
  public class RecordingExtData : IExtensibleData {

    public string ToJson() {
      return String.Empty;
    }

  }  // class RecordingExtData

}  // namespace Empiria.Land.Registration
