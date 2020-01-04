/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RealEstateExtData                              Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds extensible data for real estates.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;
using Empiria.Geography;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Holds extensible data for real estates.</summary>
  public class RealEstateExtData {

    #region Constructors and parsers

    public RealEstateExtData() {

    }

    static internal RealEstateExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RealEstateExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var data = new RealEstateExtData();
      data.LoadJson(json);

      return data;
    }

    static private readonly RealEstateExtData _empty =
                              new RealEstateExtData() { IsEmptyInstance = true };

    static public RealEstateExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public bool IsEmptyInstance {
      get;
      private set;
    } = false;


    public string CadastralKey {
      get;
      set;
    } = String.Empty;


    public string Name {
      get;
      set;
    } = String.Empty;


    public RealEstateType RealEstateType {
      get;
      set;
    } = RealEstateType.Empty;


    public string MetesAndBounds {
      get;
      set;
    } = String.Empty;


    public RecorderOffice District {
      get;
      set;
    } = RecorderOffice.Empty;


    public Municipality Municipality {
      get;
      set;
    } = Municipality.Empty;


    public string LocationReference {
      get;
      set;
    } = String.Empty;


    public Quantity LotSize {
      get;
      set;
    } = Quantity.Zero;


    public string Notes {
      get;
      set;
    } = String.Empty;


    #endregion Properties

    #region Methods

    internal void AssertIsValid() {
      this.CadastralKey = EmpiriaString.TrimAll(this.CadastralKey);
      this.Name = EmpiriaString.TrimAll(this.Name);
      this.MetesAndBounds = EmpiriaString.TrimAll(this.MetesAndBounds);
      this.LocationReference = EmpiriaString.TrimAll(this.LocationReference);
      this.Notes = EmpiriaString.TrimAll(this.Notes);
    }

    public JsonObject GetJson() {
      var json = new JsonObject();

      json.AddIfValue("CadastralKey", this.CadastralKey);
      json.AddIfValue("Name", this.Name);
      if (!this.RealEstateType.IsEmptyInstance) {
        json.Add("RealEstateTypeId", this.RealEstateType.Id);
      }
      json.AddIfValue("MetesAndBounds", this.MetesAndBounds);
      if (!this.District.IsEmptyInstance) {
        json.Add("DistrictId", this.District.Id);
      }
      if (!this.Municipality.IsEmptyInstance) {
        json.Add("MunicipalityId", this.Municipality.Id);
      }
      json.AddIfValue("LocationReference", this.LocationReference);
      if (this.LotSize != Quantity.Zero) {
        json.Add("LotSize", this.LotSize.Amount);
        json.Add("LotSizeUnitId", this.LotSize.Unit.Id);
      }
      json.AddIfValue("Notes", this.Notes);

      return json;
    }

    private void LoadJson(JsonObject json) {
      this.CadastralKey = json.Get<String>("CadastralKey", String.Empty);
      this.Name = json.Get<String>("Name", String.Empty);
      this.RealEstateType = RealEstateType.Parse(json.Get<Int32>("RealEstateTypeId", -1));
      this.MetesAndBounds = json.Get<String>("MetesAndBounds", String.Empty);
      this.District = RecorderOffice.Parse(json.Get<Int32>("DistrictId", -1));
      this.Municipality = Municipality.Parse(json.Get<Int32>("MunicipalityId", -1));
      this.LocationReference = json.Get<String>("LocationReference", String.Empty);
      this.LotSize = Quantity.Parse(Unit.Parse(json.Get<Int32>("LotSizeUnitId", -1)),
                                    json.Get<decimal>("LotSize", 0m));
      this.Notes = json.Get<String>("Notes", String.Empty);
    }

    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

  }  // class RealEstateExtData

} // namespace Empiria.Land.Registration
