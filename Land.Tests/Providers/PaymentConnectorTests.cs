/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Empiria Land Providers                     Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : PaymentConnectorTests                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests with the payments connector service.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using System.Threading.Tasks;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Tests.Providers {

  /// <summary>Integration tests with the payments connector service.</summary>
  public class PaymentConnectorTests {

    private readonly string _PAYABLE_UID = TestingConstants.EFILING_UID;

    private readonly int _PAYMENT_ORDER_ROUTE_NUMBER_LENGTH =
                            TestingConstants.PAYMENT_ORDER_ROUTE_NUMBER_LENGTH;


    [Fact]
    public async Task Should_Get_PaymentOrderData() {
      CommonMethods.Authenticate();

      IPayable payable = LRSTransaction.Parse(_PAYABLE_UID);

      PaymentOrderDTO paymentOrderData = await EPaymentsUseCases.RequestPaymentOrderData(payable);

      Assert.Equal(_PAYMENT_ORDER_ROUTE_NUMBER_LENGTH, paymentOrderData.RouteNumber.Length);
    }


  }  // class PaymentConnectorTests

}  // namespace Empiria.Land.Tests.Providers
