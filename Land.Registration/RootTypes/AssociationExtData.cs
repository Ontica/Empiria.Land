/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : AssociationExtData                             Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds extensible data for social organizations.                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Holds extensible data for social organizations.</summary>
  public class AssociationExtData  {

    #region Constructors and parsers

    private AssociationExtData() {

    }

    public AssociationExtData(string name) {
      Assertion.AssertObject(name, "name");

      this.Name = name;
    }

    static internal AssociationExtData ParseJson(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return AssociationExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);
      var instance = new AssociationExtData();

      instance.LoadJson(json);

      return instance;
    }

    static private readonly AssociationExtData _empty =
                              new AssociationExtData() { IsEmptyInstance = true };

    static public AssociationExtData Empty {
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


    public string Name {
      get;
      private set;
    } = String.Empty;


    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      json.AddIfValue(new JsonItem("Name", this.Name));

      return json;
    }


    private void LoadJson(JsonObject json) {
      this.Name = json.Get<String>("Name", String.Empty);
    }


    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

  }  // class AssociationExtData

}  // namespace Empiria.Land.Registration
