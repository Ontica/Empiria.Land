/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActType                               Pattern  : Power type                          *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Power type that defines a recording act type.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

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

    private RecordingActType(int typeId) : base(typeId) {
      // Empiria powertype types always have this constructor.
    }

    static public new RecordingActType Parse(int typeId) {
      return ObjectTypeInfo.Parse<RecordingActType>(typeId);
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
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return true;
          }
        }
        return false;
      }
    }

    public bool Autoregister {
      get {
        string[] array = autoRegisterVector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return true;
          }
        }
        return false;
      }
    }

    public bool BlockAllFields {
      get {
        string[] array = blockAllFieldsVector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return true;
          }
        }
        return false;
      }
    }

    public bool IsAnnotationType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.AnnotationAct");
      }
    }

    public bool IsDomainActType {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.DomainAct");
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
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == this.Id) {
            return true;
          }
        }
        return false;
      }
    }

    public bool UseOperationAmount {
      get {
        if (this.BlockAllFields) {
          return false;
        }
        string[] array = blockOperationAmountVector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return false;
          }
        }
        return true;
      }
    }

    #endregion Public properties

    #region Public methods

    /// <summary>Factory method to create recording acts</summary>
    internal RecordingAct CreateInstance() {
      throw new NotImplementedException();
    }

    public FixedList<RecordingActType> GetAppliesToRecordingActTypesList() {
      var json = Empiria.Data.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<RecordingActType>("AppliesTo");      
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    public FixedList<LRSLawArticle> GetFinancialLawArticles() {
      var json = Empiria.Data.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<LRSLawArticle>("FinancialConcepts");
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    public FixedList<DomainActPartyRole> GetRoles() {
      var json = Empiria.Data.JsonObject.Parse(base.ExtensionData);

      var list = json.GetList<DomainActPartyRole>("Roles");
      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list.ToFixedList();
    }

    #endregion Public methods

  } // class RecordingActType

} // namespace Empiria.Land.Registration
