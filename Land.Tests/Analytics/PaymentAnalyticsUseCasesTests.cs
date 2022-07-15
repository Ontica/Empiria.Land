/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics Services                         Component : Test cases                              *
*  Assembly : Empiria.Land.Tests.dll                     Pattern   : Use cases tests                         *
*  Type     : PaymentAnalyticsUseCasesTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for payment analytics services.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.DataTypes.Time;

using Empiria.Land.Analytics.UseCases;
using Empiria.Land.Analytics.Adapters;

namespace Empiria.Land.Tests.Analytics {

  /// <summary>Test cases for payment analytics services.</summary>
  public class PaymentAnalyticsUseCasesTests {

    #region Facts

    [Fact]
    public void Should_Get_Payment_Totals_By_Document_Type() {
      DateTime fromDate = new DateTime(2021, 3, 5);
      DateTime toDate = new DateTime(2021, 3, 10);

      using (var usecases = PaymentAnalyticsUseCases.UseCaseInteractor()) {
        var period = new TimeFrame(fromDate, toDate);

        FixedList<PaymentTotalDto> totals = usecases.GetPaymentTotalsByDocumentType(period);

        Assert.NotNull(totals);
        Assert.NotEmpty(totals);
      }
    }

    #endregion Facts

  }  // class PaymentAnalyticsUseCasesTests

}  // namespace Empiria.Land.Tests.Analytics
