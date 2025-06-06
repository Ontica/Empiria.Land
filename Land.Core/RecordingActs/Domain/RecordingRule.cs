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

using Empiria.Financial;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Indicates the legal recording status of a resource in order to apply the recording act.</summary>
  public enum ResourceRecordingStatus {
    Undefined,

    NotApply,

    Antecedent,

    Registered,

    Unregistered,

    Both          // Registered OR Unregistered
  }


  /// <summary>Indicates the resource type of which is applicable the recording act.</summary>
  public enum RecordingRuleApplication {
    Undefined,

    Structure,
    RealEstate,
    RealEstateAct,

    Association,
    AssociationAct,

    NoProperty,
    NoPropertyAct,

    Party
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
        var json = recordingActType.ExtensionData.Slice("RegistrationRule", false);

        this.AppliesTo = json.Get("AppliesTo", RecordingRuleApplication.Undefined);
        this.AutoCancel = json.Get("AutoCancel", 0);

        this.ResourceRecordingStatus = json.Get("ResourceStatus", ResourceRecordingStatus.Undefined);
        this.RecordingSection = json.Get("RecordingSectionId", RecordingSection.Empty);
        this.SpecialCase = json.Get("SpecialCase", String.Empty);
        this.RecordingActTypes = json.GetList<RecordingActType>("RecordingActTypes", false).ToArray();
        this.ReplaceableBy = json.GetList<RecordingActType>("ReplaceableBy", false).ToFixedList();
        this.AllowPartitions = json.Get("AllowPartitions", false);
        this.IsEndingAct = json.Get("IsEndingAct", false);
        this.IsActive = json.Get("IsActive", json.HasItems);
        this.AskForResourceName = json.Get("AskForResourceName", false);
        this.ResourceTypeName = json.Get("ResourceTypeName", String.Empty);
        this.DynamicActNamePattern = json.Get("DynamicActNamePattern", String.Empty);

        this.EditResource = json.Get("EditRealEstate", false);
        this.EditAppraisalAmount = json.Get("EditAppraisalAmount", false);
        this.EditOperationAmount = json.Get("EditOperationAmount", false);

        this.Kinds = json.GetList<string>("Kinds", false).ToArray();
        this.Fields = json.GetList<string>("Fields", false).ToArray();

        this.AllowNoParties = json.Get("AllowNoParties", false);
        this.AllowUncompletedResource = json.Get("AllowUncompletedResource", false);
        this.ChainedRecordingActType = json.Get("ChainedAct", RecordingActType.Empty);

        this.IsAnnotation = json.Get("IsAnnotation", false);
        this.IsSoftLimitation = json.Get("IsSoftLimitation", false);
        this.SkipPrelation = json.Get("SkipPrelation", false);

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
      var json = new JsonObject();

      json.Add("IsModification", this.recordingActType.IsModificationActType);
      json.Add("IsCancelation", this.recordingActType.IsCancelationActType);
      json.Add("IsEndingAct", this.IsEndingAct);
      json.Add("AllowPartitions", this.AllowPartitions);
      json.Add("AppliesTo", this.AppliesTo.ToString());
      json.Add("AutoCancel", this.AutoCancel);
      json.Add("AskForResourceName", this.AskForResourceName);
      json.Add("ResourceRecordingStatus", this.ResourceRecordingStatus.ToString());
      json.Add("SpecialCase", this.SpecialCase);
      json.Add("IsActive", this.IsActive);
      json.Add("EditRealEstate", this.EditResource);
      json.Add("EditAppraisalAmount", this.EditAppraisalAmount);
      json.Add("EditOperationAmount", this.EditOperationAmount);
      json.Add("AllowNoParties", this.AllowNoParties);
      json.Add("AllowUncompletedResource", this.AllowUncompletedResource);
      json.Add("IsAnnotation", this.IsAnnotation);
      json.Add("IsSoftLimitation", this.IsSoftLimitation);
      json.Add("SkipPrelation", this.SkipPrelation);

      return json;
    }

    static internal RecordingRule Parse(RecordingActType recordingActType) {
      Assertion.Require(recordingActType, "recordingActType");

      return new RecordingRule(recordingActType);
    }

    #endregion Constructors and parsers

    #region Properties

    public bool AllowPartitions {
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


    public FixedList<RecordingActType> ReplaceableBy {
      get; private set;
    } = new FixedList<RecordingActType>();


    public string SpecialCase {
      get;
      private set;
    } = String.Empty;


    public bool IsActive {
      get;
      private set;
    } = true;


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


    public FixedList<Currency> Currencies {
      get {
        var currencies = GeneralList.Parse("Land.OperationAmount.CurrenciesList");

        return currencies.GetItems<Currency>();
      }
    }

    public string[] Kinds {
      get;
      private set;
    } = new string[0];


    public string[] Fields {
      get;
      private set;
    } = new string[0];


    public bool EditResource {
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
      get {
        return recordingActType.IsLimitationActType && !IsSoftLimitation;
      }
    }

    public bool IsSoftLimitation {
      get;
      private set;
    }


    public bool SkipPrelation {
      get;
      private set;
    }

    #endregion Properties

  }  // class RecordingRule

}  // namespace Empiria.Land.Registration
