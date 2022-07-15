/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Analytics Services                         Component : Data Layer                              *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Access library                     *
*  Type     : PaymentAnalyticsData                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data layer with methods that return payment analytics data.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.DataTypes.Time;

using Empiria.Land.Analytics.Adapters;

namespace Empiria.Land.Analytics.Data {

  /// <summary>Data layer with methods that return payment analytics data.</summary>
  static internal class PaymentAnalyticsData {

    static internal FixedList<PaymentTotalDto> GetPaymentTotalsByDocumentType(TimeFrame period) {
      var op = DataOperation.Parse("rptPaymentTotalsByDocumentType",
                                   period.StartTime,
                                   period.EndTime.AddDays(1)
                                                 .AddMilliseconds(-1));

      return DataReader.GetPlainObjectFixedList<PaymentTotalDto>(op);
    }

  }  // class PaymentAnalyticsData

}  // namespace Empiria.Land.Analytics.Data
