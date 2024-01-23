/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Extended object data holder           *
*  Type     : BookEntryExtData                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds extended data for physical book entries.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Holds extended data for physical book entries.</summary>
  public class BookEntryExtData {

    #region Constructors and parsers

    static internal BookEntryExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return BookEntryExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var data = new BookEntryExtData();

      data.LoadJson(json);

      return data;
    }

    static private readonly BookEntryExtData _empty =
                          new BookEntryExtData() { IsEmptyInstance = true };

    static public BookEntryExtData Empty {
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
        json.Add("AuthorizedById", this.AuthorizedBy.Id);
      }
      if (this.EndImageIndex != -1) {
        json.AddIfValue("EndImageIndex", this.EndImageIndex);
      }
      json.AddIfValue("Notes", this.Notes);
      if (!this.ReviewedBy.IsEmptyInstance) {
        json.AddIfValue("ReviewedById", this.ReviewedBy.Id);
      }
      if (this.StartImageIndex != -1) {
        json.AddIfValue("StartImageIndex", this.StartImageIndex);
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

  }  // class BookEntryExtData

}  // namespace Empiria.Land.Registration
