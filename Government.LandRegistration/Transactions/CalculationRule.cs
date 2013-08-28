/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : CalculationRule                                Pattern  : Storage Item                        *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a calculation rule for transaction items according to the law articles.             *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/

namespace Empiria.Government.LandRegistration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class CalculationRule : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.CalculationRule";

    #endregion Fields

    #region Constructors and parsers

    public CalculationRule()
      : base(thisTypeName) {

    }

    protected CalculationRule(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public CalculationRule Empty {
      get { return BaseObject.ParseEmpty<CalculationRule>(thisTypeName); }
    }

    static public CalculationRule Unknown {
      get { return BaseObject.ParseUnknown<CalculationRule>(thisTypeName); }
    }

    static public CalculationRule Parse(int id) {
      return BaseObject.Parse<CalculationRule>(thisTypeName, id);
    }

    static public ObjectList<CalculationRule> GetList() {
      ObjectList<CalculationRule> list = GeneralObject.ParseList<CalculationRule>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class CalculationRule

} // namespace Empiria.Government.LandRegistration.Transactions