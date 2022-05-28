/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Json-based information holder           *
*  Type     : PaymentOrder                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a land transaction payment order.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.DataTypes;
using Empiria.Json;

using Empiria.Land.Integration.PaymentServices;

namespace Empiria.Land.Transactions {

  /// <summary>Holds data about a land transaction payment order.</summary>
  public class PaymentOrder : IPaymentOrder {

    private readonly Dictionary<string, object> _attributes;

    #region Constructors and parsers

    private PaymentOrder() {
      this._attributes = new Dictionary<string, object>();
    }


    public PaymentOrder(IPaymentOrder paymentOrder) {
      EnsureIsValid(paymentOrder);

      this.UID = paymentOrder.UID;
      this.Issuer = paymentOrder.Issuer;
      this.Version = paymentOrder.Version;
      this.IssueTime = paymentOrder.IssueTime;
      this.DueDate = paymentOrder.DueDate;
      this.Total = paymentOrder.Total;

      this._attributes = new Dictionary<string, object>(paymentOrder.Attributes);
    }


    private PaymentOrder(JsonObject json) {
      this.UID = json.Get("uid", this.UID);
      this.Issuer = json.Get("issuer", this.Issuer);
      this.Version = json.Get("version", this.Version);
      this.IssueTime = json.Get("issueTime", this.IssueTime);
      this.DueDate = json.Get("dueDate", this.DueDate);
      this.Total = json.Get("total", this.Total);
      this.Status = json.Get("status", this.Status);

      if (json.Contains("attributes")) {
        var attributes = json.Slice("attributes", false);

        _attributes = new Dictionary<string, object>(attributes.ToDictionary());
      }
    }


    static internal PaymentOrder Parse(JsonObject json) {
      Assertion.Require(json, "json");

      return new PaymentOrder(json);
    }


    static public PaymentOrder Empty {
      get {
        return new PaymentOrder();
      }
    }

    #endregion Constructors and parsers

    #region Fields

    public string UID {
      get; internal set;
    } = string.Empty;


    public string Issuer {
      get; internal set;
    } = string.Empty;


    public string Version {
      get; internal set;
    } = string.Empty;


    public DateTime IssueTime {
      get; internal set;
    }


    public DateTime DueDate {
      get; internal set;
    }


    public decimal Total {
      get; internal set;
    }


    public string Status {
      get; internal set;
    } = "Pendiente de pago";


    public IDictionary<string, object> Attributes {
      get {
        return _attributes;
      }
    }

    [Newtonsoft.Json.JsonIgnore]
    public bool IsEmpty {
      get {
        return String.IsNullOrEmpty(this.UID);
      }
    }

    public MediaData Media {
      get {
        if (Attributes.ContainsKey("url") && Attributes.ContainsKey("mediaType")) {
          return new MediaData((string) Attributes["mediaType"], (string) Attributes["url"]);
        }
        return MediaData.Empty;
      }
    }

    #endregion Fields

    #region Methods

    static private void EnsureIsValid(IPaymentOrder paymentOrder) {
      Assertion.Require(paymentOrder, "paymentOrder");

      Assertion.Require(paymentOrder.UID, "paymentOrder.UID");
      Assertion.Require(paymentOrder.Issuer, "paymentOrder.Issuer");
      Assertion.Require(paymentOrder.Version, "paymentOrder.Version");
      Assertion.Require(paymentOrder.IssueTime, "paymentOrder.IssueTime");
      Assertion.Require(paymentOrder.DueDate, "paymentOrder.DueDate");
      Assertion.Require(paymentOrder.Total, "paymentOrder.Total");
      Assertion.Require(paymentOrder.Status, "paymentOrder.Status");
      Assertion.Require(paymentOrder.Attributes, "paymentOrder.Attributes");
    }

    public virtual JsonObject ToJson() {
      if (this.IsEmpty) {
        return JsonObject.Empty;
      }

      var json = new JsonObject();

      json.Add("uid", this.UID);
      json.Add("issuer", this.Issuer);
      json.Add("version", this.Version);
      json.Add("issueTime", this.IssueTime);
      json.Add("dueDate", this.DueDate);
      json.Add("total", this.Total);
      json.AddIfValue("status", this.Status);

      if (this.Attributes.Count == 0) {
        return json;
      }

      var jsonAttributes = new JsonObject();

      foreach (var attribute in this.Attributes) {
        jsonAttributes.Add(attribute.Key, attribute.Value);
      }

      json.Add("attributes", jsonAttributes);

      return json;
    }

    #endregion Methods

  }  // class PaymentOrder

} // namespace Empiria.Land.Transactions
