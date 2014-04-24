/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Occupation                                     Pattern  : Storage Item                        *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a person occupation or main activity.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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

    static public FixedList<Occupation> GetList() {
      FixedList<Occupation> list = GeneralObject.ParseList<Occupation>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class Occupation

} // namespace Empiria.Land.Registration
