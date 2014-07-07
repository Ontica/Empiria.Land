/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ContractData                                   Pattern  : IExtensibleData class               *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains data about a contract.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.DataTypes;
using Empiria.Geography;

namespace Empiria.Land.Registration {

  /// <summary>Contains data about a contract.</summary>
  public class ContractData : IExtensibleData {

    #region Constructors and parsers

    public ContractData() {
      this.Date = ExecutionServer.DateMaxValue;
      this.Place = GeographicRegionItem.Empty;
      this.Number = String.Empty;
      this.Interest = Interest.Empty;
    }

    static internal ContractData Parse(Empiria.Data.JsonObject json) {
      if (json.IsEmptyInstance) {
        return ContractData.Empty;
      }

      var contract = new ContractData();
      contract.Number = json.Find<String>("Number", String.Empty);
      contract.Date = json.Find<DateTime>("Date", contract.Date);
      contract.Place = json.Find<GeographicRegionItem>("PlaceId", contract.Place);
      contract.Interest = Interest.Parse(json.Slice("Interest"));

      return contract;
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

    public Interest Interest {
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
