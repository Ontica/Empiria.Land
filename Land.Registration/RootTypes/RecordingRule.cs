﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingRule                                  Pattern  : Empiria Structure Type              *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes the conditions and business rules that have to be fulfilled                         *
*              when a RecordingAct is registered.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Indicates the legal recording status of a resource in order to apply the recording act.</summary>
  public enum ResourceRecordingStatus {
    Undefined,
    NotApply,
    Both,
    Registered,
    Unregistered,
  }

  /// <summary>Indicates the resource type of which is applicable the recording act.</summary>
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
        var json = recordingActType.ExtensionData;

        this.AppliesTo = json.Get<RecordingRuleApplication>("AppliesTo", RecordingRuleApplication.Undefined);
        this.AutoCancel = json.Get<Int32>("AutoCancel", 0);

        this.ResourceRecordingStatus = json.Get<ResourceRecordingStatus>("ResourceStatus",
                                                                         ResourceRecordingStatus.Undefined);
        this.RecordingSection = json.Get<RecordingSection>("RecordingSectionId", RecordingSection.Empty);
        this.SpecialCase = json.Get<string>("SpecialCase", String.Empty);
        this.RecordingActTypes = json.GetList<RecordingActType>("RecordingActTypes", false).ToArray();
        this.AllowsPartitions = json.Get<bool>("AllowsPartitions", false);
        this.IsEndingAct = json.Get<bool>("IsEndingAct", false);
        this.IsActive = json.Get<bool>("IsActive", false);
        this.AskForResourceName = json.Get<bool>("AskForResourceName", false);
        this.ResourceTypeName = json.Get<string>("ResourceTypeName", String.Empty);
        this.DynamicActNamePattern = json.Get<string>("DynamicActNamePattern", String.Empty);

        this.EditRealEstate = json.Get<bool>("EditRealEstate", false);
        this.EditAppraisalAmount = json.Get<bool>("EditAppraisalAmount", false);
        this.EditOperationAmount = json.Get<bool>("EditOperationAmount", false);
        this.AllowNoParties = json.Get<bool>("AllowNoParties", false);
        this.AllowUncompletedResource = json.Get<bool>("AllowUncompletedResource", false);
        this.ChainedRecordingActType = json.Get<RecordingActType>("ChainedAct", RecordingActType.Empty);
        this.IsAnnotation = json.Get<bool>("IsAnnotation", false);
        this.IsHardLimitation = json.Get<bool>("IsHardLimitation", false);
        this.SkipPrelation = json.Get<bool>("SkipPrelation", false);

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

      json.Add("IsModification", this.recordingActType.IsModificationActType);
      json.Add("IsCancelation", this.recordingActType.IsCancelationActType);
      json.Add("IsEndingAct", this.IsEndingAct);
      json.Add("AllowsPartitions", this.AllowsPartitions);
      json.Add("AppliesTo", this.AppliesTo.ToString());
      json.Add("AutoCancel", this.AutoCancel);
      json.Add("AskForResourceName", this.AskForResourceName);
      json.Add("ResourceRecordingStatus", this.ResourceRecordingStatus.ToString());
      json.Add("SpecialCase", this.SpecialCase);
      json.Add("IsActive", this.IsActive);

      json.Add("EditRealEstate", this.EditRealEstate);
      json.Add("EditAppraisalAmount", this.EditAppraisalAmount);
      json.Add("EditOperationAmount", this.EditOperationAmount);
      json.Add("AllowNoParties", this.AllowNoParties);
      json.Add("AllowUncompletedResource", this.AllowUncompletedResource);
      json.Add("IsAnnotation", this.IsAnnotation);
      json.Add("IsHardLimitation", this.IsHardLimitation);
      json.Add("SkipPrelation", this.SkipPrelation);

      return json;
    }

    static internal RecordingRule Parse(RecordingActType recordingActType) {
      Assertion.AssertObject(recordingActType, "recordingActType");

      return new RecordingRule(recordingActType);
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
    } = String.Empty;


    public bool IsActive {
      get;
      private set;
    } = false;

    public bool IsEndingAct {
      get;
      private set;
    } = false;

    public string DynamicActNamePattern {
      get;
      private set;
    } = String.Empty;


    public string ResourceTypeName {
      get;
      private set;
    } = String.Empty;


    public bool UseDynamicActNaming {
      get {
        return (this.DynamicActNamePattern.Length != 0);
      }
    }

    public bool EditAppraisalAmount {
      get;
      private set;
    } = false;

    public bool EditOperationAmount {
      get;
      private set;
    } = false;

    public bool EditRealEstate {
      get;
      private set;
    } = false;

    public bool AllowNoParties {
      get;
      private set;
    } = false;


    public bool AllowUncompletedResource {
      get;
      private set;
    } = false;


    public RecordingActType ChainedRecordingActType {
      get;
      private set;
    } = RecordingActType.Empty;


    public bool HasChainedRule {
      get {
        return !this.ChainedRecordingActType.Equals(RecordingActType.Empty);
      }
    }


    public bool IsAnnotation {
      get;
      private set;
    } = false;


    public bool IsHardLimitation {
      get;
      private set;
    } = false;


    public bool SkipPrelation {
      get;
      private set;
    }

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Land.Registration
