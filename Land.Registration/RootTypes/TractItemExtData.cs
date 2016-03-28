/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : TractItemExtData                               Pattern  : IExtensibleData class               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for TractItem instances.                                             *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for TractItem instances.</summary>
  public class TractItemExtData : IExtensibleData {

    #region Constructors and parsers

    public TractItemExtData() {
      this.Notes = String.Empty;
    }

    static public TractItemExtData Parse(string json) {
      return new TractItemExtData();
    }

    static private readonly TractItemExtData _empty = new TractItemExtData();
    static public TractItemExtData Empty {
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

  }  // class TractItemExtData

} // namespace Empiria.Land.Registration
