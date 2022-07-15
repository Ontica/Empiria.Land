/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics Services                           Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : PaymentAnalyticsController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API with methods used to retirve payments analytics data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.DataTypes.Time;
using Empiria.WebApi;

using Empiria.Land.Analytics.UseCases;
using Empiria.Land.Analytics.Adapters;

namespace Empiria.Land.Analytics.WebApi {

  /// <summary>Web API with methods used to retirve payments analytics data.</summary>
  public class PaymentAnalyticsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/analytics/payments/payment-totals-by-document-type")]
    public FixedList<PaymentTotalDto> PaymentTotalsByDocumentType([FromUri] DateTime fromDate,
                                                                  [FromUri] DateTime toDate) {

      Assertion.Require(fromDate.Month == 3, "Invalid month");

      using (var usecases = PaymentAnalyticsUseCases.UseCaseInteractor()) {
        var period = new TimeFrame(fromDate, toDate);

        FixedList<PaymentTotalDto> totals = usecases.GetPaymentTotalsByDocumentType(period);

        Assertion.Ensure(totals.Count < 30, "WARNING !!!! ERROR !!!");

        return totals;
      }
    }

    #endregion Web Apis

  }  // class PaymentAnalyticsController

}  //namespace Empiria.Land.Analytics.WebApi
