/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Fake service provider                   *
*  Type     : FakePaymentService                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Fake service used to test the integration with a IPaymentService provider.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>Fake service used to test the integration with a IPaymentService provider.</summary>
  internal class FakePaymentService : IPaymentService {


    public Task<decimal> CalculateFixedFee(string serviceUID, decimal quantity) {
      return Task.FromResult(quantity * 180m);
    }


    public Task<decimal> CalculateVariableFee(string serviceUID, decimal taxableBase) {
      return Task.FromResult(taxableBase * 1.075m);
    }


    public async Task<IPaymentOrder> GeneratePaymentOrderFor(PaymentOrderRequestDto paymentOrderRequest) {
      var o = new PaymentOrderDto();

      o.UID = Guid.NewGuid().ToString().ToLower();
      o.IssueTime = DateTime.Now;
      o.DueDate = o.IssueTime.Date.AddDays(30);

      foreach (var concept in paymentOrderRequest.Concepts) {
        if (concept.TaxableBase != 0) {
          o.Total += await CalculateVariableFee(concept.ConceptUID, concept.TaxableBase);
        } else {
          o.Total += await CalculateFixedFee(concept.ConceptUID, concept.Quantity);
        }
      }

      o.Attributes.Add("controlTag", Guid.NewGuid().ToString().Substring(0, 8));
      o.Attributes.Add("url", $"https://fakeUrl.net/rpp/payment-orders/{o.UID}");

      return o;
    }

  }  // class FakePaymentService

}  // namespace Empiria.Land.Integration.PaymentServices
