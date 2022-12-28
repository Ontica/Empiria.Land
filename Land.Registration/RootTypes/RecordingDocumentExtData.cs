/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentExtData                       Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording document.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingDocumentExtData : IExtensibleData {

    #region Constructors and parsers

    public RecordingDocumentExtData() {

    }

    static internal RecordingDocumentExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingDocumentExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

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
    } = String.Empty;


    public string ExpedientNo {
      get;
      set;
    } = String.Empty;


    public string BookNo {
      get;
      set;
    } = String.Empty;


    public string IssuedByPosition {
      get;
      set;
    } = string.Empty;


    public Contact MainWitness {
      get;
      set;
    } = Person.Empty;


    public string StartSheet {
      get;
      set;
    } = String.Empty;

    public string EndSheet {
      get;
      set;
    } = String.Empty;

    public bool IsEmptyInstance {
      get;
      private set;
    } = false;

    internal int DocumentImageSetId {
      get;
      set;
    } = -1;


    internal int AuxiliarImageSetId {
      get;
      set;
    } = -1;

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

        DocumentImageSetId = this.DocumentImageSetId,
        AuxiliarImageSetId = this.AuxiliarImageSetId
      };
    }

    private void LoadJson(JsonObject json) {
      this.DocumentNo = json.Get<String>("DocumentNo", String.Empty);
      this.BookNo = json.Get<String>("NotaryBook", String.Empty);
      this.StartSheet = json.Get<String>("StartSheet", String.Empty);
      this.EndSheet = json.Get<String>("EndSheet", String.Empty);

      this.ExpedientNo = json.Get<String>("CaseRecordNo", String.Empty);

      this.MainWitness = json.Get<Contact>("WitnessId", Person.Empty);

      this.DocumentImageSetId = json.Get<Int32>("DocumentImageSetId", -1);
      this.AuxiliarImageSetId = json.Get<Int32>("AuxiliarImageSetId", -1);
    }

    public JsonObject GetJson(RecordingDocument document) {
      var json = new JsonObject();

      switch (document.DocumentType.Id) {
        case 2410:
          json.AddIfValue("DocumentNo", document.Number);
          json.AddIfValue("NotaryBook", this.BookNo);
          json.AddIfValue("StartSheet", this.StartSheet);
          json.AddIfValue("EndSheet", this.EndSheet);
          break;

        case 2411:
          json.AddIfValue("DocumentNo", document.Number);
          json.AddIfValue("CaseRecordNo", document.ExpedientNo);
          break;

        case 2412:
          json.AddIfValue("DocumentNo", document.Number);
          json.AddIfValue("CaseRecordNo", document.ExpedientNo);
          break;

        case 2413:
          json.AddIfValue("DocumentNo", document.Number);
          if (this.MainWitness != null) {
            json.AddIfValue("WitnessId", this.MainWitness.Id);
          }
          break;

        case 2414:
          json.AddIfValue("DocumentNo", document.Number);
          break;

        default:
          json.AddIfValue("DocumentNo", document.Number);
          json.AddIfValue("NotaryBook", this.BookNo);
          json.AddIfValue("StartSheet", this.StartSheet);
          json.AddIfValue("EndSheet", this.EndSheet);
          break;

      }

      if (this.DocumentImageSetId != -1) {
        json.Add("DocumentImageSetId", this.DocumentImageSetId);
      }
      if (this.AuxiliarImageSetId != -1) {
        json.Add("AuxiliarImageSetId", this.AuxiliarImageSetId);
      }

      return json;
    }

    #endregion Methods

  }  // class RecordingDocumentExtData

} // namespace Empiria.Land.Registration
