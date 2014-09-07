/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PartiesRole                                    Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect to another party.</summary>
  public class PartiesRole : GeneralObject {

    #region Constructors and parsers

    private PartiesRole() {
      // Required by Empiria Framework.
    }

    static public PartiesRole Empty {
      get { return BaseObject.ParseEmpty<PartiesRole>(); }
    }

    static public PartiesRole Unknown {
      get { return BaseObject.ParseUnknown<PartiesRole>(); }
    }

    static public PartiesRole Parse(int id) {
      return BaseObject.ParseId<PartiesRole>(id);
    }

    static public FixedList<PartiesRole> GetList() {
      FixedList<PartiesRole> list = GeneralObject.ParseList<PartiesRole>();

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

    #region Public properties

    public string InverseRoleName {
      get {
        if (base.Name == base.Description) {
          return String.Empty;
        } else {
          return base.Description;
        }
      }
    }

    #endregion Public properties

  } // class PartiesRole

} // namespace Empiria.Land.Registration
