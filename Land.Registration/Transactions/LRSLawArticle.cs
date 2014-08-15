/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSLawArticle                                  Pattern  : Storage Item                        *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSLawArticle : GeneralObject {

    #region Fields

    private const string thisTypeName = "ObjectType.GeneralObject.LRSLawArticle";

    #endregion Fields

    #region Constructors and parsers

    public LRSLawArticle()
      : base(thisTypeName) {

    }

    protected LRSLawArticle(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
    }

    static public LRSLawArticle Empty {
      get { return BaseObject.ParseEmpty<LRSLawArticle>(thisTypeName); }
    }

    static public LRSLawArticle Unknown {
      get { return BaseObject.ParseUnknown<LRSLawArticle>(thisTypeName); }
    }

    static public LRSLawArticle Parse(int id) {
      return BaseObject.Parse<LRSLawArticle>(thisTypeName, id);
    }

    static public FixedList<LRSLawArticle> GetList() {
      FixedList<LRSLawArticle> list = GeneralObject.ParseList<LRSLawArticle>(thisTypeName);

      //list.Sort( (x,y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

    public string FinancialConceptCode {
      get { return base.NamedKey; }
    }

  } // class LRSLawArticle

} // namespace Empiria.Land.Registration.Transactions
