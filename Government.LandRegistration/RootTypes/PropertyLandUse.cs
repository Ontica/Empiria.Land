/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PropertyLandUse                                Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes the land use of a property.                                                         *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/

namespace Empiria.Government.LandRegistration {

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

} // namespace Empiria.Government.LandRegistration