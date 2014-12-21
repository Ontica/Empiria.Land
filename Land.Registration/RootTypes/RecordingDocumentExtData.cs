/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentExtData                       Pattern  : IExtensibleData class               *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording document.                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Ontology;

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
      data.BookNo = json.Get<String>("BookNo", String.Empty);
      data.IssuedByPosition = json.Get<RoleType>("IssuedByPositionId", RoleType.Empty);
      data.MainWitness = json.Get<Contact>("MainWitnessId", Person.Empty);
      data.MainWitnessPosition = json.Get<RoleType>("MainWitnessPositionId", RoleType.Empty);
      data.SecondaryWitness = json.Get<Contact>("SecondaryWitnessId", Person.Empty);
      data.SecondaryWitnessPosition = json.Get<RoleType>("SecondaryWitnessPositionId", RoleType.Empty);
      data.StartSheet = json.Get<String>("StartSheet", String.Empty);
      data.EndSheet = json.Get<String>("EndSheet", String.Empty);
      data.SealUpperPosition = json.Get<Decimal>("SealUpperPosition", -1m);

      return data;
    }

    static private readonly RecordingDocumentExtData _empty = new RecordingDocumentExtData() { IsEmptyInstance = true };
    static public RecordingDocumentExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

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

    public RoleType MainWitnessPosition {
      get;
      set;
    }

    public Contact SecondaryWitness {
      get;
      set;
    }

    public RoleType SecondaryWitnessPosition {
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

    public decimal SealUpperPosition {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public string ToJson() {
      if (!this.IsEmptyInstance) {
        return Empiria.Json.JsonConverter.ToJson(this.GetObject());
      } else {
        return String.Empty;
      }
    }

    private object GetObject() {
      return new {
        BookNo = this.BookNo,
        IssuedByPosition = this.IssuedByPosition,
        MainWitness = this.MainWitness,
        MainWitnessPosition = this.MainWitnessPosition,
        SecondaryWitness = this.SecondaryWitness,
        SecondaryWitnessPosition = this.SecondaryWitnessPosition,
        StartSheet = this.StartSheet,
        EndSheet = this.EndSheet,
        SealUpperPosition = this.SealUpperPosition,
      };
    }

    #endregion Methods

  }  // class RecordingDocumentExtData

} // namespace Empiria.Land.Registration
