/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingExtData                               Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds extended data for physical recordings.                                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Holds extended data for physical recordings.</summary>
  public class RecordingExtData {

    #region Constructors and parsers

    static internal RecordingExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var data = new RecordingExtData();

      data.LoadJson(json);

      return data;
    }

    static private readonly RecordingExtData _empty =
                          new RecordingExtData() { IsEmptyInstance = true };

    static public RecordingExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public Contact AuthorizedBy {
      get;
      internal set;
    } = Contact.Empty;


    public int EndImageIndex {
      get;
      internal set;
    } = -1;


    public bool IsEmptyInstance {
      get;
      private set;
    } = false;


    public string Notes {
      get;
      internal set;
    } = String.Empty;


    public Contact ReviewedBy {
      get;
      internal set;
    } = Contact.Empty;


    public int StartImageIndex {
      get;
      internal set;
    } = -1;

    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (!this.AuthorizedBy.IsEmptyInstance) {
        json.Add(new JsonItem("AuthorizedById", this.AuthorizedBy.Id));
      }
      if (this.EndImageIndex != -1) {
        json.AddIfValue(new JsonItem("EndImageIndex", this.EndImageIndex));
      }
      json.AddIfValue(new JsonItem("Notes", this.Notes));
      if (!this.ReviewedBy.IsEmptyInstance) {
        json.AddIfValue(new JsonItem("ReviewedById", this.ReviewedBy.Id));
      }
      if (this.StartImageIndex != -1) {
        json.AddIfValue(new JsonItem("StartImageIndex", this.StartImageIndex));
      }
      return json;
    }


    private void LoadJson(JsonObject json) {
      this.AuthorizedBy = json.Get<Contact>("AuthorizedById", Contact.Empty);
      this.EndImageIndex = json.Get<int>("EndImageIndex", -1);
      this.Notes = json.Get<String>("Notes", String.Empty);
      this.ReviewedBy = json.Get<Contact>("ReviewedById", Contact.Empty);
      this.StartImageIndex = json.Get<int>("StartImageIndex", -1);
    }


    #endregion Methods

  }  // class RecordingExtData

}  // namespace Empiria.Land.Registration
