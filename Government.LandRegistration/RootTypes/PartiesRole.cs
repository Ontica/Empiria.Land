/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : PartiesRole                                    Pattern  : Storage Item                        *
*  Version   : 5.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*                                                                                                            *
********************************* Copyright (c) 1999-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect to another party.</summary>
  public class PartiesRole : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.PartiesRole";

    #endregion Fields

    #region Constructors and parsers

    public PartiesRole()
      : base(thisTypeName) {

    }

    protected PartiesRole(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public PartiesRole Empty {
      get { return BaseObject.ParseEmpty<PartiesRole>(thisTypeName); }
    }

    static public PartiesRole Unknown {
      get { return BaseObject.ParseUnknown<PartiesRole>(thisTypeName); }
    }

    static public PartiesRole Parse(int id) {
      return BaseObject.Parse<PartiesRole>(thisTypeName, id);
    }

    static public ObjectList<PartiesRole> GetList() {
      ObjectList<PartiesRole> list = GeneralObject.ParseList<PartiesRole>(thisTypeName);

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
