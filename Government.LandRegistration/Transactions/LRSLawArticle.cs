﻿/* Empiria® Land 2013 ****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria® Land                                  System   : Land Registration System            *
*  Namespace : Empiria.Government.LandRegistration            Assembly : Empiria.Government.LandRegistration *
*  Type      : LRSLawArticle                                  Pattern  : Storage Item                        *
*  Date      : 23/Oct/2013                                    Version  : 5.2     License: CC BY-NC-SA 3.0    *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
**************************************************** Copyright © La Vía Óntica SC + Ontica LLC. 1999-2013. **/

namespace Empiria.Government.LandRegistration.Transactions {

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

    static public ObjectList<LRSLawArticle> GetList() {
      ObjectList<LRSLawArticle> list = GeneralObject.ParseList<LRSLawArticle>(thisTypeName);

      //list.Sort( (x,y) => x.Name.CompareTo(y.Name));

      return list;
    }

    #endregion Constructors and parsers

    public string FinancialConceptCode {
      get { return base.NamedKey; }
    }

  } // class LRSLawArticle

} // namespace Empiria.Government.LandRegistration.Transactions