
/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : HumanParty                                     Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a human recording act party.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  /// <summary>Represents a human recording act party.</summary>
  public class HumanParty : Party {

    #region Constructors and parsers

    private HumanParty() {
      // Required by Empiria Framework.
    }

    public HumanParty(PartyFields party) : base(party.FullName) {
      this.RFC = party.RFC;
      this.CURP = party.CURP;
      this.Notes = party.Notes;
    }

    static public new HumanParty Parse(int id) {
      return BaseObject.ParseId<HumanParty>(id);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("PartySecondaryID")]
    public string CURP {
      get;
      private set;
    }

    protected internal override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.Keywords, this.CURP);
      }
    }

    #endregion Properties

  } // class HumanParty

} // namespace Empiria.Land.Registration
