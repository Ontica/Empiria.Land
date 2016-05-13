/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : BasePartyRole                                  Pattern  : Storage Item                        *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Base party role. DomainPartyRole and SecondaryPartyRole are subclasses of this type.          *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Base party role. DomainPartyRole and SecondaryPartyRole are subclasses of this type.</summary>
  public class BasePartyRole : GeneralObject {

    #region Constructors and parsers

    protected BasePartyRole() {
      // Required by Empiria Framework.
    }

    static public BasePartyRole Empty {
      get { return BaseObject.ParseEmpty<DomainActPartyRole>(); }
    }

    static public BasePartyRole Unknown {
      get { return BaseObject.ParseUnknown<DomainActPartyRole>(); }
    }

    static public BasePartyRole Parse(int id) {
      return BaseObject.ParseId<BasePartyRole>(id);
    }

    #endregion Constructors and parsers

  } // class BasePartyRole

} // namespace Empiria.Land.Registration
