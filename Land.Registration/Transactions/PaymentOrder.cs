/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : PaymentOrder                                   Pattern  : External Interfacer                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains payment order data for use with treasury connectors.                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Contains payment order data for use with treasury connectors.</summary>
  public class PaymentOrder : IPaymentOrder {

    #region Constructors and parsers

    protected PaymentOrder() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    public PaymentOrder(string routeNumber, DateTime dueDate, string controlTag) {
      Assertion.AssertObject(routeNumber, "routeNumber");
      Assertion.Assert(dueDate < DateTime.Today.AddYears(1),
                       "dueDate must be within one year starting today.");
      Assertion.AssertObject(controlTag, "controlTag");

      this.RouteNumber = routeNumber;
      this.DueDate = dueDate;
      this.ControlTag = controlTag;
    }

    static private readonly PaymentOrder _empty = new PaymentOrder() {
      IsEmptyInstance = true
    };

    static public PaymentOrder Empty {
      get {
        return _empty;
      }
    }

    static internal PaymentOrder Parse(JsonObject jsonObject) {
      if (jsonObject.IsEmptyInstance) {
        return PaymentOrder.Empty;
      }

      var paymentOrder = new PaymentOrder();

      paymentOrder.RouteNumber = jsonObject.Get<string>("RouteNumber");
      paymentOrder.DueDate = jsonObject.Get<DateTime>("DueDate");
      paymentOrder.ControlTag = jsonObject.Get<string>("ControlTag");

      paymentOrder.IsCompleted = jsonObject.Get<bool>("IsCompleted", false);
      if (paymentOrder.IsCompleted) {
        paymentOrder.PaymentDate = jsonObject.Get<DateTime>("PaymentDate");
        paymentOrder.PaymentReference = jsonObject.Get<string>("PaymentRef");
        paymentOrder.PaymentTotal = jsonObject.Get<decimal>("PaymentTotal");
      }

      return paymentOrder;
    }

    #endregion Constructors and parsers

    #region Public properties

    public bool IsEmptyInstance {
      get;
      private set;
    } = false;


    /// <summary>Línea de captura.</summary>
    public string RouteNumber {
      get;
      private set;
    } = String.Empty;


    /// <summary>Fecha de vigencia de la línea de captura.</summary>
    public DateTime DueDate {
      get;
      private set;
    } = ExecutionServer.DateMaxValue;


    /// <summary>Token para poder verificar el estado del pago.</summary>
    public string ControlTag {
      get;
      private set;
    } = String.Empty;


    /// <summary>True si el pago fue efectuado y verificado.</summary>
    public bool IsCompleted {
      get;
      private set;
    } = false;

    /// <summary>Fecha en que se efectuó el pago.</summary>
    public DateTime PaymentDate {
      get;
      private set;
    } = ExecutionServer.DateMaxValue;


    /// <summary>Folio de referencia del pago.</summary>
    public string PaymentReference {
      get;
      private set;
    } = String.Empty;


    /// <summary>Importe total del pago.</summary>
    public decimal PaymentTotal {
      get;
      private set;
    } = decimal.Zero;


    #endregion Public properties

    #region Methods

    public virtual void AssertIsValid() {

    }

    public void SetPayment(DateTime paymentDate, decimal paymentTotal, string paymentReference) {
      Assertion.Assert(paymentDate <= DateTime.Now, "Invalid payment date.");
      Assertion.Assert(paymentTotal >= decimal.Zero, "Invalid payment total.");
      Assertion.Assert(paymentReference != null, "Payment reference can't be null.");

      this.PaymentDate = paymentDate;
      this.PaymentTotal = paymentTotal;
      this.PaymentReference = paymentReference;

      this.IsCompleted = true;
    }

    public virtual JsonObject ToJson() {
      var json = new JsonObject();

      json.Add(new JsonItem("RouteNumber", this.RouteNumber));
      json.Add(new JsonItem("DueDate", this.DueDate));
      json.Add(new JsonItem("ControlTag", this.ControlTag));

      if (this.IsCompleted) {
        json.Add(new JsonItem("IsCompleted", this.IsCompleted));
        json.Add(new JsonItem("PaymentDate", this.PaymentDate));
        json.Add(new JsonItem("PaymentRef", this.PaymentReference));
        json.Add(new JsonItem("PaymentTotal", this.PaymentTotal));
      }
      return json;
    }

    public override string ToString() {
      return this.ToJson().ToString();
    }

    #endregion Methods

  }  // class PaymentOrder

}  // namespace Empiria.Land.Registration.Transactions
