/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : MarriageStatus                                 Pattern  : Storage Item                        *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a person marriage status.                                                           *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a person marriage status.</summary>
  public class MarriageStatus : GeneralObject {

    #region Constructors and parsers

    private MarriageStatus() {
      // Required by Empiria Framework.
    }

    static public MarriageStatus Empty {
      get { return BaseObject.ParseEmpty<MarriageStatus>(); }
    }

    static public MarriageStatus Unknown {
      get { return BaseObject.ParseUnknown<MarriageStatus>(); }
    }

    static public MarriageStatus Parse(int id) {
      return BaseObject.ParseId<MarriageStatus>(id);
    }

    static public FixedList<MarriageStatus> GetList() {
      return GeneralObject.ParseList<MarriageStatus>();
    }

    #endregion Constructors and parsers

  } // class MarriageStatus

} // namespace Empiria.Land.Registration
