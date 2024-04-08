/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction services                         Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LRSTransactionPaymentData                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds transaction's payment order and payments data.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint.EPayments;

using Empiria.Land.Transactions.Payments;
using Empiria.Land.Integration.PaymentServices;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Holds transaction's payment order and payments data.</summary>
  public class LRSTransactionPaymentData : IPayable {

    #region Fields

    private readonly LRSTransaction _transaction;

    private Lazy<LRSPaymentList> _payments = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSTransactionPaymentData(LRSTransaction transaction) {
      _transaction = transaction;
      _payments = new Lazy<LRSPaymentList>(() => LRSPaymentList.Parse(_transaction));
    }


    static internal LRSTransactionPaymentData Parse(LRSTransaction transaction) {
      return new LRSTransactionPaymentData(transaction);
    }

    #endregion Constructors and parsers

    #region Properties

    public FormerPaymentOrderDTO FormerPaymentOrderData {
      get {
        return _transaction.ExtensionData.FormerPaymentOrderData;
      }
    }


    public bool HasPayment {
      get {
        return (this.Payments.Count != 0 &&
                this.Payments[0].ReceiptNo.Length != 0);
      }
    }


    public bool HasPaymentOrder {
      get {
        return (!this.PaymentOrder.IsEmpty ||
                this.FormerPaymentOrderData.RouteNumber.Length != 0);
      }
    }


    public bool IsFeeWaiverApplicable {
      get {
        if (_transaction.TransactionType.Id == 705 ||
            _transaction.TransactionType.Id == 704 ||
            _transaction.TransactionType.Id == 707) {
          return true;
        }
        return (_transaction.TransactionType.Id == 700 && _transaction.DocumentType.Id == 722);
      }
    }


    public LRSPaymentList Payments {
      get {
        return _payments.Value;
      }
    }


    public PaymentOrder PaymentOrder {
      get {
        return _transaction.ExtensionData.PaymentOrder;
      }
    }

    #endregion Properties

    #region Methods

    public void ApplyFeeWaiver() {
      // this.Payments.Add(LRSPayment.FeeWaiver);
    }


    public void CancelPayment() {
      Assertion.Require(this.HasPayment,
                       $"There are not any registered payments for transaction '{_transaction.UID}'.");

      Assertion.Require(_transaction.ControlData.CanCancelPayment,
                       $"It's not possible to cancel the payment for transaction '{_transaction.UID}'.");

      var payment = this.Payments[0];

      this.Payments.Remove(payment);

      payment.Delete();

      this.PaymentOrder.Status = String.Empty;

      _transaction.Save();
    }


    public void CancelPaymentOrder() {
      _transaction.ExtensionData.PaymentOrder = PaymentOrder.Empty;

      ((IPayable) this).SetFormerPaymentOrderData(FormerPaymentOrderDTO.Empty);

      if (!_transaction.IsNew) {
        _transaction.Save();
      }
    }


    public void SetPayment(PaymentFields paymentFields) {
      this.SetPayment(paymentFields.ReceiptNo, paymentFields.Total);

      this.PaymentOrder.Status = paymentFields.Status;

      _transaction.Save();
    }


    public void SetPayment(string receiptNo, decimal receiptTotal) {
      LRSPayment payment = null;

      if (this.Payments.Count == 0) {
        payment = new LRSPayment(_transaction, receiptNo, receiptTotal);

        this.Payments.Add(payment);

      } else {
        payment = this.Payments[0];

        payment.SetReceipt(receiptNo, receiptTotal);
      }

      payment.Save();
    }


    public void SetPaymentOrder(IPaymentOrder paymentOrder) {
      Assertion.Require(paymentOrder, nameof(paymentOrder));

      _transaction.ExtensionData.PaymentOrder = new PaymentOrder(paymentOrder);

      if (!_transaction.IsNew) {
        _transaction.Save();
      }
    }

    #endregion Methods

    #region IPayable implementation

    string IPayable.UID {
      get {
        return _transaction.UID;
      }
    }

    /// <summary>Former Version 4. Tlaxcala.</summary>
    void IPayable.SetFormerPaymentOrderData(FormerPaymentOrderDTO paymentOrderData) {
      _transaction.ExtensionData.FormerPaymentOrderData = paymentOrderData;

      if (!_transaction.IsNew) {
        _transaction.Save();
      }
    }


    FormerPaymentOrderDTO IPayable.TryGetFormerPaymentOrderData() {
      if (!_transaction.ExtensionData.FormerPaymentOrderData.IsEmptyInstance) {
        return _transaction.ExtensionData.FormerPaymentOrderData;
      } else {
        return null;
      }
    }

    #endregion IPayable implementation

  } // class LRSTransactionPaymentData

} // namespace Empiria.Land.Registration.Transactions
