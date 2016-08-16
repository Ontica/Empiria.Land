﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentExtData                       Pattern  : IExtensibleData class               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording document.                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingDocumentExtData : IExtensibleData {

    #region Constructors and parsers

    public RecordingDocumentExtData() {
      this.IssuedByPosition = RoleType.Empty;
    }

    static internal RecordingDocumentExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingDocumentExtData.Empty;
      }

      var json = Empiria.Json.JsonConverter.ToJsonObject(jsonString);

      var data = new RecordingDocumentExtData();
      data.LoadJson(json);

      return data;
    }

    static private readonly RecordingDocumentExtData _empty =
                              new RecordingDocumentExtData() { IsEmptyInstance = true };

    static public RecordingDocumentExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string DocumentNo {
      get;
      set;
    }

    public string ExpedientNo {
      get;
      set;
    }

    public string BookNo {
      get;
      set;
    }

    public RoleType IssuedByPosition {
      get;
      set;
    }

    public Contact MainWitness {
      get;
      set;
    }

    public string StartSheet {
      get;
      set;
    }

    public string EndSheet {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get;
      private set;
    }

    internal int DocumentImageId {
      get;
      set;
    }

    internal int AuxiliarImageId {
      get;
      set;
    }

    #endregion Properties

    #region Methods

    public string ToJson() {
      if (!this.IsEmptyInstance) {
        return JsonConverter.ToJson(this.GetObject());
      } else {
        return String.Empty;
      }
    }

    private object GetObject() {
      return new {
        BookNo = this.BookNo,
        IssuedByPosition = this.IssuedByPosition,
        MainWitness = this.MainWitness,
        StartSheet = this.StartSheet,
        EndSheet = this.EndSheet,

        DocumentImageId = this.DocumentImageId,
        AuxiliarImageId = this.AuxiliarImageId
      };
    }

    private void LoadJson(JsonObject json) {
      this.DocumentNo = json.Get<String>("DocumentNo", String.Empty);
      this.BookNo = json.Get<String>("NotaryBook", String.Empty);
      this.StartSheet = json.Get<String>("StartSheet", String.Empty);
      this.EndSheet = json.Get<String>("EndSheet", String.Empty);

      this.ExpedientNo = json.Get<String>("CaseRecordNo", String.Empty);

      this.MainWitness = json.Get<Contact>("WitnessId", Person.Empty);

      this.DocumentImageId = json.Get<Int32>("DocumentImageId", -1);
      this.AuxiliarImageId = json.Get<Int32>("AuxiliarImageId", -1);
    }

    public JsonObject GetJson(RecordingDocument document) {
      var json = new JsonObject();

      switch (document.DocumentType.Id) {
        case 2410:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          json.AddIfValue(new JsonItem("NotaryBook", this.BookNo));
          json.AddIfValue(new JsonItem("StartSheet", this.StartSheet));
          json.AddIfValue(new JsonItem("EndSheet", this.EndSheet));
          break;

        case 2411:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          json.AddIfValue(new JsonItem("CaseRecordNo", document.ExpedientNo));
          break;

        case 2412:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          json.AddIfValue(new JsonItem("CaseRecordNo", document.ExpedientNo));
          break;

        case 2413:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          if (this.MainWitness != null) {
            json.AddIfValue(new JsonItem("WitnessId", this.MainWitness.Id));
          }
          break;

        case 2414:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          break;

        default:
          json.AddIfValue(new JsonItem("DocumentNo", document.Number));
          json.AddIfValue(new JsonItem("NotaryBook", this.BookNo));
          json.AddIfValue(new JsonItem("StartSheet", this.StartSheet));
          json.AddIfValue(new JsonItem("EndSheet", this.EndSheet));
          break;

      }

      if (this.DocumentImageId != -1) {
        json.Add(new JsonItem("DocumentImageId", this.DocumentImageId));
      }
      if (this.AuxiliarImageId != -1) {
        json.Add(new JsonItem("AuxiliarImageId", this.AuxiliarImageId));
      }

      return json;
    }

    #endregion Methods

  }  // class RecordingDocumentExtData

} // namespace Empiria.Land.Registration
