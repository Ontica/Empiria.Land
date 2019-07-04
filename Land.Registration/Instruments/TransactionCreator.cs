using System;

using Empiria.Json;
using Empiria.Messaging;

using Empiria.OnePoint;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Instruments {

  /// <summary>Acts as an abstract class that holds data for an external transaction request, that may be
  ///  integrated into an Empiria Land transaction.</summary>
  public class TransactionCreator {

    #region Constructors and parsers

    public TransactionCreator() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    #endregion Constructors and parsers

    #region Public properties


    /// <summary>Returns the transaction type used for all pending note requests.</summary>
    internal LRSTransactionType TransactionType {
      get {
        return LRSTransactionType.Parse(699);
      }
    }

    /// <summary>Returns the document type used for all pending note requests.</summary>
    internal LRSDocumentType DocumentType {
      get {
        return LRSDocumentType.Parse(744);
      }
    }

    #endregion Public properties

    #region Methods

    private void ApplyItemsRuleToTransaction(LRSTransaction transaction) {
      const decimal baseSalaryValue = 84.50m;

      if (this.TransactionType.Id == 699 && this.DocumentType.Id == 708) {
        transaction.AddItem(RecordingActType.Parse(2284), LRSLawArticle.Parse(874), baseSalaryValue * 2);
        transaction.AddItem(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), baseSalaryValue * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 713) {
        transaction.AddItem(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), baseSalaryValue * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 710) {
        transaction.AddItem(RecordingActType.Parse(2111), LRSLawArticle.Parse(859), baseSalaryValue * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 711) {
        transaction.AddItem(RecordingActType.Parse(2112), LRSLawArticle.Parse(859), baseSalaryValue * 2);
      }
    }


    public LRSTransaction CreateTransaction(LegalInstrument instrument, JsonObject data) {

      EmpiriaLog.Debug(data.ToString());

      var transaction = new LRSTransaction(this.TransactionType);

      transaction.DocumentType = this.DocumentType;
      transaction.RequestedBy = instrument.RequestedBy;
      transaction.Agency = instrument.IssueOffice;
      transaction.DocumentDescriptor = instrument.Number;
      transaction.RecorderOffice = RecorderOffice.Parse(96);
      transaction.ExtensionData.BaseResource = instrument.Property;
      transaction.ExternalTransactionNo = instrument.UID;

      transaction.ExtensionData.RFC = data.Get("rfc", "XAXX-010101-000");

      if (data.HasValue("sendTo")) {
        transaction.ExtensionData.SendTo = new SendTo(data.Get<string>("sendTo"));
      }

      transaction.Save();

      this.ApplyItemsRuleToTransaction(transaction);

      var paymentOrderData = this.CreatePaymentOrderData();

      transaction.SetPaymentOrderData(paymentOrderData);

      return transaction;
    }

    public LRSTransaction FileTransaction(LegalInstrument instrument, JsonObject data) {
      var transaction = LRSTransaction.TryParse(instrument.TransactionUID);

      if (data.HasValue("rfc") || data.HasValue("sendTo")) {
        if (data.HasValue("rfc")) {
          transaction.ExtensionData.RFC = data.Get<string>("rfc");
        }
        if (data.HasValue("sendTo")) {
          transaction.ExtensionData.SendTo = new SendTo(data.Get<string>("sendTo"));
        }
        transaction.Save();
      }

      transaction.AddPayment(data.Get<string>("paymentReceiptNo", EmpiriaString.BuildRandomString(10, 10)).ToString(), 84.50m * 2);

      transaction.Workflow.Receive("Ingresado automáticamente desde el sistema de notarías y grandes usuarios.");

      return transaction;
    }


    private PaymentOrderData CreatePaymentOrderData() {
      var routeNumber = EmpiriaString.BuildRandomString(16, 16);
      var controlTag = EmpiriaString.BuildRandomString(6, 6);

      return new PaymentOrderData(routeNumber, DateTime.Today.AddDays(20), controlTag);
    }


    #endregion Methods

  }  // class TransactionCreator

}  // namespace Empiria.Land.Instruments