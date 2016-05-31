/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : OrganizationParty                              Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an organization recording act party.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents an organization recording act party.</summary>
  public class OrganizationParty : Party {

    #region Constructors and parsers

    private OrganizationParty() {
      // Required by Empiria Framework.
    }

    public OrganizationParty(int typeId, string fullName,
                             string uid, string uidType) : base(fullName, uid, uidType) {
      base.ReclassifyAs(Ontology.ObjectTypeInfo.Parse(typeId));
    }

    static public new OrganizationParty Parse(int id) {
      return BaseObject.ParseId<OrganizationParty>(id);
    }

    #endregion Constructors and parsers

  } // class OrganizationParty

} // namespace Empiria.Land.Registration
