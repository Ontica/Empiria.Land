/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                      Component : Domain Layer                          *
*  Assembly : Empiria.Land.Core.dll                        Pattern   : Information Holder                    *
*  Type     : LRSTransactionExtData                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains extensible data for a land registration system transaction.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.Messaging.EMailDelivery;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Registration;
using Empiria.Land.Transactions.Payments;

namespace Empiria.Land.Transactions {

  /// <summary>Contains extensible data for a land registration system transaction.</summary>
  public class LRSTransactionExtData {

    #region Constructors and parsers

    internal LRSTransactionExtData() {

    }

    public LRSTransactionExtData(Resource baseResource) {
      this.BaseResource = baseResource;
    }

    static internal LRSTransactionExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return new LRSTransactionExtData();
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var extData = new LRSTransactionExtData();

      if (json.Contains("BaseResourceId")) {
        extData.BaseResource = Resource.Parse(json.Get<int>("BaseResourceId"));
      }

      extData.RequesterNotes = json.Get<string>("RequesterNotes", String.Empty);

      extData.BillTo = json.Get<string>("BillTo", String.Empty);

      extData.RFC = json.Get<string>("RFC", String.Empty);

      if (json.Contains("paymentOrder")) {
        extData.PaymentOrder = PaymentOrder.Parse(json.Slice("paymentOrder"));
      }

      if (json.Contains("formerPaymentOrder")) {
        extData.FormerPaymentOrderData = FormerPaymentOrderDTO.Parse(json.Slice("formerPaymentOrder"));
      }

      if (json.Contains("ExternalTransaction")) {
        extData.ExternalTransaction = LRSExternalTransaction.Parse(json.Slice("ExternalTransaction"));
      }

      if (json.Contains("SendTo")) {
        extData.SendTo = SendTo.Parse(json.Slice("SendTo"));
      }

      return extData;
    }

    #endregion Constructors and parsers

    #region Properties

    public Resource BaseResource {
      get;
      internal set;
    } = Resource.Empty;


    public string RequesterNotes {
      get;
      internal set;
    } = String.Empty;


    public string BillTo {
      get;
      set;
    } = String.Empty;


    public string RFC {
      get;
      set;
    } = String.Empty;


    public LRSExternalTransaction ExternalTransaction {
      get;
      internal set;
    } = LRSExternalTransaction.Empty;


    internal PaymentOrder PaymentOrder {
      get;
      set;
    } = PaymentOrder.Empty;


    internal FormerPaymentOrderDTO FormerPaymentOrderData {
      get;
      set;
    } = FormerPaymentOrderDTO.Empty;


    public SendTo SendTo {
      get; set;
    } = SendTo.Empty;

    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (!this.BaseResource.IsEmptyInstance) {
        json.Add("BaseResourceId", this.BaseResource.Id);
      }

      json.AddIfValue("RequesterNotes", this.RequesterNotes);

      json.AddIfValue("BillTo", this.BillTo);

      json.AddIfValue("RFC", this.RFC);

      if (this.FormerPaymentOrderData.RouteNumber != String.Empty) {
        json.Add("formerPaymentOrder", this.FormerPaymentOrderData.ToJson());
      }

      if (!this.PaymentOrder.IsEmpty) {
        json.Add("paymentOrder", this.PaymentOrder.ToJson());
      }

      if (!this.ExternalTransaction.IsEmptyInstance) {
        json.Add("ExternalTransaction", this.ExternalTransaction.ToJson());
      }

      if (!this.SendTo.IsEmptyInstance) {
        json.Add("SendTo", this.SendTo.ToJson());
      }

      return json;
    }

    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

    #region TransactionFields related methods

    internal void Load(TransactionFields fields) {
      Assertion.Require(fields, "fields");

      LoadFields(fields);
    }

    internal void Update(TransactionFields fields) {
      Assertion.Require(fields, "fields");

      LoadFields(fields);
    }

    private void LoadFields(TransactionFields fields) {
      if (!String.IsNullOrWhiteSpace(fields.RequestedByEmail)) {
        this.SendTo = new SendTo(fields.RequestedByEmail);
      } else {
        this.SendTo = SendTo.Empty;
      }

      this.BillTo = fields.BillTo;
      this.RFC = fields.RFC;
    }

    #endregion TransactionFields related methods

  }  // class LRSTransactionExtData

} // namespace Empiria.Land.Transactions
