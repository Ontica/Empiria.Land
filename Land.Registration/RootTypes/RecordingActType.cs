/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActType                               Pattern  : Power type                          *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Power type that defines a recording act type.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;

using Empiria.Land.Registration.Transactions;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Power type that defines a recording act type.</summary>
  [Powertype(typeof(RecordingAct))]
  public sealed class RecordingActType : Powertype {

    #region Fields

    static readonly string allowEmptyPartiesVector = ConfigurationData.GetString("AllowEmptyParties.Vector");
    static readonly string autoRegisterVector = ConfigurationData.GetString("AutoRegister.RecordingActType");
    static readonly string blockAllFieldsVector = ConfigurationData.GetString("BlockAllFields.Vector");
    static readonly string blockFirstPropertyOwnerVector = ConfigurationData.GetString("BlockFirstPropertyOwner.Vector");
    static readonly string blockOperationAmountVector = ConfigurationData.GetString("BlockOperationAmount.Vector");
    static readonly string useCreditFieldsVector = ConfigurationData.GetString("UseCreditFields.Vector");

    private RecordingSection registerSectionType = RecordingSection.Empty;

    #endregion Fields

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

    static internal new RecordingActType Parse(string typeName) {
      return RecordingActType.Parse<RecordingActType>(typeName);
    }

    static public RecordingActType Empty {
      get {
        return RecordingActType.Parse("ObjectType.RecordingAct.InformationAct.Empty");
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    public bool AllowsEmptyParties {
      get {
        string[] array = allowEmptyPartiesVector.Split('~');

        return array.Contains(base.Id.ToString());
      }
    }

    public bool Autoregister {
      get {
        string[] array = autoRegisterVector.Split('~');

        return array.Contains(base.Id.ToString());
      }
    }

    public bool BlockAllFields {
      get {
        string[] array = blockAllFieldsVector.Split('~');

        return array.Contains(base.Id.ToString());
      }
    }

    public bool IsCancelationActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.CancelationAct");
      }
    }

    public bool IsDomainActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.DomainAct");
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
        return base.Name.StartsWith("ObjectType.RecordingAct.StructureAct");
      }
    }

    public RecordingRule RecordingRule {
      get { return RecordingRule.Parse(base.ExtensionData); }
    }

    public bool UseCreditFields {
      get {
        if (this.BlockAllFields) {
          return false;
        }
        string[] array = useCreditFieldsVector.Split('~');

        return array.Contains(base.Id.ToString());
      }
    }

    public bool UseOperationAmount {
      get {
        if (this.BlockAllFields) {
          return false;
        }
        string[] array = blockOperationAmountVector.Split('~');

        return array.Contains(base.Id.ToString());
      }
    }

    public ResourceRole ResourceRole {
      get;
      private set;
    } = ResourceRole.Informative;

    #endregion Public properties

    #region Public methods

    /// <summary>Factory method to create recording acts</summary>
    internal RecordingAct CreateInstance() {
      return base.CreateObject<RecordingAct>();
    }

    public RecordingRuleApplication AppliesTo {
      get {
        return this.RecordingRule.AppliesTo;
      }
    }

    public bool AppliesToResources {
      get {
        return (this.RecordingRule.AppliesTo == RecordingRuleApplication.Association ||
                this.RecordingRule.AppliesTo == RecordingRuleApplication.Property);
      }
    }

    public FixedList<RecordingActType> GetAppliesToRecordingActTypesList() {
      var json = Empiria.Json.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<RecordingActType>("RecordingActTypes");
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    public FixedList<LRSLawArticle> GetFinancialLawArticles() {
      var json = Empiria.Json.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<LRSLawArticle>("FinancialConcepts");
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    public FixedList<DomainActPartyRole> GetRoles() {
      var json = Empiria.Json.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<DomainActPartyRole>("Roles");
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    #endregion Public methods

  } // class RecordingActType

} // namespace Empiria.Land.Registration
