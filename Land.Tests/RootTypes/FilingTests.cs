/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Tests                                   *
*  Assembly : Empiria.Land.Tests.dll                       Pattern : Test class                              *
*  Type     : FilingTests                                  License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tests for filing objects functionality.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Xunit;

using Empiria.OnePoint;
using Empiria.OnePoint.AppServices;

using Empiria.Land.Messaging;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Tests {

  public class FilingTests {

    private readonly string FILING_UID = ConfigurationData.Get<string>("Testing.FilingUID");

    private readonly int PAYMENT_ORDER_ROUTE_NUMBER_LENGTH = 20;


    [Fact]
    public void Should_Execute_LandMessenger() {
      Exception e = null;

      try {
        LandMessenger.Execute();

      } catch (Exception exception) {
        e = exception;
      }

      Assert.Null(e);
    }


    [Fact]
    public async Task Should_Get_PaymentOrderData() {
      CommonMethods.Authenticate();

      IFiling filing = LRSTransaction.TryParse(FILING_UID);

      IPaymentOrderData paymentOrderData = await PaymentServices.RequestPaymentOrderData(filing);

      Assert.Equal(PAYMENT_ORDER_ROUTE_NUMBER_LENGTH, paymentOrderData.RouteNumber.Length);
    }


  }  // FilingTests

}  // namespace Empiria.Land.Tests
