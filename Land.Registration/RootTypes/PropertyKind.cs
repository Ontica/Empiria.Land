﻿/* Empiria Land 2014 *****************************************************************************************
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

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.PropertyKind";

    #endregion Fields

    #region Constructors and parsers

    public PropertyKind() : base(thisTypeName) {

    }

    private PropertyKind(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
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
