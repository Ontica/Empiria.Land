/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Tests                                   *
*  Assembly : Empiria.Land.Tests.dll                       Pattern : Test class                              *
*  Type     : PaymentTests                                 License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tests for payments functionality.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Xunit;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Messaging;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Tests {

  public class PaymentTests {

    private readonly string PAYABLE_UID = ConfigurationData.Get<string>("Testing.FilingUID");

    private readonly int PAYMENT_ORDER_ROUTE_NUMBER_LENGTH = 20;


    [Fact]
    public void Should_Start_LandMessenger() {
      Exception e = null;

      try {
        LandMessenger.Start();

      } catch (Exception exception) {
        e = exception;
      }

      Assert.Null(e);
    }


    [Fact]
    public async Task Should_Get_PaymentOrderData() {
      CommonMethods.Authenticate();

      IPayable payable = LRSTransaction.TryParse(PAYABLE_UID);

      PaymentOrderDTO paymentOrderData = await EPaymentsUseCases.RequestPaymentOrderData(payable);

      Assert.Equal(PAYMENT_ORDER_ROUTE_NUMBER_LENGTH, paymentOrderData.RouteNumber.Length);
    }


  }  // PaymentTests

}  // namespace Empiria.Land.Tests
