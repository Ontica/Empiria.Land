/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : RecordingActExtData                            Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a recording act.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;
using Empiria.Json;

namespace Empiria.Land.Registration {

  /// <summary>Contains extensible data for a recording act.</summary>
  public class RecordingActExtData  {

    #region Constructors and parsers

    private RecordingActExtData() {

    }

    public RecordingActExtData(Money? appraisalAmount, Money? operationAmount) {
      this.AppraisalAmount = appraisalAmount.HasValue ? appraisalAmount.Value : Money.Empty;
      this.OperationAmount = operationAmount.HasValue ? operationAmount.Value : Money.Empty;
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
      private set;
    } = Money.Empty;


    public Money OperationAmount {
      get;
      private set;
    } = Money.Empty;


    public bool IsEmptyInstance {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public void AssertIsComplete(RecordingAct recordingAct) {
      var rule = recordingAct.RecordingActType.RecordingRule;

      if (rule.EditAppraisalAmount) {
        Assertion.Require(this.AppraisalAmount != Money.Empty && this.AppraisalAmount != Money.Zero,
                         "En el acto jurídico " + recordingAct.IndexedName + " falta el importe del avalúo.");

      }
      if (rule.EditOperationAmount) {
        Assertion.Require(this.OperationAmount != Money.Empty && this.OperationAmount != Money.Zero,
                         "En el acto jurídico " + recordingAct.IndexedName + " falta el importe o monto de la operación.");
      }
    }

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (this.AppraisalAmount != Money.Empty) {
        json.Add("AppraisalAmount", this.AppraisalAmount.Amount);
        json.Add("AppraisalAmountCurrencyId", this.AppraisalAmount.Currency.Id);
      }

      if (this.OperationAmount != Money.Empty) {
        json.Add("OperationAmount", this.OperationAmount.Amount);
        json.Add("OperationAmountCurrencyId", this.OperationAmount.Currency.Id);
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
