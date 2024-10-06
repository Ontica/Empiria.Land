/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LRSLawArticle                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Law article applicable to pay a service reequested to a Recorder Office.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Measurement;

namespace Empiria.Land.Transactions {

  /// <summary>Law article applicable to pay a service reequested to a Recorder Office.</summary>
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

    #region Properties

    public bool Autocalculated {
      get {
        return base.ExtendedDataField.Get("Autocalculated", false);
      }
    }

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

    #endregion Properties

  } // class LRSLawArticle

} // namespace Empiria.Land.Transactions
