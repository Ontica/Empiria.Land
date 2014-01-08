/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : PropertyType                                   Pattern  : Storage Item                        *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Describes a kind of property.                                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a kind of property.</summary>
  public class PropertyType : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.PropertyType";

    #endregion Fields

    #region Constructors and parsers

    public PropertyType()
      : base(thisTypeName) {

    }

    protected PropertyType(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public PropertyType Empty {
      get { return BaseObject.ParseEmpty<PropertyType>(thisTypeName); }
    }

    static public PropertyType Unknown {
      get { return BaseObject.ParseUnknown<PropertyType>(thisTypeName); }
    }

    static public PropertyType Parse(int id) {
      return BaseObject.Parse<PropertyType>(thisTypeName, id);
    }

    static public ObjectList<PropertyType> GetList() {
      return GeneralObject.ParseList<PropertyType>(thisTypeName);
    }

    #endregion Constructors and parsers

  } // class PropertyType

} // namespace Empiria.Land.Registration
