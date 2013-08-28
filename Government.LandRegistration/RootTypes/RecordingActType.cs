/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingActType                               Pattern  : Power type                          *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Power type that defines a recording act type.                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/
using System;

using Empiria.Government.LandRegistration.Transactions;
using Empiria.Ontology;

namespace Empiria.Government.LandRegistration {

  /// <summary>Power type that defines a recording act type.</summary>
  public sealed class RecordingActType : PowerType<RecordingAct> {

    #region Fields

    private const string thisTypeName = "PowerType.RecordingActType";

    static readonly string allowEmptyPartiesVector = ConfigurationData.GetString("AllowEmptyParties.Vector");
    static readonly string autoRegisterVector = ConfigurationData.GetString("AutoRegister.RecordingActType");
    static readonly string blockAllFieldsVector = ConfigurationData.GetString("BlockAllFields.Vector");
    static readonly string blockFirstPropertyOwnerVector = ConfigurationData.GetString("BlockFirstPropertyOwner.Vector");
    static readonly string blockOperationAmountVector = ConfigurationData.GetString("BlockOperationAmount.Vector");
    static readonly string useCreditFieldsVector = ConfigurationData.GetString("UseCreditFields.Vector");

    private RecordingSection registerSectionType = RecordingSection.Empty;

    #endregion Fields

    #region Constructors and parsers

    private RecordingActType(int typeId)
      : base(thisTypeName, typeId) {
      // Empiria Power type pattern classes always has this constructor. Don't delete
    }

    static public new RecordingActType Parse(int typeId) {
      return PowerType<RecordingAct>.Parse<RecordingActType>(typeId);
    }

    static internal RecordingActType Parse(ObjectTypeInfo typeInfo) {
      return PowerType<RecordingAct>.Parse<RecordingActType>(typeInfo);
    }

    static public RecordingActType Empty {
      get {
        return RecordingActType.Parse(ObjectTypeInfo.Parse("ObjectType.RecordingAct.InformationAct.Empty"));
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

    public bool IsAnnotation {
      get {
        return base.Name.StartsWith("ObjectType.RecordingAct.AnnotationAct");
      }
    }

    public RecordingRule RecordingRule {
      get {
        string s = base.GetAttribute<string>("RecordingActType_RecordingRule");
        return RecordingRule.Parse(s);
      }
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

    public bool UseFirstPropertyOwner {
      get {
        if (this.BlockAllFields) {
          return false;
        }
        string[] array = blockFirstPropertyOwnerVector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == this.Id) {
            return false;
          }
        }
        return true;
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

    public ObjectList<RecordingActType> GetAppliesToRecordingActTypesList() {
      var list = base.GetTypeLinks<RecordingActType>("RecordingActType_AppliesToRecordingAct");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    public ObjectList<LRSLawArticle> GetFinancialLawArticles() {
      var list = base.GetLinks<LRSLawArticle>("RecordingActType_FinancialLawArticle");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    public ObjectList<DomainActPartyRole> GetRoles() {
      var list = base.GetLinks<DomainActPartyRole>("RecordingActType_Roles");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Public methods

  } // class RecordingActType

} // namespace Empiria.Government.LandRegistration