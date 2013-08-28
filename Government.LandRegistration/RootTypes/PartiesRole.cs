/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PartiesRole                                    Pattern  : Storage Item                        *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect to another party.                          *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;

namespace Empiria.Government.LandRegistration {

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

} // namespace Empiria.Government.LandRegistration