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
      return Task.FromResult(taxableBase * 0.075m);
    }


    public Task EnsureIsPayed(string paymentOrderUID, decimal amount) {
      return Task.CompletedTask;
    }


    public async Task<IPaymentOrder> GeneratePaymentOrderFor(PaymentOrderRequestDto paymentOrderRequest) {
      var o = new PaymentOrderDto {
        UID = Guid.NewGuid().ToString().ToLower(),
        IssueTime = DateTime.Now,
        DueDate = DateTime.Now.Date.AddDays(30),
        Status = "Pendiente.FakeService",
        Issuer = "Empiria.Land.FakeService"
      };

      foreach (var concept in paymentOrderRequest.Concepts) {
        if (concept.TaxableBase != 0) {
          o.Total += await CalculateVariableFee(concept.ConceptUID, concept.TaxableBase);
        } else {
          o.Total += await CalculateFixedFee(concept.ConceptUID, concept.Quantity);
        }
      }

      return o;
    }


    public Task<string> GetPaymentStatus(string paymentOrderUID) {
      return Task.FromResult("Pendiente.FakeService");
    }

  }  // class FakePaymentService

}  // namespace Empiria.Land.Integration.PaymentServices
