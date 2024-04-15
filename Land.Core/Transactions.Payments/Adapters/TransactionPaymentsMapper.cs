/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionPaymentsMapper                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map transaction payments.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Payments.Adapters {

  /// <summary>Contains methods to map transaction payments.</summary>
  static internal class TransactionPaymentsMapper {

    #region Methods

    static internal BillingDto GetBillingDto(LRSTransaction transaction) {
      return new BillingDto {
        BillTo = transaction.ExtensionData.BillTo,
        RFC = transaction.ExtensionData.RFC,
      };
    }


    static internal PaymentDto GetPaymentDto(LRSTransaction transaction) {
      if (!transaction.PaymentData.HasPayment) {
        return null;
      }
      var payment = transaction.PaymentData.Payments[0];

      return new PaymentDto {
        ReceiptNo = payment.ReceiptNo,
        Total = payment.ReceiptTotal,
        Status = transaction.PaymentData.PaymentOrder.Status
      };
    }


    static internal PaymentOrderDto GetPaymentOrderDto(LRSTransaction transaction) {
      if (!transaction.PaymentData.HasPaymentOrder) {
        return null;
      }

      var po = transaction.PaymentData.PaymentOrder;

      return new PaymentOrderDto {
        UID = po.UID,
        DueDate = po.DueDate,
        IssueTime = po.IssueTime,
        Total = po.Total,
        Status = po.Status,
        Media = po.Media
      };
    }

    #endregion Methods

  }  // class TransactionsPaymentMapper

}  // namespace Empiria.Land.Transactions.Payments.Adapters
