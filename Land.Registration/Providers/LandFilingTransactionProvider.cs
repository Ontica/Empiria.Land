/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Integration Layer                       *
*  Assembly : Empiria.Land.dll                           Pattern   : Provider implementation                 *
*  Type     : LandFilingTransactionProvider              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides land transaction services through external electronic filing requests.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Messaging.EMailDelivery;

using Empiria.OnePoint.EFiling;
using Empiria.OnePoint.EPayments;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Providers {

  /// <summary>Provides land transaction services through external electronic filing requests.</summary>
  public class LandFilingTransactionProvider: IFilingTransactionProvider {

    #region Fields

    static private readonly decimal BASE_SALARY_VALUE = ConfigurationData.Get<decimal>("BaseSalaryValue");

    private static readonly string PRINT_SERVICES_SERVER_BASE_ADDRESS =
                                        ConfigurationData.Get<string>("PrintServicesServerBaseAddress");

    #endregion Fields

    #region Constructors and parsers

    public LandFilingTransactionProvider() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    #endregion Constructors and parsers

    #region Methods


    public IPayable CreateTransaction(EFilingRequest filingRequest) {
      Assertion.AssertObject(filingRequest, "filingRequest");

      Procedure procedure = filingRequest.Procedure;

      var transactionType = LRSTransactionType.Parse(procedure.TransactionTypeId);

      var transaction = new LRSTransaction(transactionType);

      transaction.DocumentType = LRSDocumentType.Parse(procedure.DocumentTypeId);
      transaction.RequestedBy = filingRequest.RequestedBy.Name;
      transaction.Agency = filingRequest.Agency;
      transaction.RecorderOffice = RecorderOffice.Parse(procedure.AuthorityOfficeId);
      transaction.ExternalTransactionNo = filingRequest.UID;

      if (filingRequest.RequestedBy.Rfc.Length != 0) {
        transaction.ExtensionData.RFC = filingRequest.RequestedBy.Rfc;
      }
      if (filingRequest.RequestedBy.Email.Length != 0) {
        transaction.ExtensionData.SendTo = new SendTo(filingRequest.RequestedBy.Email);
      }

      transaction.Save();

      return transaction;
    }


    public void EventProcessed(string transactionUID, string eventName) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      Assertion.AssertObject(transaction, "transaction");

      if (transaction.Workflow.IsReadyForDeliveryOrReturn) {
        transaction.Workflow.DeliveredElectronicallyToAgency();
      }
    }


    public FixedList<EFilingDocument> GetOutputDocuments(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      if (!transaction.Workflow.IsFinished) {
        return new FixedList<EFilingDocument>();
      }

      var list = new List<EFilingDocument>();

      var document = transaction.Document;

      if (!document.IsEmptyInstance && document.Security.Signed()) {
        list.Add(MapToEFilingDocumentDTO(document));
      }

      var certificates = transaction.GetIssuedCertificates();

      foreach (var certificate in certificates) {
        if (certificate.Signed()) {
          list.Add(MapToEFilingDocumentDTO(certificate));
        }
      }

      return list.ToFixedList();
    }


    public IFilingTransaction GetTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      return ConvertToDTOInterface(transaction);
    }


    public IPayable GetTransactionAsPayable(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      Assertion.AssertObject(transaction, "transaction");

      return transaction;
    }


    public IFilingTransaction SetPayment(string transactionUID, string receiptNo) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.SetPayment(receiptNo, transaction.Items.TotalFee.Total);

      return ConvertToDTOInterface(transaction);
    }



    public IFilingTransaction SetPaymentOrder(IPayable transaction,
                                              FormerPaymentOrderDTO paymentOrderData) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(paymentOrderData, "paymentOrderData");

      transaction.SetFormerPaymentOrderData(paymentOrderData);

      return ConvertToDTOInterface((LRSTransaction) transaction);
    }


    public IFilingTransaction SubmitTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.Workflow.Receive("Ingresado automáticamente desde el sistema de notarías y grandes usuarios.");

      return ConvertToDTOInterface(transaction);
    }


    public FormerPaymentOrderDTO TryGetPaymentOrderData(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      var paymentOrder = ((IPayable) transaction).TryGetFormerPaymentOrderData();

      if (paymentOrder != null) {
        paymentOrder.PaymentTotal = transaction.Items.TotalFee.Total;
      }

      return paymentOrder;
    }


    public IFilingTransaction UpdateTransaction(EFilingRequest filingRequest) {
      Assertion.AssertObject(filingRequest, "filingRequest");
      Assertion.Assert(filingRequest.HasTransaction, "filingRequest.HasTransaction must be true.");

      var transaction = LRSTransaction.TryParse(filingRequest.Transaction.UID, true);

      transaction.RequestedBy = filingRequest.RequestedBy.Name;

      if (filingRequest.RequestedBy.Rfc.Length != 0) {
        transaction.ExtensionData.RFC = filingRequest.RequestedBy.Rfc;
      } else {
        transaction.ExtensionData.RFC = String.Empty;
      }

      if (filingRequest.RequestedBy.Email.Length != 0) {
        transaction.ExtensionData.SendTo = new SendTo(filingRequest.RequestedBy.Email);
      } else {
        transaction.ExtensionData.SendTo = SendTo.Empty;
      }

      transaction.Save();

      return ConvertToDTOInterface(transaction);
    }


    #endregion Methods


    #region Utility methods


    static private IFilingTransaction ConvertToDTOInterface(LRSTransaction transaction) {
      return new FilingTransactionDTO(transaction);
    }


    static private EFilingDocument MapToEFilingDocumentDTO(Certificate certificate) {
      return new EFilingDocument() {
        UID = certificate.UID,
        Type = certificate.CertificateType.Name,
        TypeName = $"Certificado de {certificate.CertificateType.DisplayName}",
        Name = $"Certificado {certificate.UID} del trámite {certificate.Transaction.UID}",
        ContentType = "text/html",
        Uri = $"{PRINT_SERVICES_SERVER_BASE_ADDRESS}/certificate.aspx?uid={certificate.UID}&" +
              $"externalUID={certificate.Transaction.ExternalTransactionNo}"
      };
    }


    static private EFilingDocument MapToEFilingDocumentDTO(RecordingDocument document) {
      return new EFilingDocument() {
        UID = document.UID,
        Type = document.Subtype.Name,
        TypeName = "Sello registral",
        Name = $"Sello registral {document.UID} del trámite {document.GetTransaction().UID}.",
        ContentType = "text/html",
        Uri = $"{PRINT_SERVICES_SERVER_BASE_ADDRESS}/recording.seal.aspx?uid={document.UID}&" +
              $"externalUID={document.GetTransaction().ExternalTransactionNo}"
      };
    }


    #endregion Utility methods


    #region Inner class FilingTransactionDTO

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

    #endregion Inner class FilingTransactionDTO


  }  // class LandFilingTransactionProvider

}  // namespace Empiria.Land.Providers
