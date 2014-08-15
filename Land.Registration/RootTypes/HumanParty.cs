/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : HumanParty                                     Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a human recording act party.                                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a human recording act party.</summary>
  public class HumanParty : Party {

    #region Fields

    private const string thisTypeName = "ObjectType.Party.HumanParty";

    private string firstName = String.Empty;
    private string firstFamilyName = String.Empty;
    private string secondFamilyName = String.Empty;
    private string maritalFamilyName = String.Empty;
    private Gender gender = Gender.Unknown;
    private string curpNumber = String.Empty;
    private string ifeNumber = String.Empty;

    #endregion Fields

    #region Constructors and parsers

    public HumanParty()
      : base(thisTypeName) {

    }

    protected HumanParty(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new HumanParty Parse(int id) {
      return BaseObject.Parse<HumanParty>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string CURPNumber {
      get { return curpNumber; }
      set { curpNumber = EmpiriaString.TrimAll(value); }
    }

    public string FirstFamilyName {
      get { return firstFamilyName; }
      set { firstFamilyName = EmpiriaString.TrimAll(value); }
    }

    public string FirstName {
      get { return firstName; }
      set { firstName = EmpiriaString.TrimAll(value); }
    }

    public Gender Gender {
      get { return gender; }
      set { gender = value; }
    }

    public string IFENumber {
      get { return ifeNumber; }
      set { ifeNumber = EmpiriaString.TrimAll(value); }
    }

    public string MaritalFamilyName {
      get { return maritalFamilyName; }
      set { maritalFamilyName = EmpiriaString.TrimAll(value); }
    }

    public string SecondFamilyName {
      get { return secondFamilyName; }
      set { secondFamilyName = EmpiriaString.TrimAll(value); }
    }

    #endregion Public properties

    #region Public methods

    protected override string ImplementsRegistryID() {
      if (this.CURPNumber.Length != 0) {
        return this.CURPNumber;
      } else if (this.TaxIDNumber.Length != 0) {
        return this.TaxIDNumber;
      } else if (this.IFENumber.Length != 0) {
        return this.IFENumber;
      } else if (this.RegistryDate != ExecutionServer.DateMaxValue) {
        return this.RegistryDate.ToString("dd/MMM/yyyy");
      } else {
        return String.Empty;
      }
    }

    protected override void OnLoadObjectData(DataRow row) {
      base.OnLoadObjectData(row);
      this.firstName = (string) row["FirstName"];
      this.firstFamilyName = (string) row["FirstFamilyName"];
      this.secondFamilyName = (string) row["SecondFamilyName"];
      this.maritalFamilyName = (string) row["MaritalFamilyName"];
      this.gender = (Gender) Convert.ToChar(row["Gender"]);
      this.curpNumber = (string) row["CURPNumber"];
      this.ifeNumber = (string) row["IFENumber"];
    }

    protected override void OnSave() {
      base.OnSave();
      base.FullName = this.firstName + " " + this.firstFamilyName + " " + this.secondFamilyName;
      if (this.maritalFamilyName.Length != 0 && !this.maritalFamilyName.ToLowerInvariant().StartsWith("de ")) {
        base.FullName += " de " + this.maritalFamilyName;
      }
      this.Keywords = EmpiriaString.BuildKeywords(base.FullName, this.Nicknames, this.CURPNumber,
                                                  this.TaxIDNumber, this.IFENumber, this.RegistryDate.ToString("dd/MMM/yyyy"));
      PropertyData.WriteHumanParty(this);
    }

    #endregion Public methods

  } // class HumanParty

} // namespace Empiria.Land.Registration
