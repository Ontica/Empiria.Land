/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land                                   Assembly : Empiria.Land                        *
*  Type      : InterestData                                   Pattern  : IExtensibleData class               *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains data about a financial interest rate and term.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using Empiria.DataTypes;

namespace Empiria.Land.Registration {

  /// <summary>Contains data about a financial interest rate and term.</summary>
  public class InterestData : IExtensibleData {

    #region Constructors and parsers

    public InterestData() {
      this.Rate = 0.0000m;
      this.RateType = InterestRateType.Empty;
      this.TermPeriods = 0;
      this.TermUnit = Unit.Empty;
    }

    static public InterestData Empty {
      get {
        return new InterestData();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public decimal Rate {
      get;
      set;
    }

    public InterestRateType RateType {
      get; 
      set;
    }

    public int TermPeriods {
      get; 
      set;
    }

    public Unit TermUnit {
      get; 
      set;
    }

    public bool IsEmptyInstance {
      get {
        if (this.Rate == decimal.Zero && 
            this.RateType.IsEmptyInstance && 
            this.TermPeriods == 0 &&
            this.TermUnit.IsEmptyInstance) {
          return true;
        }
        return false;
      }
    }

    #endregion Properties

    #region Methods
    
    public string ToJson() {
      return Empiria.Data.JsonConverter.ToJson(this);
    }

    #endregion Methods

  }  // class InterestData

} // namespace Empiria.Land.Registration