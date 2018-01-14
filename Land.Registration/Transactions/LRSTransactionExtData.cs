﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration.Transactions         Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionExtData                          Pattern  : IExtensibleData class               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a land registration system transaction.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

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
        return LRSTransactionExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      var extData = new LRSTransactionExtData();

      if (json.Contains("BaseResourceId")) {
        extData.BaseResource = Resource.Parse(json.Get<int>("BaseResourceId"));
      }

      extData.RequesterNotes = json.Get<string>("RequesterNotes", String.Empty);

      if (json.Contains("PaymentOrder")) {
        extData.PaymentOrder = Transactions.PaymentOrder.Parse(json.Slice("PaymentOrder"));
      }

      if (json.Contains("ExternalTransaction")) {
        extData.ExternalTransaction = LRSExternalTransaction.Parse(json.Slice("ExternalTransaction"));
      }
      return extData;
    }

    static private readonly LRSTransactionExtData _empty =
                                new LRSTransactionExtData() { IsEmptyInstance = true };

    static public LRSTransactionExtData Empty {
      get {
        return _empty;
      }
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


    public LRSExternalTransaction ExternalTransaction {
      get;
      internal set;
    } = LRSExternalTransaction.Empty;


    public IPaymentOrder PaymentOrder {
      get;
      internal set;
    } = Transactions.PaymentOrder.Empty;


    public bool IsEmptyInstance {
      get;
      private set;
    } = false;

    #endregion Properties

    #region Methods

    public JsonObject GetJson() {
      var json = new JsonObject();

      if (!this.BaseResource.IsEmptyInstance) {
        json.Add(new JsonItem("BaseResourceId", this.BaseResource.Id));
      }

      json.AddIfValue(new JsonItem("RequesterNotes", this.RequesterNotes));

      if (this.PaymentOrder.RouteNumber != String.Empty) {
        json.Add("PaymentOrder", this.PaymentOrder.ToJson());
      }

      if (!this.ExternalTransaction.IsEmptyInstance) {
        json.Add("ExternalTransaction", this.ExternalTransaction.ToJson());
      }

      return json;
    }

    public override string ToString() {
      return this.GetJson().ToString();
    }

    #endregion Methods

  }  // class LRSTransactionExtData

} // namespace Empiria.Land.Registration.Transactions
