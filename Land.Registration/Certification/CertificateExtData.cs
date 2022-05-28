/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateExtData                             Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds extensible data for certificates.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.Land.Certification {

  /// <summary>Holds extensible data for certificates.</summary>
  public class CertificateExtData : IExtensibleData {

    #region Constructors and parsers

    internal CertificateExtData() {

    }

    internal CertificateExtData(CertificateDTO data) {
      Assertion.Require(data, "data");

      this.LoadDTOData(data);
    }

    static internal CertificateExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return CertificateExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var data = new CertificateExtData();
      data.LoadJson(json);

      return data;
    }

    static private readonly CertificateExtData _empty = new CertificateExtData() { IsEmptyInstance = true };

    static public CertificateExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string PropertyCommonName {
      get;
      private set;
    }

    public string PropertyLocation {
      get;
      private set;
    }

    public string PropertyMetesAndBounds {
      get;
      private set;
    }

    public string Operation {
      get;
      private set;
    }

    public DateTime OperationDate {
      get;
      private set;
    }

    public string SeekForName {
      get;
      private set;
    }

    public int StartingYear {
      get;
      private set;
    }

    public string FromOwnerName {
      get;
      private set;
    }

    public string MarginalNotes {
      get;
      private set;
    }

    public bool UseMarginalNotesAsFullBody {
      get;
      private set;
    }

    public bool IsEmptyInstance {
      get;
      private set;
    }

    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.PropertyCommonName, this.PropertyLocation,
                                           this.SeekForName, this.FromOwnerName, this.Operation);
      }
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
        PropertyCommonName = this.PropertyCommonName,
        PropertyLocation = this.PropertyLocation,
        PropertyMetesAndBounds = this.PropertyMetesAndBounds,
        Operation = this.Operation,
        OperationDate = this.OperationDate,
        SeekForName = this.SeekForName,
        StartingYear = this.StartingYear,
        FromOwnerName = this.FromOwnerName,
        MarginalNotes = this.MarginalNotes,
        UseMarginalNotesAsFullBody = this.UseMarginalNotesAsFullBody,
      };
    }

    private void LoadJson(JsonObject json) {
      this.PropertyCommonName = json.Get<String>("PropertyCommonName", String.Empty);
      this.PropertyLocation = json.Get<String>("PropertyLocation", String.Empty);
      this.PropertyMetesAndBounds = json.Get<String>("PropertyMetesAndBounds", String.Empty);
      this.Operation = json.Get<String>("Operation", String.Empty);
      this.OperationDate = json.Get<DateTime>("OperationDate", ExecutionServer.DateMaxValue);
      this.SeekForName = json.Get<String>("SeekForName", String.Empty);
      this.StartingYear = json.Get<Int32>("StartingYear", 0);
      this.FromOwnerName = json.Get<String>("FromOwnerName", String.Empty);
      this.MarginalNotes = json.Get<String>("MarginalNotes", String.Empty);
      this.UseMarginalNotesAsFullBody = json.Get<Boolean>("UseMarginalNotesAsFullBody", false);
    }

    private void LoadDTOData(CertificateDTO data) {
      this.PropertyCommonName = data.PropertyCommonName;
      this.PropertyLocation = data.PropertyLocation;
      this.PropertyMetesAndBounds = data.PropertyMetesAndBounds;
      this.Operation = data.Operation;
      this.OperationDate = data.OperationDate;
      this.SeekForName = data.SeekForName;
      this.StartingYear = data.StartingYear;
      this.FromOwnerName = data.FromOwnerName;
      this.MarginalNotes = data.MarginalNotes;
      this.UseMarginalNotesAsFullBody = data.UseMarginalNotesAsFullBody;
    }

    #endregion Methods

  }  // class CertificateExtData

} // namespace Empiria.Land.Certification
