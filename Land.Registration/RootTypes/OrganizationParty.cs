/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : OrganizationParty                              Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents an organization recording act party.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
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

    static public OrganizationParty Create(ObjectTypeInfo objectTypeInfo) {
      return BaseObject.Create<OrganizationParty>(objectTypeInfo);
    }

    static public new OrganizationParty Parse(int id) {
      return BaseObject.ParseId<OrganizationParty>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("OrgDocBookNumber")]
    public string AssocDocBookNumber {
      get;
      set;
    }

    [DataField("OrgDocNumber")]
    public string AssocDocNumber {
      get;
      set;
    }

    [DataField("OrgDocEndSheet")]
    public string AssocDocEndSheet {
      get;
      set;
    }

    [DataField("OrgDocIssueDate")]
    public DateTime AssocDocIssueDate {
      get;
      set;
    }

    [DataField("OrgDocIssuedById")]
    public Person AssocDocIssuedBy {
      get;
      set;
    }

    [DataField("OrgDocNotaryOfficeId")]
    public NotaryOffice AssocDocNotaryOffice {
      get;
      set;
    }

    [DataField("OrgDocRecordingDate")]
    public DateTime AssocDocRecordingDate {
      get;
      set;
    }

    [DataField("OrgDocRecordingNumber")]
    public string AssocDocRecordingNumber {
      get;
      set;
    }

    [DataField("OrgDocRecordingOfficeId")]
    public Organization AssocDocRecordingOffice {
      get;
      set;
    }

    [DataField("OrgDocStartSheet")]
    public string AssocDocStartSheet {
      get;
      set;
    }

    internal protected override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.FullName, this.Nicknames, this.TaxIDNumber);
      }
    }

    #endregion Public properties

    #region Public methods

    public override string RegistryID {
      get {
        if (this.TaxIDNumber.Length != 0) {
          return this.TaxIDNumber;
        } else {
          return String.Empty;
        }
      }
    }

    protected override void OnSave() {
      base.OnSave(); 
      PropertyData.WriteOrganizationParty(this);
    }

    #endregion Public methods

  } // class OrganizationParty

} // namespace Empiria.Land.Registration
