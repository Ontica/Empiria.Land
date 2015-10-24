/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : PropertyRule                                   Pattern  : Standard  Class                     *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a property recording condition that serves as a rule for recording registration.    *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Json;

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

    static internal PropertyRule Parse(JsonObject json) {
      PropertyRule rule = new PropertyRule();

      rule.Expire = json.Get<bool>("Expire", false);
      rule.IsInternalDivision = json.Get<bool>("IsInternalDivision", false);
      rule.Name = json.Get<string>("Name", String.Empty);
      rule.PropertyCount = RecordingRule.ParsePropertyCount(json.Get<string>("PropertyCount", String.Empty));
      rule.RecordableObjectStatus = json.Get<PropertyRecordingStatus>("RecordableObjectStatus",
                                                                      PropertyRecordingStatus.Undefined);
      rule.UseNumbering = json.Get<bool>("UseNumbering", false);

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
