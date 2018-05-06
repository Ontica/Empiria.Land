
/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : HumanParty                                     Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a human recording act party.                                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a human recording act party.</summary>
  public class HumanParty : Party {

    #region Constructors and parsers

    private HumanParty() {
      // Required by Empiria Framework.
    }

    public HumanParty(string fullName,
                      string officialID, string officialIDType) : base(fullName, officialID, officialIDType) {

    }

    static public new HumanParty Parse(int id) {
      return BaseObject.ParseId<HumanParty>(id);
    }

    #endregion Constructors and parsers

  } // class HumanParty

} // namespace Empiria.Land.Registration
