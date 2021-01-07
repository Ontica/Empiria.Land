/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration.Transactions         Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionExtData                          Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a land registration system transaction.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;
using Empiria.Land.Transactions.Adapters;
using Empiria.Messaging.EMailDelivery;

using Empiria.OnePoint.EPayments;

namespace Empiria.Land.Registration.Transactions {

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

      extData.RFC = json.Get<string>("RFC", String.Empty);

      if (json.Contains("PaymentOrder")) {
        extData.PaymentOrderData = PaymentOrderDTO.Parse(json.Slice("PaymentOrder"));
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


    public string RFC {
      get;
      set;
    } = String.Empty;


    public LRSExternalTransaction ExternalTransaction {
      get;
      internal set;
    } = LRSExternalTransaction.Empty;


    public PaymentOrderDTO PaymentOrderData {
      get;
      internal set;
    } = PaymentOrderDTO.Empty;


    public SendTo SendTo {
      get;
      set;
    } = SendTo.Empty;


    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (!this.BaseResource.IsEmptyInstance) {
        json.Add("BaseResourceId", this.BaseResource.Id);
      }

      json.AddIfValue("RequesterNotes", this.RequesterNotes);

      json.AddIfValue("RFC", this.RFC);

      if (this.PaymentOrderData.RouteNumber != String.Empty) {
        json.Add("PaymentOrder", this.PaymentOrderData.ToJson());
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
      Assertion.AssertObject(fields, "fields");

      if (!String.IsNullOrWhiteSpace(fields.RequestedByEmail)) {
        this.SendTo = new SendTo(fields.RequestedByEmail);
      }
    }


    internal void Update(TransactionFields fields) {
      if (!String.IsNullOrWhiteSpace(fields.RequestedByEmail)) {
        this.SendTo = new SendTo(fields.RequestedByEmail);
      }
    }


    #endregion TransactionFields related methods

  }  // class LRSTransactionExtData

} // namespace Empiria.Land.Registration.Transactions
