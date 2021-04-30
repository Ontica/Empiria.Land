/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainActPartyRole                             Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the role that plays a party with respect a domain recording act.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Describes the role that plays a party with respect a recording act.</summary>
  public class DomainActPartyRole : BasePartyRole {

    #region Constructors and parsers

    private DomainActPartyRole() {
      // Required by Empiria Framework.
    }

    static public DomainActPartyRole Parse(string uid) {
      return BaseObject.ParseKey<DomainActPartyRole>(uid);
    }

    static public DomainActPartyRole AntecedentOwnerRole {
      get {
        return DomainActPartyRole.ParseKey<DomainActPartyRole>("DomainActPartyRole.AntecedentOwner");
      }
    }

    static public FixedList<DomainActPartyRole> GetList() {
      return GeneralObject.GetList<DomainActPartyRole>();
    }

    #endregion Constructors and parsers

  } // class DomainActPartyRole

} // namespace Empiria.Land.Registration
