/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Integration Services                       Component : Empiria Land Transaction Services       *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : LandFilingTransactionProvider              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides land transaction services through external electronic filing requests.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.OnePoint.EFiling;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Integration {

  /// <summary>Provides land transaction services through external electronic filing requests.</summary>
  public class LandFilingTransactionProvider: IFilingTransactionProvider {

    #region Fields

    static private readonly decimal BASE_SALARY_VALUE = 84.50m;

    #endregion Fields

    #region Constructors and parsers

    public LandFilingTransactionProvider() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    #endregion Constructors and parsers


    #region Methods

    public IFilingTransaction CreateTransaction(EFilingRequest filingRequest) {
      var transactionType = LRSTransactionType.Parse(699);

      var transaction = new LRSTransaction(transactionType);

      transaction.DocumentType = LRSDocumentType.Parse(744);
      transaction.RequestedBy = filingRequest.RequestedBy.name;
      transaction.Agency = filingRequest.Agency;
      transaction.RecorderOffice = RecorderOffice.Parse(96);
      transaction.ExternalTransactionNo = filingRequest.UID;

      transaction.Save();

      ApplyItemsRuleToTransaction(transaction);

      var paymentOrderData = CreatePaymentOrderData();

      transaction.SetPaymentOrderData(paymentOrderData);

      return ConvertToDTOInterface(transaction);
    }


    public IFilingTransaction GetTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      return ConvertToDTOInterface(transaction);
    }


    public IFilingTransaction SetPayment(string transactionUID, string receiptNo) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.AddPayment(receiptNo, transaction.Items.TotalFee.Total);

      return ConvertToDTOInterface(transaction);
    }


    public OnePoint.EPayments.PaymentOrderDTO TryGetPaymentOrderData(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      return transaction.TryGetPaymentOrderData();
    }


    public IFilingTransaction SubmitTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.Workflow.Receive("Ingresado automáticamente desde el sistema de notarías y grandes usuarios.");

      return ConvertToDTOInterface(transaction);
    }


    public IFilingTransaction UpdateTransaction(EFilingRequest filingRequest) {
      Assertion.AssertObject(filingRequest, "filingRequest");
      Assertion.AssertObject(filingRequest.TransactionUID, "filingRequest.TransactionUID");

      var transaction = LRSTransaction.TryParse(filingRequest.TransactionUID, true);

      return ConvertToDTOInterface(transaction);
    }


    #endregion Methods


    #region Utility methods

    static private void ApplyItemsRuleToTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 699 && transaction.DocumentType.Id == 708) {
        transaction.AddItem(RecordingActType.Parse(2284), LRSLawArticle.Parse(874), BASE_SALARY_VALUE * 2);
        transaction.AddItem(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (transaction.TransactionType.Id == 702 && transaction.DocumentType.Id == 713) {
        transaction.AddItem(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (transaction.Id == 702 && transaction.DocumentType.Id == 710) {
        transaction.AddItem(RecordingActType.Parse(2111), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (transaction.TransactionType.Id == 702 && transaction.DocumentType.Id == 711) {
        transaction.AddItem(RecordingActType.Parse(2112), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      }
    }


    static private IFilingTransaction ConvertToDTOInterface(LRSTransaction transaction) {
      return new FilingTransactionDTO(transaction);
    }


    static private OnePoint.EPayments.PaymentOrderDTO CreatePaymentOrderData() {
      var routeNumber = EmpiriaString.BuildRandomString(16, 16);
      var controlTag = EmpiriaString.BuildRandomString(6, 6);

      return new OnePoint.EPayments.PaymentOrderDTO(routeNumber, DateTime.Today.AddDays(20), controlTag);
    }


    private class FilingTransactionDTO : IFilingTransaction {

      internal FilingTransactionDTO(LRSTransaction transaction) {
        this.Id = transaction.Id;
        this.UID = transaction.UID;
        this.PresentationTime = transaction.PresentationTime;
        this.StatusName = transaction.Workflow.CurrentStatusName;
      }

      public int Id {
        get;
        private set;
      }

      public string UID {
        get;
        private set;
      }

      public string StatusName {
        get;
        private set;
      }

      public DateTime PresentationTime {
        get;
        private set;
      }

    }


    #endregion Utility methods

  }  // class LandFilingTransactionProvider

}  // namespace Empiria.Land.Integration
