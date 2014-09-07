/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Occupation                                     Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a person occupation or main activity.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
      FixedList<Occupation> list = GeneralObject.ParseList<Occupation>();

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class Occupation

} // namespace Empiria.Land.Registration
