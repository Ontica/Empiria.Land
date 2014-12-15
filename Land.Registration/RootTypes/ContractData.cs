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
      this.Place = GeographicRegion.Empty;
      this.Number = String.Empty;
      this.Interest = Interest.Empty;
    }

    static internal ContractData Parse(Empiria.Json.JsonObject json) {
      if (json.IsEmptyInstance) {
        return ContractData.Empty;
      }

      var contract = new ContractData();
      contract.Number = json.Get<String>("Number", String.Empty);
      contract.Date = json.Get<DateTime>("Date", contract.Date);
      contract.Place = json.Get<GeographicRegion>("PlaceId", contract.Place);
      contract.Interest = Interest.Parse(json.Slice("Interest"));

      return contract;
    }

    static private readonly ContractData _empty = new ContractData() {
      IsEmptyInstance = true
    };

    static public ContractData Empty {
      get {
        return _empty;
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

    public GeographicRegion Place {
      get; set;
    }

    public Interest Interest {
      get; set;
    }

    public bool IsEmptyInstance {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public string ToJson() {
      return Empiria.Json.JsonConverter.ToJson(this);
    }

    #endregion Methods

  }  // class ContractData

} // namespace Empiria.Land.Registration
