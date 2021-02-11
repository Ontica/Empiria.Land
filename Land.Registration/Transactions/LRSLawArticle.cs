/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSLawArticle                                  Pattern  : Storage Item                        *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Describes a recorder office document type.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.DataTypes;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Describes a recorder office transaction type.</summary>
  public class LRSLawArticle : GeneralObject {

    #region Constructors and parsers

    private LRSLawArticle() {
      // Required by Empiria Framework.
    }

    static public LRSLawArticle Empty {
      get { return BaseObject.ParseEmpty<LRSLawArticle>(); }
    }

    static public LRSLawArticle Unknown {
      get { return BaseObject.ParseUnknown<LRSLawArticle>(); }
    }

    static public LRSLawArticle Parse(int id) {
      return BaseObject.ParseId<LRSLawArticle>(id);
    }

    static public LRSLawArticle Parse(string uid) {
      return BaseObject.ParseKey<LRSLawArticle>(uid);
    }

    static public FixedList<LRSLawArticle> GetList() {
      return GeneralObject.GetList<LRSLawArticle>();
    }

    #endregion Constructors and parsers

    public string FinancialConceptCode {
      get {
        return base.ExtendedDataField.Get("FinancialConcept", String.Empty);
      }
    }

    public bool CalculatedOverTaxableBase {
      get {
        return base.ExtendedDataField.Get("RequiresTaxableBase", false);
      }
    }

    public Unit Unit {
      get {
        return base.ExtendedDataField.Get("Unit", Unit.Empty);
      }
    }
  } // class LRSLawArticle

} // namespace Empiria.Land.Registration.Transactions
