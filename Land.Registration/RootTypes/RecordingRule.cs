/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled when a                  *
*              RecordingAct is registered.                                                                   *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

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
    RecordingAct,
    Structure,
    Association,
    Document,
  }

  /// <summary>Describes the conditions and business rules that have to be fulfilled when a
  /// RecordingAct is registered.</summary>
  public class RecordingRule {

    #region Constructors and parsers

    private RecordingRule(string jsonString) {
      Initialize();
      Load(jsonString);
    }

    private void Load(string jsonString) {
      var json = JsonObject.Parse(jsonString);

      this.AppliesTo = json.Get<RecordingRuleApplication>("AppliesTo", RecordingRuleApplication.Undefined);
      this.AutoCancel = json.Get<Int32>("AutoCancel", 0);

      this.InputProperties = json.GetList<PropertyRule>("InputProperties", false).ToArray();

      this.IsAnnotation = json.Get<bool>("IsAnnotation", false);
      this.IsCancelation = json.Get<bool>("IsCancelation", false);

      this.NewProperty = PropertyRule.Parse(json.Slice("NewProperties", false));

      this.PropertyCount = ParsePropertyCount(json.Get<string>("PropertyCount", String.Empty));
      this.PropertyRecordingStatus = json.Get<PropertyRecordingStatus>("PropertyStatus",
                                                                       PropertyRecordingStatus.Undefined);
      this.RecordingSection = json.Get<RecordingSection>("RecordingSectionId", RecordingSection.Empty);
      this.FixedRecorderOffice = json.Get<RecorderOffice>("FixedRecorderOffice", RecorderOffice.Empty);
      this.SpecialCase = json.Get<string>("SpecialCase", String.Empty);
      this.RecordingActTypes = json.GetList<RecordingActType>("RecordingActTypes", false).ToArray();
      this.AllowsPartitions = json.Get<bool>("AllowsPartitions", false);
      this.IsActive = json.Get<bool>("IsActive", false);
    }

    private void Initialize() {
      this.AllowsPartitions = false;
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
      json.Add(new JsonItem("IsAnnotation", this.IsAnnotation));
      json.Add(new JsonItem("IsCancelation", this.IsCancelation));
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


    public bool AllowsPartitions {
      get;
      private set;
    }

    public bool IsActive {
      get;
      private set;
    }

    #endregion Properties




  }  // class RecordingRule

}  // namespace Empiria.Land.Registration
