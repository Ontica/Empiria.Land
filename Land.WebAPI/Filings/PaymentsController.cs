/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution : Empiria Land                                     System  : Land Web API                        *
*  Assembly : Empiria.Land.WebApi.dll                          Pattern : Web Api Controller                  *
*  Type     : PaymentsController                               License : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Services to manage filing's payments.                                                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2018. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.OnePoint;

using Empiria.Land.AppServices;

namespace Empiria.Land.WebApi.Filings {

  /// <summary>Services to manage filing's payments.</summary>
  public class PaymentsController : WebApiController {

    #region GET methods

    #endregion GET methods

    #region POST methods

    [HttpPost]
    [Route("v2/filings/{filingUID}/payment-order")]
    public async Task<SingleObjectModel> GetPaymentOrder([FromUri] string filingUID) {
      try {
        IFiling filing = FilingServices.GetFiling(filingUID);

        IPaymentOrderData paymentOrderData = await PaymentServices.RequestPaymentOrderData(filing);

        return new SingleObjectModel(this.Request, paymentOrderData.ToResponse());

      } catch (Exception e) {
        throw base.CreateHttpException(e);

      }
    }

    #endregion POST methods

  }  // class PaymentsController

}  // namespace Empiria.Land.WebApi.Filings
