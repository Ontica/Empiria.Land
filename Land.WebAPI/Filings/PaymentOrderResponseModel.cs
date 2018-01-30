/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution : Empiria Land                                     System  : Land Web API                        *
*  Assembly : Empiria.Land.WebApi.dll                          Pattern : Response methods                    *
*  Type     : PaymentOrderResponseModel                        License : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Response models for PaymentOrder objects.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint;

namespace Empiria.Land.WebApi.Filings {

  /// <summary>Response models for PaymentOrder objects.</summary>
  static internal class PaymentOrderResponseModel {

    static internal object ToResponse(this IPaymentOrderData paymentOrderData) {
      return new {
        controlTag = paymentOrderData.ControlTag
      };
    }

  }  // class PaymentOrderResponseModel

}  // namespace Empiria.Land.WebApi.Filings
