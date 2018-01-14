using System;

using Empiria.Json;

namespace Empiria.Land.Registration.Transactions {

  public interface IPaymentOrder {

    string RouteNumber {
      get;
    }

    DateTime DueDate {
      get;
    }

    string ControlTag {
      get;
    }

    bool IsCompleted {
      get;
    }

    DateTime PaymentDate {
      get;
    }

    string PaymentReference {
      get;
    }

    decimal PaymentTotal {
      get;
    }

    void SetPayment(DateTime paymentDate, decimal paymentTotal,
                    string paymentReference);

    JsonObject ToJson();

  }  // interface IPaymentOrder

}  // namespace Empiria.Land.Registration.Transactions
