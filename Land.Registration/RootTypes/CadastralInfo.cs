/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : CadastralInfo                                  Pattern  : IExtensibleData class               *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains extensible data about real estate cadastral info.                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.DataTypes;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data about real estate cadastral info.</summary>
  public class CadastralInfo : IExtensibleData {

    #region Constructors and parsers

    public CadastralInfo() {
      this.CadastralCode = String.Empty;
      this.CadastralSysLinkId = -1;

      this.LotNo = String.Empty;
      this.BlockNo = String.Empty;
      this.SectionNo = String.Empty;
      this.RegionNo = String.Empty;

      this.MetesAndBounds = String.Empty;

      this.CommonArea = Quantity.Parse(Unit.Empty, 0m);
      this.FloorArea = Quantity.Parse(Unit.Empty, 0m);
      this.TotalArea = Quantity.Parse(Unit.Empty, 0m);
    }

    static internal CadastralInfo FromJson(string json) {
      return new CadastralInfo();
    }

    static public CadastralInfo Empty {
      get {
        return new CadastralInfo();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string CadastralCode {
      get;
      set;
    }

    public int CadastralSysLinkId {
      get;
      set;
    }

    public string LotNo {
      get;
      set;
    }

    public string BlockNo {
      get;
      set;
    }

    public string SectionNo {
      get;
      set;
    }

    public string RegionNo {
      get;
      set;
    }

    public string MetesAndBounds {
      get;
      set;
    }

    public Quantity CommonArea {
      get;
      set;
    }

    public Quantity FloorArea {
      get;
      set;
    }

    public Quantity TotalArea {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get {
        if (String.IsNullOrWhiteSpace(this.LotNo)) {
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
      //List<KeyValuePair> list = new List<KeyValuePair>();

      //Empiria.KeyValuePair
      return new {
        LotNo = this.LotNo,
      };
    }

    #endregion Methods

  }  // class CadastralInfo

} // namespace Empiria.Land.Registration
