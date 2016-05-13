/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainActPartyRole                             Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect a domain recording act.                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect a recording act.</summary>
  public class DomainActPartyRole : BasePartyRole {

    #region Constructors and parsers

    private DomainActPartyRole() {
      // Required by Empiria Framework.
    }

    static public FixedList<DomainActPartyRole> GetList() {
      return GeneralObject.ParseList<DomainActPartyRole>();
    }

    static public DomainActPartyRole Usufructuary {
      get { return BaseObject.ParseKey<DomainActPartyRole>("DomainActPartyRole.Usufructuary"); }
    }

    #endregion Constructors and parsers

  } // class DomainActPartyRole

} // namespace Empiria.Land.Registration
