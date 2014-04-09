/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyLandUse                                Pattern  : Storage Item                        *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the land use of a property.                                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the land use of a property.</summary>
  public class PropertyLandUse : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.PropertyLandUse";

    #endregion Fields

    #region Constructors and parsers

    public PropertyLandUse()
      : base(thisTypeName) {

    }

    private PropertyLandUse(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public PropertyLandUse Parse(int id) {
      return BaseObject.Parse<PropertyLandUse>(thisTypeName, id);
    }

    static public PropertyLandUse Empty {
      get { return BaseObject.ParseEmpty<PropertyLandUse>(thisTypeName); }
    }

    static public PropertyLandUse Unknown {
      get { return BaseObject.ParseUnknown<PropertyLandUse>(thisTypeName); }
    }

    static public ObjectList<PropertyLandUse> GetList() {
      return GeneralObject.ParseList<PropertyLandUse>(thisTypeName);
    }

    #endregion Constructors and parsers

  } // class PropertyLandUse

} // namespace Empiria.Land.Registration
