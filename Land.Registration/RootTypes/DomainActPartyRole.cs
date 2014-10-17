/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainActPartyRole                             Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect a domain recording act.                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect a recording act.</summary>
  public class DomainActPartyRole : GeneralObject {

    #region Constructors and parsers

    private DomainActPartyRole() {
      // Required by Empiria Framework.
    }

    static public DomainActPartyRole Parse(int id) {
      return BaseObject.ParseId<DomainActPartyRole>(id);
    }

    static public FixedList<DomainActPartyRole> GetList() {
      return GeneralObject.ParseList<DomainActPartyRole>();
    }

    static public DomainActPartyRole Empty {
      get { return BaseObject.ParseEmpty<DomainActPartyRole>(); }
    }

    static public DomainActPartyRole Unknown {
      get { return BaseObject.ParseUnknown<DomainActPartyRole>(); }
    }

    static public DomainActPartyRole Usufructuary {
      get { return BaseObject.ParseKey<DomainActPartyRole>("DomainActPartyRole.Usufructuary"); }
    }

    #endregion Constructors and parsers

  } // class DomainActPartyRole

} // namespace Empiria.Land.Registration
