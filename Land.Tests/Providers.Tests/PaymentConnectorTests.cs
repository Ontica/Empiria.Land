﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Payments Connector                         Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Test class                              *
*  Type     : PaymentConnectorTests                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Integration tests with the payments connector service.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Xunit;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Tests;

namespace Empiria.Land.Providers.Tests {

  /// <summary>Integration tests with the payments connector service.</summary>
  public class PaymentConnectorTests {

    private readonly string PAYABLE_UID = ConfigurationData.Get<string>("Testing.FilingUID");

    private readonly int PAYMENT_ORDER_ROUTE_NUMBER_LENGTH = 20;


    [Fact]
    public async Task Should_Get_PaymentOrderData() {
      CommonMethods.Authenticate();

      IPayable payable = LRSTransaction.Parse(PAYABLE_UID);

      PaymentOrderDTO paymentOrderData = await EPaymentsUseCases.RequestPaymentOrderData(payable);

      Assert.Equal(PAYMENT_ORDER_ROUTE_NUMBER_LENGTH, paymentOrderData.RouteNumber.Length);
    }


  }

}