/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActExtData                            Pattern  : IExtensibleData class               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording act.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.DataTypes;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingActExtData  {

    #region Constructors and parsers

    public RecordingActExtData() {

    }

    static internal RecordingActExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return RecordingActExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var data = new RecordingActExtData();
      data.LoadJson(json);

      return data;
    }

    static private readonly RecordingActExtData _empty =
                                new RecordingActExtData() { IsEmptyInstance = true };

    static public RecordingActExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public Money AppraisalAmount {
      get;
      set;
    } = Money.Empty;


    public Money OperationAmount {
      get;
      set;
    } = Money.Empty;


    public bool IsEmptyInstance {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (this.AppraisalAmount != Money.Empty) {
        json.Add(new JsonItem("AppraisalAmount", this.AppraisalAmount.Amount));
        json.Add(new JsonItem("AppraisalAmountCurrencyId", this.AppraisalAmount.Currency.Id));
      }

      if (this.OperationAmount != Money.Empty) {
        json.Add(new JsonItem("OperationAmount", this.OperationAmount.Amount));
        json.Add(new JsonItem("OperationAmountCurrencyId", this.OperationAmount.Currency.Id));
      }

      return json;
    }


    private void LoadJson(JsonObject json) {
      this.AppraisalAmount = Money.Parse(Currency.Parse(json.Get<Int32>("AppraisalAmountCurrencyId", -1)),
                                         json.Get<decimal>("AppraisalAmount", 0m));

      this.OperationAmount = Money.Parse(Currency.Parse(json.Get<Int32>("OperationAmountCurrencyId", -1)),
                                         json.Get<decimal>("OperationAmount", 0m));
    }


    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

  }  // class RecordingActData

} // namespace Empiria.Land.Registration
