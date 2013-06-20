/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : RecordingActType                               Pattern  : Power type                          *
*  Date      : 25/Jun/2013                                    Version  : 5.1     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Power type that defines a recording act type.                                                 *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1994-2013. **/

using Empiria.Government.LandRegistration.Transactions;
using Empiria.Ontology;

namespace Empiria.Government.LandRegistration {

  /// <summary>Power type that defines a recording act type.</summary>
  public sealed class RecordingActType : PowerType<RecordingAct> {

    #region Fields

    private const string thisTypeName = "PowerType.RecordingActType";

    private static readonly string allowEmptyPartiesVector = ConfigurationData.GetString("AllowEmptyParties.Vector");
    private static readonly string autoRegisterVector = ConfigurationData.GetString("AutoRegister.RecordingActType");
    private static readonly string blockAllFieldsVector = ConfigurationData.GetString("BlockAllFields.Vector");
    private static readonly string blockFirstPropertyOwnerVector = ConfigurationData.GetString("BlockFirstPropertyOwner.Vector");
    private static readonly string blockOperationAmountVector = ConfigurationData.GetString("BlockOperationAmount.Vector");
    private static readonly string useCreditFieldsVector = ConfigurationData.GetString("UseCreditFields.Vector");

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

    public bool AllowsRegisteredProperties {
      get {
        string vector = ConfigurationData.GetString("AllowsRegisteredProperties.Vector");
        string[] array = vector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return true;
          }
        }
        return true;
      }
    }

    public bool AllowsUnregisteredProperties {
      get {
        string vector = ConfigurationData.GetString("AllowsUnregisteredProperties.Vector");
        string[] array = vector.Split('~');
        for (int i = 0; i < array.Length; i++) {
          if (int.Parse(array[i]) == base.Id) {
            return true;
          }
        }
        return true;
      }
    }

    public bool IsPropertyLinked {
      get {
        return this.AllowsRegisteredProperties || this.AllowsUnregisteredProperties;
      }
    }

    public bool IsRecordingActLinked {
      get {
        return false;
      }
    }

    public RecordingSectionType MainRecordingSectionType {
      get {
        return RecordingSectionType.Parse(1060);
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
      ObjectList<RecordingActType> list = base.GetTypeLinks<RecordingActType>("RecordingActType_AppliesToRecordingAct");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }


    public ObjectList<DomainActPartyRole> GetRoles() {
      ObjectList<DomainActPartyRole> list = base.GetLinks<DomainActPartyRole>("RecordingActType_Roles");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    public ObjectList<LRSLawArticle> GetFinancialLawArticles() {
      ObjectList<LRSLawArticle> list = base.GetLinks<LRSLawArticle>("RecordingActType_FinancialLawArticle");

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Public methods

  } // class RecordingActType

} // namespace Empiria.Government.LandRegistration