/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : OrganizationParty                              Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an organization recording act party.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Represents an organization recording act party.</summary>
  public class OrganizationParty : Party {

    #region Constructors and parsers

    private OrganizationParty() {
      // Required by Empiria Framework.
    }

    public OrganizationParty(string UID, string fullName) : base(UID, fullName) {

    }

    static public new OrganizationParty Parse(int id) {
      return BaseObject.ParseId<OrganizationParty>(id);
    }

    #endregion Constructors and parsers

    #region Public methods

    protected override void OnSave() {
      base.OnSave();
      PropertyData.WriteOrganizationParty(this);
    }

    #endregion Public methods

  } // class OrganizationParty

} // namespace Empiria.Land.Registration
