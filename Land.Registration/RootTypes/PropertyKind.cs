/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyKind                                   Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the kind of a property type.                                                        *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the kind of a property type.</summary>
  public class PropertyKind : GeneralObject {

    #region Constructors and parsers

    private PropertyKind() {
      // Required by Empiria Framework.
    }

    static public PropertyKind Parse(int id) {
      return BaseObject.ParseId<PropertyKind>(id);
    }

    static public PropertyKind Empty {
      get {
        return BaseObject.ParseEmpty<PropertyKind>();
      }
    }

    static public PropertyKind Unknown {
      get {
        return BaseObject.ParseUnknown<PropertyKind>();
      }
    }

    static public FixedList<PropertyKind> GetList() {
      return GeneralObject.ParseList<PropertyKind>();
    }

    #endregion Constructors and parsers

  } // class PropertyKind

} // namespace Empiria.Land.Registration
