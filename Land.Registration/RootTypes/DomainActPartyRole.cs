/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainActPartyRole                             Pattern  : Storage Item                        *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect a domain recording act.                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/


namespace Empiria.Land.Registration {

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

} // namespace Empiria.Land.Registration
