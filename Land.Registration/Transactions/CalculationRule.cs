/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : CalculationRule                                Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a calculation rule for transaction items according to the law articles.             *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

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

    static public FixedList<CalculationRule> GetList() {
      FixedList<CalculationRule> list = GeneralObject.ParseList<CalculationRule>(thisTypeName);

      list.Sort((x, y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

  } // class CalculationRule

} // namespace Empiria.Land.Registration.Transactions
