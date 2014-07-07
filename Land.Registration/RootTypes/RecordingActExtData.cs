/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActExtData                            Pattern  : IExtensibleData class               *
*  Version   : 1.5        Date: 25/Jun/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording act.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.DataTypes;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingActExtData : IExtensibleData {

    #region Constructors and parsers

    public RecordingActExtData() {
      this.AppraisalAmount = Money.Empty;
      this.OperationAmount = Money.Empty;
      this.Contract = ContractData.Empty;
    }

    static internal RecordingActExtData Parse(string jsonString) {   
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingActExtData.Empty;
      }

      var json = Empiria.Data.JsonConverter.ToJsonObject(jsonString);

      var data = new RecordingActExtData();
      data.AppraisalAmount = Money.Parse(json.Slice("AppraisalAmount"));
      data.OperationAmount = Money.Parse(json.Slice("OperationAmount"));
      data.Contract = ContractData.Parse(json.Slice("Contract"));

      return data;
    }

    static public RecordingActExtData Empty {
      get {
        return new RecordingActExtData();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public Money AppraisalAmount {
      get;
      set;
    }

    public ContractData Contract {
      get;
      private set;
    }

    public Money OperationAmount {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get {
        if (this.AppraisalAmount == Money.Empty && 
            this.OperationAmount == Money.Empty && 
            this.Contract.IsEmptyInstance) {
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
        AppraisalAmount = new {
          CurrencyId = this.AppraisalAmount.Currency.Id,
          Value = this.AppraisalAmount.Amount.ToString("N2"),
        },
        OperationAmount = new {
          CurrencyId = this.OperationAmount.Currency.Id,
          Value = this.OperationAmount.Amount.ToString("N2"),
        },
        Contract = new {
          Number = this.Contract.Number,
          Date = this.Contract.Date,
          PlaceId = this.Contract.Place.Id,
          Interest = new {
            Rate = this.Contract.Interest.Rate.ToString("N4"),
            RateTypeId = this.Contract.Interest.RateType.Id,
            TermPeriods = this.Contract.Interest.TermPeriods,
            TermUnit = this.Contract.Interest.TermUnit.Id,
          }
        }
      };
    }

    #endregion Methods

  }  // class RecordingActData

} // namespace Empiria.Land.Registration
