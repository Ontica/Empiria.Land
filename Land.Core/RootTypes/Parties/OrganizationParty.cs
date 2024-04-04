/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : OrganizationParty                              Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an organization recording act party.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.Registration {

  /// <summary>Represents an organization recording act party.</summary>
  public class OrganizationParty : Party {

    #region Constructors and parsers

    private OrganizationParty() {
      // Required by Empiria Framework.
    }

    internal OrganizationParty(PartyFields party): base(party.FullName) {
      this.RFC = party.RFC;
      this.Notes = party.Notes;
    }

    static public new OrganizationParty Parse(int id) {
      return BaseObject.ParseId<OrganizationParty>(id);
    }

    #endregion Constructors and parsers

  } // class OrganizationParty

} // namespace Empiria.Land.Registration
