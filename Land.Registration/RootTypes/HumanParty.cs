
/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : HumanParty                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a human recording act party.                                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a human recording act party.</summary>
  public class HumanParty : Party {

    #region Constructors and parsers

    private HumanParty() {
      // Required by Empiria Framework.
    }

    public HumanParty(string UID, string firstName,
                      string lastName, string lastName2) :
                                  base(UID, HumanParty.BuildFullName(firstName, lastName, lastName2)) {
      this.FirstName = EmpiriaString.TrimAll(firstName);
      this.LastName = EmpiriaString.TrimAll(lastName);
      this.LastName2 = EmpiriaString.TrimAll(lastName2);
    }

    static private string BuildFullName(string firstName, string lastName, string lastName2) {
      Assertion.AssertObject(firstName, "firstName");
      Assertion.AssertObject(lastName, "lastName");

      firstName = EmpiriaString.TrimAll(firstName);
      lastName = EmpiriaString.TrimAll(lastName);
      lastName2 = EmpiriaString.TrimAll(lastName2);

      string fullName = firstName + " " + lastName;
      if (lastName2.Length != 0) {
        fullName += " " + lastName2;
      }
      return fullName;
    }

    static public new HumanParty Parse(int id) {
      return BaseObject.ParseId<HumanParty>(id);
    }

    static public HumanParty Empty {
      get {
        return BaseObject.ParseEmpty<HumanParty>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("FirstName")]
    public string FirstName {
      get;
      private set;
    }

    [DataField("LastName")]
    public string LastName {
      get;
      private set;
    }

    [DataField("LastName2")]
    public string LastName2 {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      base.OnSave();
      PropertyData.WriteHumanParty(this);
    }

    #endregion Public methods

  } // class HumanParty

} // namespace Empiria.Land.Registration
