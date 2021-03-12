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


    public string MetesAndBounds {
      get;
      set;
    } = String.Empty;


    public string Notes {
      get;
      set;
    } = String.Empty;


    #endregion Properties

    #region Methods

    internal void AssertIsValid() {
      this.CadastralKey = EmpiriaString.TrimAll(this.CadastralKey);
      this.MetesAndBounds = EmpiriaString.TrimAll(this.MetesAndBounds);
      this.Notes = EmpiriaString.TrimAll(this.Notes);
    }

    public JsonObject GetJson() {
      var json = new JsonObject();

      json.AddIfValue("CadastralKey", this.CadastralKey);
      json.AddIfValue("MetesAndBounds", this.MetesAndBounds);
      json.AddIfValue("Notes", this.Notes);

      return json;
    }

    private void LoadJson(JsonObject json) {
      this.CadastralKey = json.Get<String>("CadastralKey", String.Empty);
      this.MetesAndBounds = json.Get<String>("MetesAndBounds", String.Empty);
      this.Notes = json.Get<String>("Notes", String.Empty);
    }

    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

  }  // class RealEstateExtData

} // namespace Empiria.Land.Registration
