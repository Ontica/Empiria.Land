/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payment Services                           Component : Integration Layer                       *
*  Assembly : Empiria.Land.Integration.dll               Pattern   : Service integration interface           *
*  Type     : IPaymentService                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines the services that must be implemented by a payment service provider.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Threading.Tasks;

namespace Empiria.Land.Integration.PaymentServices {

  /// <summary>Defines the services that must be implemented by a payment service provider.</summary>
  public interface IPaymentService {

    Task<decimal> CalculateFixedFee(string serviceUID, decimal quantity);

    Task<decimal> CalculateVariableFee(string serviceUID, decimal taxableBase);

    Task<IPaymentOrder> GeneratePaymentOrderFor(PaymentOrderRequestDto paymentOrderRequest);

  }  // interface IPaymentService

}  // namespace Empiria.Land.Integration.PaymentServices
