/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActTargetExtData                      Pattern  : IExtensibleData class               *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for RecordingActTarget items.                                        *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using Empiria.DataTypes;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for RecordingActTarget items.</summary>
  public class RecordingActTargetExtData : IExtensibleData {

    #region Constructors and parsers

    public RecordingActTargetExtData() {
      this.Notes = String.Empty;
    }

    static public RecordingActTargetExtData Parse(string json) {
      return new RecordingActTargetExtData();
    }

    static private readonly RecordingActTargetExtData _empty = new RecordingActTargetExtData();
    static public RecordingActTargetExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string Notes {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get {
        if (String.IsNullOrWhiteSpace(this.Notes)) {
          return true;
        }
        return false;
      }
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
        Notes = this.Notes,
      };
    }

    #endregion Methods

  }  // class RecordingActTargetExtData

} // namespace Empiria.Land.Registration
