/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : DomainActPartyRole                             Pattern  : Storage Item                        *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect a domain recording act.                    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/


namespace Empiria.Government.LandRegistration {

  /// <summary>Describes the role that plays a party with respect a recording act.</summary>
  public class DomainActPartyRole : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.DomainActPartyRole";

    #endregion Fields

    #region Constructors and parsers

    public DomainActPartyRole()
      : base(thisTypeName) {

    }

    protected DomainActPartyRole(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public DomainActPartyRole Parse(int id) {
      return BaseObject.Parse<DomainActPartyRole>(thisTypeName, id);
    }

    static public ObjectList<DomainActPartyRole> GetList() {
      ObjectList<DomainActPartyRole> list = GeneralObject.ParseList<DomainActPartyRole>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    static public DomainActPartyRole Empty {
      get { return BaseObject.ParseEmpty<DomainActPartyRole>(thisTypeName); }
    }

    static public DomainActPartyRole Unknown {
      get { return BaseObject.ParseUnknown<DomainActPartyRole>(thisTypeName); }
    }

    static public DomainActPartyRole Usufructuary {
      get { return BaseObject.Parse<DomainActPartyRole>(thisTypeName, "DomainActPartyRole.Usufructuary"); }
    }

    #endregion Constructors and parsers

  } // class DomainActPartyRole

} // namespace Empiria.Government.LandRegistration