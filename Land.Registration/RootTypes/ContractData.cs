/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ContractData                                   Pattern  : IExtensibleData class               *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains data about a contract.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using Empiria.Geography;

namespace Empiria.Land.Registration {

  /// <summary>Contains data about a contract.</summary>
  public class ContractData : IExtensibleData {

    #region Constructors and parsers

    public ContractData() {
      this.Date = ExecutionServer.DateMaxValue;
      this.Place = GeographicRegionItem.Empty;
      this.Number = String.Empty;
      this.Interest = InterestData.Empty;
    }

    static public ContractData Empty {
      get {
        return new ContractData();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public DateTime Date {
      get; set;
    }

    public string Number {
      get; set;
    }

    public GeographicRegionItem Place {
      get; set;
    }

    public InterestData Interest {
      get; set;
    }

    public bool IsEmptyInstance {
      get {
        if (this.Date == ExecutionServer.DateMaxValue &&
            this.Place.IsEmptyInstance &&
            this.Number == String.Empty &&
            this.Interest.IsEmptyInstance) {
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

  }  // class ContractData

} // namespace Empiria.Land.Registration
