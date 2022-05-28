/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing Services                            Component : Filing Forms                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Information Holder                      *
*  Type     : RealPropertyDescription                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a real property already registered or not, or in need of amendments.       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Geography;
using Empiria.Json;

namespace Empiria.Land.Registration.Forms {

  /// <summary>Contains data about a real property already registered or not,
  /// or in need of amendments.</summary>
  public class RealPropertyDescription {

    #region Constructors and parsers


    internal RealPropertyDescription(JsonObject json) {
      Load(json);
    }


    static internal RealPropertyDescription Parse(JsonObject json) {
      Assertion.Require(json, "json");

      return new RealPropertyDescription(json);
    }


    private void Load(JsonObject json) {
      if (json.HasValue("propertyUID")) {

        this.PropertyUID = json.Get<string>("propertyUID");

        if (json.HasValue("amendment")) {
          this.LoadAmendmentData(json.Slice("amendment"));
        }

      } else if (json.HasValue("recordingSeekData")) {
        LoadBooksRecordingData(json.Slice("recordingSeekData"));

      }
    }


    private void LoadAmendmentData(JsonObject json) {
      this.Municipality = Municipality.Parse(json.Get<int>("municipalityUID"));
      this.CadastralKey = json.Get("cadastralKey", String.Empty);
      this.RealPropertyType = RealEstateType.Parse(json.Get<int>("propertyType"));
      this.RealPropertyName = json.Get("propertyName", String.Empty);
      this.Location = json.Get("location", String.Empty);
      this.MetesAndBounds = json.Get("metesAndBounds", String.Empty);
      this.SearchNotes = json.Get("searchNotes", String.Empty);
    }


    private void LoadBooksRecordingData(JsonObject json) {
      this.RecorderOffice = RecorderOffice.Parse(json.Get<int>("districtUID"));
      this.Municipality = Municipality.Parse(json.Get<int>("municipalityUID"));
      this.RecordingBook = RecordingBook.Parse(json.Get<int>("recordingBookUID"));
      this.RecordingNo = json.Get("recordingNo", String.Empty);
      this.RecordingFraction = json.Get("recordingFraction", String.Empty);
      this.CadastralKey = json.Get("cadastralKey", String.Empty);
      this.RealPropertyType = RealEstateType.Parse(json.Get<int>("propertyType"));
      this.RealPropertyName = json.Get("propertyName", String.Empty);
      this.Location = json.Get("location", String.Empty);
      this.MetesAndBounds = json.Get("metesAndBounds", String.Empty);
      this.SearchNotes = json.Get("searchNotes", String.Empty);
    }


    #endregion Constructors and parsers


    #region Properties

    public string PropertyUID {
      get;
      private set;
    } = String.Empty;


    public RecorderOffice RecorderOffice {
      get;
      private set;
    } = RecorderOffice.Empty;


    public Municipality Municipality {
      get;
      private set;
    } = Municipality.Empty;


    public RecordingBook RecordingBook {
      get;
      private set;
    } = RecordingBook.Empty;


    public string RecordingNo {
      get;
      private set;
    } = String.Empty;


    public string RecordingFraction {
      get;
      private set;
    } = String.Empty;


    public string CadastralKey {
      get;
      private set;
    } = String.Empty;


    public RealEstateType RealPropertyType {
      get;
      private set;
    } = RealEstateType.Empty;


    public string RealPropertyName {
      get;
      private set;
    } = String.Empty;


    public string Location {
      get;
      private set;
    } = String.Empty;


    public string MetesAndBounds {
      get;
      private set;
    } = String.Empty;


    public string SearchNotes {
      get;
      private set;
    } = String.Empty;


    public bool OverRegistredPropertyUID {
      get {
        return this.PropertyUID.Length != 0;
      }
    }


    public bool RecordedDataNeedAmendment {
      get;
      private set;
    }


    #endregion Properties


  }  // class RealPropertyDescription

}  // namespace Empiria.Land.Registration.Forms
