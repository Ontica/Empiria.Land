/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled when a                  *
*              RecordingAct is registered.                                                                   *  
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

namespace Empiria.Land.Registration {

  public enum PropertyCount {
    Undefined,
    One,
    OneOrMore,
    Two,
    TwoOrMore,
  }

  public enum PropertyRecordingStatus {
    Undefined,
    NotApply,
    Both,
    Registered,
    Unregistered,
  }

  public enum RecordingRuleApplication {
    Undefined,
    None,
    Property,
    LegalPerson,
    PersonalProperty,
    RecordingAct,
    Structure,
  }

  /// <summary>Describes the conditions and business rules that have to be fulfilled when a
  /// RecordingAct is registered.</summary>
  public class RecordingRule : Structure {

    #region Fields

    private const string thisTypeName = "StructureType.RecordingRule";

    #endregion Fields

    #region Constructors and parsers

    public RecordingRule() : base(thisTypeName) {
      this.AppliesTo = RecordingRuleApplication.Undefined;
      this.AutoCancel = 0;
      this.InputProperties = new PropertyRule[0];
      this.IsAnnotation = false;
      this.IsCancelation = false;
      this.NewProperty = new PropertyRule();
      this.PropertyCount = Land.Registration.PropertyCount.Undefined;
      this.PropertyRecordingStatus = PropertyRecordingStatus.Undefined;
      this.RecordingActTypes = new RecordingActType[0];
      this.RecordingSection = RecordingSection.Empty;
      this.FixedRecorderOffice = RecorderOffice.Empty;
      this.SpecialCase = "None";
    }

    internal static RecordingRule Parse(string jsonString) {
      IDictionary<string, object> json = Empiria.Data.JsonConverter.ToObject(jsonString);
      RecordingRule rule = new RecordingRule();

      if (json.ContainsKey("AppliesTo")) {
        rule.AppliesTo = (RecordingRuleApplication) Enum.Parse(typeof(RecordingRuleApplication), 
                                                               (string) json["AppliesTo"]);
      }
      if (json.ContainsKey("AutoCancel")) {
        rule.AutoCancel = Convert.ToInt32(json["AutoCancel"]);
      }
      if (json.ContainsKey("InputProperties")) {
        List<object> array = (List<object>) json["InputProperties"];
        rule.InputProperties = new PropertyRule[array.Count];
        for (int i = 0; i < array.Count; i++) {
          IDictionary<string, object> item = (IDictionary<string, object>) array[i];
          rule.InputProperties[i] = PropertyRule.Parse(item);
        }
      }
      if (json.ContainsKey("IsAnnotation")) {
        rule.IsAnnotation = (bool) json["IsAnnotation"];
      }
      if (json.ContainsKey("IsCancelation")) {
        rule.IsCancelation = (bool) json["IsCancelation"];
      }
      if (json.ContainsKey("NewProperties")) {
        IDictionary<string, object> item = (IDictionary<string, object>) json["NewProperties"];
        rule.NewProperty = PropertyRule.Parse(item);
      }
      if (json.ContainsKey("PropertyCount")) {
        rule.PropertyCount = ParsePropertyCount(Convert.ToString(json["PropertyCount"]));
      }
      if (json.ContainsKey("PropertyStatus")) {
        rule.PropertyRecordingStatus = (PropertyRecordingStatus) Enum.Parse(typeof(PropertyRecordingStatus),
                                                                  (string) json["PropertyStatus"]);
      }
      if (json.ContainsKey("RecordingSectionId")) {
        rule.RecordingSection = RecordingSection.Parse(Convert.ToInt32(json["RecordingSectionId"]));
      }
      if (json.ContainsKey("FixedRecorderOffice")) {
        rule.FixedRecorderOffice = RecorderOffice.Parse(Convert.ToInt32(json["FixedRecorderOffice"]));
      }
      if (json.ContainsKey("SpecialCase")) {
        rule.SpecialCase = (string) json["SpecialCase"];
      }
      if (json.ContainsKey("RecordingActTypes")) {
        List<object> array = (List<object>) json["RecordingActTypes"];
        rule.RecordingActTypes = new RecordingActType[array.Count];
        for (int i = 0; i < array.Count; i++) {
          IDictionary<string, object> item = (IDictionary<string, object>) array[i];
          rule.RecordingActTypes[i] = RecordingActType.Parse(Convert.ToInt32(item["Id"]));
        }
      }
      return rule;
    }

    public static PropertyCount ParsePropertyCount(string propertyCount) {
      switch (propertyCount) {
        case "1":
          return PropertyCount.One;
        case "1+":
          return PropertyCount.OneOrMore;
        case "2":
          return PropertyCount.Two;
        case "2+":
          return PropertyCount.TwoOrMore;
      }
      return PropertyCount.Undefined;
    }

    #endregion Constructors and parsers

    #region Properties

    public RecordingRuleApplication AppliesTo {
      get;
      private set;
    }

    public int AutoCancel {
      get;
      private set;
    }

    public PropertyRule[] InputProperties {
      get;
      private set;
    }

    public bool IsAnnotation {
      get;
      private set;
    }

    public bool IsCancelation {
      get;
      private set;
    }

    public bool IsMainRecording {
      get {
        return (!this.RecordingSection.IsEmptyInstance && 
                !this.IsAnnotation && !IsCancelation);
      }
    }

    public PropertyRule NewProperty {
      get;
      private set;
    }

    public PropertyCount PropertyCount {
      get;
      private set;
    }

    public PropertyRecordingStatus PropertyRecordingStatus {
      get;
      private set;
    }

    public RecorderOffice FixedRecorderOffice { 
      get; 
      private set;
    }

    public RecordingSection RecordingSection {
      get;
      private set;
    }

    public RecordingActType[] RecordingActTypes {
      get;
      private set;
    }

    public string SpecialCase {
      get;
      private set;
    }

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Land.Registration