﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : OrganizationParty                              Pattern  : Empiria Object Type                 *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Represents an organization recording act party.                                               *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Government.LandRegistration.Data;
using Empiria.Ontology;

namespace Empiria.Government.LandRegistration {

  /// <summary>Represents an organization recording act party.</summary>
  public class OrganizationParty : Party {

    #region Fields

    private const string thisTypeName = "ObjectType.Party.OrganizationParty";
    private string assocDocBookNumber = String.Empty;
    private string assocDocNumber = String.Empty;
    private string assocDocStartSheet = String.Empty;
    private string assocDocEndSheet = String.Empty;
    private NotaryOffice assocDocNotaryOffice = NotaryOffice.Empty;
    private Person assocDocIssuedBy = Person.Empty;
    private DateTime assocDocIssueDate = ExecutionServer.DateMaxValue;
    private Organization assocDocRecordingOffice = Organization.Empty;
    private DateTime assocDocRecordingDate = ExecutionServer.DateMaxValue;
    private string assocDocRecordingNumber = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    protected OrganizationParty(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public OrganizationParty Create(ObjectTypeInfo objectTypeInfo) {
      return BaseObject.Create<OrganizationParty>(objectTypeInfo);
    }

    static public new OrganizationParty Parse(int id) {
      return BaseObject.Parse<OrganizationParty>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string AssocDocBookNumber {
      get { return assocDocBookNumber; }
      set { assocDocBookNumber = EmpiriaString.TrimAll(value); }
    }

    public string AssocDocNumber {
      get { return assocDocNumber; }
      set { assocDocNumber = EmpiriaString.TrimAll(value); }
    }

    public string AssocDocEndSheet {
      get { return assocDocEndSheet; }
      set { assocDocEndSheet = EmpiriaString.TrimAll(value); }
    }

    public DateTime AssocDocIssueDate {
      get { return assocDocIssueDate; }
      set { assocDocIssueDate = value; }
    }

    public Person AssocDocIssuedBy {
      get { return assocDocIssuedBy; }
      set { assocDocIssuedBy = value; }
    }

    public NotaryOffice AssocDocNotaryOffice {
      get { return assocDocNotaryOffice; }
      set { assocDocNotaryOffice = value; }
    }

    public DateTime AssocDocRecordingDate {
      get { return assocDocRecordingDate; }
      set { assocDocRecordingDate = value; }
    }

    public string AssocDocRecordingNumber {
      get { return assocDocRecordingNumber; }
      set { assocDocRecordingNumber = EmpiriaString.TrimAll(value); }
    }

    public Organization AssocDocRecordingOffice {
      get { return assocDocRecordingOffice; }
      set { assocDocRecordingOffice = value; }
    }

    public string AssocDocStartSheet {
      get { return assocDocStartSheet; }
      set { assocDocStartSheet = EmpiriaString.TrimAll(value); }
    }

    #endregion Public properties

    #region Public methods

    protected override string ImplementsRegistryID() {
      if (this.TaxIDNumber.Length != 0) {
        return this.TaxIDNumber;
      } else {
        return String.Empty;
      }
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      base.ImplementsLoadObjectData(row);
      this.assocDocBookNumber = (string) row["OrgDocBookNumber"];
      this.assocDocNumber = (string) row["OrgDocNumber"];
      this.assocDocStartSheet = (string) row["OrgDocStartSheet"];
      this.assocDocEndSheet = (string) row["OrgDocEndSheet"];
      this.assocDocNotaryOffice = NotaryOffice.Parse((int) row["OrgDocNotaryOfficeId"]);
      this.assocDocIssuedBy = Person.Parse((int) row["OrgDocIssuedById"]);
      this.assocDocIssueDate = (DateTime) row["OrgDocIssueDate"];
      this.assocDocRecordingOffice = Organization.Parse((int) row["OrgDocRecordingOfficeId"]);
      this.assocDocRecordingDate = (DateTime) row["OrgDocRecordingDate"];
      this.assocDocRecordingNumber = (string) row["OrgDocRecordingNumber"];
    }

    protected override void ImplementsSave() {
      base.ImplementsSave();
      this.Keywords = EmpiriaString.BuildKeywords(base.FullName, this.Nicknames, this.TaxIDNumber);
      PropertyData.WriteOrganizationParty(this);
    }

    #endregion Public methods

  } // class OrganizationParty

} // namespace Empiria.Government.LandRegistration