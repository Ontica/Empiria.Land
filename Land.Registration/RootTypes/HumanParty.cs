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

    #endregion Fields

    #region Constructors and parsers

    public HumanParty() : base(thisTypeName) {

    }

    protected HumanParty(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public new HumanParty Parse(int id) {
      return BaseObject.Parse<HumanParty>(thisTypeName, id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("CURPNumber")]
    public string CURPNumber {
      get;
      set;
    }

    [DataField("FirstFamilyName")]
    public string FirstFamilyName {
      get;
      set;
    }

    [DataField("FirstName")]
    public string FirstName {
      get;
      set;
    }

    [DataField("Gender", Default = Gender.Unknown)]
    public Gender Gender {
      get;
      set;
    }

    [DataField("IFENumber")]
    public string IFENumber {
      get;
      set;
    }
    
    public override string Keywords {
      get {
        return EmpiriaString.BuildKeywords(base.FullName, this.Nicknames, this.CURPNumber,
                                           this.TaxIDNumber, this.IFENumber, 
                                           this.RegistryDate.ToString("dd/MMM/yyyy"));
      }
    }

    [DataField("MaritalFamilyName")]
    public string MaritalFamilyName {
      get;
      set;
    }

    [DataField("SecondFamilyName")]
    public string SecondFamilyName {
      get;
      set;
    }

    public override string RegistryID {
      get {
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
    }

    public override string FullName {
      get {
        string fullName = this.FirstName + " " + this.FirstFamilyName + " " + this.SecondFamilyName;
        if (this.MaritalFamilyName.Length != 0 &&
           !this.MaritalFamilyName.ToLowerInvariant().StartsWith("de ")) {
          fullName += " de " + this.MaritalFamilyName;
        }
        return fullName;
      }
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
