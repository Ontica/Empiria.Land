/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled                         *
*              when a RecordingAct is registered.                                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration {

  public enum ResourceCount {
    Undefined,
    One,
    OneOrMore,
    Two,
    TwoOrMore,
  }

  public enum ResourceRecordingStatus {
    Undefined,
    NotApply,
    Both,
    Registered,
    Unregistered,
  }

  public enum RecordingRuleApplication {
    Undefined,
    RealEstate,
    Association,
    NoProperty,
    RecordingAct,
    Structure,
    Party,
  }

  /// <summary>Describes the conditions and business rules that have to be fulfilled
  ///  when a RecordingAct is registered.</summary>
  public class RecordingRule {

    #region Fields

    private RecordingActType recordingActType = null;

    #endregion Fields

    #region Constructors and parsers

    private RecordingRule(RecordingActType recordingActType) {
      this.recordingActType = recordingActType;

      this.Load();
    }

    private void Load() {
      try {
        var json = JsonObject.Parse(recordingActType.ExtensionData);

        this.AppliesTo = json.Get<RecordingRuleApplication>("AppliesTo", RecordingRuleApplication.Undefined);
        this.AutoCancel = json.Get<Int32>("AutoCancel", 0);

        this.InputResources = json.GetList<RealEstateRule>("InputResources", false).ToArray();

        this.NewResource = RealEstateRule.Parse(json.Slice("NewResource", false));

        this.ResourceCount = ParseResourceCount(json.Get<string>("ResourceCount", String.Empty));
        this.ResourceRecordingStatus = json.Get<ResourceRecordingStatus>("ResourceStatus",
                                                                         ResourceRecordingStatus.Undefined);
        this.RecordingSection = json.Get<RecordingSection>("RecordingSectionId", RecordingSection.Empty);
        this.SpecialCase = json.Get<string>("SpecialCase", String.Empty);
        this.RecordingActTypes = json.GetList<RecordingActType>("RecordingActTypes", false).ToArray();
        this.AllowsPartitions = json.Get<bool>("AllowsPartitions", false);
        this.IsActive = json.Get<bool>("IsActive", false);
        this.AskForResourceName = json.Get<bool>("AskForResourceName", false);
        this.ResourceTypeName = json.Get<string>("ResourceTypeName", String.Empty);
        this.UseDynamicActNaming = json.Get<bool>("UseDynamicActNaming", false);
      } catch (Exception e) {
        throw new LandRegistrationException(LandRegistrationException.Msg.MistakeInRecordingRuleConfig, e,
                                            this.recordingActType.Id);
      }
    }

    private JsonObject _json = null;
    public JsonObject ToJson() {
      if (_json == null) {
        _json = this.ConvertToJson();
      }
      return _json;
    }

    private JsonObject ConvertToJson() {
      JsonObject json = new JsonObject();
      json.Add(new JsonItem("IsModification", this.recordingActType.IsModificationActType));
      json.Add(new JsonItem("IsCancelation", this.recordingActType.IsCancelationActType));
      json.Add(new JsonItem("AllowsPartitions", this.AllowsPartitions));
      json.Add(new JsonItem("AppliesTo", this.AppliesTo.ToString()));
      json.Add(new JsonItem("AutoCancel", this.AutoCancel));
      json.Add(new JsonItem("AskForResourceName", this.AskForResourceName));
      json.Add(new JsonItem("ResourceCount", this.ResourceCount.ToString()));
      json.Add(new JsonItem("ResourceRecordingStatus", this.ResourceRecordingStatus.ToString()));
      json.Add(new JsonItem("SpecialCase", this.SpecialCase));
      json.Add(new JsonItem("IsActive", this.IsActive));

      return json;
    }

    static internal RecordingRule Parse(RecordingActType recordingActType) {
      Assertion.AssertObject(recordingActType, "recordingActType");

      return new RecordingRule(recordingActType);
    }

    static public ResourceCount ParseResourceCount(string resourceCount) {
      switch (resourceCount) {
        case "1":
          return ResourceCount.One;
        case "1+":
          return ResourceCount.OneOrMore;
        case "2":
          return ResourceCount.Two;
        case "2+":
          return ResourceCount.TwoOrMore;
      }
      return ResourceCount.Undefined;
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

    public RealEstateRule[] InputResources {
      get;
      private set;
    } = new RealEstateRule[0];

    public RealEstateRule NewResource {
      get;
      private set;
    } = new RealEstateRule();

    public ResourceCount ResourceCount {
      get;
      private set;
    } = ResourceCount.Undefined;

    public ResourceRecordingStatus ResourceRecordingStatus {
      get;
      private set;
    } = ResourceRecordingStatus.Undefined;

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

    public bool UseDynamicActNaming {
      get;
      private set;
    } = false;

    public string ResourceTypeName {
      get;
      internal set;
    } = String.Empty;

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Land.Registration
