/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution : Empiria Land                                     System  : Land Application Services           *
*  Assembly : Empiria.Land.AppServices.dll                     Pattern : Application services                *
*  Type     : PaymentServices                                  License : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Application services for Empiria Land filing payments.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.OnePoint;

namespace Empiria.Land.AppServices {

  /// <summary>Application services for Empiria Land filing payments.</summary>
  static public class PaymentServices {

    #region Application services

    static public async Task<IPaymentOrderData> RequestPaymentOrderData(IFiling filing) {
      IPaymentOrderData paymentOrderData = filing.TryGetPaymentOrderData();

      if (paymentOrderData != null && paymentOrderData.RouteNumber != "") {
        return paymentOrderData;
      }

      ITreasuryConnector connector = ServiceLocator.GetTreasuryConnector();

      paymentOrderData = await connector.GeneratePaymentOrder(filing);

      filing.SetPaymentOrderData(paymentOrderData);

      return paymentOrderData;
    }

    //private void Initialize() {
    //  transaction = LRSTransaction.Parse(int.Parse(Request.QueryString["id"]));

    //  if (!transaction.HasPaymentOrder) {
    //    var treasuryConnector = new TreasuryConnector();

    //    this.paymentOrderData = treasuryConnector.RequestPaymentOrderData(this.transaction).Result;

    //    transaction.SetPaymentOrder(paymentOrderData);
    //  }
    //  this.paymentOrderData = transaction.PaymentOrder;
    //}

    static public async Task<IPaymentOrderData> RefreshPaymentOrder(IFiling filing) {
      IPaymentOrderData paymentOrderData = filing.TryGetPaymentOrderData();

      Assertion.AssertObject(paymentOrderData,
                             $"Transaction {filing.UID} doesn't have a registered payment order.");

      if (paymentOrderData.IsCompleted) {
        return paymentOrderData;
      }

      ITreasuryConnector connector = ServiceLocator.GetTreasuryConnector();

      paymentOrderData = await connector.RefreshPaymentOrder(paymentOrderData);

      if (paymentOrderData.IsCompleted) {
        filing.SetPaymentOrderData(paymentOrderData);
      }

      return paymentOrderData;
    }

    #endregion Application services

  }  // class PaymentServices

}  // namespace Empiria.Land.AppServices
