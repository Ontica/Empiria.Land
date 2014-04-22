/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : TractIndexItemExtData                          Pattern  : IExtensibleData class               *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains extensible data about a tract index item, an association recording act/property.     *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using Empiria.DataTypes;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data about a tract index item, an association 
  /// recording act/property. </summary>
  public class TractIndexItemExtData : IExtensibleData {

    #region Constructors and parsers

    public TractIndexItemExtData() {
      this.Notes = String.Empty;
    }

    static public TractIndexItemExtData Parse(string json) {
      return new TractIndexItemExtData();
    }

    static public TractIndexItemExtData Empty {
      get {
        return new TractIndexItemExtData();
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
        return Empiria.Data.JsonConverter.ToJson(this.GetObject());
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

  }  // class TractIndexItemExtData

} // namespace Empiria.Land.Registration