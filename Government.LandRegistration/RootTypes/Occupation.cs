/* Empiria® Land 2014 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : Occupation                                     Pattern  : Storage Item                        *
*  Date      : 28/Mar/2014                                    Version  : 5.5     License: CC BY-NC-SA 4.0    *
*                                                                                                            *
*  Summary   : Describes a person occupation or main activity.                                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2014. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes a person occupation or main activity.</summary>
  public class Occupation : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.Occupation";

    #endregion Fields

    #region Constructors and parsers

    public Occupation()
      : base(thisTypeName) {

    }

    protected Occupation(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public Occupation Empty {
      get { return BaseObject.ParseEmpty<Occupation>(thisTypeName); }
    }

    static public Occupation Unknown {
      get { return BaseObject.ParseUnknown<Occupation>(thisTypeName); }
    }

    static public Occupation Parse(int id) {
      return BaseObject.Parse<Occupation>(thisTypeName, id);
    }

    static public ObjectList<Occupation> GetList() {
      ObjectList<Occupation> list = GeneralObject.ParseList<Occupation>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class Occupation

} // namespace Empiria.Land.Registration
