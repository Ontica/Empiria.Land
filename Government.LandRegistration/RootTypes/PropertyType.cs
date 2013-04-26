/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PropertyType                                   Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a kind of property.                                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/


namespace Empiria.Government.LandRegistration {

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

} // namespace Empiria.Government.LandRegistration