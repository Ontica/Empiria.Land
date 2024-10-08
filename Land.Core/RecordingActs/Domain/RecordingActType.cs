﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActType                               Pattern  : Power type                          *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that defines a recording act type.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Measurement;
using Empiria.Ontology;

using Empiria.Land.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Power type that defines a recording act type.</summary>
  [Powertype(typeof(RecordingAct))]
  public sealed class RecordingActType : Powertype {

    #region Constructors and parsers

    private RecordingActType() {
      // Empiria powertype types always have this constructor.
    }

    static public new RecordingActType Parse(int typeId) {
      if (typeId != -1) {
        return ObjectTypeInfo.Parse<RecordingActType>(typeId);
      } else {
        return RecordingActType.Empty;
      }
    }

    static public new RecordingActType Parse(string typeName) {
      return RecordingActType.Parse<RecordingActType>(typeName);
    }

    static public RecordingActType Empty {
      get {
        return RecordingActType.Parse("ObjectType.RecordingAct.InformationAct.Empty");
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public bool IsAmendmentActType {
      get {
        return (this.IsCancelationActType || this.IsModificationActType);
      }
    }

    public bool IsCancelationActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.CancelationAct");
      }
    }

    public bool IsDomainActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.DomainAct") ||
               this.Id == 2200;
      }
    }

    public bool IsInformationActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.InformationAct");
      }
    }

    public bool IsLimitationActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.LimitationAct");
      }
    }

    public bool IsModificationActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.ModificationAct");
      }
    }

    public bool IsStructureActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.StructureAct") ||
                                    EmpiriaMath.IsMemberOf(this.Id, new int[] { 2335, 2784 , 2786, 2787, 2788 });
      }
    }

    public RecordingRule RecordingRule {
      get { return RecordingRule.Parse(this); }
    }

    public ResourceRole ResourceRole {
      get;
      private set;
    } = ResourceRole.Informative;

    #endregion Public properties

    #region Public methods

    internal void AssertIsApplicableResource(Resource resourceToApply) {
      Assertion.Require(resourceToApply, "resourceToApply");

      switch (this.RecordingRule.AppliesTo) {
        case RecordingRuleApplication.Association:
        case RecordingRuleApplication.AssociationAct:
          Assertion.Require(resourceToApply is Association,
            "This recording act is not applicable to associations.");
          return;
        case RecordingRuleApplication.NoProperty:
        case RecordingRuleApplication.NoPropertyAct:
          Assertion.Require(resourceToApply is NoPropertyResource,
            "This recording act is applicable only to documents (no property resources).");
          return;
        case RecordingRuleApplication.RealEstate:
        case RecordingRuleApplication.RealEstateAct:
          Assertion.Require(resourceToApply is RealEstate,
            "This recording act is applicable only to real estate resources.");
          return;
        case RecordingRuleApplication.Structure:
          Assertion.Require(resourceToApply is RealEstate,
                           "Structure acts are only applicable to real estate resources.");
          return;
        case RecordingRuleApplication.Undefined:
          Assertion.Require(this.Equals(InformationAct.Empty.RecordingActType),
                  "RecordingRuleApplication.Undefined is only applicable to the empty information act.");
          return;
        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }

    /// <summary>Factory method to create recording acts</summary>
    internal RecordingAct CreateInstance() {
      return base.CreateObject<RecordingAct>();
    }

    public RecordingRuleApplication AppliesTo {
      get {
        return this.RecordingRule.AppliesTo;
      }
    }

    public bool AppliesToARecordingAct {
      get {
        return (this.AppliesTo == RecordingRuleApplication.AssociationAct ||
                this.AppliesTo == RecordingRuleApplication.NoPropertyAct ||
                this.AppliesTo == RecordingRuleApplication.RealEstateAct);
      }
    }


    public FixedList<RecordingActType> GetAppliesToRecordingActTypesList() {
      var json = base.ExtensionData;

      var list = json.GetList<RecordingActType>("RegistrationRule/RecordingActTypes", false);

      list.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

      return list.ToFixedList();
    }

    /// <summary>Returns the default fee units (typically minimal-wage days)
    /// for this recording act type.</summary>
    /// <returns>The default fee units or -1 if the fee depends on each case.</returns>
    public decimal GetFeeUnits() {
      var json = base.ExtensionData;

      return json.Get<decimal>("FeeUnits", -1);
    }

    public FixedList<LRSLawArticle> GetFinancialLawArticles() {
      var json = base.ExtensionData;

      var list = json.GetList<LRSLawArticle>("FinancialConcepts", false);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    public FixedList<Unit> GetPartyPartUnits() {
      var list = GeneralList.Parse("Land.PartyPart.Units");

      return list.GetItems<Unit>();
    }


    public FixedList<DomainActPartyRole> GetPrimaryRoles() {
      var json = base.ExtensionData;

      var list = json.GetList<DomainActPartyRole>("RegistrationRule/Roles", false);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    #endregion Public methods

  } // class RecordingActType

} // namespace Empiria.Land.Registration
