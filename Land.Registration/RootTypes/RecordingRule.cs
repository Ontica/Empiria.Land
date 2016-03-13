/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled                         *
*              when a RecordingAct is registered.                                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

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
    Property,
    Association,
    Document,
    RecordingAct,
    Structure,
    Party,
  }

  /// <summary>Describes the conditions and business rules that have to be fulfilled
  ///  when a RecordingAct is registered.</summary>
  public class RecordingRule {

    #region Constructors and parsers

    private RecordingRule(string jsonString) {
      this.Load(jsonString);
    }

    private void Load(string jsonString) {
      var json = JsonObject.Parse(jsonString);

      this.AppliesTo = json.Get<RecordingRuleApplication>("AppliesTo", RecordingRuleApplication.Undefined);
      this.AutoCancel = json.Get<Int32>("AutoCancel", 0);

      this.InputProperties = json.GetList<PropertyRule>("InputProperties", false).ToArray();

      this.NewProperty = PropertyRule.Parse(json.Slice("NewProperties", false));

      this.PropertyCount = ParsePropertyCount(json.Get<string>("PropertyCount", String.Empty));
      this.PropertyRecordingStatus = json.Get<PropertyRecordingStatus>("PropertyStatus",
                                                                       PropertyRecordingStatus.Undefined);
      this.RecordingSection = json.Get<RecordingSection>("RecordingSectionId", RecordingSection.Empty);
      this.SpecialCase = json.Get<string>("SpecialCase", String.Empty);
      this.RecordingActTypes = json.GetList<RecordingActType>("RecordingActTypes", false).ToArray();
      this.AllowsPartitions = json.Get<bool>("AllowsPartitions", false);
      this.IsActive = json.Get<bool>("IsActive", false);
      this.AskForResourceName = json.Get<bool>("AskForResourceName", false);
    }

    private JsonObject _json = null;
    public JsonObject ToJson() {
      if (_json == null) {
        _json = ConvertToJson();
      }
      return _json;
    }

    private JsonObject ConvertToJson() {
      JsonObject json = new JsonObject();
      json.Add(new JsonItem("AllowsPartitions", this.AllowsPartitions));
      json.Add(new JsonItem("AppliesTo", this.AppliesTo.ToString()));
      json.Add(new JsonItem("AutoCancel", this.AutoCancel));
      json.Add(new JsonItem("AskForResourceName", this.AskForResourceName));
      json.Add(new JsonItem("PropertyCount", this.PropertyCount.ToString()));
      json.Add(new JsonItem("PropertyRecordingStatus", this.PropertyRecordingStatus.ToString()));
      json.Add(new JsonItem("SpecialCase", this.SpecialCase));
      json.Add(new JsonItem("IsActive", this.IsActive));

      return json;
    }

    static internal RecordingRule Parse(string jsonString) {
      return new RecordingRule(jsonString);
    }

    static public PropertyCount ParsePropertyCount(string propertyCount) {
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

    public bool AllowsPartitions {
      get;
      private set;
    } = false;

    public RecordingRuleApplication AppliesTo {
      get;
      private set;
    } = RecordingRuleApplication.Undefined;

    public bool AskForResourceName {
      get;
      private set;
    } = false;

    public int AutoCancel {
      get;
      private set;
    } = 0;

    public PropertyRule[] InputProperties {
      get;
      private set;
    } = new PropertyRule[0];

    public PropertyRule NewProperty {
      get;
      private set;
    } = new PropertyRule();

    public PropertyCount PropertyCount {
      get;
      private set;
    } = PropertyCount.Undefined;

    public PropertyRecordingStatus PropertyRecordingStatus {
      get;
      private set;
    } = PropertyRecordingStatus.Undefined;

    public RecordingSection RecordingSection {
      get;
      private set;
    } = RecordingSection.Empty;

    public RecordingActType[] RecordingActTypes {
      get;
      private set;
    } = new RecordingActType[0];

    public string SpecialCase {
      get;
      private set;
    } = "None";

    public bool IsActive {
      get;
      private set;
    } = false;

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Land.Registration
