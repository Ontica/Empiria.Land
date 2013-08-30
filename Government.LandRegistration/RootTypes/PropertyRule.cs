/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : PropertyRule                                   Pattern  : Standard  Class                     *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a property recording condition that serves as a rule for recording registration.    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;
using System.Collections.Generic;

namespace Empiria.Government.LandRegistration {

  /// <summary>Describes a property recording condition that serves as a rule for 
  /// recording registration.</summary>
  public class PropertyRule {

    #region Constructors and parsers

    internal PropertyRule() {
      this.Expire = false;
      this.IsInternalDivision = false;
      this.Name = String.Empty;
      this.PropertyCount = LandRegistration.PropertyCount.Undefined;
      this.PropertyStatus = PropertyRecordingStatus.Undefined;
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
      if (json.ContainsKey("PropertyStatus")) {
        rule.PropertyStatus = (PropertyRecordingStatus) Enum.Parse(typeof(PropertyRecordingStatus),
                                                                  (string) json["PropertyStatus"]);
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

    public PropertyRecordingStatus PropertyStatus {
      get;
      internal set;
    }

    public bool UseNumbering {
      get;
      internal set;
    }

    #endregion Properties

  }  // class PropertyRule

}  // namespace Empiria.Government.LandRegistration
