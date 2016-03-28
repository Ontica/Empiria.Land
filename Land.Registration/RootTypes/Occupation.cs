/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Occupation                                     Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a person occupation or main activity.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a person occupation or main activity.</summary>
  public class Occupation : GeneralObject {

    #region Constructors and parsers

    private Occupation() {
      // Required by Empiria Framework.
    }

    static public Occupation Empty {
      get { return BaseObject.ParseEmpty<Occupation>(); }
    }

    static public Occupation Unknown {
      get { return BaseObject.ParseUnknown<Occupation>(); }
    }

    static public Occupation Parse(int id) {
      return BaseObject.ParseId<Occupation>(id);
    }

    static public FixedList<Occupation> GetList() {
      return GeneralObject.ParseList<Occupation>();
    }

    #endregion Constructors and parsers

  } // class Occupation

} // namespace Empiria.Land.Registration
