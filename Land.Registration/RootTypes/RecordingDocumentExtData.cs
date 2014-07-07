/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingDocumentExtData                       Pattern  : IExtensibleData class               *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording document.                                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingDocumentExtData : IExtensibleData {

    #region Fields

    private bool _isEmptyInstance = false;

    #endregion Fields

    #region Constructors and parsers

    public RecordingDocumentExtData() {
      this.IssuedByPosition = TypeAssociationInfo.Empty;
    }

    static internal RecordingDocumentExtData Parse(string jsonString) {   
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingDocumentExtData.Empty;
      }

      var json = Empiria.Data.JsonConverter.ToJsonObject(jsonString);

      var data = new RecordingDocumentExtData();
      data.BookNo = json.Find<String>("BookNo", String.Empty);
      data.IssuedByPosition = json.Find<TypeAssociationInfo>("IssuedByPositionId", 
                                                             TypeAssociationInfo.Empty);
      data.MainWitness = json.Find<Contact>("MainWitnessId", Person.Empty);
      data.MainWitnessPosition = json.Find<TypeAssociationInfo>("MainWitnessPositionId",
                                                                TypeAssociationInfo.Empty);
      data.SecondaryWitness = json.Find<Contact>("SecondaryWitnessId", Person.Empty);
      data.SecondaryWitnessPosition = json.Find<TypeAssociationInfo>("SecondaryWitnessPositionId",
                                                                     TypeAssociationInfo.Empty);
      data.StartSheet = json.Find<String>("StartSheet", String.Empty);
      data.EndSheet = json.Find<String>("EndSheet", String.Empty);
      data.SealUpperPosition = json.Find<Decimal>("SealUpperPosition", -1m);

      return data;
    }

    static public RecordingDocumentExtData Empty {
      get {
        var data = new RecordingDocumentExtData();
        data._isEmptyInstance = true;
        return data;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string BookNo {
      get;
      set;
    }

    public TypeAssociationInfo IssuedByPosition {
      get;
      set;
    }

    public Contact MainWitness {
      get;
      set;
    }

    public TypeAssociationInfo MainWitnessPosition {
      get;
      set;
    }

    public Contact SecondaryWitness {
      get;
      set;
    }

    public TypeAssociationInfo SecondaryWitnessPosition {
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
      get { return _isEmptyInstance; }
    }

    #endregion Properties

    #region Methods

    public string ToJson() {
      if (!this.IsEmptyInstance) {
        return Empiria.Data.JsonConverter.ToJson(this.GetObject());
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
