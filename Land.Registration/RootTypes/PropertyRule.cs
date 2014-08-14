/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyRule                                   Pattern  : Standard  Class                     *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a property recording condition that serves as a rule for recording registration.    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

namespace Empiria.Land.Registration {

  /// <summary>Describes a property recording condition that serves as a rule for 
  /// recording registration.</summary>
  public class PropertyRule {

    #region Constructors and parsers

    internal PropertyRule() {
      this.Expire = false;
      this.IsInternalDivision = false;
      this.Name = String.Empty;
      this.PropertyCount = Land.Registration.PropertyCount.Undefined;
      this.RecordableObjectStatus = PropertyRecordingStatus.Undefined;
      this.UseNumbering = false;
    }

    static internal PropertyRule Parse(IDictionary<string, object> json) {
      PropertyRule rule = new PropertyRule();

      if (json.ContainsKey("Expire")) {
        rule.Expire = (bool) json["Expire"];
      }
      if (json.ContainsKey("IsInternalDivision")) {
        rule.IsInternalDivision = (bool) json["IsInternalDivision"];
      }
      if (json.ContainsKey("Name")) {
        rule.Name = (string) json["Name"];
      }
      if (json.ContainsKey("PropertyCount")) {
        rule.PropertyCount = RecordingRule.ParsePropertyCount(Convert.ToString(json["PropertyCount"]));
      }
      if (json.ContainsKey("RecordableObjectStatus")) {
        rule.RecordableObjectStatus = (PropertyRecordingStatus) Enum.Parse(typeof(PropertyRecordingStatus),
                                                                           (string) json["RecordableObjectStatus"]);
      }
      if (json.ContainsKey("UseNumbering")) {
        rule.IsInternalDivision = (bool) json["UseNumbering"];
      }
      return rule;
    }


    #endregion Constructors and parsers

    #region Properties

    public bool Expire {
      get;
      internal set;
    }

    public bool IsInternalDivision {
      get;
      internal set;
    }

    public string Name {
      get;
      internal set;
    }

    public PropertyCount PropertyCount {
      get;
      internal set;
    }

    public PropertyRecordingStatus RecordableObjectStatus {
      get;
      internal set;
    }

    public bool UseNumbering {
      get;
      internal set;
    }

    #endregion Properties

  }  // class PropertyRule

}  // namespace Empiria.Land.Registration
