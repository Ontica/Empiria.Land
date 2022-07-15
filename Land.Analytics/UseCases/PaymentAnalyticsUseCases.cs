/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics Services                         Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : PaymentAnalyticsUseCases                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that returns payment analytics data.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes.Time;
using Empiria.Services;

using Empiria.Land.Analytics.Data;
using Empiria.Land.Analytics.Adapters;

namespace Empiria.Land.Analytics.UseCases {

  /// <summary>Use cases that returns payment analytics data.</summary>
  public class PaymentAnalyticsUseCases : UseCase {

    #region Constructors and parsers

    static public PaymentAnalyticsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<PaymentAnalyticsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<PaymentTotalDto> GetPaymentTotalsByDocumentType(TimeFrame period) {
      return PaymentAnalyticsData.GetPaymentTotalsByDocumentType(period);
    }

    #endregion Use cases

  }  // class PaymentAnalyticsUseCases

}  // namespace Empiria.Land.Analytics.UseCases
